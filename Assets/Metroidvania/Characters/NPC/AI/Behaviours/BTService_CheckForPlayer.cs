using Buzzrick.AISystems.BehaviourTree.Graph;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTService_CheckForPlayer", menuName = "BehaviourTree/Services/Check For Player")]
    public class BTService_CheckForPlayer : BTServiceSO
    {
        [SerializeField] float _fleeDistance = 5f;

        public override void OnTick(float deltaTime, Blackboard<BlackboardKey> blackboard)
        {
            var playerDetector = blackboard.GetGeneric<NPCPlayerDetector>(NPCBlackboardKeys.PlayerDetector);
            blackboard.Set(NPCBlackboardKeys.ShouldFlee,
                playerDetector.PlayerDistanceSqr < _fleeDistance * _fleeDistance);
        }
    }
}
