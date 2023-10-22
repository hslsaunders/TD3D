using UnityEngine;

namespace TD3D.Core.Runtime {
    public static class BezierCurveUtility {
        public static Vector3 EvaluateQuadratic(Vector3 a1, Vector3 cp1, Vector3 a2, float t) {
            Vector3 p0 = Vector3.Lerp(a1, cp1, t);
            Vector3 p1 = Vector3.Lerp(cp1, a2, t);
            return Vector3.Lerp(p0, p1, t);
        }
        
        public static Vector3 EvaluateCubic(Vector3 a1, Vector3 cp1, Vector3 cp2, Vector3 a2, float t) {
            //return p3 * (t * t * t) - p2 * (3f * (t - 1) * (t * t)) + p1 * (3f * Mathf.Pow(t - 1, 2) * t) - p0 * Mathf.Pow(t - 1, 3);
            Vector3 p0 = EvaluateQuadratic(a1, cp1, cp2, t);
            Vector3 p1 = EvaluateQuadratic(cp1, cp2, a2, t);
            return Vector3.Lerp(p0, p1, t);
            
        }

        public static Vector3 EvaluateCubicDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            //return (3 * (1 - t) * (1 - t) * (p1 - p0) + 6 * (1 - t) * t * (p2 - p1) + 3 * t * t * (p3 - p2)).normalized;
            return (3f * Mathf.Pow(1f - t, 2) * (p1 - p0) + 6f * (1 - t) * t * (p2 - p1) + 3f * Mathf.Pow(t, 2) * (p3 - p2)).normalized;
            //return 3f * ((p3 - 3f * p2 + 3f * p1 - p0) * (t * t) + (2f * p2 - 4f * p1 + 2f * p0) * t + p1 - p0);
        }
        
        public static Vector3 EvaluateSecondCubicDerivative (Vector3 a1, Vector3 c1, Vector3 c2, Vector3 a2, float t) {
            t = Mathf.Clamp01 (t);
            return 6f * (1f - t) * (c2 - 2f * c1 + a1) + 6f * t * (a2 - 2f * c2 + c1);
        }

        //https://en.wikipedia.org/wiki/Frenet–Serret_formulas
        public static Vector3 EvaluateFrenetBinormal(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            Vector3 a = EvaluateCubicDerivative(p0, p1, p2, p3, t).normalized;
            Vector3 b = (a + EvaluateSecondCubicDerivative(p0, p1, p2, p3, t)).normalized;
            Vector3 r = Vector3.Cross(b, a).normalized;
            return Vector3.Cross(r, a).normalized;
            /*
            Vector3 derivative = EvaluateCubicDerivative(p0, p1, p2, p3, t);
            Vector3 secondDerivative = EvaluateSecondCubicDerivative(p0, p1, p2, p3, t);
            Vector3 numerator = Vector3.Cross(derivative, secondDerivative);
            float length = numerator.magnitude;

            return numerator / numerator.magnitude;
            */
        }

        public static Vector3 GetRotationalAxis(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            Vector3 a = EvaluateCubicDerivative(p0, p1, p2, p3, t).normalized;
            Vector3 b = (a + EvaluateSecondCubicDerivative(p0, p1, p2, p3, t)).normalized;
            return Vector3.Cross(b, a).normalized;
        }
        
        public static float ApproximateCurveLength (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            float controlNetLength = (p0 - p1).magnitude + (p1 - p2).magnitude + (p2 - p3).magnitude;
            float estimatedCurveLength = (p0 - p3).magnitude + controlNetLength / 2f;
            return estimatedCurveLength;
        }
    }
}