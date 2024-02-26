using Metroidvania.Characters.Player;

namespace Metroidvania.Interactables
{
    public interface IPlayerEnterTriggerZone
    {
        void OnPlayerEnteredZone(PlayerRoot player);
    }
}