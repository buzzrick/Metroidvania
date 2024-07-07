using Cysharp.Threading.Tasks;
using Metroidvania.Cameras;
using Metroidvania.Debugging;
using Metroidvania.Lighting;
using Metroidvania.MultiScene;
using Metroidvania.Characters.Player;
using Metroidvania.UI;
using Metroidvania.World;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace Metroidvania.GameCore
{
    public class GameCore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly SceneAnchorCore _sceneAnchorCore;
        private readonly LightingCore _lightingCore;
        private readonly PlayerCore _playerCore;
        private readonly UICore _uiCore;
        private readonly CameraController _cameraController;
        private readonly DebuggingCore _debuggingCore;
        private readonly WorldUnlockData _worldUnlockData;
        private readonly WorldCharacterStatsData _characterStatsProvider;

        public GameCore(ISceneLoader sceneLoader,
            SceneAnchorCore sceneAnchorCore,
            LightingCore lightingCore,
            PlayerCore playerCore,
            UICore uiCore,
            CameraController cameraController,
            DebuggingCore debuggingCore,
            WorldUnlockData worldUnlockData,
            WorldCharacterStatsData characterStatsProvider)
        {
            _sceneLoader = sceneLoader;
            _sceneAnchorCore = sceneAnchorCore;
            _lightingCore = lightingCore;
            _playerCore = playerCore;
            _uiCore = uiCore;
            _cameraController = cameraController;
            _debuggingCore = debuggingCore;
            _worldUnlockData = worldUnlockData;
            _characterStatsProvider = characterStatsProvider;
            StartCore().Forget();
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting GameCore");
            await _sceneLoader.StartCore();
            await _worldUnlockData.LoadData();
            await _characterStatsProvider.LoadData();
            await _lightingCore.StartCore();
            await _playerCore.StartCore();
            await _uiCore.StartCore();  //  UI Core requires GameCore loaded
            await _sceneAnchorCore.StartCore();
            await _cameraController.StartCore();
            await _debuggingCore.StartCore();

            //  now that the entire scene should be loaded, start the player
            await _playerCore.StartPlayer();    
            Debug.Log($"Starting GameCore Complete");
        }

        public async UniTask ResetGame()
        {
            //  TODO: Register these in some sort of ordered list to automatically sequence loading/unloading them.
            Debug.Log($"Resetting GameCore");
            await _worldUnlockData.SaveData();  //  ensure that we've actually saved the game.
            await _debuggingCore.StopCore();
            await _cameraController.StopCore();
            await _sceneAnchorCore.StopCore();
            await _uiCore.StopCore();
            await _playerCore.StopCore();
            await _lightingCore.StopCore();
            Debug.Log($"Resetting GameCore Complete");
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            SceneManager.LoadScene("SplashScene");
            //await StartCore();
        }
    }
}