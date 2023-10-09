using Sirenix.OdinInspector;
using TD3D.Core.Runtime.Runtime.Configs;
using UFlow.Addon.ECS.Core.Runtime;

namespace TD3D.Core.Runtime.Runtime {
    public class BarrageTurretConfigRef : IEcsComponent {
        [Required] public BarrageTurretConfig value;
    }
}