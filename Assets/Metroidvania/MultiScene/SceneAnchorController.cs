using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.MultiScene
{
    public class SceneAnchorController : MonoBehaviour, IView, ICore
    {
        private List<string> _loadedScenes = new();
        private List<string> _pendingScenes = new();

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        public void HandleSceneAnchorLoading(string sceneName)
        {
            _pendingScenes.Add(sceneName);
        }

        public void HandleSceneAnchorLoaded(string sceneName)
        {
            _pendingScenes.Remove(sceneName);
            _loadedScenes.Add(sceneName);
        }

        public void HandleSceneAnchorUnLoaded(string sceneName)
        {
            _loadedScenes.Remove(sceneName);
        }

        public async UniTask StartCore()
        {
            await UniTask.WaitUntil(IsReadyToStartGame);
        }

        private bool IsReadyToStartGame()
        {
            return _loadedScenes.Count > 0 && _pendingScenes.Count == 0;
        }
    }
}