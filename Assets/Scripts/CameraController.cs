using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Camera Settings")]
    public Camera camera; 

    private CameraConfiguration _configuration; 
    private CameraConfiguration _targetConfiguration; 

    [Header("Smoothing Settings")]
    [Range(0f, 1f)]
    public float smoothingSpeed = 0.1f; 

    private List<AView> _activeViews = new List<AView>();
    private bool _isCutRequested = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _configuration = ComputeAverage();
        _targetConfiguration = _configuration;
    }

    public void Cut()
    {
        _isCutRequested = true;
    }

    private void Update()
    {
        _targetConfiguration = ComputeAverage();

        if (_isCutRequested)
        {
            ApplyConfiguration();
            _isCutRequested = false;
        }
        SmoothConfiguration();

        ApplyConfiguration();
    }

    private void SmoothConfiguration()
    {
        _configuration.yaw = Mathf.LerpAngle(_configuration.yaw, _targetConfiguration.yaw, smoothingSpeed);
        _configuration.pitch = Mathf.Lerp(_configuration.pitch, _targetConfiguration.pitch, smoothingSpeed);
        _configuration.roll = Mathf.Lerp(_configuration.roll, _targetConfiguration.roll, smoothingSpeed);
        _configuration.pivot = Vector3.Lerp(_configuration.pivot, _targetConfiguration.pivot, smoothingSpeed);
        _configuration.distance = Mathf.Lerp(_configuration.distance, _targetConfiguration.distance, smoothingSpeed);
        _configuration.fov = Mathf.Lerp(_configuration.fov, _targetConfiguration.fov, smoothingSpeed);
    }

    private void ApplyConfiguration()
    {
        if (camera != null)
        {
            camera.transform.position = _configuration.GetPosition();
            camera.transform.rotation = _configuration.GetRotation();
        }
    }

    public void AddView(AView view)
    {
        if (!_activeViews.Contains(view))
            _activeViews.Add(view);
    }

    public void RemoveView(AView view)
    {
        if (_activeViews.Contains(view))
            _activeViews.Remove(view);
    }

    public CameraConfiguration ComputeAverage()
    {
        if (_activeViews.Count == 0)
            return _configuration; 

        CameraConfiguration average = new CameraConfiguration();
        Vector2 yawSum = Vector2.zero;
        float pitchSum = 0f;
        float rollSum = 0f;
        float distanceSum = 0f;
        float fovSum = 0f;
        float totalWeight = 0f;
        Vector3 pivotSum = Vector3.zero;

        foreach (AView view in _activeViews)
        {
            CameraConfiguration config = view.GetConfiguration();
            float weight = view.weight;

            yawSum += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad), Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * weight;

            pitchSum += config.pitch * weight;
            rollSum += config.roll * weight;
            distanceSum += config.distance * weight;
            fovSum += config.fov * weight;
            pivotSum += config.pivot * weight;
            totalWeight += weight;
        }

        average.yaw = Mathf.Atan2(yawSum.y, yawSum.x) * Mathf.Rad2Deg;

        average.pitch = pitchSum / totalWeight;
        average.roll = rollSum / totalWeight;
        average.distance = distanceSum / totalWeight;
        average.fov = fovSum / totalWeight;
        average.pivot = pivotSum / totalWeight;

        return average;
    }

    private void OnDrawGizmos()
    {
        if (camera != null)
        {
            _configuration.DrawGizmos(Color.blue);
        }
    }
}