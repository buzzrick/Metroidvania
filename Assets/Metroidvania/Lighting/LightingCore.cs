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

        public async UniTask StopCore()
        {
            await _sceneLoader.UnloadSceneAsync("LightingScene", _lightingController);
            _lightingController = null;
        }


        public void EnableLightSetup(string setupName, string sourceName) 
        {
            _lightingController.EnableLightSetup(setupName, sourceName);
        }

        public void DisableLightSetup(string setupName, string sourceName)
        {
            _lightingController.DisableLightSetup(setupName, sourceName);
        }

    }
}