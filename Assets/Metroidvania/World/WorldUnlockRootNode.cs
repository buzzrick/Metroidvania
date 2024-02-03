using Metroidvania.UI;
using Zenject;

namespace Metroidvania.World
{
    public class WorldUnlockRootNode : WorldUnlockNodeBase
    {
        private WorldUnlockData _worldData;
        private GameCore.GameCore _gameCore;
        private UICore _uiCore;
        public string ZoneID;

        [Inject]
        private void Initialise(WorldUnlockData worldData,
            GameCore.GameCore gameCore,
            UICore  uiCore)
        {
            _worldData = worldData;
            _gameCore = gameCore;
            _uiCore = uiCore;
        }

        private void OnEnable()
        {
            _uiCore.RegisterListener("ResetWorldData", DebugResetWorldData);
        }

        private void OnDisable()
        {
            _uiCore.UnregisterListener("ResetWorldData", DebugResetWorldData);
        }

        // Start is called before the first frame update
        private async void Start()
        {
            //await _gameCore.StartCore();

            LoadData(_worldData, ZoneID, true);
            await _worldData.SaveData();
        }

        public async void DebugResetWorldData()
        {
            _worldData.ResetWorldData();
            LoadData(_worldData, ZoneID, true);
            await _worldData.SaveData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldUnlockData"></param>
        /// <param name="zoneID"></param>
        /// <param name="parentIsActive">Never false for the root node</param>
        public override void LoadData(WorldUnlockData worldUnlockData, string zoneID, bool parentIsActive)
        {
            //  because this is the root node, this needs to be automatically set active;
            worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID).IsUnlocked = true;
            base.LoadData(worldUnlockData, zoneID, true);
        }

    }
}