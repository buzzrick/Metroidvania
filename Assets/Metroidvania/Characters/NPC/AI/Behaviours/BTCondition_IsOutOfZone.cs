using Buzzrick.AISystems.BehaviourTree.Graph;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTCondition_IsOutOfZone", menuName = "BehaviourTree/Conditions/Is Out Of Zone")]
    public class BTCondition_IsOutOfZone : BTConditionSO
    {
        [SerializeField] float _wanderRadius = 5f;

        public override bool Evaluate(Blackboard<BlackboardKey> blackboard)
        {
            Transform t = blackboard.GetGeneric<Transform>(NPCBlackboardKeys.Transform);
            return (blackboard.GetVector3(NPCBlackboardKeys.StartPosition) - t.position).sqrMagnitude
                   > _wanderRadius * _wanderRadius;
        }
    }
}
