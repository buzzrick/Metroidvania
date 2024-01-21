using UnityEngine;

namespace Metroidvania.Player.Animation
{

    /// <summary>
    /// Detects if ANY animation is playing on the given animation layer.
    /// </summary>
    public class ActiveAnimatorDetector
    {
        private int _actionLayerIndex;
        private Animator _animator;

        public ActiveAnimatorDetector(Animator animator, int actionLayerIndex)
        {
            _animator = animator;
            _actionLayerIndex = actionLayerIndex;
        }


        /// <summary>
        /// Detect if an action animation is playing
        /// </summary>
        /// <returns></returns>
        public bool IsActionAnimationRunning()
        {
            AnimatorClipInfo[] info = _animator.GetCurrentAnimatorClipInfo(_actionLayerIndex);
            //  return true if there is any animation running on this layer
            return (info != null && info.Length > 0);
        }
    }
}