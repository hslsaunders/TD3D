using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;

namespace TD3D.Core.Runtime {
    public struct BezierCurveMovement : IEcsComponent {
        public CurveCreator curve;
        public float distFromCurve;
        [ReadOnly] public float distAlongCurve;
    }
}