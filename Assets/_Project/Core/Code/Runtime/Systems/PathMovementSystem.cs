using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
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
            var moveSpeed = entity.Get<MovementSpeed>().value;
            
            if (movement.pathPoints.Count == 0) return;

            if (movement.pathPointIndex == 0) {
                transform.position = movement.pathPoints[0];
            }

            var dist = moveSpeed * delta;
            while (dist > 0f) {
                var nextTargetPos = movement.pathPoints[movement.pathPointIndex];
                var position = transform.position;
                var nextTargetPosOffset = nextTargetPos - position;
                movement.direction = nextTargetPosOffset.normalized;
                var actualTravelDist = Mathf.Min(dist, nextTargetPosOffset.magnitude);
                position += movement.direction * actualTravelDist;
                transform.position = position;
                transform.forward = movement.direction;
                dist -= actualTravelDist;
                if (Vector3.Distance(transform.position, nextTargetPos) < .01f)
                    movement.pathPointIndex = (movement.pathPointIndex + 1) % movement.pathPoints.Count;
            }
        }
    }
}