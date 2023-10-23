using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class RotateToMoveDirectionSystem : BaseSetIterationDeltaSystem {
        public RotateToMoveDirectionSystem(in World world) : base(in world, world.BuildQuery()
                                                                      .With<MoveDirection>()
                                                                      .With<TransformRef>()) {}

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            ref var transform = ref entity.Get<TransformRef>().value;
            ref var moveDirection = ref entity.Get<MoveDirection>();

            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                           Quaternion.LookRotation(moveDirection.value), delta * 10f);
        }
    }
}