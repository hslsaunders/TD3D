using System.Collections.Generic;
using UnityEngine;

namespace TD3D.Core.Runtime {
    public class CurveCreator : MonoBehaviour {
        public BezierCurve curve;
        //public List<ControlPoint> selectedControlPoints = new();
        public float anchorSize;
        public float controlPointSize;

        [SerializeField, HideInInspector] public bool snapping;
        [SerializeField, HideInInspector] public float snappingSize;
    }
}