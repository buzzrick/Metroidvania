using Metroidvania.Player;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Metroidvania.MultiScene
{
    public class ScenePartAnchor : MonoBehaviour
    {
        public enum SceneLoadCheckMethod
        {
            Distance,
            Trigger
        }

        public SceneLoadCheckMethod CheckMethod;
        public float LoadRange = 25f;

        private bool _isLoaded;
        private bool _shouldLoad;

        [Inject]
        private PlayerRoot PlayerRoot;

        private void Awake()
        {
            if (SceneManager.sceneCount > 0)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.name == gameObject.name)
                    {
                        _isLoaded = true;
                        break;
                    }
                }
            }
        }

        private void Update()
        {
            switch (CheckMethod)
            {
                case SceneLoadCheckMethod.Distance:
                    DistanceCheck();
                    break;
                case SceneLoadCheckMethod.Trigger:
                    TriggerCheck();
                    break;
            }
        }


        private void DistanceCheck()
        {
            if (Vector3.Distance(PlayerRoot.transform.position, transform.position) < LoadRange)
            {
                LoadScene();
            }
            else
            {
                UnloadScene();
            }
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

        private void LoadScene()
        {
            if (!_isLoaded)
            {
                SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
                _isLoaded = true;
            }
        }

        private void UnloadScene()
        {
            if (_isLoaded)
            {
                SceneManager.UnloadSceneAsync(gameObject.name);
                _isLoaded = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("LevelLoader"))
            {
                Debug.Log($"OnEnterTrigger Anchor:{name}, other:{other.name}");
                _shouldLoad = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("LevelLoader"))
            {

                Debug.Log($"OnExitTrigger Anchor:{name}, other:{other.name}");
                _shouldLoad = false;
            }
        }
    }
}

