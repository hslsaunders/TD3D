using System;
using System.Collections.Generic;
using UnityEngine;

namespace TD3D.Core.Runtime {
    [Serializable]
    public sealed class BezierCurve {
        public List<ControlPoint> controlPoints;
        public BakedBezierCurve bakedCurve;

        public Action OnCurveEdit;
        public Action OnBake;
        
        public bool HasBakedCurve => bakedCurve != null;
        [SerializeField] private List<ControlPoint> m_lastBakedControlPointSet = new();

        public BezierCurve(Vector3 start, Vector3 end) {
            SetControlPoints(new List<Vector3>{start, end});
        }

        public BezierCurve(List<Vector3> points = null) {
            if (points == null) return;
            SetControlPoints(points);
        }
        
        public ControlPoint this[int i] => controlPoints[i];

        public int NumSegments() => (controlPoints.Count - 4) / 3 + 1;
        public int GetSegmentStart(int segmentIndex) => segmentIndex * 3;
        public bool IsAnchor(int index) => index % 3 == 0;

        public bool IsCurrentCurveBaked() {
            if (!HasBakedCurve)
                return false;
            for (var i = 0; i < controlPoints.Count; i++) {
                var currPoint = controlPoints[i];
                if (currPoint.point != m_lastBakedControlPointSet[i].point)
                    return false;
            }

            return true;
        }

        public void BakeCurve(float pointSpacing, bool xLock, bool yLock, bool zLock, float width = 0f) {
            m_lastBakedControlPointSet = new List<ControlPoint>();
            foreach (var point in controlPoints)
                m_lastBakedControlPointSet.Add(new ControlPoint(point));

            CurveAxes axes = CurveAxes.xyz;
            if (yLock && !xLock && !zLock)
                axes = CurveAxes.xz;
            
            bakedCurve = new BakedBezierCurve(this, pointSpacing, axes, width);
            OnBake?.Invoke();
        }

        private void SetControlPoints(List<Vector3> points) {
            controlPoints = new List<ControlPoint>();
            foreach (var point in points) {
                controlPoints.Add(new ControlPoint(point));
            }
        }

        public void MoveControlPoint(int index, Vector3 newPos) {
            ref Vector3 controlPoint = ref controlPoints[index].point;
            Vector3 offset = newPos - controlPoint;
            
            controlPoint = newPos;
            if (IsAnchor(index)) {
                if (index != controlPoints.Count - 1)
                    controlPoints[index + 1].point += offset;
                if (index != 0)
                    controlPoints[index - 1].point += offset;
            }
            else if (index != 1 && index != controlPoints.Count - 2) {
                int anchorListDirection = (index - 1) % 3 == 0 ? -1 : 1;
                ref Vector3 otherControlPoint = ref controlPoints[index + anchorListDirection * 2].point;
                Vector3 anchorPos = controlPoints[index + anchorListDirection].point;
                var otherControlPointDistToAnchor = Vector3.Distance(otherControlPoint, anchorPos);

                Vector3 offsetFromAnchor = anchorPos - newPos;
                Vector3 newDirToAnchor = offsetFromAnchor.normalized;

                otherControlPoint = anchorPos + offsetFromAnchor;
            }
            
            OnCurveEdit?.Invoke();
        }

        public void AppendNewAnchorWithControlPoint(Vector3 pos) {
            // p[^1] + (p[^1] - p[^2]) = 2 * p[^1] - p[^2]
            controlPoints.Add(new ControlPoint(controlPoints[^1].point * 2f - controlPoints[^2].point));
            controlPoints.Add(new ControlPoint(Vector3.Lerp(controlPoints[^1].point, pos, .5f)));
            controlPoints.Add(new ControlPoint(pos));
        }

        private Vector3 SumControlPoints(float t)
        {
            Vector3 sum = Vector3.zero;
            int numPoints = controlPoints.Count;

            sum += Mathf.Pow(1 - t, numPoints + 1) * controlPoints[0].weight * controlPoints[0].point; 
            sum += Mathf.Pow(t, numPoints + 1) * controlPoints[^1].weight * controlPoints[^1].point;
            
            for (int index = 1; index < numPoints - 1; index++)
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
            
            weightDivisor += controlPoints[0].weight * Mathf.Pow(1 - t, numPoints + 1);
            weightDivisor += controlPoints[^1].weight * Mathf.Pow(t, numPoints + 1);

            for (int index = 1; index < numPoints - 1; index++)
                weightDivisor += (numPoints + 1) 
                                 * Mathf.Pow(1 - t, numPoints - index) * Mathf.Pow(t, index + 1)
                                 * controlPoints[index].weight;

            if (Math.Abs(weightDivisor) < .0001f)
                return controlPoints[0].point;
            return vectorSum / weightDivisor;
        }

        private const float c_tan_sample_delta = .0001f;
        public Vector3 EvaluateCurveTangent(float t) {
            t = Mathf.Clamp(t, 0f, 1f);
            float sampleFlip = 1f;
            if (1f - t < c_tan_sample_delta) {
                sampleFlip = -1f;
            }
            Vector3 sample1 = EvaluateCurvePoint(t);
            Vector3 sample2 = EvaluateCurvePoint(t + c_tan_sample_delta * sampleFlip);
            return (sample2 - sample1).normalized * sampleFlip;

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
    public class ControlPoint : IEquatable<ControlPoint> {
        public Vector3 point;
        public float weight;
        public Vector3 lockValues;
        public bool lockX;
        public bool lockY;
        public bool lockZ;

        public ControlPoint(Vector3 point, float weight = 1f) {
            this.point = point;
            this.weight = weight;
        }

        public ControlPoint(ControlPoint point) : this(point.point, point.weight)  { }

        public bool Equals(ControlPoint other) {
            if (ReferenceEquals(null, other)) return false;
            return point.Equals(other.point) && weight.Equals(other.weight);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((ControlPoint)obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(point, weight, lockValues, lockX, lockY, lockZ);
        }
    }
}