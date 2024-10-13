using UnityEngine;

public class FixedFollowView : AView
{
    public float roll;
    public float fov;
    public Transform target;
    public Transform centralPoint;
    public float yawOffsetMax = 45.0f;
    public float pitchOffsetMax = 30.0f;

    public override CameraConfiguration GetConfiguration()
    {
        Vector3 direction = (target.position - transform.position).normalized;

        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;

        Vector3 centralDir = (centralPoint.position - transform.position).normalized;
        float centralYaw = Mathf.Atan2(centralDir.x, centralDir.z) * Mathf.Rad2Deg;
        float centralPitch = -Mathf.Asin(centralDir.y) * Mathf.Rad2Deg;

        yaw = Mathf.Clamp(yaw, centralYaw - yawOffsetMax, centralYaw + yawOffsetMax);
        pitch = Mathf.Clamp(pitch, centralPitch - pitchOffsetMax, centralPitch + pitchOffsetMax);

        CameraConfiguration config = new CameraConfiguration();
        config.yaw = yaw;
        config.pitch = pitch;
        config.roll = roll;
        config.pivot = transform.position;
        config.distance = 0; 
        config.fov = fov;

        return config;
    }
}
