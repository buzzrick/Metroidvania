using Cysharp.Threading.Tasks;
using Metroidvania.ResourceTypes;

namespace Metroidvania.Interactables
{
    public interface IPlayerInteractable
    {
        UniTask<bool> InteractAsync();
    }
}