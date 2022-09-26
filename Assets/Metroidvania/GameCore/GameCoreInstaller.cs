using Metroidvania.Camera;
using Metroidvania.Debugging;
using Metroidvania.Lighting;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using Zenject;

namespace Metroidvania.GameCore
{
    public class GameCoreInstaller : MonoInstaller
    {
        public CameraController CameraControllerInstance;

        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().FromNew().AsSingle();
            Container.BindInstance(CameraControllerInstance).AsSingle();
            Container.Bind<LightingCore>().FromNew().AsSingle();
            Container.Bind<SceneAnchorCore>().FromNew().AsSingle();
            Container.Bind<PlayerCore>().FromNew().AsSingle();
            Container.Bind<DebuggingCore>().FromNew().AsSingle();
            Container.Bind<GameCore>().FromNew().AsSingle().NonLazy();
        }
    } 
}