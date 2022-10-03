using Cysharp.Threading.Tasks;
using Metroidvania.Player.Animation;
using UnityEngine;
using System.Linq;

namespace Metroidvania.Player
{
    public class PlayerMovementController_NoIK : MonoBehaviour, ICharacterMovementDriver
    {
        public bool LogIsGrounded = false;
        public bool SphereCastGrounded = false;
        public PlayerMovementStatsSO PlayerMovementStats;

        protected CharacterController _characterController;
        private Rigidbody _rigidBody;
        protected Animator _animator;
        private Transform _cameraTransform;
        private int _floorMask;
        private float _turnSmoothVelocity;
        protected bool _isGrounded = false;
        private Vector3 _horizontalVelocity = Vector3.zero;
        protected Vector3 _verticalVelocity = Vector3.zero;
        private ICharacterAnimationView _characterAnimationView;
        private CharacterBlinker _blinker;
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";
        private const string Jump = "Jump";


        [Range(0.1f, 5f)] public float GravityTweak = 2f;         //  Gravity multiplier, so the character doesn't feel so "Floaty". Haven't figured out why we need this
        private const float JumpButtonGracePeriod = 0.05f;
        private const float GroundedDetectionRange = 0.1f;


        private bool _firstMovement = true;
        private float? _lastGroundedTime;
        private float? _jumpButtonPressedTime;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _cameraTransform = UnityEngine.Camera.main.transform;

            _floorMask = ~LayerMask.GetMask("Player", "Untagged", "SceneAnchors", "CameraIgnored");

            if (PlayerMovementStats == null)
            {
                //  just used the default.
                PlayerMovementStats = ScriptableObject.CreateInstance<PlayerMovementStatsSO>();
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            if (!_isEnabled || !_characterController.enabled)
                return;

            CalculateJumpRequest();
            Vector3 inputDirection = CalculateInputVector();

            RecalculateGrounded();

            bool isTurningAllowed = (_isGrounded || PlayerMovementStats.IsMidAirTurningAllowed());

            if (isTurningAllowed)
            {
                _horizontalVelocity = Vector3.zero;
            }

            if (inputDirection.sqrMagnitude > 0.1f)
            {
                float movementSpeed = CalculateMovementSpeed(inputDirection.magnitude);

                _firstMovement = false;

                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                    + _cameraTransform.eulerAngles.y;   //  rotate move direction based on camera
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, PlayerMovementStats.TurnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                if (isTurningAllowed)
                {
                    Vector3 inputDirectionWorld = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward);
                    if (inputDirectionWorld.sqrMagnitude > 1f)
                    {
                        inputDirectionWorld.Normalize();
                    }

                    _horizontalVelocity = inputDirectionWorld * movementSpeed;
                    _characterAnimationView.SetSpeed(_horizontalVelocity.magnitude);
                }
            }
            else
            {
                _characterAnimationView.SetSpeed(0f);
            }

            //  if we're midair, damp our horizontal movement.
            if (!_isGrounded && !PlayerMovementStats.IsMidAirTurningAllowed())
            {
                _horizontalVelocity = _horizontalVelocity * (1f - Time.deltaTime);
            }

            //  Add Gravity
            _verticalVelocity.y += PlayerMovementStats.Gravity * GravityTweak * Time.deltaTime;

            //  allow Coyote Time.
            if (IsRecentlyGrounded())
            {
                ////  for animation movement?
                //_characterController.stepOffset = _orginalStepOffset;

                //  reset the fall speed (but only if we're falling DOWN)
                if (_verticalVelocity.y <= 0f)
                {
                    _verticalVelocity.y = -0.5f;
                }

                if (IsJumpRequestedAndValid())
                {
                    PerformJump();
                    _characterAnimationView.SetJumping(true);
                }
            }
            else
            {
                ////  for animation movement?
                //_characterController.stepOffset = 0;
            }

            Vector3 finalVelocity = AdjustVelocityToSlope(_horizontalVelocity);
            if (!_firstMovement)
            {
                finalVelocity += _verticalVelocity;
            }

            MoveInUpdate(finalVelocity);
        }

