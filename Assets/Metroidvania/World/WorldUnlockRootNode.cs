#nullable enable
using Cysharp.Threading.Tasks;
using Metroidvania.Player;
using Metroidvania.UI;
using UnityEngine;
using Zenject;

namespace Metroidvania.World
{
    public class WorldUnlockRootNode : WorldUnlockNodeBase
    {
        private WorldUnlockData _worldData = default!;
        private GameCore.GameCore _gameCore = default!;
        private PlayerCore _playerCore = default!;
        private UICore? _uiCore;
        public string ZoneID = "ZoneID";
        public Vector3 PlayerStartPosition = Vector3.zero;

        [Inject]
        private void Initialise(WorldUnlockData worldData,
            GameCore.GameCore gameCore,
            UICore  uiCore,
            PlayerCore playerCore)
        {
            _worldData = worldData;
            _gameCore = gameCore;
            _uiCore = uiCore;
            _playerCore = playerCore;
        }

        private void OnEnable()
        {
            _uiCore.RegisterListener("ResetPosition", TeleportPlayerToStartPosition);
        }

        private void OnDisable()
        {
            _uiCore.UnregisterListener("ResetPosition", TeleportPlayerToStartPosition);
        }

        // Start is called before the first frame update
        private async void Start()
        {
            //await _gameCore.StartCore();
            LoadData(ZoneID, _worldData, null, true);
            await _worldData.SaveData();
            TeleportPlayerToStartPosition();
        }

        public void DebugResetWorldData()
        {
            _worldData.ResetWorldData();
            // _playerCore.GetPlayerRoot().PlayerInventoryManager.ResetInventory();
            Start();
        }

        private void TeleportPlayerToStartPosition()
        {
            _playerCore.GetPlayerRoot().SetWorldPosition(PlayerStartPosition);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldUnlockData"></param>
        /// <param name="zoneID"></param>
        /// <param name="parentIsActive">Never false for the root node</param>
        public override void LoadData(string zoneID, WorldUnlockData worldUnlockData, WorldUnlockData.WorldUnlockNodeData parentData, bool firstLoad)
        {
            //  because this is the root node, this needs to be automatically set active;
            worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID).IsUnlocked = true;
            base.LoadData(zoneID, worldUnlockData, parentData, firstLoad);
        }

    }
}