using TD3D.Core.Runtime.Runtime;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class PathMovementSystem : BaseSetIterationDeltaSystem {
        public PathMovementSystem(in World world) : base(in world, world.BuildQuery()
                                                             .With<PathMovement>()
                                                             .With<TransformRef>()
                                                             .With<MovementSpeed>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            var transform = entity.Get<TransformRef>().value;
            ref var movement = ref entity.Get<PathMovement>();
            float moveSpeed = entity.Get<MovementSpeed>().value;
            
            if (movement.pathPoints.Count == 0) return;

            if (movement.pathPointIndex == 0) {
                transform.position = movement.pathPoints[0];
            }

            float dist = moveSpeed * delta;
            while (dist > 0f) {
                Vector3 nextTargetPos = movement.pathPoints[movement.pathPointIndex];
                Vector3 nextTargetPosOffset = nextTargetPos - transform.position;
                Vector3 dir = nextTargetPosOffset.normalized;
                float actualTravelDist = Mathf.Min(dist, nextTargetPosOffset.magnitude);
                transform.position += dir * actualTravelDist;
                dist -= actualTravelDist;
                if (Vector3.Distance(transform.position, nextTargetPos) < .01f)
                    movement.pathPointIndex = (movement.pathPointIndex + 1) % movement.pathPoints.Count;
            }
        }
    }
}