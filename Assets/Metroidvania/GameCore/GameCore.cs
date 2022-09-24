using Cysharp.Threading.Tasks;
using Metroidvania.Lighting;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using UnityEngine;

namespace Metroidvania.GameCore
{
    public class GameCore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly SceneAnchorCore _sceneAnchorCore;
        private readonly LightingCore _lightingCore;
        private readonly PlayerRoot _playerRoot;

        public GameCore(ISceneLoader sceneLoader,
            SceneAnchorCore sceneAnchorCore,
            LightingCore lightingCore,
            PlayerRoot playerRoot)
        {
            _sceneLoader = sceneLoader;
            _sceneAnchorCore = sceneAnchorCore;
            _lightingCore = lightingCore;
            _playerRoot = playerRoot;
            StartCore().Forget();
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting GameCore");
            await _sceneLoader.StartCore();
            await _lightingCore.StartCore();
            await _sceneAnchorCore.StartCore();
            await _playerRoot.StartCore();
            Debug.Log($"Starting GameCore Complete");
        }
    }
}