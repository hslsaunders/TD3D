using System;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime.Behaviors {
    [ExecuteAlways]
    public class Billboard : MonoBehaviour {
        [SerializeField] private bool m_ignoreZ;

        private void Awake() {
            Camera.onPreRender += UpdateOrientation;
        }

        private void OnDestroy() {
            Camera.onPreRender -= UpdateOrientation;
        }

        private void UpdateOrientation(Camera cam) {
            if (cam != null) {
                float oldZ = transform.eulerAngles.z;
                transform.forward = -cam.transform.forward;
                if (m_ignoreZ)
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, oldZ);
            }
        }
    }
}