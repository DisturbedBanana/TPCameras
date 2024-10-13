using UnityEngine;

public class FixedView : AView
{
    [Header("Fixed View Parameters")]
    [Range(-180f, 180f)]
    public float yaw;

    [Range(-90f, 90f)]
    public float pitch;

    [Range(-180f, 180f)]
    public float roll;

    [Range(0f, 179f)]
    public float fov;

    public override CameraConfiguration GetConfiguration()
    {
        return new CameraConfiguration
        {
            yaw = yaw,
            pitch = pitch,
            roll = roll,
            fov = fov,
            pivot = transform.position,
            distance = 0f 
        };
    }
}