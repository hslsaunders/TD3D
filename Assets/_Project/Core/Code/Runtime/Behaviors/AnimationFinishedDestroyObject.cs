using UnityEngine;

namespace TD3D.Core.Runtime {
    public class AnimationFinishedDestroyObject : MonoBehaviour {
        [SerializeField] private GameObject m_targetObject;
        public void DestroyObject(float delay) {
            Destroy(m_targetObject, delay);
        }
    }
}