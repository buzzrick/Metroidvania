using Metroidvania.Player;
using UnityEngine;

public class Portal : MonoBehaviour, IPlayerEnterTriggerZone
{
    [SerializeField] Transform ExitPoint;
    [SerializeField] Portal TargetPortal;

    public Vector3 GetExitPoint() => ExitPoint.position;

    private void Awake()
    {
        if (TargetPortal == this)
        {
            Debug.LogError($"Portal {name} is targeting itself");
        }
    }

    public void OnPlayerEnteredZone(PlayerRoot player)
    {
        if (TargetPortal != null)
        {
            player.SetWorldPosition(TargetPortal.GetExitPoint());
        }
    }
}
