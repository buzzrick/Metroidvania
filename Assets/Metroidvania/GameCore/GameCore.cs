using Cysharp.Threading.Tasks;
using Metroidvania.Cameras;
using Metroidvania.Debugging;
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
        private readonly DebuggingCore _debuggingCore;

        public GameCore(ISceneLoader sceneLoader,
            SceneAnchorCore sceneAnchorCore,
            LightingCore lightingCore,
            PlayerCore playerCore,
            CameraController cameraController,
            DebuggingCore debuggingCore)
        {
            _sceneLoader = sceneLoader;
            _sceneAnchorCore = sceneAnchorCore;
            _lightingCore = lightingCore;
            _playerCore = playerCore;
            _cameraController = cameraController;
            _debuggingCore = debuggingCore;
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
            await _debuggingCore.StartCore();

            //  now that the entire scene should be loaded, start the player
            await _playerCore.StartPlayer();    
            Debug.Log($"Starting GameCore Complete");
        }
    }
}