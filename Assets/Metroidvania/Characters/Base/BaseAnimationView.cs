using System.Collections;
using UnityEngine;

namespace Metroidvania.Characters.Base
{
    public class BaseAnimationView: MonoBehaviour, ICharacterAnimationView
    {
        [SerializeField] private GameObject _model;
        [SerializeField] private Vector3 _swimmingOffset = Vector3.zero;
        protected Animator _animator;
        protected int _swimmingLayerID;
        protected Transform _root;
        protected Transform _headBone;
        protected Rigidbody[] _boneRig;
        protected readonly int HashSpeed = Animator.StringToHash("Speed");
        protected readonly int HashJump = Animator.StringToHash("Jump");
        protected readonly int HashGrounded = Animator.StringToHash("Grounded");
        protected float mass = 0.1f;  // Mass of each bone
        protected bool _isSwimming;
        protected float _speed;

        public Animator GetAnimator() => _animator;
        
        protected void Setup()
        {
            _animator = gameObject.GetComponent<Animator>();
            _swimmingLayerID = _animator.GetLayerIndex("SwimmingLayer");
   
            if (_root == null)
                _root = transform.Find("Root");
            if (_headBone == null)
                _headBone = transform.Find("Head");
            _boneRig = gameObject.GetComponentsInChildren<Rigidbody>();
            DisableRagdoll();
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
                    ragdoll.GetComponent<Rigidbody>().AddForce(force * Random.value);
            }
            GetComponent<Animator>().enabled = false;
            GetComponent<Collider>().enabled = false;

            //Destroy(GetComponent<BotControlScript>());

            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }

    }
}