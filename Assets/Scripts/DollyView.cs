using UnityEngine;

public class DollyView : AView
{
    [Header("Dolly View Parameters")]
    [Range(-180f, 180f)]
    public float roll;

    [Range(0f, 179f)]
    public float fov;

    public Transform target; // Target to follow

    [Header("Rail Settings")]
    public Rail rail; // Rail to move along
    public float distanceOnRail = 0f; // Current distance along the rail
    public float speed = 5f; // Speed of manual movement along the rail

    [Header("Automatic Follow Settings")]
    public bool isAuto = false; // Determines if the dolly follows automatically

    private void Update()
    {
        if (rail == null)
        {
            Debug.LogWarning("DollyView: Rail is not assigned.");
            return;
        }

        if (isAuto && target != null)
        {
            // Find the nearest point on the rail to the target
            distanceOnRail = FindNearestPointOnRail(target.position);
        }
        else
        {
            // Player input to control movement along the rail
            float input = Input.GetAxis("Horizontal"); // Assuming "Horizontal" axis controls dolly movement
            distanceOnRail += input * speed * Time.deltaTime;

            // Clamp or loop distance based on rail settings
            if (rail.isLoop)
            {
                distanceOnRail %= rail.GetLength();
                if (distanceOnRail < 0f)
                    distanceOnRail += rail.GetLength();
            }
            else
            {
                distanceOnRail = Mathf.Clamp(distanceOnRail, 0f, rail.GetLength());
            }
        }
    }

    // Find the nearest point on the rail to the target position
    private float FindNearestPointOnRail(Vector3 targetPosition)
    {
        float nearestDistance = 0f;
        float smallestDistance = Mathf.Infinity;

        int nodeCount = rail.transform.childCount;
        if (nodeCount < 2)
            return distanceOnRail;

        for (int i = 0; i < nodeCount; i++)
        {
            int nextIndex = (i + 1 < nodeCount) ? i + 1 : (rail.isLoop ? 0 : -1);
            if (nextIndex == -1)
                break;

            Vector3 a = rail.transform.GetChild(i).position;
            Vector3 b = rail.transform.GetChild(nextIndex).position;

            Vector3 nearestPoint = MathUtils.GetNearestPointOnSegment(a, b, targetPosition);
            float distance = Vector3.Distance(nearestPoint, targetPosition);

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                // Calculate distance along the rail up to this segment
                float segmentLength = Vector3.Distance(a, b);
                float distanceToNearestPoint = Vector3.Distance(a, nearestPoint);
                nearestDistance = rail.GetLength() + distanceToNearestPoint;
            }
        }

        // Clamp or loop the nearestDistance
        if (rail.isLoop)
        {
            nearestDistance %= rail.GetLength();
            if (nearestDistance < 0f)
                nearestDistance += rail.GetLength();
        }
        else
        {
            nearestDistance = Mathf.Clamp(nearestDistance, 0f, rail.GetLength());
        }

        return nearestDistance;
    }

    // Override to provide this view's camera configuration based on rail position
    public override CameraConfiguration GetConfiguration()
    {
        // Get position on rail
        Vector3 railPosition = rail.GetPosition(distanceOnRail);

        // Calculate direction towards target
        Vector3 direction = (target.position - railPosition).normalized;

        // Calculate yaw and pitch
        float calculatedYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float calculatedPitch = -Mathf.Asin(Mathf.Clamp(direction.y, -1f, 1f)) * Mathf.Rad2Deg;

        // Create and return the camera configuration
        return new CameraConfiguration
        {
            yaw = calculatedYaw,
            pitch = calculatedPitch,
            roll = roll,
            fov = fov,
            pivot = railPosition,
            distance = 0f // Fixed position on rail; adjust if needed
        };
    }
}
