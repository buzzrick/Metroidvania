using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using UnityEngine;

namespace Metroidvania.MultiScene
{
    public class SceneAnchorCore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private SceneAnchorController _controller;

        public SceneAnchorCore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting SceneAnchor Core");
            _controller = await _sceneLoader.LoadUISceneAsync<SceneAnchorController>("SceneAnchors", false);
            await _controller.StartCore();
        }

        public async UniTask StopCore()
        {

            await _sceneLoader.UnloadSceneAsync("SceneAnchors", _controller);
            _controller = null;
        }

        public bool ForceCalculation()
        {
            return _controller.ForceCalculation();
        }
    }
}