using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    // Singleton Instance
    public static CameraController Instance { get; private set; }

    [Header("Camera Settings")]
    public Camera camera; // The camera to control

    private CameraConfiguration configuration; // Current configuration applied to the camera
    private CameraConfiguration targetConfiguration; // Target configuration for smoothing

    [Header("Smoothing Settings")]
    [Range(0f, 1f)]
    public float smoothingSpeed = 0.1f; // Determines how quickly the camera transitions to the target configuration

    private List<AView> activeViews = new List<AView>(); // List of active views

    private void Awake()
    {
        // Implement Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if needed
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize configurations
        configuration = ComputeAverage();
        targetConfiguration = configuration;
    }

    private void Update()
    {
        // Compute the target configuration based on active views
        targetConfiguration = ComputeAverage();

        // Smoothly interpolate between current and target configurations
        SmoothConfiguration();

        // Apply the smoothed configuration to the camera
        ApplyConfiguration();
    }

    // Smoothly interpolate between current and target configurations
    private void SmoothConfiguration()
    {
        configuration.yaw = Mathf.LerpAngle(configuration.yaw, targetConfiguration.yaw, smoothingSpeed);
        configuration.pitch = Mathf.Lerp(configuration.pitch, targetConfiguration.pitch, smoothingSpeed);
        configuration.roll = Mathf.Lerp(configuration.roll, targetConfiguration.roll, smoothingSpeed);
        configuration.pivot = Vector3.Lerp(configuration.pivot, targetConfiguration.pivot, smoothingSpeed);
        configuration.distance = Mathf.Lerp(configuration.distance, targetConfiguration.distance, smoothingSpeed);
        configuration.fov = Mathf.Lerp(configuration.fov, targetConfiguration.fov, smoothingSpeed);
    }

    // Apply the current configuration to the camera
    private void ApplyConfiguration()
    {
        if (camera != null)
        {
            camera.transform.position = configuration.GetPosition();
            camera.transform.rotation = configuration.GetRotation();
        }
    }

    // Add a view to the activeViews list
    public void AddView(AView view)
    {
        if (!activeViews.Contains(view))
            activeViews.Add(view);
    }

    // Remove a view from the activeViews list
    public void RemoveView(AView view)
    {
        if (activeViews.Contains(view))
            activeViews.Remove(view);
    }

    // Compute the weighted average of all active views
    public CameraConfiguration ComputeAverage()
    {
        if (activeViews.Count == 0)
            return configuration; // Return current configuration if no active views

        CameraConfiguration average = new CameraConfiguration();
        Vector2 yawSum = Vector2.zero;
        float pitchSum = 0f;
        float rollSum = 0f;
        float distanceSum = 0f;
        float fovSum = 0f;
        float totalWeight = 0f;
        Vector3 pivotSum = Vector3.zero;

        foreach (AView view in activeViews)
        {
            CameraConfiguration config = view.GetConfiguration();
            float weight = view.weight;

            // Sum up the yaw using vector representation to handle angle wrapping
            yawSum += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad), Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * weight;

            // Sum other parameters weighted by view weight
            pitchSum += config.pitch * weight;
            rollSum += config.roll * weight;
            distanceSum += config.distance * weight;
            fovSum += config.fov * weight;
            pivotSum += config.pivot * weight;
            totalWeight += weight;
        }

        // Calculate the average yaw from the summed vectors
        average.yaw = Mathf.Atan2(yawSum.y, yawSum.x) * Mathf.Rad2Deg;

        // Calculate average for other parameters
        average.pitch = pitchSum / totalWeight;
        average.roll = rollSum / totalWeight;
        average.distance = distanceSum / totalWeight;
        average.fov = fovSum / totalWeight;
        average.pivot = pivotSum / totalWeight;

        return average;
    }

    // Draw Gizmos for the current configuration
    private void OnDrawGizmos()
    {
        if (camera != null)
        {
            configuration.DrawGizmos(Color.blue);
        }
    }
}