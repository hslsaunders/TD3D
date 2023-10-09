using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(LateFrameRenderSystemGroup))]
    public sealed class LightingApplierSystem : BaseSetIterationSystem {
        private readonly DynamicEntitySet m_worldCameraSet;

        public LightingApplierSystem(in World world) : base(in world, world.BuildQuery()
            .With<LightingConfigRef>()) {
            m_worldCameraSet = world.BuildQuery()
                .With<WorldCameraRef>()
                .AsSet();
        }

        protected override void IterateEntity(World world, in Entity entity) {
            ref var config = ref entity.Get<LightingConfigRef>().value;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.ambientLight = config.AmbientColor;
            foreach (var e in m_worldCameraSet)
                e.Get<WorldCameraRef>().value.backgroundColor = config.SkyboxColor;
        }
    }
}