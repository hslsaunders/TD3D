using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    public class PlayerCameraInitializerSystem : BaseSetCallbackSystem {
        public PlayerCameraInitializerSystem(in World world) : base(in world, world.BuildQuery()
                                                                        .With<PlayerCamera>()
                                                                        .With<TransformRef>()) { }

        protected override void EntityAdded(World world, in Entity entity) {
            var transform = entity.Get<TransformRef>().value;
            ref var playerCam = ref entity.Get<PlayerCamera>();
            playerCam.unlerpedPosition = transform.position;
        }
    }
}