using Cysharp.Threading.Tasks;

namespace Metroidvania.Interactables
{
    public interface IPlayerInteractable
    {
        UniTask<bool> InteractAsync();
    }
}