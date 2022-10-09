using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.Debugging
{
    public class DebuggingCore : ICore
    {
        private ISceneLoader _sceneLoader;
        private DebuggingView _debuggingView;

        public DebuggingCore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }


        public async UniTask StartCore()
        {
            Debug.Log($"Starting DebuggingCore");
            _debuggingView = await _sceneLoader.LoadUISceneAsync<DebuggingView>("DebuggingScene");
            await _debuggingView.StartCore();
        }
    }
}