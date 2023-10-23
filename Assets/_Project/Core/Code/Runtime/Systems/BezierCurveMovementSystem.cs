using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class BezierCurveMovementSystem : BaseSetIterationDeltaSystem {
        public BezierCurveMovementSystem(in World world) : base(in world, world.BuildQuery()
                                                                    .With<BezierCurveMovement>()
                                                                    .With<TransformRef>()
                                                                    .With<MovementSpeed>()) { }

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            var transform = entity.Get<TransformRef>().value;
            ref var curveMovement = ref entity.Get<BezierCurveMovement>();
            var moveSpeed = entity.Get<MovementSpeed>().value;

            var curve = curveMovement.curve;
            float distFromCurve = curveMovement.distFromCurve;
            
            curveMovement.distAlongCurve += moveSpeed * delta;
            float curveLength = curve.curve.bakedCurve.GetApproximateCurveLengthWithDistFromCenter(distFromCurve);
            if (curveMovement.distAlongCurve > curveLength)
                curveMovement.distAlongCurve -= curveLength;
            transform.position = curve.curve.bakedCurve.EvaluatePointAtDistanceWithWidth(curveMovement.distAlongCurve, distFromCurve);
            transform.forward = curve.curve.bakedCurve.EvaluateDirectionAtDistanceWithWidth(curveMovement.distAlongCurve, distFromCurve);
        }
    }
}