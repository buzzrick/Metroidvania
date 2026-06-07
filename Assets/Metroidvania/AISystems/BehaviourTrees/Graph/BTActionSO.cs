using Buzzrick.AISystems.BehaviourTree;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree.Graph
{
    public abstract class BTActionSO : ScriptableObject
    {
        public virtual BehaviourTree.ENodeStatus OnEnter(Blackboard<BlackboardKey> blackboard)
            => BehaviourTree.ENodeStatus.InProgress;

        public abstract BehaviourTree.ENodeStatus OnTick(Blackboard<BlackboardKey> blackboard);
    }
}
