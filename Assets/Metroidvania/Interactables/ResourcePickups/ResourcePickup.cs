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
        private float _allowPickupTimer = 0.75f;
        private const float UpImpulseForce = 3.0f;
        private const float HorizontImpulseForce = 1.5f;
        private const float RandomImpulseForce = 0.2f;

        public void AddImpulseToward(Vector3 targetPosition)
        {
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

        public bool IsPickupAllowed => _allowPickupTimer <= 0f;
    }
}