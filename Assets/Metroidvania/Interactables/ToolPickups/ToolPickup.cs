#nullable enable

using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Player;
using Metroidvania.Player.Animation;
using Metroidvania.Player.Inventory;
using UnityEngine;
using Zenject;

namespace Metroidvania.Interactables.ToolPickups
{
    [RequireComponent(typeof(Collider))]
    public class ToolPickup : MonoBehaviour, IPlayerEnterTriggerZone
    {
        public PlayerAnimationTool ToolType;
        [SerializeField, RequiredField] private Collider _collider = default!;
        [SerializeField, RequiredField] private MeshRenderer _meshRenderer = default!;
        private bool _isActive;
        private PlayerInventoryManager _playerInventoryManager = default!;

        [Inject]
        public void Initialise(PlayerInventoryManager playerInventoryManager)
        {
            _playerInventoryManager = playerInventoryManager;
            _playerInventoryManager.OnInventoryReset += ResetActiveStatus;
        }

        private void OnDestroy()
        {
            _playerInventoryManager.OnInventoryReset += ResetActiveStatus;
        }

        private void OnEnable()
        {
            ResetActiveStatus();
        }

        private void ResetActiveStatus()
        {
            _isActive = !_playerInventoryManager.IsToolUnlocked(ToolType);
            _collider.enabled = _isActive;
            _meshRenderer.enabled = _isActive;
        }

        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            if (_isActive)
            {
                _playerInventoryManager.SetToolUnlocked(ToolType, true);
                ResetActiveStatus();
            }
        }
    }
}
