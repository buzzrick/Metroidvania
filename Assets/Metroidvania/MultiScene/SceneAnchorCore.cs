using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using System.Collections;
using System.Collections.Generic;
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

        public bool ForceCalculation()
        {
            return _controller.ForceCalculation();
        }
    }
}