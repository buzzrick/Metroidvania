using Cysharp.Threading.Tasks;

namespace Metroidvania.MultiScene
{
    public interface IView
    {
        UniTask CleanupSelf();
    }
}