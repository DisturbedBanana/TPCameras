using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public new Camera camera;
    [SerializeField] private CameraConfiguration configurationCible;  // La configuration cible
    [SerializeField] private CameraConfiguration configurationCourante;  // La configuration courante
    private bool isTransitioning = false;
    public float smoothTime = 0.2f;  // Temps de lissage pour la transition
    private float yawVelocity, pitchVelocity, rollVelocity, fovVelocity, distanceVelocity;  // Vélocités pour le lissage

    private List<AView> activeViews = new List<AView>();

    private void Start()
    {
        // Initialisation des configurations au démarrage
        configurationCible = ComputeAverage();
        configurationCourante = configurationCible;
    }

    public void AddView(AView view)
    {
        activeViews.Add(view);
    }

    public void RemoveView(AView view)
    {
        activeViews.Remove(view);
    }

    private void ApplyConfiguration()
    {
        // Appliquer la configuration courante à la caméra
        camera.transform.position = configurationCourante.GetPosition();
        camera.transform.rotation = configurationCourante.GetRotation();
        camera.fieldOfView = configurationCourante.fov;
    }

    private void Update()
    {
        // Calcul de la configuration cible en fonction des vues actives
        configurationCible = ComputeAverage();

        // Lisser la transition entre configurationCourante et configurationCible
        configurationCourante.yaw = Mathf.SmoothDampAngle(configurationCourante.yaw, configurationCible.yaw, ref yawVelocity, smoothTime);
        configurationCourante.pitch = Mathf.SmoothDamp(configurationCourante.pitch, configurationCible.pitch, ref pitchVelocity, smoothTime);
        configurationCourante.roll = Mathf.SmoothDamp(configurationCourante.roll, configurationCible.roll, ref rollVelocity, smoothTime);
        configurationCourante.distance = Mathf.SmoothDamp(configurationCourante.distance, configurationCible.distance, ref distanceVelocity, smoothTime);
        configurationCourante.fov = Mathf.SmoothDamp(configurationCourante.fov, configurationCible.fov, ref fovVelocity, smoothTime);
        configurationCourante.pivot = Vector3.Lerp(configurationCourante.pivot, configurationCible.pivot, Time.deltaTime / smoothTime);

        // Appliquer la configuration lissée à la caméra
        ApplyConfiguration();
    }

    CameraConfiguration ComputeAverage()
    {
        CameraConfiguration average = new CameraConfiguration();
        Vector2 Sum = new Vector2(0, 0);
        float totalWeight = 0;

        foreach (AView view in activeViews)
        {
            var viewConfig = view.GetCameraConfiguration();
            totalWeight += view.weight;

            average.pitch += viewConfig.pitch * view.weight;
            Sum += new Vector2(Mathf.Cos(viewConfig.yaw * Mathf.Deg2Rad), Mathf.Sin(viewConfig.yaw * Mathf.Deg2Rad)) * view.weight;
            average.roll += viewConfig.roll * view.weight;
            average.pivot += viewConfig.pivot * view.weight;
            average.distance += viewConfig.distance * view.weight;
            average.fov += viewConfig.fov * view.weight;
        }

        if (totalWeight > 0)
        {
            average.pitch /= totalWeight;
            average.yaw = Vector2.SignedAngle(Vector2.right, Sum);  // Calcul du yaw
            average.roll /= totalWeight;
            average.pivot /= totalWeight;
            average.distance /= totalWeight;
            average.fov /= totalWeight;
        }
        else
        {
            Debug.LogWarning("Total weight is zero, returning default camera configuration.");
            return new CameraConfiguration();  // Retourner une configuration par défaut si pas de vues actives
        }

        // Vérification des NaN pour éviter les erreurs de calcul
        if (float.IsNaN(average.yaw) || float.IsNaN(average.pitch) || float.IsNaN(average.roll) || float.IsNaN(average.distance))
        {
            Debug.LogError("Invalid camera configuration detected (NaN values). Returning default configuration.");
            return new CameraConfiguration();  // Retourner une configuration par défaut si NaN détecté
        }

        return average;
    }
}