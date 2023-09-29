using UFlow.Core.Runtime;
using UnityEngine;

namespace _Project.Core.Content.Runtime.ContentGroups {
    [CreateAssetMenu(
        fileName = FILE_NAME + nameof(DevContentGroup),
        menuName = MENU_NAME + nameof(DevContentGroup))]
    public class DevContentGroup : BaseContentGroup<DevContentGroup> {
        [field: SerializeField, ContentRefField]
        public ContentRef<GameObject> TestTurret { get; private set; }
    }
}