        protected virtual void PerformJump()
        {
            _verticalVelocity.y = Mathf.Sqrt(PlayerMovementStats.JumpHeight * -2 * PlayerMovementStats.Gravity * GravityTweak);
        }

        protected virtual void MoveInUpdate(Vector3 finalVelocity)
        {
            _characterController.Move(finalVelocity * Time.deltaTime);
        }


        private Vector3 AdjustVelocityToSlope(Vector3 velocity)
        {
            var ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
            {
                var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                var adjustedVelocity = slopeRotation * velocity;
                if (adjustedVelocity.y < 0)
                {
                    return adjustedVelocity;
                }
            }
            return velocity;
        }


        private void CalculateJumpRequest()
        {
            if (Input.GetButtonDown(Jump))
            {
                _jumpButtonPressedTime = Time.time;
            }
        }
        private bool IsRecentlyGrounded()
        {
            return Time.time - _lastGroundedTime <= JumpButtonGracePeriod;
        }

        private bool IsJumpRequestedAndValid()
        {
            if (Time.time - _jumpButtonPressedTime <= JumpButtonGracePeriod)
            {
                _jumpButtonPressedTime = null;
                _lastGroundedTime = null;
                return true;
            }
            return false;
        }

        private float CalculateMovementSpeed(float inputMagnitude)
        {
            float magnitude = Mathf.Clamp01(inputMagnitude) * PlayerMovementStats.Speed;
            //  sprint
            if (IsSprinting())
            {
                magnitude *= PlayerMovementStats.SprintMultiplier;
            }

            return magnitude;
        }

        private Vector3 CalculateInputVector()
        {
            //  calculate horizontal movement
            float horizontal = Input.GetAxisRaw(Horizontal);
            float vertical = Input.GetAxisRaw(Vertical);
            Vector3 inputVector = new Vector3(horizontal, 0f, vertical);

            //  if we're going diagonally, we would go faster, so let's limit the total magnitude to 1
            if (inputVector.sqrMagnitude > 1f)
            {
                inputVector.Normalize();
            }

            return inputVector;
        }

        private bool IsSprinting()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        protected void RecalculateGrounded()
        {
            //bool updatedIsGrounded = _characterController.isGrounded;
            bool updatedIsGrounded = DetectGrounded();
            //  toggle isGrounded if it's changed.
            if (_isGrounded != updatedIsGrounded)
            {
                _isGrounded = updatedIsGrounded;
                if (LogIsGrounded)
                {
                    Debug.Log($"Character is {(_isGrounded ? "" : "NOT")} Grounded");
                }
                //_blinker.Blink(2, 0.1f);
                _characterAnimationView.SetGrounded(_isGrounded);
            }

            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;
            }
        }

        private RaycastHit[] _raycastHits = new RaycastHit[1];
        private bool _isEnabled;

        protected virtual bool DetectGrounded()
        {
            if (SphereCastGrounded)
            {
                return DetectGroundedSphereCast();
            }
            return DetectGroundedController();
        }


        protected virtual bool DetectGroundedSphereCast()
        {
            Ray floorDetectionRay = new Ray(transform.position, Vector3.down);

            int numResults = Physics.SphereCastNonAlloc(floorDetectionRay, GroundedDetectionRange, _raycastHits, GroundedDetectionRange, _floorMask);
            bool isGrounded = numResults > 0;
            if (isGrounded)
            {
                Debug.Log($"Grounded on {numResults} items {string.Join("|", _raycastHits.Select(i => i.collider.name).ToArray())}");
            }
            return isGrounded;
        }
        protected virtual bool DetectGroundedController() => _characterController.isGrounded;

        public void RegisterCharacterAnimationView(ICharacterAnimationView characterAnimationView)
        {
            _characterAnimationView = characterAnimationView;
            _characterAnimationView.SetGrounded(true);
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

            _rigidBody.position = position;
            transform.position = position;
            //await UniTask.Delay(1500, DelayType.UnscaledDeltaTime);  //  wait for camera to catch up
            _characterController.enabled = true;
        }

        public void Enable(bool isEnabled)
        {
            _isEnabled = isEnabled;
        }
    }
}