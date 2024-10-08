using UnityEngine;

public class Rail : MonoBehaviour
{
    [Header("Rail Settings")]
    public bool isLoop = false; // Determines if the rail is a loop

    private float length = 0f; // Total length of the rail
    private Vector3[] nodes; // Positions of the rail nodes

    private void Start()
    {
        InitializeRail();
    }

    // Initialize the rail by calculating the total length and storing node positions
    private void InitializeRail()
    {
        int childCount = transform.childCount;
        nodes = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            nodes[i] = transform.GetChild(i).position;
        }

        // Calculate total length
        length = 0f;
        for (int i = 0; i < childCount - 1; i++)
        {
            length += Vector3.Distance(nodes[i], nodes[i + 1]);
        }

        if (isLoop && childCount > 1)
        {
            length += Vector3.Distance(nodes[childCount - 1], nodes[0]);
        }
    }

    // Get the total length of the rail
    public float GetLength()
    {
        return length;
    }

    // Get a position on the rail based on distance from the first node
    public Vector3 GetPosition(float distance)
    {
        if (nodes.Length == 0)
            return Vector3.zero;

        // Handle looping
        if (isLoop)
        {
            distance %= length;
        }
        else
        {
            distance = Mathf.Clamp(distance, 0f, length);
        }

        float accumulatedDistance = 0f;
        int segmentCount = nodes.Length;
        for (int i = 0; i < segmentCount; i++)
        {
            int nextIndex = (i + 1) % segmentCount;
            Vector3 start = nodes[i];
            Vector3 end = nodes[nextIndex];
            float segmentLength = Vector3.Distance(start, end);

            if (accumulatedDistance + segmentLength >= distance)
            {
                float remainingDistance = distance - accumulatedDistance;
                return Vector3.Lerp(start, end, remainingDistance / segmentLength);
            }

            accumulatedDistance += segmentLength;
        }

        return nodes[nodes.Length - 1];
    }

    // Draw Gizmos to visualize the rail
    private void OnDrawGizmos()
    {
        Gizmos.color = isLoop ? Color.green : Color.yellow;
        int childCount = transform.childCount;
        if (childCount < 2)
            return;

        Vector3 previous = transform.GetChild(0).position;
        for (int i = 1; i < childCount; i++)
        {
            Vector3 current = transform.GetChild(i).position;
            Gizmos.DrawLine(previous, current);
            previous = current;
        }

        if (isLoop)
        {
            Gizmos.DrawLine(previous, transform.GetChild(0).position);
        }
    }
}

