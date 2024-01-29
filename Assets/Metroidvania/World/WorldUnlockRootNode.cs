using Cysharp.Threading.Tasks;
using Zenject;

namespace Metroidvania.World
{
    public class WorldUnlockRootNode : WorldUnlockNodeBase
    {
        private WorldUnlockData _worldData;
        private GameCore.GameCore _gameCore;
        public string ZoneID;

        [Inject]
        private void Initialise(WorldUnlockData worldData,
            GameCore.GameCore gameCore)
        {
            _worldData = worldData;
            _gameCore = gameCore;
        }

        // Start is called before the first frame update
        private async void Start()
        {
            //await _gameCore.StartCore();

            LoadData(_worldData, ZoneID);
            await _worldData.SaveData();
        }
    }
}