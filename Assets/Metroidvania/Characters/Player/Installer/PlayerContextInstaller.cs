using Metroidvania.Characters.Player.Animation;
using Metroidvania.Characters.Player.Inventory;
using Zenject;

namespace Metroidvania.Characters.Player.Installer
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
            Container.Bind<PlayerControls>().FromNew().AsSingle();

            Container.BindFactory<PlayerAnimationView, PlayerAnimationActionsHandler, PlayerAnimationActionsHandler.Factory>().AsSingle();
        }
    }
}