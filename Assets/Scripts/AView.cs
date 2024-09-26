using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AView : MonoBehaviour
{
    public float weight;
    public bool isActiveOnStart;
    public virtual CameraConfiguration GetCameraConfiguration()
    {
        return new CameraConfiguration();
    }
    public void OnDrawGizmos()
    {
        GetCameraConfiguration().DrawGizmos(Color.blue);
    }

    private void Start()
    {
        if(isActiveOnStart)
        {
            SetActive(true);
        }
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            CameraController.Instance.AddView(this);
        }
        else
        {
            CameraController.Instance.RemoveView(this);
        }
    }

}
