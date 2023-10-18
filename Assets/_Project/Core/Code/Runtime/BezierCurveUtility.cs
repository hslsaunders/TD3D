using UnityEngine;

namespace TD3D.Core.Runtime {
    public static class BezierCurveUtility {
        public static Vector3 EvaluateQuadratic(Vector3 a1, Vector3 cp1, Vector3 a2, float t) {
            Vector3 p0 = Vector3.Lerp(a1, cp1, t);
            Vector3 p1 = Vector3.Lerp(cp1, a2, t);
            return Vector3.Lerp(p0, p1, t);
        }
        
        public static Vector3 EvaluateCubic(Vector3 a1, Vector3 cp1, Vector3 cp2, Vector3 a2, float t) {
            Vector3 p0 = EvaluateQuadratic(a1, cp1, cp2, t);
            Vector3 p1 = EvaluateQuadratic(cp1, cp2, a2, t);
            return Vector3.Lerp(p0, p1, t);
        }

        public static Vector3 EvaluateCubicDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            return 3f * Mathf.Pow(1f - t, 3) * (p1 - p0) + 6f * (1 - t) * t * (p2 - p1) + 3f * Mathf.Pow(t, 2) * (p3 - p2);
        }
        
        public static float ApproximateCurveLength (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            float controlNetLength = (p0 - p1).magnitude + (p1 - p2).magnitude + (p2 - p3).magnitude;
            float estimatedCurveLength = (p0 - p3).magnitude + controlNetLength / 2f;
            return estimatedCurveLength;
        }
    }
}