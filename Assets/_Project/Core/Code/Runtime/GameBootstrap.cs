﻿using Cysharp.Threading.Tasks;
using TD3D.Core.Runtime.Runtime.ContentGroups;
using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime {
    [CreateAssetMenu(
        fileName = FILE_NAME + nameof(GameBootstrap),
        menuName = MENU_NAME + nameof(GameBootstrap))]
    public class GameBootstrap : BaseBootstrap {
        public override async UniTask Boot(Context context) {
            QualitySettings.vSyncCount = 0;
            await EntityContentGroup.LoadAsync();
            await DevContentGroup.LoadAsync();
            EcsModule.Load();
            await TurretTestModule.LoadAsync();
        }
    }
}