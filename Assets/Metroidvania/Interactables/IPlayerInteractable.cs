using Cysharp.Threading.Tasks;
using Metroidvania.Characters.Player.Animation;

namespace Metroidvania.Interactables
{
    public interface IPlayerInteractable
    {
        bool IsInteractionEnabled { get; }

        InteractionActionType GetInteractionType();
        UniTask<bool> Interact(InteractionActionType interactionActionType);
    }
}