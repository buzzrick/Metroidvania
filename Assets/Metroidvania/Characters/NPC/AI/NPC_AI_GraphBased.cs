using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.AISystems.BehaviourTree.Graph;
using Metroidvania.AISystems.Blackboard;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI
{
    [CreateAssetMenu(fileName = "NPC AI Graph", menuName = "Metroidvania/AI/NPC AI Graph")]
    public class NPC_AI_GraphBased : NPC_AI_Base
    {
        [SerializeField] protected BehaviourTreeRuntimeData _graphData;

        public override BTNodeBase BuildBehaviourTree(BehaviourTree linkedBT, Blackboard<BlackboardKey> blackboard)
        {
            if (_graphData == null || _graphData.Nodes.Count == 0)
            {
                Debug.LogWarning($"{name}: NPC_AI_GraphBased has no graph data assigned.", this);
                return null;
            }

            var rootData  = _graphData.Nodes[_graphData.RootNodeIndex];
            BTNodeBase firstNode = null;

            foreach (int childIdx in rootData.ChildrenIndices)
            {
                var node = BuildNode(childIdx, linkedBT, blackboard, null);
                if (node == null) continue;
                linkedBT.RootNode.Add(node);
                firstNode ??= node;
            }

            return firstNode;
        }

        BTNodeBase BuildNode(int nodeIndex, BehaviourTree linkedBT, Blackboard<BlackboardKey> blackboard, BTNodeBase parent)
        {
            if (nodeIndex < 0 || nodeIndex >= _graphData.Nodes.Count) return null;

            var data = _graphData.Nodes[nodeIndex];

            switch (data.Type)
            {
                case EBTNodeType.Sequence:
                {
                    var node = new BTNode_Sequence { Name = data.NodeName };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Selector:
                {
                    var node = new BTNode_Selector { Name = data.NodeName };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Random:
                {
                    var node = new BTNode_Random { Name = data.NodeName };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Parallel:
                {
                    var node = new BTNode_Parallel { Name = data.NodeName };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.ReturnResult:
                {
                    var node = new BTNode_ReturnResult(data.ResultStatus, data.NodeName);
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Action:
                {
                    if (data.Action == null) return new BTNode_Action(data.NodeName, null, null);
                    var action = data.Action;
                    var bb     = blackboard;
                    return new BTNode_Action(data.NodeName,
                        () => action.OnEnter(bb),
                        () => action.OnTick(bb));
                }

                case EBTNodeType.Decorator:
                {
                    if (data.ChildrenIndices.Count == 0) return null;
                    var child = BuildNode(data.ChildrenIndices[0], linkedBT, blackboard, null);
                    if (child == null) return null;
                    if (data.Condition != null)
                    {
                        var condition = data.Condition;
                        var bb        = blackboard;
                        child.AddDecorator<BTDecoratorBase>(data.NodeName, () => condition.Evaluate(bb));
                    }
                    return child;
                }

                case EBTNodeType.Service:
                {
                    if (data.Service != null && parent != null)
                    {
                        var service = data.Service;
                        var bb      = blackboard;
                        parent.AddService<BTServiceBase>(data.NodeName, dt => service.OnTick(dt, bb));
                    }
                    return null;
                }

                default:
                    Debug.LogWarning($"{name}: Unknown node type {data.Type} at index {nodeIndex}");
                    return null;
            }
        }
    }
}
