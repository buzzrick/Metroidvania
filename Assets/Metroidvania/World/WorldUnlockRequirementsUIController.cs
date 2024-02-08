#nullable enable
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using Metroidvania.ResourceTypes;

namespace Metroidvania.World
{
    public class WorldUnlockRequirementsUIController : ICore
    {
        private WorldUnlockNode? _displayedNode;
        private WorldUnlockData.WorldUnlockNodeData? _displayedNodeData;
        private SerializedDictionary<ResourceTypeSO, int>? _requiredResourceTotals;
        private readonly ISceneLoader _sceneLoader;
        private WorldUnlockRequirementsUIView? _uiView;

        private static string SceneName = "WorldUnlockRequirementsUIView";
        
        public WorldUnlockRequirementsUIController(
            ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

       
        public UniTask StartCore()
        {
            return UniTask.CompletedTask;
        }


        public async UniTask ShowRequirements(WorldUnlockNode unlockNode)
        {
            //  hide any existing UI
            await Hide();
            
            _displayedNode = unlockNode;
            _displayedNodeData = unlockNode.NodeData;
            _requiredResourceTotals = unlockNode.ResourceAmounts;
            
            _uiView = await _sceneLoader.LoadUISceneAsync<WorldUnlockRequirementsUIView>(SceneName, true);
            await _uiView.ShowRequirements(unlockNode);
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