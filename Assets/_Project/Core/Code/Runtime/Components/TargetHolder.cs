﻿using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime {
    public struct TargetHolder : IEcsComponent {
        [Required] public Transform value;
    }
}