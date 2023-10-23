using Sirenix.OdinInspector;
using UFlow.Addon.ECS.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime {
    public struct PlayerCamera : IEcsComponent {
        public Camera camera;
        [ReadOnly] public Vector3 unlerpedPosition;
        [ReadOnly] public float zoomProgress;
        [ReadOnly] public float lerpedZoomProgress;
    }
}