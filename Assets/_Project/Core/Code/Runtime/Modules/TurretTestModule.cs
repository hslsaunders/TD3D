using Cysharp.Threading.Tasks;
using TD3D.Core.Runtime.Runtime.ContentGroups;
using UFlow.Core.Runtime;

namespace TD3D.Core.Runtime.Runtime {
    public sealed class TurretTestModule : BaseSceneModule<TurretTestModule> {
        public override string SceneName => "TurretTest";

        protected override UniTask PostSceneLoadAsync() {
            EntityContentGroup.Get().RenderSettings.Instantiate();
            DevContentGroup.Get().TestTurret.Instantiate();
            return base.PostSceneLoadAsync();
        }
    }
}