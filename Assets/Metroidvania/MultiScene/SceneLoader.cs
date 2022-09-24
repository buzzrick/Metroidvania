using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.MultiScene
{
    public class SceneLoader : ISceneLoader
    {
        private static IList<string> _loadedUiScenes = new List<string>();
        private static IList<string> _autoUnloadUiScenes = new List<string>();
        public event Action OnSceneUnloading;
        public string CurrentScene => SceneManager.GetActiveScene().name;

        public async Task LoadMainSceneAsync(string sceneName)
        {
            if (!Application.isPlaying) return;
            Debug.Log($"Loading Scene {sceneName}");
            OnSceneUnloading?.Invoke();
            await AutoUnloadUIScenes();
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        public async Task SwitchMainScene(string newScene)
        {
            if (!Application.isPlaying) return;
            Scene currentMainScene = SceneManager.GetActiveScene();
            if (newScene != currentMainScene.name)
            {
                await LoadMainSceneAsync(newScene);
                await SceneManager.UnloadSceneAsync(currentMainScene);
            }
        }

        public async Task<T> LoadUISceneAsync<T>(string uiScene, bool autoUnloadOnSceneChange = true)
            where T : class, IView
        {
            if (!IsUILoaded(uiScene))
            {
                var loadTask = SceneManager.LoadSceneAsync(uiScene, LoadSceneMode.Additive);
                await loadTask;
                _loadedUiScenes.Add(uiScene);
                if (autoUnloadOnSceneChange)
                {
                    _autoUnloadUiScenes.Add(uiScene);
                }
            }

            //  find the object we need with the same name as the SceneName
            //  TODO: see if we can cache this.

            // Changed this slightly to handle cases where the root gameobject doesn't have the same name as the 
            // the scene, or is nested.
            Scene scene = SceneManager.GetSceneByName(uiScene);
            GameObject[] gameObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in gameObjects)
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


            Debug.LogWarning($"Unable to find UI Scene object at path {uiScene}");
            return null;
        }


        public bool IsUILoaded(string uiScene)
        {
            return _loadedUiScenes.Contains(uiScene);
        }


        public async Task UnloadUIAsync(string uiScene, IView view)
        {
            //Debug.Log($"UnloadUIAsync {ui}");
            // Debug.LogFormat("UnloadUIAsync:{0}", uiScene);
            if (!Application.isPlaying) return;
            if (IsUILoaded(uiScene))
            {
                if (view != null)
                {
                    await view.CleanupSelf();
                }
                _loadedUiScenes.Remove(uiScene);
                _autoUnloadUiScenes.Remove(uiScene);
                await SceneManager.UnloadSceneAsync(uiScene);
            }
        }

        public async Task UnloadAllScenesAndReloadScene(string sceneName)
        {
            Debug.Log($"Unloading all scenes then reloading {sceneName}");
            await AutoUnloadUIScenes();

            await LoadMainSceneAsync(sceneName);
        }

        private async Task AutoUnloadUIScenes()
        {
            List<string> scenesToUnload = new List<string>(_loadedUiScenes);
            List<UniTask> unloaders = new List<UniTask>();

            foreach (string uiScene in _autoUnloadUiScenes)
            {
                unloaders.Add(UnloadUIAsync(uiScene, null).AsUniTask());
            }
            await UniTask.WhenAll(unloaders);
        }

        public bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene != null;
        }

        public bool IsCurrentMainScene(string sceneName) => CurrentScene == sceneName;

    }
}

