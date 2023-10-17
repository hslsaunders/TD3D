using Sirenix.OdinInspector;
using UnityEngine;

namespace TD3D.Core.Runtime {
    public class CurveCreator : MonoBehaviour {
        public BezierCurve curve;
        //public List<ControlPoint> selectedControlPoints = new();
        public float anchorSize = .25f;
        public float controlPointSize = .15f;

        [HideInInspector] public bool snapping;
        [HideInInspector] public float snappingSize;
        public bool showTestPoint;
        [ShowIf("showTestPoint")] [PropertyRange(0f, 1f), SerializeField] private float m_testPointProgress;
        [SerializeField] private float m_testPointSize = .25f;
        public AnchorOptions currAnchorOptions;
        [HideInInspector] public bool anchorOptionsFoldoutOpen;

        private void OnDrawGizmosSelected() {
            if (showTestPoint) {
                Gizmos.DrawWireSphere(curve.EvaluateCurvePoint(m_testPointProgress),  m_testPointSize);
            }
        }
    }
}