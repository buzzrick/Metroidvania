using Metroidvania.Interactables;
using Metroidvania.Characters.Player;
using UnityEngine;

namespace Assets.Metroidvania.World
{
    public class WaterDetectionZone : MonoBehaviour, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            player.PlayerAnimationView.SetSwimming(true);
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            player.PlayerAnimationView.SetSwimming(false);
        }

    }
}