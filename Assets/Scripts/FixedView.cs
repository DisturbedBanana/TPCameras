using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedView : AView
{
    [Range(0, 360)]
    public float yaw;
    [Range(-90, 90)]
    public float pitch;
    [Range(-180, 180)]
    public float roll;
    [Range(0, 179)]
    public float fov;

    public override CameraConfiguration GetCameraConfiguration()
    {
        return new CameraConfiguration()
        {
            pivot = transform.position,
            distance = 0,
            yaw = yaw,
            pitch = pitch,
            roll = roll,
            fov = fov

        };
    }

}
