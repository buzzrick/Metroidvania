using Assets.Metroidvania.Debugging.DebugMenu;
using Cysharp.Threading.Tasks;
using Metroidvania.Configuration;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using Zenject;

namespace Metroidvania.Debugging
{
    public class DebuggingView : MonoBehaviour, IView, ICore
    {
        public PlayerMovementStatsSO[] MovementStats;
        private PlayerCore _playerCore;
        private GameConfiguration _gameConfiguration;

        [Inject]
        private void Initialise(PlayerCore playerCore,
            GameConfiguration gameConfiguration)
        {
            _playerCore = playerCore;
            _gameConfiguration = gameConfiguration;
        }

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        public UniTask StartCore()
        {
            //Debug.Log($"Starting DebuggingView");
            LoadDebugPages();
            return UniTask.CompletedTask;
        }
        private void LoadDebugPages()
        {
            Debug.Log($"Creating Debug Menus");

            // Get or create the root page.
            var rootPage = DebugSheet.Instance.GetOrCreateInitialPage();

            rootPage.AddPageLinkButton<DebugPlayerMenu>("Player Character",
                onLoad: page => page.Setup(MovementStats, _playerCore, _gameConfiguration));
            // You must call Reload() after adding cells.
            rootPage.Reload();
        }
    }
}