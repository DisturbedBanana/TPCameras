using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static System.TimeZoneInfo;

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


    public void OnDrawGizmos()
    {
        configuration.DrawGizmos(Color.red);
    }

    public new Camera camera;
    [SerializeField] private CameraConfiguration configuration;
    private CameraConfiguration currentConfiguration;
    private bool isTransitioning = false;
    float elapsedTime = 0;
    public float transitionTime = 1;


    private List<AView> activeViews = new List<AView>();


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
        camera.transform.position = configuration.GetPosition();
        camera.transform.rotation = configuration.GetRotation();
    }

    private void Update()
    {
        configuration = ComputeAverage();
        ApplyConfiguration();

        if (isTransitioning)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / transitionTime);

            currentConfiguration = SmoothedCameraMovement(currentConfiguration, ComputeAverage(), t);

            ApplyConfiguration();

            if (elapsedTime >= transitionTime)
            {
                isTransitioning = false;
            }
        }
    }

    CameraConfiguration ComputeAverage()
    {
        CameraConfiguration average = new CameraConfiguration();
        Vector2 Sum = new Vector2(0, 0);
        float totalWeight = 0;

        foreach (AView view in activeViews)
        {
            CameraConfiguration config = view.GetCameraConfiguration(); // Fetch the view's camera config

            totalWeight += view.weight;
            average.pitch += config.pitch * view.weight;
            Sum += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad), Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * view.weight;
            average.roll += config.roll * view.weight;
            average.pivot += config.pivot * view.weight;
            average.distance += config.distance * view.weight;
            average.fov += config.fov * view.weight;
        }

        average.pitch /= totalWeight;
        average.yaw = Vector2.SignedAngle(Vector2.right, Sum);
        average.roll /= totalWeight;
        average.pivot /= totalWeight;
        average.distance /= totalWeight;
        average.fov /= totalWeight;

        return average;
    }


    CameraConfiguration SmoothedCameraMovement(CameraConfiguration startConfig,CameraConfiguration destinationConfig,float t)
    {
        CameraConfiguration result = new CameraConfiguration
        {
            yaw = Mathf.Lerp(startConfig.yaw, destinationConfig.yaw, t),
            pitch = Mathf.Lerp(startConfig.pitch, destinationConfig.pitch, t),
            roll = Mathf.Lerp(startConfig.roll, destinationConfig.roll, t),
            pivot = Vector3.Lerp(startConfig.pivot, destinationConfig.pivot, t),
            distance = Mathf.Lerp(startConfig.distance, destinationConfig.distance, t),
            fov = Mathf.Lerp(startConfig.fov, destinationConfig.fov, t)
        };

        return result;
    }


}
