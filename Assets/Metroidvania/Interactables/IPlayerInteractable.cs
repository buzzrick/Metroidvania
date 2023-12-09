using Cysharp.Threading.Tasks;
using Metroidvania.Player.Animation;

namespace Metroidvania.Interactables
{
    public interface IPlayerInteractable
    {
        InteractionActionType GetInteractionType();
        bool Interact(InteractionActionType interactionActionType);
    }
}