using Buzzrick.UnityLibs.Attributes;
using UnityEngine;

namespace Metroidvania.Characters.NPC
{
    public class NPCController : MonoBehaviour
    {
        [SerializeField, RequiredField] private Animator _animator;
        [SerializeField, RequiredField] private NPCCharacterController _npcCharacterController;
        [SerializeField, RequiredField] private NPCCharacterAI _npcCharacterAI;
        
        private void Awake()
        {
            _npcCharacterController.Enable(true);
            _animator ??= GetComponentInChildren<Animator>();
        }

        public void PlayAnimation(string animationName)
        {
            _animator.Play(animationName);
        }
    }
}