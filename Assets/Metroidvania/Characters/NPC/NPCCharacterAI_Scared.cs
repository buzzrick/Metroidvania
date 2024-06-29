using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.UnityLibs.Attributes;
using KinematicCharacterController.Examples;
using Metroidvania.AISystems.Blackboard;
using Metroidvania.Characters.NPC;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.Metroidvania.Characters.NPC
{
    [RequireComponent(typeof(BehaviourTree), typeof(NPCPlayerDetector))]
    public class NPCCharacterAI_Scared : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, RequiredField] protected NPCCharacterController _npcCharacterController;
        [SerializeField, RequiredField] protected NPCPlayerDetector _playerDetector;
        [SerializeField, RequiredField] protected BehaviourTree LinkedBT;

        [Header("NPC Settings")]
        [SerializeField] private float _maxVelocity = 0.25f;
        [SerializeField] private float _fleeDistance = 5f;
        [SerializeField] private float _wanderRadius = 5f;
        [SerializeField] private float _minIdleTime = 3f;
        [SerializeField] private float _maxIdleTime = 10f;

        private Blackboard<BlackboardKey> _blackboard;

        private float _fleeDistanceSqr;
        //  the input details that will be sent through to the character controller 
        private AICharacterInputs _inputs;
        //private bool _shouldFlee;

        //private Vector3 _startPosition;         //  center of random wander radius
        private float _wanderRadiusSqr;         //  the wander radius squared
        private Vector3 _targetWanderPosition;  //  the current random target position to wander to
        private float _wanderVelocity;          //  the speed at which we are wandering
        private Vector3 _wanderVector;
        private float _wanderTimer;             //  how much longer we'll wander for (if we're wandering for too long it means we can't find a path, so just fail after a while.)
        private float _idleTimer;               //  how much longer we'll idle for

        private readonly BlackboardKey _shouldFleeKey = new BlackboardKey { Name = "ShouldFlee" };
        private readonly BlackboardKey _characterControllerKey = new BlackboardKey { Name = "CharacterController" };
        private readonly BlackboardKey _transformKey = new BlackboardKey { Name = "Transform" };
        private readonly BlackboardKey _startPositionKey = new BlackboardKey { Name = "StartPosition" };
        private readonly BlackboardKey _wanderRadiusKey = new BlackboardKey { Name = "WanderRadius" };
        private readonly BlackboardKey _playerDetectorKey = new BlackboardKey { Name = "PlayerDetector" };
        private readonly BlackboardKey _inputsKey = new BlackboardKey { Name = "Inputs" };

        private void Awake()
        {
            _fleeDistanceSqr = _fleeDistance * _fleeDistance;
            _wanderRadiusSqr= _wanderRadius * _wanderRadius;
            _blackboard = BlackboardManager.Instance.GetIndividualBlackboard<BlackboardKey>(this);

            _blackboard.SetGeneric(_shouldFleeKey, false);
            _blackboard.SetGeneric(_characterControllerKey, _npcCharacterController);
            _blackboard.SetGeneric(_transformKey, transform);
            _blackboard.SetGeneric(_startPositionKey, transform.position);
            _blackboard.SetGeneric(_wanderRadiusKey, _wanderRadius);
            _blackboard.SetGeneric(_playerDetectorKey, _playerDetector);
            _blackboard.SetGeneric(_inputsKey, _inputs);
        }

        private void Reset()
        {
            LinkedBT = GetComponent<BehaviourTree>();
            _playerDetector = GetComponent<NPCPlayerDetector>();
        }

        protected virtual void Start()
        {
            //LocalMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackboardKey>(this);

            var BTRoot = LinkedBT.RootNode.Add<BTNode_Selector>("Scared NPC Base Logic");

            // This service will check if the player is closer than the minimum flee distance
            BTRoot.AddService<BTServiceBase>("Check for Player", (deltaTime) =>
            {
                _blackboard.Set(_shouldFleeKey, (_playerDetector.PlayerDistanceSqr < _fleeDistanceSqr));
            });

            var fleeNode = BTRoot.Add<BTNode_Sequence>("Flee Player");
            fleeNode.AddDecorator<BTDecoratorBase>("Should Flee", () => _blackboard.GetBool(_shouldFleeKey));

            fleeNode.Add<BTNode_Action>(new BTNode_Action("Flee Action",
                () => //    OnEnter state
                {
                    return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>   //  OnTick state
                {
                    Vector3 MoveVector = -_playerDetector.PlayerDirection.normalized;
                    _inputs.MoveVector = MoveVector * _maxVelocity;
                    _inputs.LookVector = MoveVector;    //  look the way that we're moving

                    _blackboard.GetGeneric<NPCCharacterController>(_characterControllerKey).SetInputs(ref _inputs);

                    return _blackboard.GetBool(_shouldFleeKey) ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Succeeded;
                }));

            BTNode_Action wanderAction = new BTNode_Action("Wander Action",
                () =>
                {
                    //  determine a random spot within our wander radius, and start moving there
                    Vector3 _startPosition = _blackboard.GetGeneric<Vector3>(_startPositionKey);
                    _targetWanderPosition = _startPosition + new Vector3(Random.Range(-_wanderRadius, _wanderRadius), 0f, Random.Range(-_wanderRadius, _wanderRadius));
                    _wanderVelocity = Random.Range(_maxVelocity * 0.25f, _maxVelocity);
                    MoveTowardsTarget(_targetWanderPosition, _wanderVelocity);
                    _wanderTimer = 10f;

                    //Debug.Log($"Wander starting");
                    return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>
                {
                    MoveTowardsTarget(_targetWanderPosition, _wanderVelocity);

                    //Debug.Log($"Wander in progress {DistanceToTargetSqr(_targetWanderPosition)}");

                    _wanderTimer -= Time.deltaTime;
                    if (_wanderTimer < 0f)
                    {
                        StopMovement();
                        return BehaviourTree.ENodeStatus.Failed;
                    }

                    if (DistanceToTargetSqr(_targetWanderPosition) < 0.05f)
                    {
                        StopMovement();
                        return BehaviourTree.ENodeStatus.Succeeded;
                    }

                    //  check when we've arrived, and return success when we are there
                    return BehaviourTree.ENodeStatus.InProgress;
                });

            var returnToZoneNode = BTRoot.Add<BTNode_Sequence>("Return to Zone");
            returnToZoneNode.AddDecorator<BTDecoratorBase>("Is out of zone", () => (_blackboard.GetGeneric<Vector3>(_startPositionKey) - transform.position).sqrMagnitude > _wanderRadiusSqr);

            returnToZoneNode.Add(wanderAction);

            var randomIdleNode = BTRoot.Add<BTNode_Random>("Random Idle");

            var idleNode = randomIdleNode.Add(new BTNode_Action("Idle Animation",
                () =>
                {
                    _idleTimer = Random.Range(_minIdleTime, _maxIdleTime);
                    //Debug.Log($"Standing Idle for {_idleTimer}");
                    return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>
                {
                    _idleTimer -= Time.deltaTime;
                    StopMovement();
                    //Debug.Log($"Standing Idle for {_idleTimer}");
                    if (_idleTimer > 0f)
                        return BehaviourTree.ENodeStatus.InProgress;
                    else
                        return BehaviourTree.ENodeStatus.Succeeded;
                }));

            var wanderNode = randomIdleNode.Add<BTNode_Sequence>("Wander");
            wanderNode.Add<BTNode_Action>(wanderAction);


            //  
        }

        private void MoveTowardsTarget(Vector3 target, float velocity)
        {
            _wanderVector = (target - transform.position);
            //  flatten the wander vector to 2D
            _wanderVector = new Vector3(_wanderVector.x, 0, _wanderVector.z);
            _wanderVector = _wanderVector.normalized * velocity;
            _inputs.MoveVector = _wanderVector;
            _inputs.LookVector = _wanderVector;    //  look the way that we're moving
            _npcCharacterController.SetInputs(ref _inputs);
        }

        private void StopMovement()
        {
            _wanderVector = Vector3.zero;
            _inputs.MoveVector = Vector2.zero;
            _npcCharacterController.SetInputs(ref _inputs);
        }



        private float DistanceToTargetSqr(Vector3 target)
        {
            //  get the target distance on the XZ plane
            Vector3 distToTarget = (transform.position - target); 
            distToTarget.y = 0;
            return distToTarget.sqrMagnitude;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (Application.isPlaying)
            {
                Gizmos.DrawLine(transform.position, _targetWanderPosition);
                Gizmos.DrawWireSphere(_blackboard.GetGeneric<Vector3>(_startPositionKey), _wanderRadius); //  shows it's wander area
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, _wanderRadius); //  shows it's wander area
            }
        }

        [Button("Debug BehaviourTree")]
        private void DebugBT()
        {
            Debug.Log(LinkedBT.GetDebugText());
        }
    }
}