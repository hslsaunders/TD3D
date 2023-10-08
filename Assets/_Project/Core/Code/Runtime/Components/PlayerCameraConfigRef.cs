using Sirenix.OdinInspector;
using TD3D.Core.Runtime.Runtime.Configs;
using UFlow.Addon.ECS.Core.Runtime;

namespace TD3D.Core.Runtime.Runtime {
    public struct PlayerCameraConfigRef : IEcsComponent {
        [Required] public PlayerCameraConfig value;
    }
}