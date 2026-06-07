using System;
using System.Collections.Generic;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree.Graph
{
    public enum EBTNodeType
    {
        Root,
        Sequence,
        Selector,
        Parallel,
        Random,
        Action,
        ReturnResult,
        Decorator,
        Service,
    }

    [Serializable]
    public class BTRuntimeNodeData
    {
        public EBTNodeType Type;
        public string NodeName;
        public BehaviourTree.ENodeStatus ResultStatus;
        public List<int> ChildrenIndices = new();

        public BTActionSO    Action;
        public BTConditionSO Condition;
        public BTServiceSO   Service;
    }

    public class BehaviourTreeRuntimeData : ScriptableObject
    {
        public List<BTRuntimeNodeData> Nodes = new();
        public int RootNodeIndex;
    }
}
