using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;

namespace TD3D.Core.Runtime.Runtime {
    public struct BarrageRocket : IEcsComponent {
        public BezierCurve curve;
        public float travelTime;
        [ReadOnly] public float time;
    }
}