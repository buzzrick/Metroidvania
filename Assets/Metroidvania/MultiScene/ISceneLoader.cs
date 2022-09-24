using System.Threading.Tasks;

namespace Metroidvania.MultiScene
{
    public interface ISceneLoader
    {
        bool IsCurrentMainScene(string sceneName);
        bool IsUILoaded(string uiScene);
        Task LoadMainSceneAsync(string sceneName);
        Task<T> LoadUISceneAsync<T>(string uiScene, bool autoUnloadOnSceneChange = true) where T : class, IView;
        Task UnloadAllScenesAndReloadScene(string sceneName);
        Task UnloadUIAsync(string uiScene, IView view);
        Task SwitchMainScene(string newScene);
    }
}
