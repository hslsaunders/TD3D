using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;

namespace TD3D.Core.Runtime {
    public struct FireCooldown : IEcsComponent {
        [ReadOnly] public float perMissileTime;
        [ReadOnly] public float barrageTime;
    }
}