using Metroidvania.Cameras;
using Metroidvania.Debugging;
using Metroidvania.Interactables.ResourcePickups;
using Metroidvania.Lighting;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using Metroidvania.ResourceTypes;
using Metroidvania.UI;
using Zenject;

namespace Metroidvania.GameCore
{
    public class GameCoreInstaller : MonoInstaller
    {
        public CameraController CameraControllerInstance;
        public ResourceTypeDB ResourceTypeDB;

        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().FromNew().AsSingle();
            Container.BindInstance(CameraControllerInstance).AsSingle();
            Container.Bind<LightingCore>().FromNew().AsSingle();
            Container.Bind<SceneAnchorCore>().FromNew().AsSingle();
            Container.Bind<PlayerCore>().FromNew().AsSingle();
            Container.Bind<DebuggingCore>().FromNew().AsSingle();
            Container.Bind<GameCore>().FromNew().AsSingle().NonLazy();
            Container.Bind<UICore>().FromNew().AsSingle().NonLazy();
            Container.Bind<ResourcePickupGenerator>().FromNew().AsSingle().NonLazy();

            Container.BindInstance(ResourceTypeDB).AsSingle();
        }
    } 
}