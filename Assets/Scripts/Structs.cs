using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public struct CameraConfiguration
    {
        [Range(-180, 180)]
        public float yaw;
        [Range(-90, 90)]
        public float pitch;
        [Range(-180, 180)]
        public float roll;
        public Vector3 pivot;
        public float distance;
        [Range(0, 179)]
        public float fov;

        public Quaternion GetRotation() => Quaternion.Euler(pitch, yaw, roll);

        public Vector3 GetPosition()
        {
            Vector3 direction = GetRotation() * Vector3.back;
            return pivot + direction * distance;
        }

        public void DrawGizmos(Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(pivot, 0.25f);
            Vector3 position = GetPosition();
            Gizmos.DrawLine(pivot, position);
            Gizmos.matrix = Matrix4x4.TRS(position, GetRotation(), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, fov, 0.5f, 0f, Camera.main.aspect);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

