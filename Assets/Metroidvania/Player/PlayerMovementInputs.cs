using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

namespace Metroidvania.Player
{
    public class PlayerMovementInputs : MonoBehaviour
    {
        public PlayerMovementController PlayerMovementController;
        public PlayerCameraController PlayerCameraController;
        private int _screenWidth;
        private PlayerControls _playerControls;
        private Transform _cameraTransform;
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        private Vector2 _touchMoveDelta = Vector2.zero;
        private float _touchScalar;
        private const float TouchScalarMultiplier = 2.5f;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _cameraTransform = Camera.main.transform;
            _playerControls.Enable();

            _playerControls.World.Jump.performed += ctx => characterInputs.JumpDown = true;
            _playerControls.World.Crouch.performed += ctx => characterInputs.CrouchDown = true; 
            _playerControls.World.Crouch.canceled += ctx => characterInputs.CrouchUp = true;

            _playerControls.World.TouchMoveStart.performed += TouchMoveStart;
            _playerControls.World.TouchMoveStart.canceled += TouchMoveEnd;
            _playerControls.World.TouchMoveAxis.performed += TouchMoveAxis;

            CalculateTouchScalar();
#if UNITY_EDITOR
            TouchSimulation.Enable();
#endif
        }


        private void CalculateTouchScalar()
        {
            _touchScalar = 1f / Screen.width * TouchScalarMultiplier;

            Debug.Log($"Found TouchScalar = {_touchScalar}");
        }


        private void TouchMoveAxis(InputAction.CallbackContext context)
        {
            _touchMoveDelta += context.ReadValue<Vector2>();
        }

        private void TouchMoveStart(InputAction.CallbackContext context)
        {
            _touchMoveDelta = Vector2.zero; //  reset the start position
        }

        private void TouchMoveEnd(InputAction.CallbackContext context)
        {
            _touchMoveDelta = Vector2.zero; //  reset the start position
        }

        private void Update()
        {
            HandleCharacterInput();
        }


        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (PlayerCameraController.RotateWithPhysicsMover && PlayerMovementController.Motor.AttachedRigidbody != null)
            {
                PlayerCameraController.PlanarDirection = PlayerMovementController.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * PlayerCameraController.PlanarDirection;
                PlayerCameraController.PlanarDirection = Vector3.ProjectOnPlane(PlayerCameraController.PlanarDirection, PlayerMovementController.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }


        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            Vector2 cameraDelta = _playerControls.World.CameraRotate.ReadValue<Vector2>();
            float mouseLookAxisUp = cameraDelta.y;
            float mouseLookAxisRight = cameraDelta.x;
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = _playerControls.World.CameraZoom.ReadValue<float>();
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            PlayerCameraController.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (Input.GetMouseButtonDown(1))
            {
                PlayerCameraController.TargetDistance = (PlayerCameraController.TargetDistance == 0f) ? PlayerCameraController.DefaultDistance : 0f;
            }
        }


        private void HandleCharacterInput()
        {
            Vector2 moveAxis = _playerControls.World.MoveAxis.ReadValue<Vector2>();

            //  If we don't have physical movement input, then try using Touch movement axis
            if (moveAxis.sqrMagnitude < 0.01f) 
            {
                moveAxis = _touchMoveDelta * _touchScalar;
                // if (moveAxis.sqrMagnitude > 0.01f)
                //     Debug.Log($"TouchMove Delta:{_touchMoveDelta}, Scalar:{_touchScalar}, Final:{moveAxis}");
            }

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = moveAxis.y;
            characterInputs.MoveAxisRight = moveAxis.x;
            characterInputs.CameraRotation = _cameraTransform.rotation;

            // Apply inputs to character
            PlayerMovementController.SetInputs(ref characterInputs);

            //  clear the flags for the next frame
            characterInputs.JumpDown = false;
            characterInputs.CrouchDown = false;
            characterInputs.CrouchUp = false;

        }
    }
}