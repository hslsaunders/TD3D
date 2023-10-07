using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(GizmoSystemGroup))]
    public sealed class PathDebugSystem : BaseSetIterationSystem {
        public PathDebugSystem(in World world) : base(in world, world.BuildQuery()
                                                          .With<PathMovement>()) { }

        protected override void IterateEntity(World world, in Entity entity) {
            var movement = entity.Get<PathMovement>();
            Gizmos.color = Color.white;
            for (int i = 0; i < movement.pathPoints.Count; i++) {
                Gizmos.DrawWireSphere(movement.pathPoints[i], .125f);
                if (i < movement.pathPoints.Count - 1) {
                    Gizmos.DrawLine(movement.pathPoints[i], movement.pathPoints[i + 1]);
                }
            }
        }
    }
}