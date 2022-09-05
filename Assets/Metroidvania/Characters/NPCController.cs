using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Metroidvania.Characters
{
    public class NPCController : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void PlayAnimation(string animationName)
        {
            _animator.Play(animationName);
        }
    }
}