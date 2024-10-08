using UnityEngine;

public class Curve : MonoBehaviour
{
    public Vector3 A, B, C, D; // Control points

    // Get position on the curve based on parameter t
    public Vector3 GetPosition(float t)
    {
        return MathUtils.CubicBezier(A, B, C, D, t);
    }

    // Get world position on the curve based on parameter t and a transformation matrix
    public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix)
    {
        Vector3 localPos = GetPosition(t);
        return localToWorldMatrix.MultiplyPoint(localPos);
    }

    // Draw the curve using Gizmos
    public void DrawGizmo(Color color, Matrix4x4 localToWorldMatrix)
    {
        Gizmos.color = color;

        // Transform control points to world space
        Vector3 A_world = localToWorldMatrix.MultiplyPoint(A);
        Vector3 B_world = localToWorldMatrix.MultiplyPoint(B);
        Vector3 C_world = localToWorldMatrix.MultiplyPoint(C);
        Vector3 D_world = localToWorldMatrix.MultiplyPoint(D);

        // Draw control points
        Gizmos.DrawSphere(A_world, 0.1f);
        Gizmos.DrawSphere(B_world, 0.1f);
        Gizmos.DrawSphere(C_world, 0.1f);
        Gizmos.DrawSphere(D_world, 0.1f);

        // Draw lines between control points
        Gizmos.DrawLine(A_world, B_world);
        Gizmos.DrawLine(B_world, C_world);
        Gizmos.DrawLine(C_world, D_world);

        // Draw the Bezier curve
        Vector3 previousPoint = GetPosition(0f, localToWorldMatrix);
        int steps = 20;
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 currentPoint = GetPosition(t, localToWorldMatrix);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}

