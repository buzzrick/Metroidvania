using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.AISystems.BehaviourTree.Graph;
using KinematicCharacterController.Examples;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTAction_Attack", menuName = "BehaviourTree/Actions/Attack")]
    public class BTAction_Attack : BTActionSO
    {
        [SerializeField] float  _attackDuration = 1.2f;
        [SerializeField] string _attackTrigger  = "Attack";

        public override BehaviourTree.ENodeStatus OnEnter(Blackboard<BlackboardKey> blackboard)
        {
            blackboard.Set(NPCBlackboardKeys.AttackTimer, _attackDuration);

            // Stop movement and face the player
            var playerDetector = blackboard.GetGeneric<NPCPlayerDetector>(NPCBlackboardKeys.PlayerDetector);
            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(NPCBlackboardKeys.Inputs);
            inputs.MoveVector = Vector3.zero;
            inputs.LookVector = playerDetector.PlayerDirection.normalized;
            blackboard.GetGeneric<NPCCharacterController>(NPCBlackboardKeys.CharacterController).SetInputs(ref inputs);

            // Trigger attack animation if an Animator is present
            if (!string.IsNullOrEmpty(_attackTrigger) &&
                blackboard.TryGetGeneric<Animator>(NPCBlackboardKeys.Animator, out var animator, null))
            {
                animator.SetTrigger(_attackTrigger);
            }

            return BehaviourTree.ENodeStatus.InProgress;
        }

        public override BehaviourTree.ENodeStatus OnTick(Blackboard<BlackboardKey> blackboard)
        {
            float timer = blackboard.GetFloat(NPCBlackboardKeys.AttackTimer) - Time.deltaTime;
            blackboard.Set(NPCBlackboardKeys.AttackTimer, timer);
            return timer > 0f ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Succeeded;
        }
    }
}
