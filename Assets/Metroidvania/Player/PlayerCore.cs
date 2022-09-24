using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerCore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private PlayerRoot _playerRoot;

        public PlayerCore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting PlayerCore");
            _playerRoot = await _sceneLoader.LoadUISceneAsync<PlayerRoot>("PlayerScene", false);
            await _playerRoot.StartCore();
        }

        public PlayerRoot GetPlayerRoot() => _playerRoot;
    }
}