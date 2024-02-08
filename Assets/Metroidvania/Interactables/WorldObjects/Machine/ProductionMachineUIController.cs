#nullable enable
using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;

namespace Metroidvania.Interactables.WorldObjects.Machine
{
    public class ProductionMachineUIController : ICore
    {
        private ProductionMachineUIView? _uiView;
        private static string SceneName = "ProductionMachineUIView";
        private readonly ISceneLoader _sceneLoader;
        private ProductionMachine? _displayedMachine;

        public ProductionMachineUIController(
            ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public UniTask StartCore()
        {
            return UniTask.CompletedTask;
        }

        public async UniTask ShowUI(ProductionMachine productionMachine)
        {
            //  hide any existing UI
            await Hide();
            
            _displayedMachine = productionMachine;
            // _displayedNodeData = unlockNode.NodeData;
            // _requiredResourceTotals = unlockNode.ResourceAmounts;
            
            _uiView = await _sceneLoader.LoadUISceneAsync<ProductionMachineUIView>(SceneName, true);
            await _uiView.ShowRequirements(_displayedMachine);
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