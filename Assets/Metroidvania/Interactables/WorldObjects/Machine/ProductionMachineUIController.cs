#nullable enable
using CandyCoded.HapticFeedback;
using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using Metroidvania.Player.Inventory;

namespace Metroidvania.Interactables.WorldObjects.Machine
{
    public class ProductionMachineUIController : ICore
    {
        private ProductionMachineUIView? _uiView;
        private static string SceneName = "ProductionMachineUIView";
        private readonly ISceneLoader _sceneLoader;
        private ProductionMachine? _displayedMachine;
        private PlayerRoot _playerRoot;
        private PlayerInventoryManager _playerInventory;

        public ProductionMachineUIController(
            ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public UniTask StartCore()
        {
            return UniTask.CompletedTask;
        }

        public async UniTask ShowUI(ProductionMachine productionMachine, PlayerRoot playerRoot)
        {
            //  hide any existing UI
            await Hide();

            _displayedMachine = productionMachine;
            _playerRoot = playerRoot;
            _playerInventory = _playerRoot.PlayerInventoryManager;

            // _displayedNodeData = unlockNode.NodeData;
            // _requiredResourceTotals = unlockNode.ResourceAmounts;
            
            _uiView = await _sceneLoader.LoadUISceneAsync<ProductionMachineUIView>(SceneName, true);
            await _uiView.ShowRequirements(_displayedMachine);
            _uiView.OnCreateRequested += HandleOnCreateRequested;
            RecalculateResourceAmounts();
        }

        private void RecalculateResourceAmounts()
        {
            bool canAfford1 = _playerInventory.CanAffordResources(_displayedMachine.InputAmounts, 1);
            bool canAfford10 = _playerInventory.CanAffordResources(_displayedMachine.InputAmounts, 10);
            bool canAfford100 = _playerInventory.CanAffordResources(_displayedMachine.InputAmounts, 100);
            _uiView.UpdatePurchaseButtons(canAfford1, canAfford10, canAfford100);
        }
        
        private void HandleOnCreateRequested(int count)
        {
            if (ProcessResources(count))
            {
                HapticFeedback.HeavyFeedback();
            }
        }

        private bool ProcessResources(int count)
        {
            if (_playerInventory.TryConsumeResources(_displayedMachine.InputAmounts, count))
            {
                _playerInventory.AddResources(_displayedMachine.OutputAmounts, count);
                RecalculateResourceAmounts();
                return true;
            }
            return false;
        }

        public async UniTask Hide()
        {
            if (_uiView != null)
            {
                await _sceneLoader.UnloadSceneAsync(SceneName, _uiView);
                _uiView = null;
            }
        }
    }
}