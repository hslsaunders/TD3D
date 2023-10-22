using System;
using System.Collections.Generic;
using UnityEngine;

namespace TD3D.Core.Runtime {
    [Serializable]
    public class BakedBezierCurve {
        public const float MIN_SAMPLING_SIZE = .005f;
        private const float c_divisions_per_unit_length = 10;
        
        public Vector3[] points;
        public Vector3[] tangents;
        public Vector3[] normals;
        public float[] cumulativeDistances;
        public float[] times;
        public float totalLength;
        public int numPoints;

        public BakedBezierCurve(BezierCurve curve, float pointSpacing, CurveAxes axes) {
            pointSpacing = Mathf.Max(pointSpacing, MIN_SAMPLING_SIZE);
            PathSplitData splitData = SplitPath(curve, pointSpacing);
            points = new Vector3[splitData.numPoints];
            tangents = new Vector3[splitData.numPoints];
            normals = new Vector3[splitData.numPoints];
            cumulativeDistances = new float[splitData.numPoints];
            times = new float[splitData.numPoints];
            
            numPoints = splitData.numPoints;
            totalLength = splitData.totalLength;

            //https://pomax.github.io/bezierinfo/#derivatives
            Vector3 oldPos = points[0];
            Vector3 oldNormal = BezierCurveUtility.EvaluateFrenetBinormal(curve[0].point, curve[1].point,
                                                                          curve[2].point, curve[3].point, 0f);
            Vector3 oldR = BezierCurveUtility.GetRotationalAxis(curve[0].point, curve[1].point, curve[2].point, curve[3].point, 0f);
            if (oldR == Vector3.zero) {
                if (axes == CurveAxes.xyz) {
                    oldNormal = Vector3.up;
                    oldR = Vector3.left;
                }
                else if (axes == CurveAxes.xz) {
                    oldNormal = Vector3.forward;
                    oldR = Vector3.down;
                }
            }
            
            normals[0] = oldNormal;
            Vector3 oldT = tangents[0];
            
            
            for (int i = 0; i < numPoints; i++) {
                points[i] = splitData.points[i];
                tangents[i] = splitData.tangents[i];
                cumulativeDistances[i] = splitData.cumulativeDistances[i];
                times[i] = cumulativeDistances[i] / totalLength;

                if (i == 0) continue;
                Vector3 v1 = points[i] - oldPos;
                float c1 = Vector3.Dot(v1, v1);
                Vector3 riL = oldR - v1 * 2f / c1 * Vector3.Dot(v1, oldR);
                Vector3 tiL = oldT - v1 * 2f / c1 * Vector3.Dot(v1, oldT);

                Vector3 v2 = tangents[i] - tiL;
                float c2 = Vector3.Dot(v2, v2);

                Vector3 newRotationalAxis = riL - v2 * 2 / c2 * Vector3.Dot(v2, riL);
                Vector3 normal = Vector3.Cross(newRotationalAxis, tangents[i]);
                normals[i] = normal;

                oldT = tangents[i];
                oldR = newRotationalAxis;
            }
        }

        private static PathSplitData SplitPath(BezierCurve curve, float pointSpacing) {
            PathSplitData data = new PathSplitData();
            
            int numSegments = curve.NumSegments();
            float totalDist = 0f;

            Vector3 prevPointAlongSegment = curve[0].point;
            Vector3 lastVertexPos = curve[0].point;
            Vector3 startTangent =
                BezierCurveUtility.EvaluateCubicDerivative(curve[0].point, curve[1].point, curve[2].point, curve[3].point, 0f);
            Vector3 startNormal = 
                BezierCurveUtility.EvaluateFrenetBinormal(curve[0].point, curve[2].point, curve[3].point, curve[4].point, 0f);
            float distToLastVertex = 0f;

            int vertexCount = 1;
            AddNewLocationToData(ref data, lastVertexPos, startTangent, startNormal, 0f);

            for (int segmentIndex = 0; segmentIndex < numSegments; segmentIndex++) {
                int i = curve.GetSegmentStart(segmentIndex);
                Vector3 p0 = curve[i + 0].point, p1 = curve[i + 1].point, p2 = curve[i + 2].point, p3 = curve[i + 3].point;

                float approxSegmentLength = BezierCurveUtility.ApproximateCurveLength(p0, p1, p2, p3);
                int numDivisions = Mathf.CeilToInt(approxSegmentLength * c_divisions_per_unit_length);
                float tIncrementSize = 1f / numDivisions;

                for (float t = tIncrementSize; t <= 1f; t += tIncrementSize) {
                    bool isLastPointOnEntireCurve = t + tIncrementSize >= 1f && segmentIndex == numSegments - 1;
                    
                    if (isLastPointOnEntireCurve) 
                        t = 1f;
                    
                    Vector3 pointAlongSegment = BezierCurveUtility.EvaluateCubic(p0, p1, p2, p3, t);
                    distToLastVertex += Vector3.Distance(pointAlongSegment, prevPointAlongSegment);

                    if (distToLastVertex > pointSpacing) {
                        float overshootDistance = distToLastVertex - pointSpacing;
                        pointAlongSegment += (prevPointAlongSegment - pointAlongSegment).normalized * overshootDistance;
                        t -= tIncrementSize;
                    }
                    
                    if (distToLastVertex >= pointSpacing || isLastPointOnEntireCurve) {
                        Vector3 tangent = BezierCurveUtility.EvaluateCubicDerivative(p0, p1, p2, p3, t);
                        Vector3 normal = BezierCurveUtility.EvaluateFrenetBinormal(p0, p1, p2, p3, t);

                        totalDist += Vector3.Distance(lastVertexPos, pointAlongSegment);
                        AddNewLocationToData(ref data, pointAlongSegment, tangent, normal, totalDist);
                        lastVertexPos = pointAlongSegment;
                        vertexCount++;
                        distToLastVertex = 0f;
                    }

                    prevPointAlongSegment = pointAlongSegment;
                }
            }

            data.numPoints = vertexCount;
            data.totalLength = data.cumulativeDistances[vertexCount - 1];

            return data;
        }
        
