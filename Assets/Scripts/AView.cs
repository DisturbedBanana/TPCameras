using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class AView : MonoBehaviour
{
    [Header("View Settings")]
    public float weight = 1.0f;

    protected virtual void Start()
    {
   
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
            CameraController.Instance.AddView(this);
        else
            CameraController.Instance.RemoveView(this);
    }

    public abstract CameraConfiguration GetConfiguration();



}