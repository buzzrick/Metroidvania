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
        }

        private void HandleOnCreateRequested()
        {
            if (ProcessResources())
            {
                HapticFeedback.HeavyFeedback();
            }
        }

        private bool ProcessResources()
        {
            if (_playerInventory.TryConsumeResources(_displayedMachine.InputAmounts))
            {
                _playerInventory.AddResources(_displayedMachine.OutputAmounts);
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