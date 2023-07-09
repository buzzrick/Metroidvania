using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


namespace Metroidvania.Player.Animation
{
    public class PlayerAnimationActionsView
    {
        private PlayerAnimationView _playerAnimationView;
        private Animator _animator;
        private int _actionLayerID;
        public readonly int HashActionInteract = Animator.StringToHash("ActionInteract");
        public readonly int HashActionChopDiagonal = Animator.StringToHash("ActionChopDiagonal");
        public readonly int HashActionMining = Animator.StringToHash("ActionMining");

        private float _timer = 0f;

        public PlayerAnimationActionsView(PlayerAnimationView playerAnimationView, Animator animator)
        {
            _playerAnimationView = playerAnimationView;
            _animator = animator;
            _actionLayerID = _animator.GetLayerIndex("ActionLayer");

            GetRightHandBone();
        }

        private void GetRightHandBone()
        {
            foreach (var bone in _animator.avatar.humanDescription.human)
            {
                if (bone.humanName == "RightHand")
                {
                    Debug.Log(bone.boneName);
                }
            }
        }

        public void StartActionAnimation(int actionID)
        {
            _animator.SetTrigger(actionID);
            SetToolForAnimation(actionID);

            FadeInOutLayerWeight(2);
        }

        private void SetToolForAnimation(int actionID)
        {
            if (actionID == HashActionChopDiagonal)
            {

            }
        }

        private async void FadeInOutLayerWeight(float seconds, float maxWeight = 1f)
        {
            _timer = seconds;
            _animator.SetLayerWeight(_actionLayerID, maxWeight);
            await UniTask.WaitUntil(()=>!IsActionAnimationRunning());
            _animator.SetLayerWeight(_actionLayerID, 0);
        }

        public void Tick()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
        }

        public bool IsActionAnimationRunning()
        {
            return _timer > 0f;
        }

        public void Reset()
        {
            _animator.SetLayerWeight(_actionLayerID, 0);
        }
    }
}