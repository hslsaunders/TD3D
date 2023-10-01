using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime {
    public struct BezierTester : IEcsComponent {
        [Range(0f,1f)] public float sampleValue;
        public float debugSize;
        public BezierCurve curve;
    }
}