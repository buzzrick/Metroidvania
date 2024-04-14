#nullable enable
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;

namespace Metroidvania.UI
{

    public class UICore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private UIView? _uiView;
        private Dictionary<string, Action> _pendingListeners = new Dictionary<string, Action>();

        public UICore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartCore()
        {
            _uiView = await _sceneLoader.LoadUISceneAsync<UIView>("UIView", false);
            await _uiView.StartCore();

            LoadPendingListeners();
        }

        private void LoadPendingListeners()
        {
            foreach (var pendingListener in _pendingListeners)
            {
                _uiView!.RegisterListener(pendingListener.Key, pendingListener.Value);
            }
            _pendingListeners.Clear();
        }

        public void RegisterListener(string messageID, Action callback)
        {
            if (_uiView == null)
            {
                _pendingListeners.Add(messageID, callback);   
            }
            else
            {
                _uiView!.RegisterListener(messageID, callback);
            }
        }
        public void UnregisterListener(string messageID, Action callback)
        {
            _uiView!.UnregisterListener(messageID, callback);
        }
    }
}