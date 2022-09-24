using Cysharp.Threading.Tasks;
using Metroidvania.Lighting;
using Metroidvania.MultiScene;
using Metroidvania.Player;
using UnityEngine;

namespace Metroidvania.GameCore
{
    public class GameCore : ICore
    {
        private readonly LightingCore _lightingCore;
        private readonly MultiSceneLoader _multiSceneLoader;
        private readonly PlayerRoot _playerRoot;

        public GameCore(LightingCore lightingCore,
            MultiSceneLoader multiSceneLoader,
            PlayerRoot playerRoot)
        {
            _lightingCore = lightingCore;
            _multiSceneLoader = multiSceneLoader;
            _playerRoot = playerRoot;
            StartCore().Forget();
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting GameCore");
            await _lightingCore.StartCore();
            await _multiSceneLoader.StartCore();
            await _playerRoot.StartCore();
            Debug.Log($"Starting GameCore Complete");
        }
    }
}