﻿#nullable enable
using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.Characters.Base;
using Metroidvania.Characters.Player.Animation;
using Metroidvania.Characters.Player.Inventory;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using PsychoticLab;
using UnityEngine;
using Zenject;

namespace Metroidvania.Characters.Player
{
    public class PlayerRoot : MonoBehaviour, ICore, IView
    {
        PlayerMovementController_NoIK? _playerMovementOld;
        PlayerTriggerDetector _playerTriggerDetector = default!;
        public bool UseOldPlayerMovementController = false;
        [SerializeField, RequiredField] PlayerMovementInputs _playerMovementInputs = default!;
        private PlayerMovementController _playerMovement = default!;
        [SerializeField, RequiredField] private Transform _cameraTarget = default!;
        [SerializeField, RequiredField] private CharacterRandomizer _characterRandomizer = default!;
        public Transform CameraTarget => _cameraTarget;

        [Inject] private PlayerInventoryManager _playerInventoryManager = default!;

        public bool IsPlayerMoving => _playerMovementInputs.IsInputActive;
        
        /// <summary>
        /// Accessors
        /// </summary>
        public PlayerInventoryManager PlayerInventoryManager => _playerInventoryManager;
        public BaseAnimationView AnimationView => _playerMovement.AnimationView;
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
            _playerMovementInputs ??= GetComponent<PlayerMovementInputs>();
        }

        public bool IsStarted { get; private set; }

        private void Start()
        {
            if (UseOldPlayerMovementController)
            {
                _playerMovementOld!.Enable(false);
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
            var playerConfiguration = _characterRandomizer.GetConfiguration();
            string playerConfigJson = JsonUtility.ToJson(playerConfiguration);

            Debug.Log($"playerConfigJson:\n{playerConfigJson}");

            await _playerInventoryManager.SaveData();
        }

        public void SetWorldPosition(Vector3 position)
        {
            if (UseOldPlayerMovementController)
            {
                _playerMovementOld!.Teleport(position);
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
                _playerMovementOld!.Enable(true);
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