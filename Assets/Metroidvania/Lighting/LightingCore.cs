using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.Lighting
{
    public class LightingCore : ICore
    {
        private readonly ISceneLoader _sceneLoader;
        private LightingController _lightingController;

        public LightingCore(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting LightingCore");
            _lightingController = await _sceneLoader.LoadUISceneAsync<LightingController>("LightingScene", false);
            await _lightingController.StartCore();
        }

        public void EnableLightSetup(string setupName) 
        {
            _lightingController.EnableLightSetup(setupName);
        }

    }
}