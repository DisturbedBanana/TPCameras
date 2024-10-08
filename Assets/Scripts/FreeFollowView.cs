using UnityEngine;

public class FreeFollowView : AView
{
    [Header("Free Follow View Parameters")]
    [Range(0f, 179f)]
    public float[] pitch = new float[3]; 
    [Range(-180f, 180f)]
    public float yaw;
    [Range(-180f, 180f)]
    public float yawSpeed;

    [Range(-180f, 180f)]
    public float[] roll = new float[3]; 

    [Range(0f, 179f)]
    public float[] fov = new float[3]; 

    public Transform target; 
    public Curve curve; 
    [Range(0f, 1f)]
    public float curvePosition = 0f; 
    public float curveSpeed = 0.5f; 

    private void Update()
    {
        if (target == null || curve == null)
        {
            Debug.LogWarning("FreeFollowView: Target or Curve is not assigned.");
            return;
        }

        // Handle player input for yaw
        float inputYaw = Input.GetAxis("Horizontal"); // Assuming "Horizontal" axis controls yaw
        yaw += inputYaw * yawSpeed * Time.deltaTime;

        // Handle player input for curve position
        //float inputCurve = Input.GetAxis("Vertical"); // Assuming "Vertical" axis controls curve position
        //curvePosition += inputCurve * curveSpeed * Time.deltaTime;
        //curvePosition = Mathf.Clamp01(curvePosition);

        // Calculate the transformation matrix based on yaw and target position
        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 targetPosition = target.position;
        Matrix4x4 curveToWorldMatrix = Matrix4x4.TRS(targetPosition, yawRotation, Vector3.one);

        // Get the position on the curve
        Vector3 curvePoint = curve.GetPosition(curvePosition, curveToWorldMatrix);

        // Determine which segment of the curve to use for interpolation
        // Assuming bottom (t=0), middle (t=0.5), top (t=1)
        float t = curvePosition;

        // Interpolate camera configuration based on curve position
        float interpolatedPitch = Mathf.Lerp(
            pitch[0], // Bottom
            Mathf.Lerp(pitch[0], pitch[1], 2f * t), // Between bottom and middle
            t < 0.5f ? 2f * t : 1f
        );

        float interpolatedRoll = Mathf.Lerp(
            roll[0], // Bottom
            Mathf.Lerp(roll[0], roll[1], 2f * t), // Between bottom and middle
            t < 0.5f ? 2f * t : 1f
        );

        float interpolatedFov = Mathf.Lerp(
            fov[0], // Bottom
            Mathf.Lerp(fov[0], fov[1], 2f * t), // Between bottom and middle
            t < 0.5f ? 2f * t : 1f
        );

        // You can extend this interpolation to handle top position as well
        // For simplicity, let's assume we only interpolate between bottom and middle

        // Create and set the camera configuration
        CameraConfiguration config = new CameraConfiguration
        {
            yaw = yaw,
            pitch = interpolatedPitch,
            roll = interpolatedRoll,
            fov = interpolatedFov,
            pivot = curvePoint,
            distance = 0f // Adjust if needed
        };

    }

    // Override to provide this view's camera configuration
    public override CameraConfiguration GetConfiguration()
    {
        return new CameraConfiguration
        {
            yaw = yaw,
            pitch = pitch.Length > 0 ? pitch[1] : 0f, // Middle pitch as default
            roll = roll.Length > 0 ? roll[1] : 0f, // Middle roll as default
            fov = fov.Length > 0 ? fov[1] : 60f, // Middle fov as default
            pivot = curve.GetPosition(curvePosition, Matrix4x4.identity), // Position on curve
            distance = 0f // Adjust if needed
        };
    }
}
