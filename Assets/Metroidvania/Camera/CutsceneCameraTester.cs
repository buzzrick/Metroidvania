using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Characters.Player;
using Metroidvania.Interactables;
using UnityEngine;

namespace Assets.Metroidvania.Camera
{
    [RequireComponent(typeof(Collider))]
    public class CutsceneCameraTester : MonoBehaviour, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        [SerializeField, RequiredField] private CutsceneSimple _cutsceneSimple;

        public async void OnPlayerEnteredZone(PlayerRoot player)
        {
            await _cutsceneSimple.RunCutscene();
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            //await _cameraController.CancelCutscene();
        }
    }
}