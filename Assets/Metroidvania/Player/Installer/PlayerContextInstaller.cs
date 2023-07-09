using Metroidvania.Player.Animation;
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

            Container.BindFactory<PlayerAnimationView, PlayerAnimationActionsHandler, PlayerAnimationActionsHandler.Factory>().AsSingle();
        }
    }
}