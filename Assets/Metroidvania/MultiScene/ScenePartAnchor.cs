﻿using Cysharp.Threading.Tasks;
using Metroidvania.Interactables;
using Metroidvania.Player;
using UnityEngine;
using Zenject;

namespace Metroidvania.MultiScene
{
    public class ScenePartAnchor : MonoBehaviour, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        public float LoadRange = 25f;

        private bool _isLoaded;
        private bool _shouldLoad;
        private ISceneLoader _sceneLoader;
        private SceneAnchorController _sceneAnchorController;

        [Inject]
        private void Initialise(ISceneLoader sceneLoader, SceneAnchorController sceneAnchorController)
        {
            _sceneLoader = sceneLoader;
            _sceneAnchorController = sceneAnchorController;

            _isLoaded = _sceneLoader.IsSceneLoaded(gameObject.name);
            //  handle the case wehre we were already loaded
            if (_isLoaded)
            {
                _sceneAnchorController.HandleSceneAnchorLoaded(gameObject.name);
            }
        }

        private async void Update()
        {
            await TriggerCheck();
        }


        private async UniTask TriggerCheck()
        {
            if (_shouldLoad)
            {
                await LoadScene();
            }
            else
            {
                await UnloadScene();
            }
        }

        private async UniTask LoadScene()
        {
            if (!_isLoaded)
            {
                //Debug.Log($"Loading Scene {gameObject.name} via SceneAnchor");
                _isLoaded = true;
                _sceneAnchorController.HandleSceneAnchorLoading(gameObject.name);
                await _sceneLoader.LoadAdditiveSceneAsync(gameObject.name);
                _sceneAnchorController.HandleSceneAnchorLoaded(gameObject.name);
            }
        }

        private async UniTask UnloadScene()
        {
            if (_isLoaded)
            {
                _isLoaded = false;
                await _sceneLoader.UnloadSceneAsync(gameObject.name, null);
                _sceneAnchorController.HandleSceneAnchorUnLoaded(gameObject.name);
            }
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            //Debug.Log($"OnPlayerExitedZone {name}");
            _shouldLoad = false;
        }

        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            //Debug.Log($"OnPlayerEnteredZone {name}");
            _shouldLoad = true;
        }
    }
}

