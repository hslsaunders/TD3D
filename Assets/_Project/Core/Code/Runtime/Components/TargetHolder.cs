using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace _Project.Core.Code.Components {
    public struct TargetHolder : IEcsComponent {
        [Required] public Transform value;
    }
}