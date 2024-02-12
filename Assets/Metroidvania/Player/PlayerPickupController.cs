using Metroidvania.Interactables.ResourcePickups;
using Metroidvania.Player.Inventory;
using UnityEngine;
using Buzzrick.UnityLibs;
using CandyCoded.HapticFeedback;
using Zenject;

namespace Metroidvania.Player
{
    /// <summary>
    /// Controls looking for pickups in the world
    /// Requires a collider on the player - this should be set up as a trigger, and much larger than then player's size
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerPickupController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private AudioSource _pickupAudio;
        private PlayerInventoryManager _playerInventoryManager;

        [Inject]
        private void Init(PlayerInventoryManager playerInventoryManager)
        {
            _playerInventoryManager = playerInventoryManager;
        }

        private void OnTriggerEnter(Collider other)
        {
            DetectResourcePickup(other);
        }

        private void OnTriggerStay(Collider other)
        {
            DetectResourcePickup(other);
        }

        private void DetectResourcePickup(Collider other)
        {
            if (_layerMask.IsInLayerMask(other.gameObject.layer))
            {
                if (other.TryGetComponent<ResourcePickup>(out var pickup))
                {
                    if (!pickup.IsPickupAllowed)
                    {
                        return;
                    }
                    //Debug.Log($"Player found {pickup.Amount} {pickup.ResourceType.name}");
                    _playerInventoryManager.IncrementInventory(pickup.ResourceType, pickup.Amount);
                    GameObject.Destroy(other.gameObject);
                    _pickupAudio.Play();
                    HapticFeedback.LightFeedback();
                }
            }
        }
    }
}