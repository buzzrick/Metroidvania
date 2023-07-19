using KinematicCharacterController.Examples;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerMovementInputs : MonoBehaviour
    {
        public PlayerMovementController PlayerMovementController;
        private PlayerControls _playerControls;
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Enable();

            _playerControls.World.Jump.performed += ctx => characterInputs.JumpDown = true;
            _playerControls.World.Crouch.performed += ctx => characterInputs.CrouchDown = true; 
            _playerControls.World.Crouch.canceled += ctx => characterInputs.CrouchUp = true;
        }

        private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            HandleCharacterInput();
        }


        private void HandleCharacterInput()
        {
            Vector2 moveAxis = _playerControls.World.MoveAxis.ReadValue<Vector2>();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = moveAxis.y;
            characterInputs.MoveAxisRight = moveAxis.x;

            // Apply inputs to character
            PlayerMovementController.SetInputs(ref characterInputs);

            //  clear the flags for the next frame
            characterInputs.JumpDown = false;
            characterInputs.CrouchDown = false;
            characterInputs.CrouchUp = false;
        }
    }
}