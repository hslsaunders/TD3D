using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    [ExecuteAfter(typeof(TurretTargetHolderAssignerSystem))]
    public class TurretNoTargetSpinSystem : BaseSetIterationDeltaSystem {
        public TurretNoTargetSpinSystem(in World world) : base(in world, world.BuildQuery()
                                                                   .With<TurretTargetHolder>()
                                                                   .With<RotationPivot>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            ref var targetHolder = ref entity.Get<TurretTargetHolder>();
            if (targetHolder.targetEntity.IsAlive()) return;
            
            ref var rotationPivot = ref entity.Get<RotationPivot>().value;
            Vector3 eulerAngles = rotationPivot.eulerAngles;
            rotationPivot.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y + 30f * delta, 0f);
        }
    }
}