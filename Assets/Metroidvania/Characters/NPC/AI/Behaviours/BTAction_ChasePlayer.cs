using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.AISystems.BehaviourTree.Graph;
using KinematicCharacterController.Examples;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTAction_ChasePlayer", menuName = "BehaviourTree/Actions/ChasePlayer")]
    public class BTAction_ChasePlayer : BTActionSO
    {
        [SerializeField] float _maxVelocity = 0.5f;

        public override BehaviourTree.ENodeStatus OnTick(Blackboard<BlackboardKey> blackboard)
        {
            var playerDetector = blackboard.GetGeneric<NPCPlayerDetector>(NPCBlackboardKeys.PlayerDetector);
            Vector3 direction = playerDetector.PlayerDirection.normalized;

            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(NPCBlackboardKeys.Inputs);
            inputs.MoveVector = direction * _maxVelocity;
            inputs.LookVector = direction;
            blackboard.GetGeneric<NPCCharacterController>(NPCBlackboardKeys.CharacterController).SetInputs(ref inputs);

            return BehaviourTree.ENodeStatus.InProgress;
        }
    }
}
