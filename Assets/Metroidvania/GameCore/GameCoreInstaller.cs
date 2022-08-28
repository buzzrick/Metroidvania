using Metroidvania.Player;
using Zenject;

namespace Metroidvania.GameCore
{
    public class GameCoreInstaller : MonoInstaller
    {
        public PlayerRoot PlayerRoot;
        public override void InstallBindings()
        {
            Container.BindInstance(PlayerRoot).AsSingle();
        }
    } 
}