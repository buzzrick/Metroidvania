using Metroidvania.Player;

namespace Metroidvania.Interactables.PlayerZones
{
    public interface IPlayerExitTriggerZone 
    {
        void OnPlayerExitedZone(PlayerRoot player);
    }
}