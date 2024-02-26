using Metroidvania.Characters.Player;

namespace Metroidvania.Interactables
{
    public interface IPlayerExitTriggerZone 
    {
        void OnPlayerExitedZone(PlayerRoot player);
    }
}