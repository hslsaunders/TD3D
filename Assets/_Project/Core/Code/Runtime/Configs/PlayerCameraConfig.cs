using UFlow.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime.Configs {
    [CreateAssetMenu(
         fileName = FILE_NAME + nameof(PlayerCameraConfig), 
         menuName = MENU_NAME + nameof(PlayerCameraConfig))]
    public sealed class PlayerCameraConfig : BaseConfig {
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float LerpSpeed { get; private set; }
    }
}