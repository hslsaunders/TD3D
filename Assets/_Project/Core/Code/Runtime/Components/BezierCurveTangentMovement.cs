using UFlow.Addon.ECS.Core.Runtime;

namespace TD3D.Core.Runtime {
    public struct BezierCurveTangentMovement : IEcsComponent {
        public CurveCreator curve;
        public float distFromCurve;
        public float distTraveled;
    }
}