using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine.Scripting;

namespace TD3D.Core.Runtime {
    [Preserve]
    [ExecuteInWorld(typeof(DefaultWorld))]
    public class BezierCurveTangentMovementInitializerSystem : BaseSetCallbackSystem {
        public BezierCurveTangentMovementInitializerSystem(in World world) : base(in world, world.BuildQuery()
                                                                                      .With<BezierCurveTangentMovement>()
                                                                                      .With<TransformRef>()
                                                                                      .With<MoveDirection>()) { }

        protected override void EntityAdded(World world, in Entity entity) {
            var transform = entity.Get<TransformRef>().value;
            ref var curveMovement = ref entity.Get<BezierCurveTangentMovement>();
            ref var moveDirection = ref entity.Get<MoveDirection>().value;

            transform.position = curveMovement.curve.curve.bakedCurve.EvaluatePointAtDistanceWithWidth(0f, 
                curveMovement.distFromCurve);
            moveDirection = curveMovement.curve.curve.bakedCurve.EvaluateDirectionAtDistanceWithWidth(0f, 
                curveMovement.distFromCurve);
        }}
}