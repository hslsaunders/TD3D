using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    public class PathMovementInitializerSystem : BaseSetCallbackSystem {
        public PathMovementInitializerSystem(in World world) : base(in world, world.BuildQuery()
                                                                        .With<PathMovement>()
                                                                        .With<TransformRef>()) { }

        protected override void EntityAdded(World world, in Entity entity) {
            var transform = entity.Get<TransformRef>().value;
            ref var movement = ref entity.Get<PathMovement>();

            if (movement.pathPoints.Count > 0)
                transform.position = movement.pathPoints[0];
        }
    }
}