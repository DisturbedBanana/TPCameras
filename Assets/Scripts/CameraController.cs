using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
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


    public void OnDrawGizmos()
    {
        configuration.DrawGizmos(Color.red);
    }

    public new Camera camera;
    [SerializeField] private CameraConfiguration configuration;

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
        
    }

  CameraConfiguration ComputeAverage()
    {
        CameraConfiguration average = new CameraConfiguration();
        CameraConfiguration config = new CameraConfiguration();
        Vector2 Sum = new Vector2(0, 0);
        float totalWeight = 0;

        foreach (AView view in activeViews)
        {
            totalWeight += view.weight;
            average.pitch += view.GetCameraConfiguration().pitch * view.weight;
            Sum += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad),Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * view.weight;
            average.roll += view.GetCameraConfiguration().roll * view.weight;
            average.pivot += view.GetCameraConfiguration().pivot * view.weight;
            average.distance += view.GetCameraConfiguration().distance * view.weight;
            average.fov += view.GetCameraConfiguration().fov * view.weight;
        }

        average.pitch /= totalWeight;
        average.yaw = Vector2.SignedAngle(Vector2.right, Sum);
        average.roll /= totalWeight;
        average.pivot /= totalWeight;
        average.distance /= totalWeight;
        average.fov /= totalWeight;


        return average;
    }
}
