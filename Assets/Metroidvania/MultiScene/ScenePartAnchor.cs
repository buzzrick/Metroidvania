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
        private PlayerRoot _playerRoot;
        private MultiSceneLoader _multiSceneLoader;

        [Inject]
        private void Initialise(PlayerRoot playerRoot, MultiSceneLoader multiSceneLoader)
        {
            _playerRoot = playerRoot;
            _multiSceneLoader = multiSceneLoader;

            _isLoaded = _multiSceneLoader.IsSceneLoaded(gameObject.name);
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
                await _multiSceneLoader.LoadScene(gameObject.name);
            }
        }

        private async void UnloadScene()
        {
            if (_isLoaded)
            {
                _isLoaded = false;
                await _multiSceneLoader.UnloadScene(gameObject.name);
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

