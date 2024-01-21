using Metroidvania.Interactables.ResourcePickups;
using Metroidvania.Player.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Metroidvania.Player
{
    /// <summary>
    /// Controls looking for pickups in the world
    /// Requires a collider on the player - this should be set up as a trigger, and much larger than then player's size
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PlayerPickupController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask = LayerMask.NameToLayer("PlayerPickup");
        private PlayerInventoryManager _playerInventoryManager;

        [Inject]
        private void Init(PlayerInventoryManager playerInventoryManager)
        {
            _playerInventoryManager = playerInventoryManager;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsInLayerMask(other.gameObject.layer, _layerMask))
            {
                if (other.TryGetComponent<ResourcePickup>(out var pickup))
                {
                    Debug.Log($"Player found {pickup.Amount} {pickup.ResourceType.name}");
                    _playerInventoryManager.IncrementInventory(pickup.ResourceType, pickup.Amount);
                    GameObject.Destroy(other.gameObject);
                }
            }
        }

        private bool IsInLayerMask(int objectLayer, LayerMask layerMask)
        {
            //  calculating layermask: https://forum.unity.com/threads/raycast-layermask-parameter.944194/#post-6161542
            return (layerMask & (1 << objectLayer)) != 0;
        }
    }
}