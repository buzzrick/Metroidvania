#nullable enable
using Assets.Metroidvania.Debugging.DebugMenu;
using Cysharp.Threading.Tasks;
using Metroidvania.Configuration;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using Metroidvania.Characters.Player;
using Metroidvania.World;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using Zenject;

namespace Metroidvania.Debugging
{
    public class DebuggingView : MonoBehaviour, IView, ICore
    {
        public PlayerMovementStatsSO[] MovementStats = default!;
        private GameCore.GameCore _gameCore;
        private PlayerCore _playerCore = default!;
        private GameConfiguration _gameConfiguration = default!;
        private WorldUnlockData _worldData = default!;
        private WorldUnlockRootNode? _rootNode;

        [Inject]
        private void Initialise(
            GameCore.GameCore gameCore,
            PlayerCore playerCore,
            GameConfiguration gameConfiguration,
            WorldUnlockData worldUnlockData)
        {
            _gameCore = gameCore;
            _playerCore = playerCore;
            _gameConfiguration = gameConfiguration;
            _worldData = worldUnlockData;
        }

        public UniTask CleanupSelf()
        {
            Destroy(DebugSheet.Instance);
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
            //Debug.Log($"Creating Debug Menus");

            // Get or create the root page.
            var rootPage = DebugSheet.Instance.GetOrCreateInitialPage();

            rootPage.AddSwitch(_gameConfiguration.FreeWorldUnlocksDebugging, "Free Unlocks", null, null, null, null, null, ToggleFreeWorldUnlocks);
            rootPage.AddButton("Reset Game", clicked: ResetGame);
            rootPage.AddButton("Reset World Data", clicked: ResetWorldData);
            rootPage.AddButton("Toggle Graphy", clicked: ToggleGraphy);
            
            rootPage.AddPageLinkButton<DebugPlayerMenu>("Player Character",
                onLoad: page => page.Setup(MovementStats, _playerCore));
            // You must call Reload() after adding cells.
            rootPage.Reload();
        }

        
        private bool _graphyEnabled = false;
        private void ToggleGraphy()
        {
            if (_graphyEnabled)
            {
                GraphyManager.Instance.Disable();
                _graphyEnabled = false;
            }
            else
            {
                GraphyManager.Instance.Enable();
                _graphyEnabled = true;
            }
        }


        private void ToggleFreeWorldUnlocks(bool newValue)
        {
            _gameConfiguration.FreeWorldUnlocksDebugging = newValue;
        }

        public void RegisterRootNode(WorldUnlockRootNode rootNode)
        {
            _rootNode = rootNode;
        }

        private async void ResetWorldData()
        {
            _gameConfiguration.FreeWorldUnlocksDebugging = false;
            await _playerCore.GetPlayerRoot().PlayerInventoryManager.ResetInventory();

            //Debug.Log($"Resetting World Data on RootNode:{(_rootNode == null ? "NULL" : _rootNode.name)}");
            if (_rootNode != null)
            {
                await _rootNode.DebugResetWorldData();
            }
            ResetGame();
        }

        private void ResetGame()
        {
            DebugSheet.Instance.Hide();
            _gameCore.ResetGame().Forget();
        }
    }
}