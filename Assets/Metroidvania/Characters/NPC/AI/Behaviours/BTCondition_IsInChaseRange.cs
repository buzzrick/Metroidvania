using Buzzrick.AISystems.BehaviourTree.Graph;
using Metroidvania.AISystems.Blackboard;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [UnityEngine.CreateAssetMenu(fileName = "BTCondition_IsInChaseRange", menuName = "BehaviourTree/Conditions/IsInChaseRange")]
    public class BTCondition_IsInChaseRange : BTConditionSO
    {
        public override bool Evaluate(Blackboard<BlackboardKey> blackboard)
            => blackboard.GetBool(NPCBlackboardKeys.ShouldChase);
    }
}
