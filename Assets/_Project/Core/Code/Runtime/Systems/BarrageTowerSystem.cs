using System.Collections.Generic;
using TD3D.Core.Runtime.Runtime.ContentGroups;
using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    [ExecuteAfter(typeof(TurretTargetHolderAssignerSystem))]
    public sealed class BarrageTowerSystem : BaseSetIterationDeltaSystem {
        public BarrageTowerSystem(in World world) : base(in world, world.BuildQuery()
                                                             .With<BarrageTower>()
                                                             .With<FireCooldown>()
                                                             .With<TurretTargetHolder>()
                                                             .With<RotationPivot>()
                                                             .With<BarrageTurretConfigRef>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            ref var tower = ref entity.Get<BarrageTower>();
            ref var fireCooldown = ref entity.Get<FireCooldown>();
            ref var targetHolder = ref entity.Get<TurretTargetHolder>();
            ref var source = ref entity.Get<RotationPivot>().value;
            var config = entity.Get<BarrageTurretConfigRef>().value;
            
            if (fireCooldown.barrageTime >= 0f)
                fireCooldown.barrageTime -= delta;
            
            if (fireCooldown.barrageTime <= 0f) {
                if (fireCooldown.perMissileTime <= 0f && targetHolder.targetEntity.IsAlive()) {
                    var rocketEntity = DevContentGroup.Get().BarrageRocket.Instantiate().AsEntity();
                    ref var rocket = ref rocketEntity.Get<BarrageRocket>();
                    ref var rocketTransform = ref rocketEntity.Get<TransformRef>().value;
                    
                    Vector3 targetEntityPos = targetHolder.targetEntity.Get<TransformRef>().value.position;
                    
                    Vector3 displacement = targetEntityPos - source.position;
                    float dist = displacement.magnitude;
                    Vector3 dir = displacement.normalized;
                    //Vector3 cross = Vector3.Cross(source.position, target.position).normalized;

                    var fireSource = tower.fireSources[tower.currBarrageCount];
                    
                    Vector3 sourcePos = fireSource.rocketSpawnTransform.position;

                    Vector3 randomTargetOffset = Random.insideUnitSphere * config.barrageSpreadRange;
                    randomTargetOffset.y = 0f;
                    Vector3 targetPos = targetEntityPos + randomTargetOffset + new Vector3(0f, 1f, 0f);

                    Vector3 pointAlongLine = Vector3.Lerp(sourcePos, targetPos, .6f);
                    float distFromPointToEnd = Vector3.Distance(pointAlongLine, targetPos);

                    Vector3 launchDirection = tower.launcherTransform.up;

                    Vector3 launchControlPoint = sourcePos + launchDirection * Mathf.Min(1f, dist);
                    
                    Vector3 curveControlPoint = pointAlongLine +
                                  new Vector3(Random.Range(-distFromPointToEnd, distFromPointToEnd) * .6f, 
                                              Random.Range(-distFromPointToEnd, distFromPointToEnd) * .6f);
                    
                    rocket.curve = new BezierCurve(sourcePos, targetPos, 
                                                   new List<Vector3> { launchControlPoint, curveControlPoint });
                    
                    rocketTransform.position = sourcePos;
                    rocketTransform.forward = rocket.curve.EvaluateCurveTangent(0f);

                    tower.currBarrageCount++;
                    if (tower.currBarrageCount >= config.barrageSize) {
                        fireCooldown.barrageTime += config.barrageDelay;
                        tower.currBarrageCount = 0;
                    }

                    fireCooldown.perMissileTime += config.fireDelay;
                }

                if (fireCooldown.perMissileTime >= 0f)
                    fireCooldown.perMissileTime -= delta;
            }
        }
    }
}