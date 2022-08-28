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
            Debug.Log($"Loading Scene {sceneName}");
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene != null && scene.isLoaded)
            {
                continue;
            }
            
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
