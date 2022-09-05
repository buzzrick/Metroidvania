using Metroidvania.Interactables;
using Metroidvania.Player;
using UnityEngine;
using Zenject;

namespace Metroidvania.Camera
{
    [RequireComponent(typeof(Collider))]
    public class CameraZone : MonoBehaviour, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        public string CameraName;

        private CameraController _cameraController;

        [Inject]
        private void Initialise(CameraController cameraController)
        {
            _cameraController = cameraController;
        }

        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            _cameraController.SetPrioritisedCamera(this);
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            _cameraController.ClearPrioritisedCamera(this);
        }
    }
}