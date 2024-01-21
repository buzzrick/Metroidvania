using Cysharp.Threading.Tasks;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.UI
{

    public class UIView : MonoBehaviour, IView
    {
        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        internal UniTask StartCore()
        {
            return UniTask.CompletedTask;
        }
    }
}