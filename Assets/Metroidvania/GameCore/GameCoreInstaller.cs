using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Cameras;
using Metroidvania.Debugging;
using Metroidvania.Interactables.ResourcePickups;
using Metroidvania.Lighting;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using Metroidvania.ResourceTypes;
using Metroidvania.UI;
using Metroidvania.World;
using UnityEngine;
using Zenject;

namespace Metroidvania.GameCore
{
    public class GameCoreInstaller : MonoInstaller
    {
        [SerializeField, RequiredField] private CameraController CameraControllerInstance;
        [SerializeField, RequiredField] private ResourceTypeDB ResourceTypeDB;
        [SerializeField, RequiredField] private GameLifecycleManager GameLifecycleManager;

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
            Container.Bind<WorldUnlockData>().FromNew().AsSingle();
            Container.Bind<WorldUnlockRequirementsUIController>().FromNew().AsSingle();
            Container.Bind<WorldManager>().FromNew().AsSingle().NonLazy();

            Container.BindInstance(GameLifecycleManager).AsSingle();
            Container.BindInstance(ResourceTypeDB).AsSingle();
        }
    } 
}