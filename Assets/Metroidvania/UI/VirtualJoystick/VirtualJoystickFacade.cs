#nullable enable
using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Characters.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Zenject;

namespace Metroidvania.UI.VirtualJoystick
{
    /// <summary>
    /// A fake Virtual joystick. It displays on screen when there is no touch input, and is hidden as soon as touch starts.
    /// </summary>
    public class VirtualJoystickFacade : MonoBehaviour
    {
        [FormerlySerializedAs("DisplayObejct")] [SerializeField, RequiredField] private GameObject DisplayObject = default!;
        private PlayerControls _playerControls = default!;

        [Inject]
        private void Initialise(PlayerControls playerControls)
        {
            _playerControls = playerControls;
            _playerControls.World.TouchMoveStart.performed += TouchMoveStart;
            _playerControls.World.TouchMoveStart.canceled += TouchMoveEnd;
        }

        private void OnDestroy()
        {
            _playerControls.World.TouchMoveStart.performed -= TouchMoveStart;
            _playerControls.World.TouchMoveStart.canceled -= TouchMoveEnd;
        }

        private void TouchMoveEnd(InputAction.CallbackContext obj)
        {
            DisplayObject.SetActive(true);
        }

        private void TouchMoveStart(InputAction.CallbackContext obj)
        {
            DisplayObject.SetActive(false);
        }
    }
}
