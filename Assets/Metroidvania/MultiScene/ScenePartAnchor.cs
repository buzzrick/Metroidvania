﻿using Metroidvania.Interactables;
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
        }

        private void Update()
        {
            TriggerCheck();
        }


        private void TriggerCheck()
        {
            if (_shouldLoad)
            {
                LoadScene();
            }
            else
            {
                UnloadScene();
            }
        }

        private async void LoadScene()
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                _sceneAnchorController.HandleSceneAnchorLoading(gameObject.name);
                await _sceneLoader.LoadAdditiveSceneAsync(gameObject.name);
                _sceneAnchorController.HandleSceneAnchorLoaded(gameObject.name);
            }
        }

        private async void UnloadScene()
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
            _shouldLoad = false;
        }

        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            _shouldLoad = true;
        }
    }
}

