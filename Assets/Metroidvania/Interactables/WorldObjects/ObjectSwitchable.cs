using Cysharp.Threading.Tasks;
using Metroidvania.Player.Animation;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public class ObjectSwitchable : ObjectSwitchableBase
    {
        public Animator SwitchAnimator;
        private ActiveAnimatorDetector _animationDetector;
        private readonly int SwitchHash = Animator.StringToHash("Switch");

        private void Awake()
        {
            _animationDetector = new ActiveAnimatorDetector(SwitchAnimator, 0);
        }

        public override bool GetSwitchState()
        {
            return SwitchAnimator.GetBool(SwitchHash);
        }

        public override async UniTask SetSwitchState(bool state)
        {
            SwitchAnimator.SetBool(SwitchHash, state);
            await UniTask.WaitUntil(() => _animationDetector.IsActionAnimationRunning());
            await UniTask.WaitUntil(() => !_animationDetector.IsActionAnimationRunning());
        }
    }
}