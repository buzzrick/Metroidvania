using Metroidvania.Player.Animation;
using Metroidvania.Player.Inventory;
using Zenject;

namespace Metroidvania.Player.Installer
{
    public class PlayerContextInstaller : MonoInstaller
    {
        public PlayerAnimationView PlayerAnimationView;
        public ToolPrefabs ToolPrefabs;


        public override void InstallBindings()
        {
            Container.BindInstance(PlayerAnimationView).AsSingle();
            Container.BindInstance(ToolPrefabs).AsSingle();
            Container.Bind<PlayerInventoryManager>().AsSingle();

            Container.BindFactory<PlayerAnimationView, PlayerAnimationActionsHandler, PlayerAnimationActionsHandler.Factory>().AsSingle();
        }
    }
}