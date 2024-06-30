using Metroidvania.Interactables.ResourcePickups;
using Metroidvania.ResourceTypes;
using UnityEngine;

namespace Metroidvania.Characters.NPC
{
    /// <summary>
    /// Detects the direction to the nearest object of a given Resource Type within a given radius.
    /// </summary>
    public class NPCResourceDetector : MonoBehaviour
    {
        [Tooltip("The number of frames to skip between updates - used for reducing overhead before we get to an ECS implementation")]
        [SerializeField] private int RateLimiter = 1;
        [SerializeField] public float DetectionRadius = 5;
        [SerializeField] public ResourceTypeSO ResourceToDetect;     //  todo: change this to a list of supported ResourceTypes
        [SerializeField] public LayerMask DetectionLayerMask;

        public Vector3 ResourceDirection { get; private set; }
        public Vector3 ResourceWorldPosition { get; private set; }
        public float ResourceDistanceSqr { get; private set; }
        public bool IsResourceDetected { get; private set; }

        private int _frameCounter = 0;
        private RaycastHit[] _hits = new RaycastHit[5];

        private void Reset()
        {
            DetectionLayerMask = LayerMask.GetMask("PlayerPickup");
        }

        // Update is called once per frame
        private void Update()
        {
            if (_frameCounter < RateLimiter)
            {
                _frameCounter++;
                return;
            }

            // search in a raduis around this object for the target object
            //  if the target object is found, set the ObjectDirection and ObjectDistanceSqr

            int hitCount = Physics.SphereCastNonAlloc(transform.position, DetectionRadius, Vector3.up, _hits, 0f, DetectionLayerMask, QueryTriggerInteraction.Ignore);

            ResourceDirection = Vector3.zero;
            ResourceDistanceSqr = float.MaxValue;
            IsResourceDetected = false;
            if (hitCount > 0)
            {
                Transform closestTransform = null;
                for (int hitNum = 0; hitNum < hitCount; hitNum++)
                {
                    RaycastHit hit = _hits[hitNum];
                    
                    if (hit.collider != null)
                    {
                        Collider collider = hit.collider;

                        if (collider.TryGetComponent<ResourcePickup>(out var pickup) 
                            && pickup.IsPickupAllowed
                            && pickup.ResourceType == ResourceToDetect)
                        {
                            Transform hitTransform = collider.transform;

                            //  if the closest object is not set, set it to the current object  
                            if (closestTransform == null)
                            {
                                closestTransform = hitTransform;
                            }
                            else
                            {
                                //  if the current object is closer than the closest object, set the closest object to the current object
                                if ((hitTransform.position - transform.position).sqrMagnitude < (closestTransform.position - transform.position).sqrMagnitude)
                                {
                                    closestTransform = hitTransform;
                                }
                            }
                        }
                    }
                }


                if (closestTransform != null)
                {
                    IsResourceDetected = true;
                    ResourceWorldPosition = closestTransform.position;
                    ResourceDirection = closestTransform.position - transform.position;
                    //  flatten the PlayerDirection vector to 2D
                    ResourceDirection = new Vector3(ResourceDirection.x, 0, ResourceDirection.z);
                    ResourceDistanceSqr = ResourceDirection.sqrMagnitude;
                }
            }
            _frameCounter = 0;
        }
    }
}