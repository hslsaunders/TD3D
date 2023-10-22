using JetBrains.Annotations;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TD3D.Core.Runtime {
    public class CurveCreator : MonoBehaviour {
        [HideInInspector] public BezierCurve curve;
        //public List<ControlPoint> selectedControlPoints = new();
        public bool autoBakeAfterChange;
        public float anchorSize = .25f;
        public float controlPointSize = .15f;
        [OnValueChanged("QueueBakeCurve")] [OnValueChanged("RepaintScene")] [PropertyRange("@BakedBezierCurve.MIN_SAMPLING_SIZE", 1f)]
        public float bakedCurveVertexSpacing = .1f;
        [HideInInspector] public bool snapping;
        [HideInInspector] public float snappingSize;
        [SerializeField] private bool m_showTestPoint;
        [ShowIf("m_showTestPoint")] [SerializeField] private bool m_useDist;
        [ShowIf("@m_showTestPoint && !m_useDist")] [PropertyRange(0f, 1f), SerializeField] private float m_testPointProgress;
        [ShowIf("@m_showTestPoint && m_useDist")] [SerializeField] private float m_testPointDist;
        [ShowIf("m_showTestPoint")] [SerializeField] private float m_testPointDistFromCurve;
        [ShowIf("m_showTestPoint")] [SerializeField] private float m_testPointSize = .25f;
        public bool debugBakedCurveVertices;
        [ShowIf("debugBakedCurveVertices")] [SerializeField] private float m_vertexDebugSize = .1f;
        [HideInInspector] public AnchorOptions currAnchorOptions;
        [HideInInspector] public bool anchorOptionsFoldoutOpen;
        [HideInInspector] public bool bakedCurveInfoFoldoutOpen;
        [HideInInspector] public bool queueRebake;

        public void QueueBakeCurve() {
            if (autoBakeAfterChange) {
                queueRebake = true;
            }
        }
        

#if UNITY_EDITOR
        [UsedImplicitly]
        private void RepaintScene() {
            SceneView.RepaintAll();
        }
#endif
        
        private void OnDrawGizmosSelected() {
            if (curve.HasBakedCurve) {
                if (debugBakedCurveVertices) {
                    for (int i = 0; i < curve.bakedCurve.numPoints; i++) {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(curve.bakedCurve.points[i],  m_vertexDebugSize);
                        Gizmos.color = Color.red;
                        Gizmos.DrawRay(curve.bakedCurve.points[i], curve.bakedCurve.tangents[i] * m_vertexDebugSize * 2f);
                        Gizmos.color = Color.blue;
                        Vector3 normal = curve.bakedCurve.normals[i];
                        Gizmos.DrawRay(curve.bakedCurve.points[i],  normal * m_vertexDebugSize * 2f);
                    }
                }
                if (m_showTestPoint) {
                    Gizmos.color = Color.white;
                    Vector3 point;
                    Vector3 dir;
                    Vector3 upwards;
                    if (m_useDist) {
                        point = curve.bakedCurve.EvaluatePointAtDistance(m_testPointDist);
                        dir = curve.bakedCurve.EvaluateDirectionAtDistance(m_testPointDist);
                        upwards = curve.bakedCurve.EvaluateNormalAtDistance(m_testPointDist);
                    }
                    else {
                        //point = curve.bakedCurve.EvaluatePointAtTimeWithWidth(m_testPointProgress, m_testPointDistFromCurve);
                        point = curve.bakedCurve.EvaluatePointAtTime(m_testPointProgress);
                        dir = curve.bakedCurve.EvaluateDirectionAtTime(m_testPointProgress);
                        upwards = curve.bakedCurve.EvaluateNormalAtTime(m_testPointProgress);
                    }

                    Gizmos.matrix =
                        Matrix4x4.TRS(point, Quaternion.LookRotation(dir, upwards),
                                      Vector3.one);
                    Gizmos.DrawWireSphere(Vector3.zero, m_testPointSize);
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(Vector3.zero, Vector3.forward * 1.5f);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(Vector3.zero, Vector3.up * 1.5f);
                }
            }
        }
    }
}