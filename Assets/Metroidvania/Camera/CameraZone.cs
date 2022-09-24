using Metroidvania.Interactables;
using Metroidvania.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Metroidvania.Camera
{
    [RequireComponent(typeof(Collider))]
    public class CameraZone : MonoBehaviour, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        public string CameraName;
        public string ActiveLightingScene;

        private CameraController _cameraController;

        [Inject]
        private void Initialise(CameraController cameraController)
        {
            _cameraController = cameraController;
        }

        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            _cameraController.SetPrioritisedCamera(this);

            //  switch the active scene. This causes the current LightingSettings to change
            if (!string.IsNullOrEmpty(ActiveLightingScene))
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(ActiveLightingScene));
            }
            else
            {
                SetDefaultScene();
            }
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            _cameraController.ClearPrioritisedCamera(this);
        }

        private void SetDefaultScene()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameCoreScene"));
        }
    }
}