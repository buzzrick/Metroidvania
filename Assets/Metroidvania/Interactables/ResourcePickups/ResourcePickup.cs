using Buzzrick.UnityLibs.Attributes;
using Metroidvania.ResourceTypes;
using UnityEngine;

namespace Metroidvania.Interactables.ResourcePickups
{
    [RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
    public class ResourcePickup : MonoBehaviour
    {
        [SerializeField, RequiredField] private Rigidbody _rigidBody;
        public ResourceTypeSO ResourceType;
        public int Amount = 1;
        private float TimeBeforePickupAllowed = 0.75f;
        private float _allowPickupTimer = 0;
        private bool _interactionEnabled = true;
        private Collider _collider;
        private const float UpImpulseForce = 3.0f;
        private const float HorizontImpulseForce = 1.5f;
        private const float RandomImpulseForce = 0.2f;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void AddImpulseToward(Vector3 targetPosition)
        {
            _allowPickupTimer = TimeBeforePickupAllowed;
            Vector3 playerDirectionForce = (targetPosition - transform.position).normalized * HorizontImpulseForce;
            Vector3 upForce = Vector3.up * UpImpulseForce;
            Vector3 randomForce = Random.rotation.eulerAngles.normalized * RandomImpulseForce;
            randomForce.y = Mathf.Abs(randomForce.y);       //  ensure the random extra force is always upwards

            _rigidBody.AddForce(playerDirectionForce + upForce + randomForce, ForceMode.Impulse);
        }

        private void Reset()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_allowPickupTimer > 0f)
            {
                _allowPickupTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// enables/disables the ability to pick up this object
        /// </summary>
        /// <param name="isEnabled"></param>
        public void AllowPickups(bool isEnabled)
        {
            _interactionEnabled = isEnabled;
            //  if we are disabling the ability to pick up, make the object kinematic
            //  (because it's likely to be held by the actor)
            _rigidBody.isKinematic = !isEnabled;
            _collider.enabled = isEnabled;
        }

        public bool IsPickupAllowed =>
            _interactionEnabled 
            && (_allowPickupTimer <= 0f);
    }
}