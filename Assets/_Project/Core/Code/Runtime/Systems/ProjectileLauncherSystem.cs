using System.Collections.Generic;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public sealed class ProjectileLauncherSystem : BaseSetIterationSystem {
        public ProjectileLauncherSystem(in World world) : base(in world, world.BuildQuery()
                                                                   .With<TargetHolder>()
                                                                   .With<RotationPivot>()
                                                                   .With<BezierTester>()) { }

        protected override void IterateEntity(World world, in Entity entity) {
            ref var source = ref entity.Get<RotationPivot>().value;
            ref var target = ref entity.Get<TargetHolder>().value;
            ref var bezierTester = ref entity.Get<BezierTester>();

            if (Input.GetKeyDown(KeyCode.F)) {
                bezierTester.curve = new BezierCurve(source.position, target.position, new List<Vector3>());
            }
        }
    }
}