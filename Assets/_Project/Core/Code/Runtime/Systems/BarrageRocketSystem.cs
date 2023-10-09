using TD3D.Core.Runtime.Runtime.ContentGroups;
using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class BarrageRocketSystem : BaseSetIterationDeltaSystem {
        public BarrageRocketSystem(in World world) : base(in world, world.BuildQuery()
                                                              .With<BarrageRocket>()
                                                              .With<TransformRef>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            ref var rocket = ref entity.Get<BarrageRocket>();
            ref var rocketTransform = ref entity.Get<TransformRef>().value;
            
            rocket.time += delta;
            float t = rocket.time / rocket.travelTime;
            rocketTransform.position = rocket.curve.EvaluateCurvePoint(t);
            rocketTransform.forward = rocket.curve.EvaluateCurveTangent(t);
            if (t >= 1f) {
                //entity.Destroy();
                var explosionObj = DevContentGroup.Get().Explosion.Instantiate();
                explosionObj.transform.position = rocketTransform.position;
                
                var explosionBehavior = explosionObj.GetComponent<ExplosionBehavior>();
                var currEulerAngles = explosionBehavior.billboardTransform.localEulerAngles;
                explosionBehavior.billboardTransform.localEulerAngles
                    = new Vector3(currEulerAngles.x, currEulerAngles.y, Random.Range(0f, 360f));
                explosionBehavior.smokeTransform.SetParent(null);
                
                rocket.lingeringEffects.SetParent(null);
                rocket.trail.emit = false;
                var emission = rocket.smokeParticleSystem.emission;
                emission.rateOverTimeMultiplier = 0f;
                Object.Destroy(rocket.lingeringEffects.gameObject, 1f);
                CommandBuffer.Destroy(entity);
            }
        }
    }
}