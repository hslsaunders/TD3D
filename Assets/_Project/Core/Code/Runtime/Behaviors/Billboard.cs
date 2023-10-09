using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

namespace TD3D.Core.Runtime {
    [ExecuteAlways]
    public sealed class Billboard : MonoBehaviour {
        [SerializeField] private bool m_ignoreZ;

        [UsedImplicitly]
        private void Awake() => RenderPipelineManager.beginCameraRendering += UpdateOrientation;

        [UsedImplicitly]
        private void OnDestroy() => RenderPipelineManager.beginCameraRendering -= UpdateOrientation;

        private void UpdateOrientation(ScriptableRenderContext ctx, Camera cam) {
            if (cam == null) return;
            var trs = transform;
            var oldZ = trs.eulerAngles.z;
            trs.forward = cam.transform.position - trs.position;
            if (m_ignoreZ) return;
            var euler = trs.eulerAngles;
            euler.z = oldZ;
            trs.eulerAngles = euler;
        }
    }
}