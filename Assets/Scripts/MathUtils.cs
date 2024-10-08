using UnityEngine;

public static class MathUtils
{
    // Returns the nearest point on segment AB to the target point
    public static Vector3 GetNearestPointOnSegment(Vector3 a, Vector3 b, Vector3 target)
    {
        Vector3 ab = b - a;
        float abLengthSquared = ab.sqrMagnitude;
        if (abLengthSquared == 0f)
            return a;

        float t = Vector3.Dot(target - a, ab) / abLengthSquared;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }

    // Linear Bezier interpolation between A and B
    public static Vector3 LinearBezier(Vector3 A, Vector3 B, float t)
    {
        return Vector3.Lerp(A, B, t);
    }

    // Quadratic Bezier interpolation between A, B, and C
    public static Vector3 QuadraticBezier(Vector3 A, Vector3 B, Vector3 C, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(A, B, t), Vector3.Lerp(B, C, t), t);
    }

    // Cubic Bezier interpolation between A, B, C, and D
    public static Vector3 CubicBezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        return Vector3.Lerp(
            Vector3.Lerp(LinearBezier(A, B, t), LinearBezier(B, C, t), t),
            Vector3.Lerp(LinearBezier(B, C, t), LinearBezier(C, D, t), t),
            t
        );
    }
}

