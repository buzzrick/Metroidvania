using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.AISystems.BehaviourTree.Graph;
using KinematicCharacterController.Examples;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTAction_Wander", menuName = "BehaviourTree/Actions/Wander")]
    public class BTAction_Wander : BTActionSO
    {
        [SerializeField] float _maxVelocity  = 0.25f;
        [SerializeField] float _wanderRadius = 5f;
        [SerializeField] float _wanderTimeout = 10f;

        public override BehaviourTree.ENodeStatus OnEnter(Blackboard<BlackboardKey> blackboard)
        {
            Vector3 start = blackboard.GetVector3(NPCBlackboardKeys.StartPosition);
            Vector3 target = start + new Vector3(
                Random.Range(-_wanderRadius, _wanderRadius), 0f,
                Random.Range(-_wanderRadius, _wanderRadius));
            float velocity = Random.Range(_maxVelocity * 0.25f, _maxVelocity);
            blackboard.Set(NPCBlackboardKeys.WanderTarget,   target);
            blackboard.Set(NPCBlackboardKeys.WanderVelocity, velocity);
            blackboard.Set(NPCBlackboardKeys.WanderTimer,    _wanderTimeout);
            MoveTowards(target, velocity, blackboard);
            return BehaviourTree.ENodeStatus.InProgress;
        }

        public override BehaviourTree.ENodeStatus OnTick(Blackboard<BlackboardKey> blackboard)
        {
            Vector3 target   = blackboard.GetVector3(NPCBlackboardKeys.WanderTarget);
            float   velocity = blackboard.GetFloat(NPCBlackboardKeys.WanderVelocity);
            MoveTowards(target, velocity, blackboard);

            float timer = blackboard.GetFloat(NPCBlackboardKeys.WanderTimer) - Time.deltaTime;
            blackboard.Set(NPCBlackboardKeys.WanderTimer, timer);

            if (timer < 0f)
            {
                StopMovement(blackboard);
                return BehaviourTree.ENodeStatus.Failed;
            }

            Transform t = blackboard.GetGeneric<Transform>(NPCBlackboardKeys.Transform);
            Vector3 d = t.position - target;
            d.y = 0f;
            if (d.sqrMagnitude < 0.05f)
            {
                StopMovement(blackboard);
                return BehaviourTree.ENodeStatus.Succeeded;
            }

            return BehaviourTree.ENodeStatus.InProgress;
        }

        void MoveTowards(Vector3 target, float velocity, Blackboard<BlackboardKey> blackboard)
        {
            Transform t   = blackboard.GetGeneric<Transform>(NPCBlackboardKeys.Transform);
            Vector3   dir = new Vector3(target.x - t.position.x, 0f, target.z - t.position.z).normalized * velocity;
            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(NPCBlackboardKeys.Inputs);
            inputs.MoveVector = dir;
            inputs.LookVector = dir;
            blackboard.GetGeneric<NPCCharacterController>(NPCBlackboardKeys.CharacterController).SetInputs(ref inputs);
        }

        void StopMovement(Blackboard<BlackboardKey> blackboard)
        {
            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(NPCBlackboardKeys.Inputs);
            inputs.MoveVector = Vector2.zero;
            blackboard.GetGeneric<NPCCharacterController>(NPCBlackboardKeys.CharacterController).SetInputs(ref inputs);
        }
    }
}
