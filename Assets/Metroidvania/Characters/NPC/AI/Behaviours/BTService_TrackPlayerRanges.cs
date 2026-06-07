using Buzzrick.AISystems.BehaviourTree.Graph;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI.Behaviours
{
    [CreateAssetMenu(fileName = "BTService_TrackPlayerRanges", menuName = "BehaviourTree/Services/TrackPlayerRanges")]
    public class BTService_TrackPlayerRanges : BTServiceSO
    {
        [SerializeField] float _chaseRange  = 5f;
        [SerializeField] float _attackRange = 1f;

        public override void OnTick(float deltaTime, Blackboard<BlackboardKey> blackboard)
        {
            var playerDetector = blackboard.GetGeneric<NPCPlayerDetector>(NPCBlackboardKeys.PlayerDetector);
            float distSqr = playerDetector.PlayerDistanceSqr;
            blackboard.Set(NPCBlackboardKeys.ShouldChase,  distSqr < _chaseRange  * _chaseRange);
            blackboard.Set(NPCBlackboardKeys.ShouldAttack, distSqr < _attackRange * _attackRange);
        }
    }
}
