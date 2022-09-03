using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MultiSceneLoader : MonoBehaviour
{
    public string[] RequiredScenes;

    private async void Awake()
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

    public async UniTask LoadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene != null && scene.isLoaded)
        {
            Debug.Log($"Loading Scene {sceneName} (already loaded)");
            return;
        }

        Debug.Log($"Loading Scene {sceneName}");

        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
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
}
