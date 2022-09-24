using Metroidvania.MultiScene;
using Metroidvania.Player;
using UnityEngine;
using Zenject;

namespace Metroidvania.Interactables.WorldObjects
{
    public class Portal : MonoBehaviour, IPlayerEnterTriggerZone
    {
        [SerializeField] private Transform ExitPoint;
        [SerializeField] private Portal TargetPortal;

        [SerializeField] private string TargetPortalScene;
        [SerializeField] private string TargetPortalName;

        private MultiSceneLoader _multiSceneLoader;

        public Vector3 GetExitPoint() => ExitPoint.position;

        [Inject]
        private void Initialise(MultiSceneLoader multiSceneLoader)
        {
            _multiSceneLoader = multiSceneLoader;
        }

        private void Awake()
        {
            if (TargetPortal == this)
            {
                Debug.LogError($"Portal {name} is targeting itself");
            }
        }

        public async void OnPlayerEnteredZone(PlayerRoot player)
        {
            if (TargetPortal != null)
            {
                player.SetWorldPosition(TargetPortal.GetExitPoint());
            }
            else if (!string.IsNullOrEmpty(TargetPortalScene)
                && !string.IsNullOrEmpty(TargetPortalName))
            {
                Portal targetPortal = await _multiSceneLoader.LoadSceneAndGetObject<Portal>(TargetPortalScene, TargetPortalName);
                player.SetWorldPosition(targetPortal.GetExitPoint());
            }
        }
    }
}