using Metroidvania.Player;
using UnityEngine;

public class PlayerTriggerDetector : MonoBehaviour
{
    private PlayerRoot _playerRoot;

    private void Awake()
    {
        _playerRoot = GetComponentInParent<PlayerRoot>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Player OnTriggerEnter {other.name}");
        var triggerZone = other.GetComponent<IPlayerEnterTriggerZone>();
        if (triggerZone != null)
        {

            Debug.Log($"Triggering {other.name}");
            triggerZone.OnPlayerEnteredZone(_playerRoot);
        }
    }
}
