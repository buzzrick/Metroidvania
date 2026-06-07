using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree.Graph
{
    public abstract class BTConditionSO : ScriptableObject
    {
        public abstract bool Evaluate(Blackboard<BlackboardKey> blackboard);
    }
}
