using UFlow.Core.Runtime;
using UnityEngine;

namespace TD3D.Core.Runtime.Runtime.Configs {
    [CreateAssetMenu(
        fileName = FILE_NAME + nameof(BarrageTurretConfig), 
        menuName = MENU_NAME + nameof(BarrageTurretConfig))]
    public class BarrageTurretConfig : BaseConfig {
        public float fireDelay;
        public float barrageDelay;
        public int barrageSize;
        public float barrageSpreadRange;
    }
}