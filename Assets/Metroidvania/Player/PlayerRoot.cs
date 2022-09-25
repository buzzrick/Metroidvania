using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerRoot : MonoBehaviour, ICore, IView
    {
        PlayerMovementController_NoIK _playerMovement;
        PlayerTriggerDetector _playerTriggerDetector;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovementController_NoIK>();
            _playerTriggerDetector = GetComponent<PlayerTriggerDetector>();
        }

        public void SetWorldPosition(Vector3 position)
        {
            _playerMovement.Teleport(position);
            _playerTriggerDetector.OnTeleport();
        }

        public UniTask StartCore()
        {
            Debug.Log($"Starting PlayerRoot");
            _playerMovement.Enable(true);
            return UniTask.CompletedTask;
        }

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }
    }
}