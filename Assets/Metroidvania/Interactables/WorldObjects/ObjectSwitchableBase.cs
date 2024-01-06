using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public abstract class ObjectSwitchableBase : MonoBehaviour
    {
        public bool DefaultToggleState = false;

        private void Awake()
        {
            SetSwitchState(DefaultToggleState);
        }

        public void ToggleSwitchState()
        {
            SetSwitchState(!GetSwitchState());
        }

        public abstract bool GetSwitchState();

        public abstract void SetSwitchState(bool state);
    }
}