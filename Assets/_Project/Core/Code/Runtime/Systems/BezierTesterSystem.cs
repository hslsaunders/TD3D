using TD3D.Core.Runtime.Runtime;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(GizmoSystemGroup))]
    public sealed class BezierTesterSystem : BaseSetIterationSystem {
        public BezierTesterSystem(in World world) : base(in world, world.BuildQuery()
                                                             .With<BezierTester>()) { }

        protected override void IterateEntity(World world, in Entity entity) {
            ref var bezierTester = ref entity.Get<BezierTester>();
            if (bezierTester.curve == null) return; 
            Gizmos.DrawWireSphere(bezierTester.curve.EvaluateCurvePoint(bezierTester.sampleValue), bezierTester.debugSize);
        }
    }
}