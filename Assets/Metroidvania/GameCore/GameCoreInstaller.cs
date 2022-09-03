using Metroidvania.Player;
using Zenject;

namespace Metroidvania.GameCore
{
    public class GameCoreInstaller : MonoInstaller
    {
        public PlayerRoot PlayerRoot;
        public MultiSceneLoader MultiSceneLoaderInstance;
        public override void InstallBindings()
        {
            Container.BindInstance(PlayerRoot).AsSingle();
            Container.BindInstance(MultiSceneLoaderInstance).AsSingle();
        }
    } 
}