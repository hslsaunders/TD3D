using UnityEngine;

namespace TD3D.Core.Runtime {
    public class AnimationFinishedCallback : StateMachineBehaviour {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (animator.TryGetComponent(out AnimationFinishedDestroyObject behavior))
                behavior.DestroyObject(stateInfo.length);
        }
    }
}