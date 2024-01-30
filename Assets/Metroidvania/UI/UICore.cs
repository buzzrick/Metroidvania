using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using System;

namespace Metroidvania.UI
{

    public class UICore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private UIView? _uiView;

        public UICore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartCore()
        {
            _uiView = await _sceneLoader.LoadUISceneAsync<UIView>("UIView", false);
            await _uiView.StartCore();
        }

        public void RegisterListener(string messageID, Action callback)
        {
            _uiView!.RegisterListener(messageID, callback);
        }
        public void UnregisterListener(string messageID, Action callback)
        {
            _uiView!.UnregisterListener(messageID, callback);
        }
    }
}