using System;
using UnityEngine;

namespace Metroidvania.Characters.NPC
{
    public class NPCObjectDetector<T> : MonoBehaviour 
        where T : MonoBehaviour
    {
        [SerializeField] private int RateLimiter = 1;
        [SerializeField] public float DetectionRadius = 5;
        [SerializeField] public LayerMask DetectionLayerMask;

        public Vector3 ObjectDirection { get; private set; }
        public Vector3 ObjectWorldPosition { get; private set; }
        public float ObjectDistanceSqr { get; private set; }
        public bool IsObjectDetected { get; private set; }
        public T DetectedObject { get; private set; }

        private int _frameCounter = 0;
        private RaycastHit[] _hits = new RaycastHit[5];
        private Collider _thisCollider;
        private float _pauseTimer;
        public float PauseTimerRemaining => _pauseTimer;    

        public Func<T, bool> IsDetectionValid { get; set; }

        private void Awake()
        {
            _thisCollider = GetComponent<Collider>();
        }

        private void Reset()
        {
            DetectionLayerMask = LayerMask.GetMask("NPCs");
        }

        // Update is called once per frame
        private void Update()
        {
            if (_pauseTimer > 0)
            {
                _pauseTimer -= Time.deltaTime;
                return;
            }
            else if (_frameCounter < RateLimiter)
            {
                _frameCounter++;
                return;
            }

            // search in a raduis around this object for the target object
            //  if the target object is found, set the ObjectDirection and ObjectDistanceSqr
            int hitCount = Physics.SphereCastNonAlloc(transform.position, DetectionRadius, Vector3.up, _hits, 0f, DetectionLayerMask,
                QueryTriggerInteraction.Collide);   //  collide option is used to detect triggers as well as colliders

            ObjectDirection = Vector3.zero;
            ObjectDistanceSqr = float.MaxValue;
            IsObjectDetected = false;
            DetectedObject = null;
            if (hitCount > 0)
            {
                Transform closestTransform = null;
                for (int hitNum = 0; hitNum < hitCount; hitNum++)
                {
                    RaycastHit hit = _hits[hitNum];
                    Collider collider = hit.collider;
                    if (collider != null 
                        && collider != _thisCollider)   //  ignore self
                    {
                        if (collider.TryGetComponent<T>(out var target)
                            && IsDetectionValid(target))
                        {
                            Transform hitTransform = collider.transform;

                            //  if the closest object is not set, set it to the current object  
                            if (closestTransform == null)
                            {
                                closestTransform = hitTransform;
                                DetectedObject = target;
                            }
                            else
                            {
                                //  if the current object is closer than the closest object, set the closest object to the current object
                                if ((hitTransform.position - transform.position).sqrMagnitude < (closestTransform.position - transform.position).sqrMagnitude)
                                {
                                    closestTransform = hitTransform;
                                    DetectedObject = target;
                                }
                            }
                        }
                    }
                }


                if (closestTransform != null)
                {
                    IsObjectDetected = true;
                    ObjectWorldPosition = closestTransform.position;
                    ObjectDirection = closestTransform.position - transform.position;
                    //  flatten the PlayerDirection vector to 2D
                    ObjectDirection = new Vector3(ObjectDirection.x, 0, ObjectDirection.z);
                    ObjectDistanceSqr = ObjectDirection.sqrMagnitude;
                }
            }
            _frameCounter = 0;
        }

        public void PauseDetection(float duration)
        {
            _pauseTimer = duration;
        }
    }
}