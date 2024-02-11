using Metroidvania.Interactables;
using Metroidvania.Lighting;
using Metroidvania.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Metroidvania.Cameras
{
    [RequireComponent(typeof(Collider))]
    public class CameraZone : MonoBehaviour, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        public string CameraName;
        public CameraNames Camera = CameraNames.None;
        public int CameraPriority = 10;
        [Tooltip("The scene which should be set active to load the correct LightSettings when in this trigger zone")]
        public string ActiveLightingScene;
        [Tooltip("The set of lights that should be active when in this trigger zone")]
        public string ActiveLightSetup;

        private CameraController _cameraController;
        private LightingCore _lightingCore;

        [Inject]
        private void Initialise(CameraController cameraController,
            LightingCore lightingCore)
        {
            _cameraController = cameraController;
            _lightingCore = lightingCore;
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

            _lightingCore.EnableLightSetup(ActiveLightSetup, name);
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            _cameraController.ClearPrioritisedCamera(this);
            _lightingCore.DisableLightSetup(ActiveLightSetup, name);
        }

        private void SetDefaultScene()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameCoreScene"));
        }
    }
}