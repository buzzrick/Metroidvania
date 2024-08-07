﻿using Cysharp.Threading.Tasks;
using Metroidvania.Characters.Player.Inventory;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.Characters.Player
{
    public class PlayerCore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly GameLifecycleManager _gameLifecycleManager;
        private PlayerRoot _playerRoot;
        public bool IsPlayerMoving => _playerRoot.IsPlayerMoving;

        public PlayerCore(ISceneLoader sceneLoader,
            GameLifecycleManager gameLifecycleManager)
        {
            _sceneLoader = sceneLoader;
            _gameLifecycleManager = gameLifecycleManager;
            _gameLifecycleManager.OnGamePaused += HandleOnGamePaused;
            _gameLifecycleManager.OnGameQuit += HandleOnGameQuit;
        }

        private void HandleOnGamePaused(bool isPaused)
        {
            if (isPaused)
            {
                SaveAllData().Forget();
            }
        }

        private void HandleOnGameQuit()
        {
            SaveAllData().Forget();
        }

        private async UniTask SaveAllData()
        {
            if (_playerRoot != null && _playerRoot.IsStarted)
            {
                await _playerRoot.SaveAllData();
            }
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting PlayerCore");
            _playerRoot = await _sceneLoader.LoadUISceneAsync<PlayerRoot>("PlayerScene", false);
            await _playerRoot.LoadAllData();
        }

        public async UniTask StopCore()
        {
            await _sceneLoader.UnloadSceneAsync("PlayerScene", _playerRoot);
            _playerRoot = null;
        }


        public PlayerRoot GetPlayerRoot() => _playerRoot;

        internal async UniTask StartPlayer()
        {
            await _playerRoot.StartCore();
        }

        public PlayerInventoryManager GetInventoryManager() => GetPlayerRoot().PlayerInventoryManager;
    }
}