        private static void AddNewLocationToData(ref PathSplitData data, Vector3 point, Vector3 tangent, Vector3 normal, float cumulativeDist) {
            data.points.Add(point);
            data.tangents.Add(tangent);
            data.normals.Add(normal);
            data.cumulativeDistances.Add(cumulativeDist);
        }

        // https://github.com/SebLague/Path-Creator/blob/master/Assets/PathCreator/Core/Runtime/Objects/VertexPath.cs
        public Vector3 EvaluatePointAtTime(float t) {
            VertexPair pair = FindClosestVerticesToTime(t);
            return Vector3.Lerp(points[pair.v1Index], points[pair.v2Index], pair.percentageBetween);
        }
        
        public Vector3 EvaluateDirectionAtTime(float t) {
            VertexPair pair = FindClosestVerticesToTime(t);
            return Vector3.Lerp(tangents[pair.v1Index], tangents[pair.v2Index], pair.percentageBetween);
        }

        public Vector3 EvaluateNormalAtTime(float t) {
            VertexPair pair = FindClosestVerticesToTime(t);
            return Vector3.Lerp(normals[pair.v1Index], normals[pair.v2Index], pair.percentageBetween);
        }
        
        public Vector3 EvaluatePointAtDistance(float dist) {
            return EvaluatePointAtTime(dist / totalLength);
        }

        public Vector3 EvaluateDirectionAtDistance(float dist) {
            return EvaluateDirectionAtTime(dist / totalLength); 
        }
        
        public Vector3 EvaluateNormalAtDistance(float dist) {
            return EvaluateNormalAtTime(dist / totalLength); 
        }

        public Vector3 EvaluatePointAtTimeWithWidth(float t, float width) {
            Vector3 pointOnCenter = EvaluatePointAtTime(t);
            Vector3 normalAtPoint = EvaluateNormalAtTime(t).normalized;
            return pointOnCenter + normalAtPoint * width;
        }
        
        public int GetClosestVertexToPos(Vector3 pos) {
            int closestIndex = 0;
            float closestDist = Mathf.Infinity;
            for (int i = 0; i < numPoints; i++) {
                float dist = Vector3.Distance(points[i], pos);
                if (dist < closestDist) {
                    closestIndex = i;
                    closestDist = dist;
                }
            }
            return closestIndex;
        }

        private VertexPair FindClosestVerticesToTime(float t) {
            t = Mathf.Clamp01(t);
            
            int prevIndex = 0;
            int nextIndex = numPoints - 1;
            int i = Mathf.RoundToInt(t * (numPoints - 1));
            while (true) {
                if (t <= times[i])
                    nextIndex = i;
                else
                    prevIndex = i;
                
                i = (nextIndex + prevIndex) / 2;

                if (nextIndex - prevIndex <= 1)
                    break;
            }

            return new VertexPair(prevIndex, nextIndex, 
                                  Mathf.InverseLerp(times[prevIndex], times[nextIndex], t));
        }
    }

    public class PathSplitData {
        public List<Vector3> points = new List<Vector3>();
        public List<Vector3> tangents = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
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

    public enum CurveAxes {
        xz,
        xyz
    }
}