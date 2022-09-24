using Cysharp.Threading.Tasks;

namespace Metroidvania.GameCore
{
    public interface ICore 
    {
        UniTask StartCore();
    }
}