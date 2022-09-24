using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.MultiScene
{
    public class MultiSceneLoader : MonoBehaviour, ICore
    {
        public string[] RequiredScenes;

        private Dictionary<string, Scene> _loadedScenes = new();

        public async UniTask StartCore()
        {
            Debug.Log($"Starting MultiSceneLoader");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    Debug.Log($"Found {scene.name} already loaded");
                    _loadedScenes.Add(scene.name, scene);
                }
            }

            await LoadRequiredScenes();
        }



        private async UniTask LoadRequiredScenes()
        {
            foreach (string sceneName in RequiredScenes)
            {
                await LoadScene(sceneName);
            }
        }

        public async UniTask UnloadScene(string sceneName)
        {
            if (IsSceneLoaded(sceneName))
            {
                await SceneManager.UnloadSceneAsync(sceneName);
                _loadedScenes.Remove(sceneName);
            }
        }

        public async UniTask LoadScene(string sceneName)
        {
            if (IsSceneLoaded(sceneName))
            {
                return;
            }

            Debug.Log($"Loading Scene {sceneName}");
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _loadedScenes.Add(sceneName, SceneManager.GetSceneByName(sceneName));
        }

        public async UniTask<T> LoadSceneAndGetObject<T>(string sceneName, string objectPath)
            where T : MonoBehaviour
        {
            await LoadScene(sceneName);
            GameObject targetObject = GameObject.Find(objectPath);
            if (targetObject != null)
            {
                return targetObject.GetComponent<T>();
            }
            return null;
        }

        public bool IsSceneLoaded(string name)
        {
            return _loadedScenes.ContainsKey(name);
        }
    }
}