using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using Metroidvania.World;
using UnityEngine;

namespace Metroidvania.Debugging
{
    public class DebuggingCore : ICore
    {
        private ISceneLoader _sceneLoader;
        private DebuggingView _debuggingView;
        private WorldUnlockRootNode _rootNode;

        public DebuggingCore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }


        public async UniTask StartCore()
        {
            //Debug.Log($"Starting DebuggingCore");
            _debuggingView = await _sceneLoader.LoadUISceneAsync<DebuggingView>("DebuggingScene");
            await _debuggingView.StartCore();
            if (_rootNode != null)
            {
                //Debug.Log($"DebuggingCore Setting RootNode PostLoad {_rootNode.name}");
                _debuggingView.RegisterRootNode(_rootNode);
            }
        }

        public void RegisterRootNode(WorldUnlockRootNode rootNode)
        {
            _rootNode = rootNode;
            if (_debuggingView != null)
            {
                //Debug.Log($"DebuggingCore Setting RootNode {_rootNode.name}");
                _debuggingView.RegisterRootNode(_rootNode);
            }
        }
    }
}