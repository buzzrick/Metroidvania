using Cysharp.Threading.Tasks;
using Metroidvania.MultiScene;
using UnityEngine;
using Zenject;

namespace Metroidvania.World
{
    public class WorldUnlockScene : MonoBehaviour
    {
        public string SceneName;
        public string SceneRootNode;
        private ISceneLoader _sceneLoader;
        private WorldUnlockRootNode _node;

        [Inject]
        private void Initialise(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask<WorldUnlockRootNode> LoadChildNode()
        {
            if ( _node == null)
            {
                _node = await _sceneLoader.LoadUISceneAsync<WorldUnlockRootNode>(SceneName, SceneRootNode, true);

                Debug.Log($"Found Node:{(_node == null ? "Null" : _node.ToString())}");
            }
            return _node;
        }

        public async UniTask UnloadChildNode()
        {
            if (_node != null)
            {
                await _sceneLoader.UnloadSceneAsync(SceneName, _node);
            }
        }

        private void OnDestroy()
        {
            if (_node != null)
            {
                _sceneLoader.UnloadSceneAsync(SceneName, _node).Forget();
                _node = null;
            }
        }
    }
}