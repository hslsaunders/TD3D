using JetBrains.Annotations;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TD3D.Core.Runtime {
    public class CurveCreator : MonoBehaviour {
        public BezierCurve curve;
        //public List<ControlPoint> selectedControlPoints = new();
        public float anchorSize = .25f;
        public float controlPointSize = .15f;
        [OnValueChanged("RepaintScene")] public float bakedCurveVertexSpacing = .1f;
        [HideInInspector] public bool snapping;
        [HideInInspector] public float snappingSize;
        public bool showTestPoint;
        [ShowIf("showTestPoint")] [PropertyRange(0f, 1f), SerializeField] private float m_testPointProgress;
        [SerializeField] private float m_testPointSize = .25f;
        [SerializeField] private float m_vertexDebugSize = .1f;
        public bool debugBakedCurveVertices;
        public AnchorOptions currAnchorOptions;
        [HideInInspector] public bool anchorOptionsFoldoutOpen;

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
                        Gizmos.DrawWireSphere(curve.bakedCurve.points[i],  m_vertexDebugSize);
                    }
                }
                if (showTestPoint) {
                    Vector3 point = curve.bakedCurve.EvaluatePointAtTime(m_testPointProgress);
                    Gizmos.matrix =
                        Matrix4x4.TRS(point, Quaternion.LookRotation(curve.bakedCurve.EvaluateDirectionAtTime(m_testPointProgress)),
                                      Vector3.one);
                    Gizmos.DrawWireSphere(Vector3.zero, m_testPointSize);
                }
            }
        }
    }
}