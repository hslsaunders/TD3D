using System.Collections.Generic;
using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime {
    public struct PathMovement : IEcsComponent {
        public List<Vector3> pathPoints;
        [ReadOnly] public int pathPointIndex;
        [ReadOnly] public Vector3 direction;
    }
}