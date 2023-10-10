using TD3D.Core.Runtime.Runtime;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public sealed class TurretTargetHolderAssignerSystem : BaseSetIterationSystem {
        private readonly DynamicEntitySet m_targetableEntities;

        public TurretTargetHolderAssignerSystem(in World world) : base(in world, world.BuildQuery()
                                                                           .With<TurretTargetHolder>()
                                                                           .With<TransformRef>()
                                                                           .With<AttackRange>()) {
            m_targetableEntities = world.BuildQuery().With<TurretTarget>().With<TransformRef>().AsSet();
        }

        protected override void IterateEntity(World world, in Entity entity) {
            var position = entity.Get<TransformRef>().value.position;
            ref var targetHolder = ref entity.Get<TurretTargetHolder>();
            float attackRange = entity.Get<AttackRange>().value;
            
            Entity targetEntity = default;
            float shortestDist = Mathf.Infinity;
            foreach (var possibleTarget in m_targetableEntities) {
                Vector3 targetPos = possibleTarget.Get<TransformRef>().value.position;
                float dist = Vector3.Distance(targetPos, position);
                if (dist > attackRange) continue;
                
                if (dist < shortestDist) {
                    shortestDist = dist;
                    targetEntity = possibleTarget;
                }
            }

            targetHolder.targetEntity = targetEntity;
        }
    }
}