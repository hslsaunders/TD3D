using UFlow.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime.Configs {
    [CreateAssetMenu(
        fileName = FILE_NAME + nameof(LightingConfig),
        menuName = MENU_NAME + nameof(LightingConfig))]
    public sealed class LightingConfig : BaseConfig {
        [field: SerializeField] public Color SkyboxColor { get; private set; }
        [field: SerializeField] public Color AmbientColor { get; private set; }
    }
}