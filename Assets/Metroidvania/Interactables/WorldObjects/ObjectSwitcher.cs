using Metroidvania.Player.Animation;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{

    [RequireComponent(typeof(Collider))]
    public class ObjectSwitcher : MonoBehaviour, IPlayerInteractable
    {
        public ObjectSwitchableBase[] ObjectsToSwitch;

        public InteractionActionType GetInteractionType() => InteractionActionType.Interact;

        public bool Interact(InteractionActionType interactionActionType)
        {
            if (interactionActionType == GetInteractionType())
            {
                Debug.Log($"Toggling Switcher {name}");
                if (ObjectsToSwitch.Length == 0)
                    return false;

                foreach (var switcher in ObjectsToSwitch)
                {
                    switcher.ToggleSwitchState();
                }

                return true;
            }

            return false;
        }
    }
}