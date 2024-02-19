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
            await Hide(null);
            
            _displayedNode = unlockNode;
            _displayedNodeData = unlockNode.NodeData;
            _requiredResourceTotals = unlockNode.ResourceAmounts;
            
            _uiView = await _sceneLoader.LoadUISceneAsync<WorldUnlockRequirementsUIView>(SceneName, true);
            await _uiView.ShowRequirements(unlockNode);
        }

        public async UniTask Hide(WorldUnlockNode? unlockNode)
        {
            if (_uiView != null)
            {
                //  only hide the UI if we have been given the currently displayed node
                //  or null to always hide the UI
                if ( unlockNode == null || unlockNode == _displayedNode)
                {
                    await _sceneLoader.UnloadSceneAsync(SceneName, _uiView);
                    _uiView = null;
                }
            }
        }
    }
}