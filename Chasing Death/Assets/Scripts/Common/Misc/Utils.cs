using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common.Misc {
    public class Utils {

        // CCW < 0 ~ clockwise
        public static float CCW (Vector2 p1, Vector2 p2, Vector2 p3) {
            return (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y)
                * (p3.x - p1.x);
        }

        public static Vector2 Radian2Vector2(float radian) {
            return new Vector2 (Mathf.Cos (radian), Mathf.Sin (radian));
        }

        public static Vector2 Degree2Vector2(float degree) {
            return Radian2Vector2 (degree * Mathf.Deg2Rad);
        }

        //Find delta angle of 2 rotations in degree
        //Ret < 0 ~ clockwise
        public static float RotationDeltaAngle (Quaternion from, Quaternion to) {
            return DeltaAngle (from.eulerAngles.z, to.eulerAngles.z);
        }

        public static float AbsRotationDeltaAngle (Quaternion from, Quaternion to) {
            return AbsDeltaAngle (from.eulerAngles.z, to.eulerAngles.z);
        }

        public static float Vector2Angle (Vector2 vector) {
            if (vector.y < 0) 
                return  360 - Vector2.Angle (vector, Vector2.right);
            else
                return Vector2.Angle (vector, Vector2.right);
        }

        public static float AbsDeltaAngle (float from, float to) {
            return Mathf.Abs (Mathf.DeltaAngle (from, to));
        }

        // DeltaAngle < 0 ~ clockwise
        public static float DeltaAngle (float from, float to) {
            return Mathf.DeltaAngle (from, to);
        }

        public static float Rand01f () {
            return UnityEngine.Random.Range (0f, 1f);
        }
    }
}
