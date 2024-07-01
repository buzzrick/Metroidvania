using Buzzrick.AISystems.BehaviourTree;
using KinematicCharacterController.Examples;
using Metroidvania.AISystems.Blackboard;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Metroidvania.Characters.NPC.AI
{
    [Serializable, CreateAssetMenu(fileName = "NPC AI Scared", menuName = "Metroidvania/AI/NPC AI Scared")]
    public class NPC_AI_Scared : NPC_AI_Base
    {
        [Header("NPC Settings")]
        [SerializeField] protected float _maxVelocity = 0.25f;
        [SerializeField] protected float _fleeDistance = 5f;
        [SerializeField] protected float _wanderRadius = 5f;
        [SerializeField] protected float _minIdleTime = 3f;
        [SerializeField] protected float _maxIdleTime = 10f;

        private float _wanderRadiusSqr;
        private float _fleeDistanceSqr;

        protected static BlackboardKey _shouldFleeKey = new BlackboardKey { Name = "ShouldFlee" };
        protected static BlackboardKey _characterControllerKey = new BlackboardKey { Name = "CharacterController" };
        protected static BlackboardKey _transformKey = new BlackboardKey { Name = "Transform" };
        protected static BlackboardKey _startPositionKey = new BlackboardKey { Name = "StartPosition" };
        protected static BlackboardKey _wanderVelocityKey = new BlackboardKey { Name = "WanderVelocity" };  //  used for random wander velocity
        protected static BlackboardKey _wanderTargetKey = new BlackboardKey { Name = "WanderTarget" };
        protected static BlackboardKey _wanderTimerKey = new BlackboardKey { Name = "WanderTimer" };
        protected static BlackboardKey _idleTimerKey = new BlackboardKey { Name = "IdleTimer" };
        protected static BlackboardKey _playerDetectorKey = new BlackboardKey { Name = "PlayerDetector" };
        protected static BlackboardKey _inputsKey = new BlackboardKey { Name = "Inputs" };          //  Should this be stored in the blackboard? probably because we're persisting look direction, even on stop movement


        public override BTNodeBase BuildBehaviourTree(BehaviourTree LinkedBT, Blackboard<BlackboardKey> blackboard)
        {
            _wanderRadiusSqr = _wanderRadius * _wanderRadius;
            _fleeDistanceSqr = _fleeDistance * _fleeDistance;

            BTNodeBase BTRoot = LinkedBT.RootNode.Add<BTNode_Selector>(name);
            BuildBehaviourTreeNodes(BTRoot, blackboard);
            return BTRoot; 
        }


        protected virtual void BuildBehaviourTreeNodes(BTNodeBase BTRoot, Blackboard<BlackboardKey> blackboard)
        {
            BuildFleePlayerNodes(BTRoot, blackboard);
            BuildRandomIdleNodes(BTRoot, blackboard);

        }


        protected void BuildFleePlayerNodes(BTNodeBase BTRoot, Blackboard<BlackboardKey> blackboard)
        {
            // This service will check if the player is closer than the minimum flee distance
            BTRoot.AddService<BTServiceBase>("Check for Player", (deltaTime) =>
            {
                NPCPlayerDetector playerDetector = blackboard.GetGeneric<NPCPlayerDetector>(_playerDetectorKey);
                blackboard.Set(_shouldFleeKey, (playerDetector.PlayerDistanceSqr < _fleeDistanceSqr));
            });

            var fleeNode = BTRoot.Add<BTNode_Sequence>("Flee Player");
            fleeNode.AddDecorator<BTDecoratorBase>("Should Flee", () => blackboard.GetBool(_shouldFleeKey));

            fleeNode.Add<BTNode_Action>(new BTNode_Action("Flee Action",
                () => //    OnEnter state
                {
                    return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>   //  OnTick state
                {
                    NPCPlayerDetector playerDetector = blackboard.GetGeneric<NPCPlayerDetector>(_playerDetectorKey);


                    Vector3 MoveVector = -playerDetector.PlayerDirection.normalized;

                    AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(_inputsKey);
                    inputs.MoveVector = MoveVector * _maxVelocity;
                    inputs.LookVector = MoveVector;    //  look the way that we're moving

                    blackboard.GetGeneric<NPCCharacterController>(_characterControllerKey).SetInputs(ref inputs);

                    return blackboard.GetBool(_shouldFleeKey) ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Succeeded;
                }));
        }

        protected void BuildRandomIdleNodes(BTNodeBase BTRoot, Blackboard<BlackboardKey> blackboard)
        {
            BTNode_Action wanderAction = new BTNode_Action("Wander Action",
                () =>
                {
                    //  determine a random spot within our wander radius, and start moving there
                    Vector3 _startPosition = blackboard.GetVector3(_startPositionKey);

                    Vector3 targetWanderPosition = _startPosition + new Vector3(Random.Range(-_wanderRadius, _wanderRadius), 0f, Random.Range(-_wanderRadius, _wanderRadius));
                    blackboard.Set(_wanderTargetKey, targetWanderPosition);

                    float wanderVelocity = Random.Range(_maxVelocity * 0.25f, _maxVelocity);
                    blackboard.Set(_wanderVelocityKey, wanderVelocity);
                    MoveTowardsTarget(targetWanderPosition, wanderVelocity, blackboard);
                    blackboard.Set(_wanderTimerKey, 10f);

                    //Debug.Log($"Wander starting");
                    return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>
                {
                    float wanderVelocity = blackboard.GetFloat(_wanderVelocityKey);
                    Vector3 targetWanderPosition = blackboard.GetVector3(_wanderTargetKey);
                    MoveTowardsTarget(targetWanderPosition, wanderVelocity, blackboard);

                    //Debug.Log($"Wander in progress {DistanceToTargetSqr(_targetWanderPosition)}");

                    float wanderTimer = blackboard.GetFloat(_wanderTimerKey);
                    wanderTimer -= Time.deltaTime;
                    blackboard.Set(_wanderTimerKey, wanderTimer);
                    if (wanderTimer < 0f)
                    {
                        StopMovement(blackboard);
                        return BehaviourTree.ENodeStatus.Failed;
                    }

                    if (DistanceToTargetSqr(targetWanderPosition, blackboard) < 0.05f)
                    {
                        StopMovement(blackboard);
                        return BehaviourTree.ENodeStatus.Succeeded;
                    }

                    //  check when we've arrived, and return success when we are there
                    return BehaviourTree.ENodeStatus.InProgress;
                });

            var returnToZoneNode = BTRoot.Add<BTNode_Sequence>("Return to Zone");
            returnToZoneNode.AddDecorator<BTDecoratorBase>("Is out of zone",
                () =>
                {
                    Transform transform = blackboard.GetGeneric<Transform>(_transformKey);
                    return (blackboard.GetVector3(_startPositionKey) - transform.position).sqrMagnitude > _wanderRadiusSqr;
                });
            returnToZoneNode.Add(wanderAction);

            var randomIdleNode = BTRoot.Add<BTNode_Random>("Random Idle");

            var idleNode = randomIdleNode.Add(new BTNode_Action("Idle Animation",
                () =>
                {
                    float idleTimer = Random.Range(_minIdleTime, _maxIdleTime);
                    blackboard.Set(_idleTimerKey, idleTimer);
                    //Debug.Log($"Standing Idle for {_idleTimer}");
                    return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>
                {
                    float idleTimer = blackboard.GetFloat(_idleTimerKey);
                    idleTimer -= Time.deltaTime;
                    blackboard.Set(_idleTimerKey, idleTimer);
                    StopMovement(blackboard);
                    //Debug.Log($"Standing Idle for {_idleTimer}");
                    if (idleTimer > 0f)
                        return BehaviourTree.ENodeStatus.InProgress;
                    else
                        return BehaviourTree.ENodeStatus.Succeeded;
                }));

            var wanderNode = randomIdleNode.Add<BTNode_Sequence>("Wander");
            wanderNode.Add<BTNode_Action>(wanderAction);
        }


        protected void MoveTowardsTarget(Vector3 target, float velocity, Blackboard<BlackboardKey> blackboard)
        {
            Transform transform = blackboard.GetGeneric<Transform>(_transformKey);

            Vector3 wanderVector = (target - transform.position);

            //  flatten the wander vector to 2D
            wanderVector = new Vector3(wanderVector.x, 0, wanderVector.z);
            wanderVector = wanderVector.normalized * velocity;

            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(_inputsKey);

            inputs.MoveVector = wanderVector;
            inputs.LookVector = wanderVector;    //  look the way that we're moving

            blackboard.GetGeneric<NPCCharacterController>(_characterControllerKey).SetInputs(ref inputs);
        }

        protected void StopMovement(Blackboard<BlackboardKey> blackboard)
        {
            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(_inputsKey);
            inputs.MoveVector = Vector2.zero;
            blackboard.GetGeneric<NPCCharacterController>(_characterControllerKey).SetInputs(ref inputs);
        }


        protected float DistanceToTargetSqr(Vector3 target, Blackboard<BlackboardKey> blackboard)
        {
            Transform transform = blackboard.GetGeneric<Transform>(_transformKey);
            Vector3 distToTarget = (transform.position - target);
            //  get the target distance on the XZ plane
            distToTarget.y = 0;
            return distToTarget.sqrMagnitude;
        }

        public override void InitialiseBlackboard(Blackboard<BlackboardKey> blackboard, Transform characterTransform)
        {
            blackboard.Set(_startPositionKey, characterTransform.position);
            blackboard.SetGeneric(_transformKey, characterTransform);
            blackboard.SetGeneric(_inputsKey, new AICharacterInputs());
            blackboard.SetGeneric(_playerDetectorKey, characterTransform.GetComponent<NPCPlayerDetector>());
            blackboard.Set(_wanderTargetKey, characterTransform.position);
            blackboard.Set(_wanderVelocityKey, 0f);
        }

        public override void RenderGizmos(Blackboard<BlackboardKey> blackboard, Transform characterTransform)
        {
            Gizmos.color = Color.red;
            if (Application.isPlaying && blackboard != null)
            {
                Vector3 targetWanderPosition = blackboard.GetVector3(_wanderTargetKey);
                Gizmos.DrawLine(characterTransform.position, targetWanderPosition);
                Gizmos.DrawWireSphere(blackboard.GetVector3(_startPositionKey), _wanderRadius); //  shows it's wander area
            }
            else
            {
                Gizmos.DrawWireSphere(characterTransform.position, _wanderRadius); //  shows it's wander area
            }
        }

        public override void InstallRequiredComponents(Transform characterTransform)
        {
            if (characterTransform.GetComponent<NPCPlayerDetector>() == null)
            {
                characterTransform.gameObject.AddComponent<NPCPlayerDetector>();
            }
        }
    }
}