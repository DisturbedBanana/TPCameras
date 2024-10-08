using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class AView : MonoBehaviour
{
    [Header("View Settings")]
    public float weight = 1.0f; // Weight for interpolation
    public bool isActiveOnStart = true; // Determines if the view is active at the start

    protected virtual void Start()
    {
        if (isActiveOnStart)
        {
            SetActive(true);
        }
    }

    // Activate or deactivate the view
    public void SetActive(bool isActive)
    {
        if (isActive)
            CameraController.Instance.AddView(this);
        else
            CameraController.Instance.RemoveView(this);
    }

    // Abstract method to get the camera configuration from the view
    public abstract CameraConfiguration GetConfiguration();
}