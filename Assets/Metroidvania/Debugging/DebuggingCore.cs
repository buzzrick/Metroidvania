using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;

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
            _debuggingView = await _sceneLoader.LoadUISceneAsync<DebuggingView>("DebuggingScene");
            await _debuggingView.StartCore();
        }
    }
}