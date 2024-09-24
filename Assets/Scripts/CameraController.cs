using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{   
    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null )
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    public Camera camera;
    public Structs.CameraConfiguration configuration;

    private void ApplyConfiguration()
    {
        camera.transform.position = configuration.GetPosition();
        camera.transform.rotation = configuration.GetRotation();
    }

    private void Update()
    {
        ApplyConfiguration();
    }
}
