using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerRoot : MonoBehaviour
    {
        ThirdPersonMovement _playerMovement;

        private void Awake()
        {
            _playerMovement = GetComponent<ThirdPersonMovement>();
        }

        public void SetWorldPosition(Vector3 position)
        {
            _playerMovement.Teleport(position);
        }
    }
}