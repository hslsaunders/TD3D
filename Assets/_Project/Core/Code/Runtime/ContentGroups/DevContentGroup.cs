using UFlow.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime.ContentGroups {
    [CreateAssetMenu(
        fileName = FILE_NAME + nameof(DevContentGroup),
        menuName = MENU_NAME + nameof(DevContentGroup))]
    public sealed class DevContentGroup : BaseContentGroup<DevContentGroup> {
        [field: SerializeField, ContentRefField]
        public ContentRef<GameObject> TestTurret { get; private set; }
        [field: SerializeField, ContentRefField]
        public ContentRef<GameObject> BarrageRocket { get; private set; }
        [field: SerializeField, ContentRefField]
        public ContentRef<GameObject> Explosion { get; private set; }
    }
}