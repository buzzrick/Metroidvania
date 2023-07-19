using Cysharp.Threading.Tasks;
using KinematicCharacterController.Examples;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerRoot : MonoBehaviour, ICore, IView
    {
        PlayerMovementController_NoIK _playerMovementOld;
        PlayerTriggerDetector _playerTriggerDetector;
        public bool UseOldPlayerMovementController = false;
        private PlayerMovementController _playerMovement;

        private void Awake()
        {
            if (UseOldPlayerMovementController)
            {
                _playerMovementOld = GetComponent<PlayerMovementController_NoIK>();
            }
            else
            {
                _playerMovement = GetComponent<PlayerMovementController>();
            }
            _playerTriggerDetector = GetComponent<PlayerTriggerDetector>();
        }

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
                await UniTask.Delay(1);
                _playerMovement.Enable(true);
            }
            //return UniTask.CompletedTask;
        }

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }
    }
}