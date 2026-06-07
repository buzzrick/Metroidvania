using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree.Graph
{
    public abstract class BTServiceSO : ScriptableObject
    {
        public abstract void OnTick(float deltaTime, Blackboard<BlackboardKey> blackboard);
    }
}
