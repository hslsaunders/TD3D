using System;
using System.Collections.Generic;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime {
    public sealed class BezierCurve {
        public ControlPoint start;
        public ControlPoint end;
        public readonly List<ControlPoint> controlPoints = new();

        public BezierCurve(Vector3 start, Vector3 end, List<Vector3> controlPoints = null) {
            this.start = new ControlPoint(start);
            this.end = new ControlPoint(end);
            
            if (controlPoints == null) return;
            foreach (var point in controlPoints) {
                this.controlPoints.Add(new ControlPoint(point));
            }
        }
        
        
        private Vector3 SumControlPoints(float t)
        {
            Vector3 sum = Vector3.zero;
            int numPoints = controlPoints.Count;

            sum += Mathf.Pow(1 - t, numPoints + 1) * start.weight * start.point; 
            sum += Mathf.Pow(t, numPoints + 1) * end.weight * end.point;

            int index = 0;
            
            for (; index < numPoints; index++)
            {
                ControlPoint point = controlPoints[index];
                sum += (numPoints + 1) * Mathf.Pow(1 - t, numPoints - index) * Mathf.Pow(t, index + 1)
                       * point.weight * point.point;
            }
            
            return sum;
        }
        
        public Vector3 EvaluateCurvePoint(float t) {
            t = Mathf.Clamp(t, 0f, 1f);
            Vector3 vectorSum = SumControlPoints(t);
            int numPoints = controlPoints.Count;

            float weightDivisor = 0f;
            weightDivisor += start.weight * Mathf.Pow(1 - t, numPoints + 1);
            weightDivisor += end.weight * Mathf.Pow(t, numPoints + 1);

            for (int index = 0; index < numPoints; index++)
                weightDivisor += (numPoints + 1) 
                                 * Mathf.Pow(1 - t, numPoints - index) * Mathf.Pow(t, index + 1)
                                 * controlPoints[index].weight;

            if (Math.Abs(weightDivisor) < .0001f)
                return start.point;
            return vectorSum / weightDivisor;
        }
        
        
        public Vector3 EvaluateCurveTangent(float t) {
            Vector3 sample1 = EvaluateCurvePoint(t);
            Vector3 sample2 = EvaluateCurvePoint(t + .0001f);
            return (sample2 - sample1).normalized;

            /*
            t = Mathf.Clamp(t, 0f, 1f);
            int n = controlPoints.Count;

            Vector3 tangent = Vector3.zero;
            //tangent += EvaluateTangentValue(start.point, start.weight, n, t, 0);
            //tangent += EvaluateTangentValue(end.point, end.weight, n, t, n);

            for (int i = 0; i < n; i++) {
                var cp = controlPoints[i];
                tangent += EvaluateTangentValue(cp.point, cp.weight, n, t, i);
            }

            return tangent.normalized;
            */
        }

        private Vector3 EvaluateTangentValue(Vector3 p, float w, int n, float t, int i) {
            return (p * (w * (n + 1) * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i) * ((n + 1) * t - i - 1))) / (t - 1);
        }
    }

    [Serializable]
    public struct ControlPoint {
        public Vector3 point;
        public float weight;

        public ControlPoint(Vector3 point, float weight = 1f) {
            this.point = point;
            this.weight = weight;
        }
    }
}