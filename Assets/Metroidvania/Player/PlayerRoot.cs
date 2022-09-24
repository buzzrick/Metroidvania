using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using System.Threading.Tasks;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerRoot : MonoBehaviour, ICore
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

        public UniTask StartCore()
        {
            Debug.Log($"Starting PlayerCore");
            _playerMovement.Enable(true);
            return UniTask.CompletedTask;
        }
    }
}