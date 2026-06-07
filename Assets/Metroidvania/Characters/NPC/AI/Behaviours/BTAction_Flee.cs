using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.AISystems.BehaviourTree.Graph;
using KinematicCharacterController.Examples;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTAction_Flee", menuName = "BehaviourTree/Actions/Flee")]
    public class BTAction_Flee : BTActionSO
    {
        [SerializeField] float _maxVelocity = 0.25f;

        public override BehaviourTree.ENodeStatus OnEnter(Blackboard<BlackboardKey> blackboard)
            => BehaviourTree.ENodeStatus.InProgress;

        public override BehaviourTree.ENodeStatus OnTick(Blackboard<BlackboardKey> blackboard)
        {
            var playerDetector = blackboard.GetGeneric<NPCPlayerDetector>(NPCBlackboardKeys.PlayerDetector);
            Vector3 moveVector = -playerDetector.PlayerDirection.normalized;
            AICharacterInputs inputs = blackboard.GetGeneric<AICharacterInputs>(NPCBlackboardKeys.Inputs);
            inputs.MoveVector = moveVector * _maxVelocity;
            inputs.LookVector = moveVector;
            blackboard.GetGeneric<NPCCharacterController>(NPCBlackboardKeys.CharacterController).SetInputs(ref inputs);
            return blackboard.GetBool(NPCBlackboardKeys.ShouldFlee)
                ? BehaviourTree.ENodeStatus.InProgress
                : BehaviourTree.ENodeStatus.Succeeded;
        }
    }
}
