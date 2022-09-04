using Metroidvania.Interactables.PlayerZones;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerTriggerDetector : MonoBehaviour
    {
        private PlayerRoot _playerRoot;

        private void Awake()
        {
            _playerRoot = GetComponentInParent<PlayerRoot>();
        }

        private void OnTriggerEnter(Collider other)
        {
            IPlayerEnterTriggerZone triggerZone = other.GetComponent<IPlayerEnterTriggerZone>();
            if (triggerZone != null)
            {

                Debug.Log($"Player Triggering {other.name}", other);
                triggerZone.OnPlayerEnteredZone(_playerRoot);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IPlayerExitTriggerZone triggerZone = other.GetComponent<IPlayerExitTriggerZone>();
            if (triggerZone != null)
            {

                Debug.Log($"Player Triggering {other.name}", other);
                triggerZone.OnPlayerExitedZone(_playerRoot);
            }
        }
    }
}