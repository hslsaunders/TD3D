using System.Collections.Generic;
using Sirenix.OdinInspector;
using TD3D.Core.Runtime.Runtime.DataAbstractions;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime {
    public struct BarrageTower : IEcsComponent {
        public float fireDelay;
        public float barrageDelay;
        public int barrageSize;
        public float barrageSpreadRange;
        public List<BarrageTowerLaunchRefs> fireSources;
        public Transform launcherTransform;
        [ReadOnly] public int currBarrageCount;
    }
}