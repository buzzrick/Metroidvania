using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MultiSceneLoader : MonoBehaviour
{
    public string[] RequiredScenes;

    private List<string> _loadedScenes = new();

    private void Awake()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded)
                _loadedScenes.Add(scene.name);
        }
    }

    private async void Start()
    {
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
        if (_loadedScenes.Contains(sceneName))
        {
            await SceneManager.UnloadSceneAsync(sceneName);
            _loadedScenes.Remove(sceneName);
        }
    }

    public async UniTask LoadScene(string sceneName)
    {
        if (_loadedScenes.Contains(sceneName))
        {
            return;
        }

        Debug.Log($"Loading Scene {sceneName}");
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        _loadedScenes.Add(sceneName);
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
        return _loadedScenes.Contains(name);
    }
}
