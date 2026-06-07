using Buzzrick.AISystems.BehaviourTree;
using Metroidvania.AISystems.Blackboard;
using System;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI
{
    [Serializable]
    public abstract class NPC_AI_Base : ScriptableObject
    {
        public abstract BTNodeBase BuildBehaviourTree(BehaviourTree LinkedBT, Blackboard<BlackboardKey> blackboard);
        public virtual void InitialiseBlackboard(Blackboard<BlackboardKey> blackboard, Transform characterTransform) { }
        public virtual void RenderGizmos(Blackboard<BlackboardKey> blackboard, Transform characterTransform) { }
        public virtual void InstallRequiredComponents(Transform characterTransform) { }
    }
}