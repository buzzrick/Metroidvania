using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.UI;

namespace Metroidvania.World
{
    public class WorldManager
    {
        private readonly GameLifecycleManager _gameLifecycleManager;
        private readonly WorldUnlockData _worldUnlockData;
        private readonly UICore _uiCore;

        public WorldManager(GameLifecycleManager gameLifecycleManager,
            WorldUnlockData worldUnlockData,
            UICore uiCore)
        {
            _gameLifecycleManager = gameLifecycleManager;
            _worldUnlockData = worldUnlockData;
            _gameLifecycleManager.OnGamePaused += HandleOnGamePaused;
            _gameLifecycleManager.OnGameQuit += HandleOnGameQuit;
            _uiCore = uiCore;
            _uiCore.RegisterListener("SaveWorldData", OnSaveRequested);
        }

        private void OnSaveRequested()
        {
            SaveData().Forget();
        }

        private void HandleOnGameQuit()
        {
            SaveData().Forget();
        }

        private void HandleOnGamePaused(bool isPaused)
        {
            if (isPaused)
            {
                SaveData().Forget();
            }
        }

        public async UniTask SaveData()
        {
            await _worldUnlockData.SaveData();
        }
    }
}