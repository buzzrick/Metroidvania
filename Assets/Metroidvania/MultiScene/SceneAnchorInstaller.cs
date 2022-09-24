using Zenject;

namespace Metroidvania.MultiScene
{
    public class SceneAnchorInstaller : MonoInstaller
    {
        public SceneAnchorController SceneAnchorController;

        public override void InstallBindings()
        {
            Container.BindInstance(SceneAnchorController).AsSingle();
        }
    }
}