using Assets.Metroidvania.Debugging.DebugMenu;
using Cysharp.Threading.Tasks;
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

        [Inject]
        private void Initialise(PlayerCore playerCore)
        {
            _playerCore = playerCore;
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

            rootPage.AddPageLinkButton<DebugMovementTypeMenu>("Movement Type",
                onLoad: page => page.Setup(MovementStats, _playerCore));

            // You must call Reload() after adding cells.
            rootPage.Reload();
        }
    }
}