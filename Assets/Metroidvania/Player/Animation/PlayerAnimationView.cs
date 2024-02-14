using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Metroidvania.Player.Animation
{
    public class PlayerAnimationView : MonoBehaviour, ICharacterAnimationView
    {
        private ICharacterMovementDriver _characterMovementDriver;
        [SerializeField] private PlayerInteractionController _playerInteractionController;
        [SerializeField] private GameObject _model;
        [SerializeField] private Vector3 _swimmingOffset = Vector3.zero;
        private Animator _animator;
        private int _swimmingLayerID;
        private PlayerAnimationActionsHandler _playerAnimationActionHandler;
        private Transform _root;
        private Transform _headBone;
        private Rigidbody[] _boneRig;
        private readonly int HashSpeed = Animator.StringToHash("Speed");
        private readonly int HashJump = Animator.StringToHash("Jump");
        private readonly int HashGrounded = Animator.StringToHash("Grounded");
        private float mass = 0.1f;  // Mass of each bone
        private bool _isSwimming;
        private float _speed;

        public Animator GetAnimator() => _animator;

        public event Action OnAnimationStriked;



        [Inject]
        private void Initialise(PlayerAnimationActionsHandler.Factory playerAnimationActionFactory)
        {
            _animator = gameObject.GetComponent<Animator>();
            _swimmingLayerID = _animator.GetLayerIndex("SwimmingLayer");
            _playerAnimationActionHandler = playerAnimationActionFactory.Create(this);
            _playerInteractionController.RegisterPlayerAnimationHandler(_playerAnimationActionHandler, this);
            if (_characterMovementDriver == null)
            {
                _characterMovementDriver = GetComponentInParent<ICharacterMovementDriver>();
            }
            _characterMovementDriver?.RegisterCharacterAnimationView(this);

            if (_root == null)
                _root = transform.Find("Root");
            if (_model == null)
            {
                var modelObject = transform.Find("MicroMale") ?? transform.Find("MicroFemale");
                _model = modelObject.gameObject;
            }
            if (_headBone == null)
                _headBone = transform.Find("Head");
            _boneRig = gameObject.GetComponentsInChildren<Rigidbody>();
            DisableRagdoll();

            _playerInteractionController.SetAutomatic(true);
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
            _animator.SetFloat(HashSpeed,Mathf.Clamp(speed, 0f, 1f));
            SetSwimmingOffset();
        }

        public void SetJumping(bool isJumping)
        {
            //_animator.SetBool(HashJump, isJumping);
        }

        public void SetSwimming(bool isSwimming)
        {
            _isSwimming = isSwimming;
            _animator.SetLayerWeight(_swimmingLayerID, _isSwimming ? 1 : 0);
            SetSwimmingOffset();
        }

        private void SetSwimmingOffset()
        {
            if (_isSwimming && (_speed > 0.1f))
            {
                transform.localPosition = _swimmingOffset;
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }

        public bool IsSwimming => _isSwimming;

        public void SetGrounded(bool isGrounded)
        {
            _animator.SetBool(HashGrounded, isGrounded);
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

        public void DisableRagdoll()
        {
            foreach (Component ragdoll in _boneRig)
            {
                if ((ragdoll.GetComponent<Collider>() != null) && ragdoll.GetComponent<Collider>() != this.GetComponent<Collider>())
                {
                    ragdoll.GetComponent<Collider>().enabled = false;
                    ragdoll.GetComponent<Rigidbody>().isKinematic = true;
                    ragdoll.GetComponent<Rigidbody>().mass = 0.01f;
                }
            }
            if (TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = true;
            }
        }

        public PlayerAnimationTool GetToolForInteraction(InteractionActionType interactionAction) =>
            _playerAnimationActionHandler.GetToolForInteraction(interactionAction);
        
        public IEnumerator EnableRagdoll(float delay, Vector3 force)
        {
            yield return new WaitForSeconds(delay);
            foreach (Component ragdoll in _boneRig)
            {
                if (ragdoll.GetComponent<Collider>() != null)
                    ragdoll.GetComponent<Collider>().enabled = true;
                ragdoll.GetComponent<Rigidbody>().isKinematic = false;
                ragdoll.GetComponent<Rigidbody>().mass = mass;
                if (force.magnitude > 0)
                    ragdoll.GetComponent<Rigidbody>().AddForce(force * UnityEngine.Random.value);
            }
            GetComponent<Animator>().enabled = false;
            GetComponent<Collider>().enabled = false;

            //Destroy(GetComponent<BotControlScript>());

            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }

    }
}