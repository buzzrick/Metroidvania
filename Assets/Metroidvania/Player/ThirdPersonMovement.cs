using Cysharp.Threading.Tasks;
using Metroidvania.Player.Animation;
using System;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour, ICharacterMovementDriver
{
    public bool LogIsGrounded = false;
    public PlayerMovementStatsSO PlayerMovementStats;

    public float GravityTweak = 1f;

    private CharacterController _characterController;
    private Transform _cameraTransform;

    private float _turnSmoothVelocity;
    private bool _isGrounded = false;
    private Vector3 _horizontalVelocity = Vector3.zero;
    private Vector3 _verticalVelocity = Vector3.zero;
    private ICharacterAnimationView _characterAnimationView;
    private CharacterBlinker _blinker;
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    private bool _firstMovement = true;

    private async void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform;

        if (PlayerMovementStats == null)
        {
            //  just used the default.
            PlayerMovementStats = ScriptableObject.CreateInstance<PlayerMovementStatsSO>();
        }
        _characterController.enabled = false;
        await UniTask.Delay(1000);
        _characterController.enabled = true;
    }


    // Update is called once per frame
    public void Update()
    {
        if (!_characterController.enabled)
            return;

        //  calculate horizontal movement
        float horizontal = Input.GetAxisRaw(Horizontal);
        float vertical = Input.GetAxisRaw(Vertical);
        Vector3 inputVector = new Vector3(horizontal, 0f, vertical).normalized;
        
        RecalculateGrounded();

        bool isTurningAllowed = (_isGrounded || PlayerMovementStats.IsMidAirTurningAllowed());

        if (isTurningAllowed)
        {
            _horizontalVelocity = Vector3.zero;
        }

        if (inputVector.magnitude > 0.1f)
        {
            _firstMovement = false;
            float targetAngle = Mathf.Atan2(inputVector.x, inputVector.z) * Mathf.Rad2Deg
                + _cameraTransform.eulerAngles.y;   //  rotate move direction based on camera

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, PlayerMovementStats.TurnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (_isGrounded || PlayerMovementStats.IsMidAirTurningAllowed())
            {
                Vector3 inputDirection = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward);
                if (inputDirection.sqrMagnitude > 1f)
                {
                    inputDirection = inputDirection.normalized;
                }
                _horizontalVelocity = inputDirection * PlayerMovementStats.Speed;
                _characterAnimationView.SetSpeed(_horizontalVelocity.magnitude);
            }
        }
        else
        {
            _characterAnimationView.SetSpeed(0f);
        }

        //  calculate gravity / jumping
        if (_isGrounded)
        {
            //  have we landed
            //if (_playerVerticalVelocity.y < 0f)
            {
                _verticalVelocity.y = -1f;
            }
            if (Input.GetButtonDown("Jump"))
            {
                _verticalVelocity.y = Mathf.Sqrt(PlayerMovementStats.JumpHeight * GravityTweak * -2 * PlayerMovementStats.Gravity) ;
                _characterAnimationView.SetJumping(true);
            }
        }

        if (!_firstMovement)
        {
            _verticalVelocity.y += (PlayerMovementStats.Gravity * Time.deltaTime) * GravityTweak;
        }
        _characterController.Move((_horizontalVelocity + _verticalVelocity) * Time.deltaTime);
    }

    private void RecalculateGrounded()
    {
        //  toggle isGrounded if it's changed.
        if (_isGrounded != _characterController.isGrounded)
        {
            _isGrounded = _characterController.isGrounded;
            if (LogIsGrounded)
            {
                Debug.Log($"Character is {(_isGrounded ? "" : "NOT")} Grounded");
            }
            //_blinker.Blink(2, 0.1f);
        }
    }

    public void RegisterCharacterAnimationView(ICharacterAnimationView characterAnimationView)
    {
        _characterAnimationView = characterAnimationView;
    }


    public void RegisterCharacterBlinker(CharacterBlinker blinker)
    {
        _blinker = blinker;
    }

    public async void Teleport(Vector3 position)
    {

        Debug.Log($"Teleporting to {position}");
        _horizontalVelocity = Vector3.zero;
        _verticalVelocity = Vector3.zero;
        _characterAnimationView.SetSpeed(0f);
        _blinker.Blink(7, 0.1f);

        _characterController.enabled = false;
        Physics.SyncTransforms();       //  https://issuetracker.unity3d.com/issues/charactercontroller-overrides-objects-position-when-teleporting-with-transform-dot-position
        transform.position = position;
        await UniTask.Delay(1500, DelayType.UnscaledDeltaTime);  //  wait for camera to catch up
        _characterController.enabled = true;
    }
}
