using Zenject;

namespace Metroidvania.MultiScene
{
    public class SceneAnchorInstaller : MonoInstaller
    {
        public SceneAnchorController SceneAnchorController;
        public SceneAnchorOverridesSO SceneAnchorOverrides;

        public override void InstallBindings()
        {
            Container.BindInstance(SceneAnchorController).AsSingle();
            Container.BindInstance(SceneAnchorOverrides).AsSingle();

            Container.BindFactory<ScenePartAnchor, ScenePartAnchor.Factory>().FromNewComponentOnNewGameObject().AsSingle();
        }
    }
}