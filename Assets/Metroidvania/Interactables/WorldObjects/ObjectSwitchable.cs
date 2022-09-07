using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public class ObjectSwitchable : MonoBehaviour
    {
        public Animator SwitchAnimator;
        public bool DefaultToggleState = false;

        private readonly int SwitchHash = Animator.StringToHash("Switch");

        private void Awake()
        {
            SetSwitchState(DefaultToggleState);   
        }

        public void ToggleSwitchState()
        {
            SetSwitchState(!GetSwitchState());
        }

        public bool GetSwitchState()
        {
            return SwitchAnimator.GetBool(SwitchHash);
        }

        public void SetSwitchState(bool state)
        {
            SwitchAnimator.SetBool(SwitchHash, state);
        }
    }
}