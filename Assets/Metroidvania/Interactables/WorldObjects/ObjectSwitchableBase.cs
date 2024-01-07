using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public abstract class ObjectSwitchableBase : MonoBehaviour
    {
        public bool DefaultToggleState = false;

        private void Awake()
        {
            SetSwitchState(DefaultToggleState).Forget();
        }

        public async UniTask ToggleSwitchState()
        {
            await SetSwitchState(!GetSwitchState());
        }

        public abstract bool GetSwitchState();

        public abstract UniTask SetSwitchState(bool state);

        public virtual bool IsSwitchAvailable => true;
    }
}