#nullable enable
using Assets.Metroidvania.Debugging.DebugMenu;
using Cysharp.Threading.Tasks;
using Metroidvania.Configuration;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using Metroidvania.World;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using Zenject;

namespace Metroidvania.Debugging
{
    public class DebuggingView : MonoBehaviour, IView, ICore
    {
        public PlayerMovementStatsSO[] MovementStats;
        private PlayerCore _playerCore = default!;
        private GameConfiguration _gameConfiguration = default!;
        private WorldUnlockData _worldData = default!;
        private WorldUnlockRootNode? _rootNode;

        [Inject]
        private void Initialise(PlayerCore playerCore,
            GameConfiguration gameConfiguration,
            WorldUnlockData worldUnlockData)
        {
            _playerCore = playerCore;
            _gameConfiguration = gameConfiguration;
            _worldData = worldUnlockData;
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
            //Debug.Log($"Creating Debug Menus");

            // Get or create the root page.
            var rootPage = DebugSheet.Instance.GetOrCreateInitialPage();

            rootPage.AddSwitch(_gameConfiguration.FreeWorldUnlocksDebugging, "Free Unlocks", null, null, null, null, null, ToggleFreeWorldUnlocks);
            rootPage.AddButton("Reset World Data", clicked: ResetWorldData);
            
            rootPage.AddPageLinkButton<DebugPlayerMenu>("Player Character",
                onLoad: page => page.Setup(MovementStats, _playerCore));
            // You must call Reload() after adding cells.
            rootPage.Reload();
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
            _rootNode?.DebugResetWorldData();
        }
    }
}