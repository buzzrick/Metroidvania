using System;
using Metroidvania.Characters.Base;
using UnityEngine;
using Zenject;

namespace Metroidvania.Characters.Player.Animation
{
    public class PlayerAnimationView : BaseAnimationView
    {
        private ICharacterMovementDriver _characterMovementDriver;
        [SerializeField] private PlayerInteractionController _playerInteractionController;
        private PlayerAnimationActionsHandler _playerAnimationActionHandler;

        public event Action OnAnimationStriked;

        [Inject]
        private void Initialise(PlayerAnimationActionsHandler.Factory playerAnimationActionFactory)
        {
            Setup();
            _playerAnimationActionHandler = playerAnimationActionFactory.Create(this);
            _playerInteractionController.RegisterPlayerAnimationHandler(_playerAnimationActionHandler, this);
            if (_characterMovementDriver == null)
            {
                _characterMovementDriver = GetComponentInParent<ICharacterMovementDriver>();
            }
            _characterMovementDriver?.RegisterCharacterAnimationView(this);
            _playerInteractionController.SetAutomatic(true);
        }
        
        public void Strike()
        {
            //Debug.Log("Strike!");
            OnAnimationStriked?.Invoke();   //  fired from chop animations
        }

        public void FootR()
        {
            Debug.Log($"FootR");    //  fired from sickle animation
        }

        public PlayerAnimationTool GetToolForInteraction(InteractionActionType interactionAction) =>
            _playerAnimationActionHandler.GetToolForInteraction(interactionAction);
        
    }
}