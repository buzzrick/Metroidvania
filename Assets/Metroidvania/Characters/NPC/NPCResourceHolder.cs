using Buzzrick.UnityLibs;
using Metroidvania.Interactables.ResourcePickups;
using Metroidvania.ResourceTypes;
using NaughtyAttributes;
using UnityEngine;


namespace Metroidvania.Characters.NPC
{
    /// <summary>
    /// Enables an NPC to pickup, hold and drop a ResourcePickup.
    /// </summary>
    public class NPCResourceHolder : MonoBehaviour
    {
        public Transform ResourceAttachPoint;
        public ResourceTypeSO ResourceType;     //  todo: change this to a list of supported ResourceTypes
        [SerializeField] private LayerMask _layerMask;
        private ResourcePickup _pickup;
        private Transform _previousParent;
        public bool IsHoldingResource => _pickup != null;
        public ResourcePickup Pickup => _pickup;


        //  todo: add a flag whether to consume items.

        [SerializeField] private float _delayBetweenPickups = 1f;
        private float _delayTimer = 0;

        private void Reset()
        {
            _layerMask = LayerMask.GetMask("PlayerPickup");
            //  find the first child object with the name "AttachPoint"
            ResourceAttachPoint = transform.parent.FindEx("AttachPoint", true);
            if (GetComponent<Collider>() == null )
            {
                var collider = gameObject.AddComponent<SphereCollider>();
                collider.isTrigger = true;
            }
        }

        private void Update()
        {
            if (_delayTimer > 0)
            {
                _delayTimer -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            DetectResourcePickup(other);
        }

        private void OnTriggerStay(Collider other)
        {
            DetectResourcePickup(other);
        }


        private void DetectResourcePickup(Collider other)
        {
            if (_delayTimer > 0)
            {
                return;
            }   

            if (_layerMask.IsInLayerMask(other.gameObject.layer))
            {
                if (other.TryGetComponent<ResourcePickup>(out var pickup))
                {
                    if (!pickup.IsPickupAllowed)
                    {
                        return;
                    }
                    _pickup = pickup;
                    //Debug.Log($"Player found {pickup.Amount} {pickup.ResourceType.name}");
                    //  store the previous parent for when we drop the object
                    _previousParent = pickup.transform.parent;
                    pickup.AllowPickups(false);
                    pickup.transform.SetParent(ResourceAttachPoint, true);
                    pickup.transform.localPosition = Vector3.zero;
                }
            }
        }

        [Button]
        public void DropResource()
        {
            if (_pickup != null)
            {
                _delayTimer = _delayBetweenPickups;
                _pickup.transform.SetParent(_previousParent, true);
                
                _pickup.AllowPickups(true);
                _pickup.AddImpulseToward(transform.position + transform.forward);
                _pickup = null;
            }
        }
    }
}