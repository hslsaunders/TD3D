using System.Collections.Generic;
using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime {
    public struct BarrageTower : IEcsComponent {
        public List<Transform> fireSources;
        public Transform launcherTransform;
        public float smokePlumeDistance;
        [ReadOnly] public int currBarrageCount;
    }
}