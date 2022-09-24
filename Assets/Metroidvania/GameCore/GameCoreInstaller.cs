using Metroidvania.Camera;
using Metroidvania.Lighting;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using Zenject;

namespace Metroidvania.GameCore
{
    public class GameCoreInstaller : MonoInstaller
    {
        public PlayerRoot PlayerRoot;
        public CameraController CameraControllerInstance;

        public override void InstallBindings()
        {
            Container.BindInstance(PlayerRoot).AsSingle();
            Container.Bind<ISceneLoader>().To<SceneLoader>().FromNew().AsSingle();
            Container.BindInstance(CameraControllerInstance).AsSingle();
            Container.Bind<LightingCore>().FromNew().AsSingle();
            Container.Bind<SceneAnchorCore>().FromNew().AsSingle();
            Container.Bind<GameCore>().FromNew().AsSingle().NonLazy();
        }
    } 
}