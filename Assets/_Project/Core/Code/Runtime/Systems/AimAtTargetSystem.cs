using Sirenix.Utilities;
using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Runtime;
using UnityEditor;
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

            Vector3 targetGroundPos = new Vector3(targetTransform.position.x, 0f, targetTransform.position.z);
            Vector3 pivotGroundPos = new Vector3(rotationPivot.position.x, 0f, rotationPivot.position.z);

            var rotation = Quaternion.LookRotation((targetGroundPos - pivotGroundPos).normalized);
            rotationPivot.rotation = Quaternion.RotateTowards(rotationPivot.rotation, rotation, turnSpeed * delta);

            //Vector2 direction = targetGroundPos - pivotGroundPos;
        }
    }
}