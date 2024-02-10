#nullable enable
using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using Metroidvania.Player.Animation;
using Metroidvania.Player.Inventory;
using UnityEngine;
using Zenject;

namespace Metroidvania.Player
{
    public class PlayerRoot : MonoBehaviour, ICore, IView
    {
        PlayerMovementController_NoIK _playerMovementOld;
        PlayerTriggerDetector _playerTriggerDetector;
        public bool UseOldPlayerMovementController = false;
        private PlayerMovementController _playerMovement;
        [SerializeField, RequiredField] private Transform _cameraTarget = default!;
        public Transform CameraTarget => _cameraTarget;

        [Inject] private PlayerInventoryManager _playerInventoryManager = default!;
        

        /// <summary>
        /// Accessors
        /// </summary>
        public PlayerInventoryManager PlayerInventoryManager => _playerInventoryManager;
        public PlayerAnimationView PlayerAnimationView => _playerMovement.PlayerAnimationView;

        private void Awake()
        {
            if (UseOldPlayerMovementController)
            {
                _playerMovementOld = GetComponent<PlayerMovementController_NoIK>();
                _playerMovementOld.Enable(false);
            }
            else
            {
                _playerMovement = GetComponent<PlayerMovementController>();
                _playerMovement.Enable(false);
            }
            _playerTriggerDetector = GetComponent<PlayerTriggerDetector>();
        }

        public bool IsStarted { get; private set; }

        private void Start()
        {
            if (UseOldPlayerMovementController)
            {
                _playerMovementOld.Enable(false);
            }
            else
            {
                _playerMovement.Enable(false);
            }
        }

        public async UniTask LoadAllData()
        {
            await _playerInventoryManager.LoadData();
        }


        public async UniTask SaveAllData()
        {
            await _playerInventoryManager.SaveData();
        }

        public void SetWorldPosition(Vector3 position)
        {
            if (UseOldPlayerMovementController)
            {
                _playerMovementOld.Teleport(position);
            }
            else
            {
                Debug.LogWarning($"Hard coded character teleport - need to upgrade");
                _playerMovement.Motor.SetPosition(position, true);
            }
            _playerTriggerDetector.OnTeleport();
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting PlayerRoot");
            if (UseOldPlayerMovementController)
            {
                _playerMovementOld.Enable(true);
            }
            else
            {
                //await UniTask.Delay(1);
                _playerMovement.Enable(true);
            }
            await _playerInventoryManager.LoadData();
            IsStarted = true;
            //return UniTask.CompletedTask;
        }

        public UniTask CleanupSelf()
        {
            IsStarted = false;
            return UniTask.CompletedTask;
        }
    }
}