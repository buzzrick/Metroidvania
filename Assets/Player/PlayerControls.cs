//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/Configuration/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Metroidvania.Player
{
    public partial class @PlayerControls: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""World"",
            ""id"": ""35223804-b4b8-436f-ac40-c01836aeffda"",
            ""actions"": [
                {
                    ""name"": ""MoveAxis"",
                    ""type"": ""Value"",
                    ""id"": ""c3c242b2-ace4-4294-ac1a-4d492e4a15bb"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""2733f72f-4c5e-4ae1-a0f8-f6ce90e8ac63"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""e397e299-e139-49d4-8626-dbdc5aad1d3f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraRotate"",
                    ""type"": ""Value"",
                    ""id"": ""da965918-0b98-4e88-98ee-6a21b5d76b6f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraZoom"",
                    ""type"": ""Value"",
                    ""id"": ""5e515959-b2da-4d47-aad3-c38f78770285"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ec5ae030-cbac-46fd-9be3-329aabbb0723"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""MoveAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""42ec9acc-bc65-40b4-bfe9-e85edf45e335"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveAxis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""28444542-80f7-4170-b5bd-bb9b91dd1e0c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""MoveAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0e39a584-4c8e-4479-b2f1-1ac03c088fd3"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""MoveAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""296260b6-a1ff-4daa-a054-91e998af129c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""MoveAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0ab0e901-06fd-4dea-b4a2-41139c08e7b3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""MoveAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""724865f1-f48a-4428-9674-028b91a64108"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71746671-6f5b-4403-a8b1-3c1eed721a45"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e30bab35-e67b-4541-8f4d-d3151cb47455"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""CameraRotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5e7808fa-6833-4ddf-9bd0-277035d1a731"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""c1b12fed-c398-4ce3-891a-0767b7796330"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""260a7254-e075-4211-a03c-59ec0574a60c"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""dac0442e-df5f-4921-9a83-efd511aa6e70"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""beaf0ed0-0123-4fa0-8e63-c49400778a5f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""dad74d06-b326-4c8e-82c9-66028c2d13cb"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""349d6586-56bf-4822-9c5d-3ca693a2ea5d"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""MouseAndKeyboard"",
            ""bindingGroup"": ""MouseAndKeyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // World
            m_World = asset.FindActionMap("World", throwIfNotFound: true);
            m_World_MoveAxis = m_World.FindAction("MoveAxis", throwIfNotFound: true);
            m_World_Jump = m_World.FindAction("Jump", throwIfNotFound: true);
            m_World_Crouch = m_World.FindAction("Crouch", throwIfNotFound: true);
            m_World_CameraRotate = m_World.FindAction("CameraRotate", throwIfNotFound: true);
            m_World_CameraZoom = m_World.FindAction("CameraZoom", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // World
        private readonly InputActionMap m_World;
        private List<IWorldActions> m_WorldActionsCallbackInterfaces = new List<IWorldActions>();
        private readonly InputAction m_World_MoveAxis;
        private readonly InputAction m_World_Jump;
        private readonly InputAction m_World_Crouch;
        private readonly InputAction m_World_CameraRotate;
        private readonly InputAction m_World_CameraZoom;
        public struct WorldActions
        {
            private @PlayerControls m_Wrapper;
            public WorldActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveAxis => m_Wrapper.m_World_MoveAxis;
            public InputAction @Jump => m_Wrapper.m_World_Jump;
            public InputAction @Crouch => m_Wrapper.m_World_Crouch;
            public InputAction @CameraRotate => m_Wrapper.m_World_CameraRotate;
            public InputAction @CameraZoom => m_Wrapper.m_World_CameraZoom;
            public InputActionMap Get() { return m_Wrapper.m_World; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(WorldActions set) { return set.Get(); }
            public void AddCallbacks(IWorldActions instance)
            {
                if (instance == null || m_Wrapper.m_WorldActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_WorldActionsCallbackInterfaces.Add(instance);
                @MoveAxis.started += instance.OnMoveAxis;
                @MoveAxis.performed += instance.OnMoveAxis;
                @MoveAxis.canceled += instance.OnMoveAxis;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @CameraRotate.started += instance.OnCameraRotate;
                @CameraRotate.performed += instance.OnCameraRotate;
                @CameraRotate.canceled += instance.OnCameraRotate;
                @CameraZoom.started += instance.OnCameraZoom;
                @CameraZoom.performed += instance.OnCameraZoom;
                @CameraZoom.canceled += instance.OnCameraZoom;
            }

            private void UnregisterCallbacks(IWorldActions instance)
            {
                @MoveAxis.started -= instance.OnMoveAxis;
                @MoveAxis.performed -= instance.OnMoveAxis;
                @MoveAxis.canceled -= instance.OnMoveAxis;
                @Jump.started -= instance.OnJump;
                @Jump.performed -= instance.OnJump;
                @Jump.canceled -= instance.OnJump;
                @Crouch.started -= instance.OnCrouch;
                @Crouch.performed -= instance.OnCrouch;
                @Crouch.canceled -= instance.OnCrouch;
                @CameraRotate.started -= instance.OnCameraRotate;
                @CameraRotate.performed -= instance.OnCameraRotate;
                @CameraRotate.canceled -= instance.OnCameraRotate;
                @CameraZoom.started -= instance.OnCameraZoom;
                @CameraZoom.performed -= instance.OnCameraZoom;
                @CameraZoom.canceled -= instance.OnCameraZoom;
            }

            public void RemoveCallbacks(IWorldActions instance)
            {
                if (m_Wrapper.m_WorldActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IWorldActions instance)
            {
                foreach (var item in m_Wrapper.m_WorldActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_WorldActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public WorldActions @World => new WorldActions(this);
        private int m_MouseAndKeyboardSchemeIndex = -1;
        public InputControlScheme MouseAndKeyboardScheme
        {
            get
            {
                if (m_MouseAndKeyboardSchemeIndex == -1) m_MouseAndKeyboardSchemeIndex = asset.FindControlSchemeIndex("MouseAndKeyboard");
                return asset.controlSchemes[m_MouseAndKeyboardSchemeIndex];
            }
        }
        private int m_ControllerSchemeIndex = -1;
        public InputControlScheme ControllerScheme
        {
            get
            {
                if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
                return asset.controlSchemes[m_ControllerSchemeIndex];
            }
        }
        public interface IWorldActions
        {
            void OnMoveAxis(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnCrouch(InputAction.CallbackContext context);
            void OnCameraRotate(InputAction.CallbackContext context);
            void OnCameraZoom(InputAction.CallbackContext context);
        }
    }
}
