using _Project.Core.Content.Runtime.ContentGroups;
using Cysharp.Threading.Tasks;
using UFlow.Core.Runtime;

namespace TD3D.Core.Runtime.Runtime {
    public class TurretTestModule : BaseSceneModule<TurretTestModule> {
        public override string SceneName => "TurretTest";

        protected override UniTask PostSceneLoadAsync() {
            DevContentGroup.Get().TestTurret.Instantiate();
            return base.PostSceneLoadAsync();
        }
    }
}