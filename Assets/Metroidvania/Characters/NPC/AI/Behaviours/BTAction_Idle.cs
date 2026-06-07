using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.AISystems.BehaviourTree.Graph;
using KinematicCharacterController.Examples;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTAction_Idle", menuName = "BehaviourTree/Actions/Idle")]
    public class BTAction_Idle : BTActionSO
    {
        [SerializeField] float _minIdleTime = 3f;
        [SerializeField] float _maxIdleTime = 10f;

        public override BehaviourTree.ENodeStatus OnEnter(Blackboard<BlackboardKey> blackboard)
        {
            blackboard.Set(NPCBlackboardKeys.IdleTimer, Random.Range(_minIdleTime, _maxIdleTime));
            return BehaviourTree.ENodeStatus.InProgress;
        }

        public override BehaviourTree.ENodeStatus OnTick(Blackboard<BlackboardKey> blackboard)
        {
            float timer = blackboard.GetFloat(NPCBlackboardKeys.IdleTimer) - Time.deltaTime;
            blackboard.Set(NPCBlackboardKeys.IdleTimer, timer);
            StopMovement(blackboard);
            return timer > 0f ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Succeeded;
        }

        void StopMovement(Blackboard<BlackboardKey> blackboard)
        {
            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(NPCBlackboardKeys.Inputs);
            inputs.MoveVector = Vector2.zero;
            blackboard.GetGeneric<NPCCharacterController>(NPCBlackboardKeys.CharacterController).SetInputs(ref inputs);
        }
    }
}
