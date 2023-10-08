using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class PlayerCameraSystem : BaseSetIterationDeltaSystem {
        public PlayerCameraSystem(in World world) : base(in world, world.BuildQuery()
                                                             .With<PlayerCamera>()
                                                             .With<PlayerCameraConfigRef>()
                                                             .With<TransformRef>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            var transform = entity.Get<TransformRef>().value;
            var config = entity.Get<PlayerCameraConfigRef>().value;
            ref var playerCam = ref entity.Get<PlayerCamera>();

            Vector3 inputAxis = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
                inputAxis.x -= 1f;
            if (Input.GetKey(KeyCode.D))
                inputAxis.x += 1f;
            if (Input.GetKey(KeyCode.S))
                inputAxis.z -= 1f;
            if (Input.GetKey(KeyCode.W))
                inputAxis.z += 1f;
            inputAxis.Normalize();

            playerCam.unlerpedPosition += inputAxis * (config.MoveSpeed * delta);
            transform.position = Vector3.Lerp(transform.position, playerCam.unlerpedPosition, delta * config.LerpSpeed);
        }
    }
}