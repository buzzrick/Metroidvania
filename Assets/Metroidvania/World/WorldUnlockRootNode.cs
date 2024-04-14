#nullable enable
using Cysharp.Threading.Tasks;
using Metroidvania.Debugging;
using Metroidvania.MultiScene;
using Metroidvania.Characters.Player;
using Metroidvania.UI;
using UnityEngine;
using Zenject;

namespace Metroidvania.World
{
    public class WorldUnlockRootNode : WorldUnlockNodeBase, IView
    {
        private WorldUnlockData _worldData = default!;
        private PlayerCore _playerCore = default!;
        private UICore? _uiCore;
        public string ZoneID = "ZoneID";
        public Vector3 PlayerStartPosition = Vector3.zero;
        [Tooltip("If this is a child scene (ie: not the World Root scene), then don't trigger player teleports etc")]
        public bool IsChildScene;

        [Inject]
        private void Initialise(WorldUnlockData worldData,
            UICore  uiCore,
            PlayerCore playerCore, 
            DebuggingCore debuggingCore)
        {
            _worldData = worldData;
            _uiCore = uiCore;
            _playerCore = playerCore;
            if (!IsChildScene )
            {
                debuggingCore.RegisterRootNode(this);
            }
        }

        private void OnEnable()
        {
            if (!IsChildScene)
            {
                _uiCore.RegisterListener("ResetPosition", TeleportPlayerToStartPosition);
            }
        }

        private void OnDisable()
        {
            if (!IsChildScene)
            {
                _uiCore.UnregisterListener("ResetPosition", TeleportPlayerToStartPosition);
            }
        }

        // Start is called before the first frame update
        private async void Start()
        {
            //await _gameCore.StartCore();
            LoadData(ZoneID, _worldData, null, true);
            await _worldData.SaveData();
            TeleportPlayerToStartPosition();
            if (!HasUnlockedChildren() && _unlockCutscene != null)
            {
                _unlockCutscene.RunCutscene().Forget();
            }
        }

        public void DebugResetWorldData()
        {
            _worldData.ResetData();
            // _playerCore.GetPlayerRoot().PlayerInventoryManager.ResetInventory();
            Start();
        }

        private void TeleportPlayerToStartPosition()
        {
            if (!IsChildScene)
            {
                _playerCore.GetPlayerRoot().SetWorldPosition(PlayerStartPosition);
            }
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

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }
    }
}