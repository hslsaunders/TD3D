using Sirenix.OdinInspector;
using UFlow.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime.ContentGroups {
    [CreateAssetMenu(
        fileName = FILE_NAME + nameof(EntityContentGroup),
        menuName = MENU_NAME + nameof(EntityContentGroup))]
    public sealed class EntityContentGroup : BaseContentGroup<EntityContentGroup> {
        private const string c_misc = "Misc";
        
        [field: SerializeField, ContentRefField, FoldoutGroup(c_misc)]
        public ContentRef<GameObject> RenderSettings { get; private set; }
    }
}