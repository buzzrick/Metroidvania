using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.MultiScene
{
    public class SceneLoader : ISceneLoader
    {
        private static Dictionary<string, Scene> _loadedScenes = new ();
        private static IList<string> _autoUnloadUiScenes = new List<string>();
        public event Action OnSceneUnloading;
        public string CurrentScene => SceneManager.GetActiveScene().name;

        public UniTask StartCore()
        {
            Debug.Log($"Starting SceneLoader");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log($"Found {scene.name} already loaded");
                _loadedScenes.Add(scene.name, scene);
            }

            return UniTask.CompletedTask;
        }

        public async UniTask LoadMainSceneAsync(string sceneName)
        {
            if (!Application.isPlaying) return;
            Debug.Log($"Loading Scene {sceneName}");
            OnSceneUnloading?.Invoke();
            await AutoUnloadUIScenes();
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        public async UniTask SwitchMainScene(string newScene)
        {
            if (!Application.isPlaying) return;
            Scene currentMainScene = SceneManager.GetActiveScene();
            if (newScene != currentMainScene.name)
            {
                await LoadMainSceneAsync(newScene);
                await SceneManager.UnloadSceneAsync(currentMainScene);
            }
        }


        public async UniTask LoadAdditiveSceneAsync(string sceneName, bool autoUnloadOnSceneChange = false)
        {
            if (!IsSceneLoaded(sceneName))
            {
                var loadTask = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                await loadTask;
                _loadedScenes.Add(sceneName, SceneManager.GetSceneByName(sceneName));
                if (autoUnloadOnSceneChange)
                {
                    _autoUnloadUiScenes.Add(sceneName);
                }
            }
        }

        public async UniTask<T> LoadUISceneAsync<T>(string uiScene, bool autoUnloadOnSceneChange = true)
            where T : class, IView
        {
            await LoadAdditiveSceneAsync(uiScene, autoUnloadOnSceneChange);


            Debug.Log($"Getting object from {uiScene}");
            //  find the object we need with the same name as the SceneName
            //  TODO: see if we can cache this.

            // Changed this slightly to handle cases where the root gameobject doesn't have the same name as the 
            // the scene, or is nested.
            Scene scene = SceneManager.GetSceneByName(uiScene);
            GameObject[] rootObjects = scene.GetRootGameObjects();
            //  look in root objects
            foreach (GameObject gameObject in rootObjects)
            {
                T targetComponent = gameObject.GetComponent<T>();
                if (targetComponent != null)
                {
                    return targetComponent;
                }
            }
            //  not found in root objects, try the children
            foreach (GameObject gameObject in rootObjects)
            {
                T targetComponent = gameObject.GetComponentInChildren<T>();
                if (targetComponent != null)
                {
                    return targetComponent;
                }
            }

            // GameObject targetObject = GameObject.Find(uiScene);
            // if (targetObject != null)
            // {
            //     T targetComponent = targetObject.GetComponent<T>();
            //     if (targetComponent == null)
            //     {
            //         string typeName = typeof(T).ToString();
            //         Debug.LogWarning($"Unable to find component of type {typeName} in object {targetObject?.name ?? "NULL"} at path {uiScene}");
            //     }
            //     return targetComponent;
            // }


            Debug.LogWarning($"Unable to find UI Scene object of type {typeof(T).Name}at path {uiScene}");
            return null;
        }

        public async UniTask<T> LoadUISceneAsync<T>(string uiScene, string objectPath, bool autoUnloadOnSceneChange = true)
            where T : class, IView
        {
            await LoadAdditiveSceneAsync(uiScene, autoUnloadOnSceneChange);

            GameObject targetObject = GameObject.Find(objectPath);
            if (targetObject != null)
            {
                T targetComponent = targetObject.GetComponent<T>();
                if (targetComponent != null)
                {
                    return targetComponent;
                }
            }

            string typeName = typeof(T).ToString();
            Debug.LogWarning($"Unable to find component of type {typeName} in object {targetObject?.name ?? "NULL"} at path {uiScene}");
            return null;
        }


        public bool IsSceneLoaded(string scene)
        {
            return _loadedScenes.ContainsKey(scene);
        }


        public async UniTask UnloadSceneAsync(string uiScene, IView view)
        {
            //Debug.Log($"UnloadUIAsync {ui}");
            // Debug.LogFormat("UnloadUIAsync:{0}", uiScene);
            if (!Application.isPlaying) return;
            if (IsSceneLoaded(uiScene))
            {
                if (view != null)
                {
                    await view.CleanupSelf();
                }
                _loadedScenes.Remove(uiScene);
                _autoUnloadUiScenes.Remove(uiScene);
                await SceneManager.UnloadSceneAsync(uiScene);
            }
        }

        public async UniTask UnloadAllScenesAndReloadScene(string sceneName)
        {
            Debug.Log($"Unloading all scenes then reloading {sceneName}");
            await AutoUnloadUIScenes();

            await LoadMainSceneAsync(sceneName);
        }

        private async UniTask AutoUnloadUIScenes()
        {
            List<UniTask> unloaders = new List<UniTask>(_autoUnloadUiScenes.Count);

            foreach (string uiScene in _autoUnloadUiScenes)
            {
                unloaders.Add(UnloadSceneAsync(uiScene, null));
            }
            await UniTask.WhenAll(unloaders);
        }

        public bool IsCurrentMainScene(string sceneName) => CurrentScene == sceneName;

    }
}

