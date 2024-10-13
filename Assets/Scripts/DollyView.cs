using UnityEngine;

public class DollyView : AView
{
    [Header("Dolly View Parameters")]
    [Range(-180f, 180f)]
    public float roll;

    [Range(0f, 179f)]
    public float fov;

    public Transform target; 

    [Header("Rail Settings")]
    public Rail rail;
    public float distanceOnRail = 0f; 
    public float speed = 5f; 

    [Header("Automatic Follow Settings")]
    public bool isAuto = false; 

    private void Update()
    {
        if (rail == null)
        {
            Debug.LogWarning("DollyView: Rail is not assigned.");
            return;
        }

        if (isAuto && target != null)
        {
            distanceOnRail = FindNearestPointOnRail(target.position);
        }
        else
        {
            float input = Input.GetAxis("Horizontal"); 
            distanceOnRail += input * speed * Time.deltaTime;

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
                float segmentLength = Vector3.Distance(a, b);
                float distanceToNearestPoint = Vector3.Distance(a, nearestPoint);
                nearestDistance = rail.GetLength() + distanceToNearestPoint;
            }
        }

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

    public override CameraConfiguration GetConfiguration()
    {
        Vector3 railPosition = rail.GetPosition(distanceOnRail);

        Vector3 direction = (target.position - railPosition).normalized;

        float calculatedYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float calculatedPitch = -Mathf.Asin(Mathf.Clamp(direction.y, -1f, 1f)) * Mathf.Rad2Deg;

        return new CameraConfiguration
        {
            yaw = calculatedYaw,
            pitch = calculatedPitch,
            roll = roll,
            fov = fov,
            pivot = railPosition,
            distance = 0f 
        };
    }
}
