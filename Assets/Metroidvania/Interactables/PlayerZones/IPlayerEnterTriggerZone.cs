using Metroidvania.Player;

namespace Metroidvania.Interactables.PlayerZones
{
    public interface IPlayerEnterTriggerZone
    {
        void OnPlayerEnteredZone(PlayerRoot player);
    }
}