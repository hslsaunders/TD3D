using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime {
    public struct WorldCameraRef : IEcsComponent {
        [Required] public Camera value;
    }
}