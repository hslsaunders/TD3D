using _Project.Core.Code.Components;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class AimAtTargetSystem : BaseSetIterationDeltaSystem {
        public AimAtTargetSystem(in World world) : base(in world, world.BuildQuery()
                                                            .With<AimAtTarget>()
                                                            .With<TargetHolder>()
                                                            .With<RotationPivot>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            ref var aimAtTarget = ref entity.Get<AimAtTarget>();
            ref var targetTransform = ref entity.Get<TargetHolder>().value;
            ref var rotationPivot = ref entity.Get<RotationPivot>().value;

            float turnSpeed = aimAtTarget.rotationSpeed;
            Debug.Log(targetTransform.name);
            
            rotationPivot.LookAt(targetTransform);
        }
    }
}