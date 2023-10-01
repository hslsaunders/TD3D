using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class BarrageRocketSystem : BaseSetIterationDeltaSystem {
        public BarrageRocketSystem(in World world) : base(in world, world.BuildQuery()
                                                              .With<BarrageRocket>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            ref var rocket = ref entity.Get<BarrageRocket>();
            ref var rocketTransform = ref entity.Get<TransformRef>().value;
            
            rocket.time += delta;
            float t = rocket.time / rocket.travelTime;
            rocketTransform.position = rocket.curve.EvaluateCurvePoint(t);
            rocketTransform.forward = rocket.curve.EvaluateCurveTangent(t);
        }
    }
}