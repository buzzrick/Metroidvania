using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;


namespace Metroidvania.UI
{

    public class UICore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private UIView _uiView;

        public UICore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartCore()
        {
            _uiView = await _sceneLoader.LoadUISceneAsync<UIView>("UIView", false);
            await _uiView.StartCore();
        }
    }
}