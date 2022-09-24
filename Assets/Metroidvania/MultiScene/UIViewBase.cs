using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.MultiScene
{
    public abstract class UIViewBase : MonoBehaviour, IView
    {
        public GameObject rootContainer;
        public Image blocker;

        public virtual UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }
    }
}