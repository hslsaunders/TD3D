using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    [ExecuteInGroup(typeof(FrameSimulationSystemGroup))]
    public class BezierCurveTangentMovementSystem : BaseSetIterationDeltaSystem {
        public BezierCurveTangentMovementSystem(in World world) : base(in world, world.BuildQuery()
                                                                           .With<BezierCurveTangentMovement>()
                                                                           .With<TransformRef>()
                                                                           .With<MovementSpeed>()
                                                                           .With<MoveDirection>()) { }
        

        protected override void IterateEntity(World world, in Entity entity, float delta) {
            var transform = entity.Get<TransformRef>().value;
            ref var curveMovement = ref entity.Get<BezierCurveTangentMovement>();
            var moveSpeed = entity.Get<MovementSpeed>().value;
            ref var moveDirection = ref entity.Get<MoveDirection>().value;
            
            var curve = curveMovement.curve;

            float approxCurveLength = curve.curve.bakedCurve.GetApproximateCurveLengthWithDistFromCenter(curveMovement.distFromCurve);
            if (curveMovement.distTraveled > approxCurveLength) {
                curveMovement.distTraveled -= approxCurveLength;
                transform.position =
                    curve.curve.bakedCurve.EvaluatePointAtDistanceWithWidth(0f, curveMovement.distFromCurve);
            }
            
            //int closestVertexIndex = curve.curve.bakedCurve.GetClosestVertexToPos(transform.position);
            float closestT = curve.curve.bakedCurve.GetClosestTimeValueToPos(transform.position);
            Debug.DrawLine(transform.position, curve.curve.bakedCurve.EvaluatePointAtTime(closestT), Color.red);

            Vector3 moveDir = curve.curve.bakedCurve.EvaluateDirectionAtTime(closestT);
            Vector3 moveVector = moveDir * (moveSpeed * delta);
            curveMovement.distTraveled += moveVector.magnitude;
            transform.position += moveVector;
            moveDirection = moveDir;
        }
    }
}