using System;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
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
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform;

        if (PlayerMovementStats == null)
        {
            //  just used the default.
            PlayerMovementStats = ScriptableObject.CreateInstance<PlayerMovementStatsSO>();
        }    
    }


    // Update is called once per frame
    public void Update()
    {
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
            }
        }

        _verticalVelocity.y += (PlayerMovementStats.Gravity * Time.deltaTime) * GravityTweak;
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
        }
    }

    public void RegisterCharacterAnimationView(ICharacterAnimationView characterAnimationView)
    {
        _characterAnimationView = characterAnimationView;
    }
}
