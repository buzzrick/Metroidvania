using Cysharp.Threading.Tasks;
using Metroidvania.Camera;
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
        private readonly PlayerCore _playerCore;
        private readonly CameraController _cameraController;

        public GameCore(ISceneLoader sceneLoader,
            SceneAnchorCore sceneAnchorCore,
            LightingCore lightingCore,
            PlayerCore playerCore,
            CameraController cameraController)
        {
            _sceneLoader = sceneLoader;
            _sceneAnchorCore = sceneAnchorCore;
            _lightingCore = lightingCore;
            _playerCore = playerCore;
            _cameraController = cameraController;
            StartCore().Forget();
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting GameCore");
            await _sceneLoader.StartCore();
            await _lightingCore.StartCore();
            await _playerCore.StartCore();
            await _sceneAnchorCore.StartCore();
            await _cameraController.StartCore();
            Debug.Log($"Starting GameCore Complete");
        }
    }
}