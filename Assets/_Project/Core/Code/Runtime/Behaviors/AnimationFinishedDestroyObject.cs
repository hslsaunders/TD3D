using UnityEngine;

namespace TD3D.Core.Runtime.Runtime.Behaviors {
    public class AnimationFinishedDestroyObject : MonoBehaviour {
        [SerializeField] private GameObject m_targetObject;
        public void DestroyObject(float delay) {
            Destroy(m_targetObject, delay);
        }
    }
}