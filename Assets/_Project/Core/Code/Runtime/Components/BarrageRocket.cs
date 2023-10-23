using Ara;
using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime {
    public struct BarrageRocket : IEcsComponent {
        public BezierCurve curve;
        public float travelTime;
        public Transform lingeringEffects;
        public AraTrail trail;
        public ParticleSystem smokeParticleSystem;
        [ReadOnly] public float time;
    }
}