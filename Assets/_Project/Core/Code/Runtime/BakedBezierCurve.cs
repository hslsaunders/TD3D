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
        public readonly int numPoints;

        public BakedBezierCurve(BezierCurve curve, float pointSpacing) {
            PathSplitData splitData = SplitPath(curve, pointSpacing);
            points = new Vector3[splitData.numPoints];
            tangents = new Vector3[splitData.numPoints];
            cumulativeDistances = new float[splitData.numPoints];
            times = new float[splitData.numPoints];
            
            numPoints = splitData.numPoints;
            totalLength = splitData.totalLength;

            for (int i = 0; i < numPoints; i++) {
                points[i] = splitData.points[i];
                tangents[i] = splitData.tangents[i];
                cumulativeDistances[i] = splitData.cumulativeDistances[i];
                times[i] = cumulativeDistances[i] / totalLength;
            }
        }

        private static PathSplitData SplitPath(BezierCurve curve, float pointSpacing) {
            PathSplitData data = new PathSplitData();
            
            int numSegments = curve.NumSegments();
            float totalDist = 0f;

            Vector3 prevPointOnPath = curve[0].point;
            Vector3 lastVertexPos = curve[0].point;
            Vector3 startTangent =
                BezierCurveUtility.EvaluateCubicDerivative(curve[0].point, curve[1].point, curve[2].point, curve[3].point, 0f);
            float distToLastVertex = 0f;

            int vertexCount = 1;
            AddNewLocationToData(ref data, lastVertexPos, startTangent, 0f);

            for (int segmentIndex = 0; segmentIndex < numSegments; segmentIndex++) {
                int i = curve.GetSegmentStart(segmentIndex);
                Vector3 p0 = curve[i + 0].point, p1 = curve[i + 1].point, p2 = curve[i + 2].point, p3 = curve[i + 3].point;

                float approxSegmentLength = BezierCurveUtility.ApproximateCurveLength(p0, p1, p2, p3);
                int numDivisions = Mathf.CeilToInt(approxSegmentLength * c_divisions_per_unit_length);

                float tIncrementSize = 1f / numDivisions;

                for (float t = 0; t <= 1f; t += tIncrementSize) {
                    bool isLastPointOnEntireCurve = t + tIncrementSize >= 1f && segmentIndex == numSegments - 1;
                    
                    if (isLastPointOnEntireCurve) 
                        t = 1f;
                    
                    Vector3 pointAlongSegment = BezierCurveUtility.EvaluateCubic(p0, p1, p2, p3, t);
                    distToLastVertex += (pointAlongSegment - prevPointOnPath).magnitude;

                    if (distToLastVertex > pointSpacing) {
                        float overshootDistance = distToLastVertex - pointSpacing;
                        pointAlongSegment += (prevPointOnPath - pointAlongSegment).normalized * overshootDistance;
                        t -= tIncrementSize;
                    }
                    
                    if (distToLastVertex >= pointSpacing || isLastPointOnEntireCurve) {
                        float placementDist = Mathf.Min(pointSpacing, distToLastVertex);
                        
                        Vector3 newVertexPoint = lastVertexPos + (pointAlongSegment - lastVertexPos) * placementDist;
                        Vector3 tangent = BezierCurveUtility.EvaluateCubicDerivative(p0, p1, p2, p3, t);

                        totalDist += placementDist;
                        AddNewLocationToData(ref data, newVertexPoint, tangent, totalDist);
                        lastVertexPos = newVertexPoint;
                        vertexCount++;
                        distToLastVertex = 0f;
                    }
                }
            }

            data.numPoints = vertexCount;

            return data;
        }

        // https://github.com/SebLague/Path-Creator/blob/master/Assets/PathCreator/Core/Runtime/Objects/VertexPath.cs
        public Vector3 EvaluatePointAtTime(float t) {
            VertexPair pair = FindClosestVerticesToTime(t);
            return Vector3.Lerp(points[pair.v1Index], points[pair.v2Index], pair.percentageBetween);
        }

        private VertexPair FindClosestVerticesToTime(float t) {
            int prevIndex = 0;
            int nextIndex = numPoints - 1;
            int i = Mathf.RoundToInt(t * (numPoints - 1));
            while (true) {
                if (t <= times[i])
                    nextIndex = i;
                else
                    prevIndex = i;

                if (nextIndex - prevIndex <= 1)
                    break;
                
                i = (nextIndex + prevIndex) / 2;
            }

            return new VertexPair(prevIndex, nextIndex, 
                                  (t - times[prevIndex]) / (times[nextIndex] - times[prevIndex]));
        }
        
        public Vector3 EvaluatePointAtDistance(float dist) {
            return Vector3.zero;
        }

        private static void AddNewLocationToData(ref PathSplitData data, Vector3 point, Vector3 tangent, float cumulativeDist) {
            data.points.Add(point);
            data.tangents.Add(tangent);
            data.cumulativeDistances.Add(cumulativeDist);
        }
    }

    public class PathSplitData {
        public List<Vector3> points = new List<Vector3>();
        public List<Vector3> tangents = new List<Vector3>();
        public List<float> cumulativeDistances = new List<float>();
        public float totalLength;
        public int numPoints;
    }

    public struct VertexPair {
        public int v1Index;
        public int v2Index;
        public float percentageBetween;

        public VertexPair(int v1Index, int v2Index, float percentageBetween) {
            this.v1Index = v1Index;
            this.v2Index = v2Index;
            this.percentageBetween = percentageBetween;
        }
    }
}