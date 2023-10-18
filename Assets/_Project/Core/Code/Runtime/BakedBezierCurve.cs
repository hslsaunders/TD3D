using System.Collections.Generic;
using UnityEngine;

namespace TD3D.Core.Runtime {
    public class BakedBezierCurve {
        //private const float c_min_sampling_size = .01f;
        private const float c_divisions_per_unit_length = 10;
        
        public readonly Vector3[] points;
        public readonly Vector3[] tangents;
        public readonly float[] cumulativeDistances;
        public readonly float[] times;
        public readonly float totalLength;


        public BakedBezierCurve(BezierCurve curve, float pointSpacing) {
            PathSplitData splitData = SplitPath(curve, pointSpacing);
        }

        private static PathSplitData SplitPath(BezierCurve curve, float pointSpacing) {
            PathSplitData data = new PathSplitData();
            
            int numSegments = curve.NumSegments();
            float totalDist = 0f;

            Vector3 lastPos = curve[0].point;
            Vector3 startTangent =
                BezierCurveUtility.EvaluateCubicDerivative(curve[0].point, curve[1].point, curve[2].point, curve[3].point, 0f);
            
            AddNewLocationToData(ref data, lastPos, startTangent, 0f, 0f);
            
            for (int segmentIndex = 0; segmentIndex < numSegments; segmentIndex++) {
                int i = curve.GetSegmentStart(segmentIndex);
                Vector3 p0 = curve[i + 0].point, p1 = curve[i + 1].point, p2 = curve[i + 2].point, p3 = curve[i + 3].point;

                float approxSegmentLength = BezierCurveUtility.ApproximateCurveLength(p0, p1, p2, p3);
                int numDivisions = Mathf.CeilToInt(approxSegmentLength * c_divisions_per_unit_length);

                float tIncrementSize = 1f / numDivisions;

                for (float t = 0; t < 1f; t += tIncrementSize) {
                    Vector3 pointAlongSegment = BezierCurveUtility.EvaluateCubic(p0, p1, p2, p3, t);
                    //bool isLastPointOnEntireCurve = 
                    
                    float distToLastVertex = Vector3.Distance(pointAlongSegment, lastPos);

                }
            }

            return data;
        }

        private static void AddNewLocationToData(ref PathSplitData data, Vector3 point, Vector3 tangent, float cumulativeDist, float time) {
            data.points.Add(point);
            data.tangents.Add(tangent);
            data.cumulativeDistance.Add(cumulativeDist);
            data.times.Add(time);
        }
    }

    public class PathSplitData {
        public List<Vector3> points = new List<Vector3>();
        public List<Vector3> tangents = new List<Vector3>();
        public List<float> cumulativeDistance = new List<float>();
        public List<float> times = new List<float>();
        public float totalLength;
    } 
}