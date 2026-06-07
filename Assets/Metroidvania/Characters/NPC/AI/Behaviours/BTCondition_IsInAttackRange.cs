using Buzzrick.AISystems.BehaviourTree.Graph;
using Metroidvania.AISystems.Blackboard;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [UnityEngine.CreateAssetMenu(fileName = "BTCondition_IsInAttackRange", menuName = "BehaviourTree/Conditions/IsInAttackRange")]
    public class BTCondition_IsInAttackRange : BTConditionSO
    {
        public override bool Evaluate(Blackboard<BlackboardKey> blackboard)
            => blackboard.GetBool(NPCBlackboardKeys.ShouldAttack);
    }
}
