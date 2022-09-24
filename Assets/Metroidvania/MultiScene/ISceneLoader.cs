using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;

namespace Metroidvania.MultiScene
{
    public interface ISceneLoader : ICore
    {
        bool IsCurrentMainScene(string sceneName);
        bool IsSceneLoaded(string uiScene);
        UniTask LoadMainSceneAsync(string sceneName);
        UniTask LoadAdditiveSceneAsync(string sceneName, bool autoUnloadOnSceneChange = false);
        UniTask<T> LoadUISceneAsync<T>(string uiScene, string objectPath, bool autoUnloadOnSceneChange = true) where T : class, IView;
        UniTask<T> LoadUISceneAsync<T>(string uiScene, bool autoUnloadOnSceneChange = true) where T : class, IView;
        UniTask UnloadAllScenesAndReloadScene(string sceneName);
        UniTask UnloadSceneAsync(string uiScene, IView view);
        UniTask SwitchMainScene(string newScene);
    }
}
