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
    public sealed class BarrageTowerSystem : BaseSetIterationDeltaSystem {
        public BarrageTowerSystem(in World world) : base(in world, world.BuildQuery()
                                                             .With<BarrageTower>()
                                                             .With<FireCooldown>()
                                                             .With<TargetHolder>()
                                                             .With<RotationPivot>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            ref var tower = ref entity.Get<BarrageTower>();
            ref var fireCooldown = ref entity.Get<FireCooldown>();
            ref var target = ref entity.Get<TargetHolder>().value;
            ref var source = ref entity.Get<RotationPivot>().value;

            fireCooldown.time -= delta;

            
            
            if (fireCooldown.time < 0) {
                ref var rocket = ref DevContentGroup.Get().BarrageRocket.Instantiate().AsEntity().Get<BarrageRocket>();
                
                Vector3 dir = (target.position - source.position).normalized;
                Vector3 cross = Vector3.Cross(source.position, target.position).normalized;


                Vector3 cp1 = Vector3.Lerp(source.position, target.position, 0.5f) + cross;
                
                rocket.curve = new BezierCurve(source.position, target.position, new List<Vector3>{cp1});
                rocket.travelTime = 30f;
                
                fireCooldown.time += tower.fireDelay;
            }
        }
    }
}