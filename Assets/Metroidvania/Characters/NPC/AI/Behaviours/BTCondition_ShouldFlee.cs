using Buzzrick.AISystems.BehaviourTree.Graph;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTCondition_ShouldFlee", menuName = "BehaviourTree/Conditions/Should Flee")]
    public class BTCondition_ShouldFlee : BTConditionSO
    {
        public override bool Evaluate(Blackboard<BlackboardKey> blackboard)
            => blackboard.GetBool(NPCBlackboardKeys.ShouldFlee);
    }
}
