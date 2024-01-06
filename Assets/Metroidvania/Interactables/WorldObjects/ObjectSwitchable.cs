using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public class ObjectSwitchable : ObjectSwitchableBase
    {
        public Animator SwitchAnimator;
        private readonly int SwitchHash = Animator.StringToHash("Switch");

        public override bool GetSwitchState()
        {
            return SwitchAnimator.GetBool(SwitchHash);
        }

        public override void SetSwitchState(bool state)
        {
            SwitchAnimator.SetBool(SwitchHash, state);
        }
    }
}