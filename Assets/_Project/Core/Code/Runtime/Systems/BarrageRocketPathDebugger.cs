using TD3D.Core.Runtime.Runtime;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(GizmoSystemGroup))]
    public sealed class BarrageRocketPathDebugger : BaseSetIterationSystem {
        public BarrageRocketPathDebugger(in World world) : base(in world, world.BuildQuery()
                                                                    .With<BarrageRocket>()
                                                                    .With<TransformRef>()) { }

        protected override void IterateEntity(World world, in Entity entity) {
            BarrageRocket rocket = entity.Get<BarrageRocket>();
            BezierCurve curve = rocket.curve;

            Gizmos.color = Color.yellow;
            foreach (var point in curve.controlPoints) {
                Gizmos.DrawWireSphere(point.point, .125f);
            }

            int n = 50;
            for (int i = 0; i < n; i++) {
                var point1 = curve.EvaluateCurvePoint(i / (float)n); 
                var point2 = curve.EvaluateCurvePoint((i + 1) / (float)n);
                Gizmos.color = Color.Lerp(Color.green, Color.red, i / (float)n);
                Gizmos.DrawLine(point1, point2);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(entity.Get<TransformRef>().value.position, curve.EvaluateCurveTangent(rocket.time / rocket.travelTime));
        }
    }
}