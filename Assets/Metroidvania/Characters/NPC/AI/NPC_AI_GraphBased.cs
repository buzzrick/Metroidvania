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
                    var node = new BTNode_Sequence { Name = Label("Sequence", data.NodeName) };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Selector:
                {
                    var node = new BTNode_Selector { Name = Label("Selector", data.NodeName) };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Random:
                {
                    var node = new BTNode_Random { Name = Label("Random", data.NodeName) };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Parallel:
                {
                    var node = new BTNode_Parallel { Name = Label("Parallel", data.NodeName) };
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.ReturnResult:
                {
                    var label = Label("Return", data.NodeName);
                    var node  = new BTNode_ReturnResult(data.ResultStatus, label);
                    foreach (int childIdx in data.ChildrenIndices)
                    {
                        var child = BuildNode(childIdx, linkedBT, blackboard, node);
                        if (child != null) node.Add(child);
                    }
                    return node;
                }

                case EBTNodeType.Action:
                {
                    var label = Label("Action", data.NodeName, data.Action);
                    if (data.Action == null) return new BTNode_Action(label, null, null);
                    var action = data.Action;
                    var bb     = blackboard;
                    return new BTNode_Action(label,
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
                        child.AddDecorator<BTDecoratorBase>(Label("Decorator", data.NodeName, data.Condition), () => condition.Evaluate(bb));
                    }
                    return child;
                }

                case EBTNodeType.Service:
                {
                    if (data.Service != null && parent != null)
                    {
                        var service = data.Service;
                        var bb      = blackboard;
                        parent.AddService<BTServiceBase>(Label("Service", data.NodeName, data.Service), dt => service.OnTick(dt, bb));
                    }
                    return null;
                }

                default:
                    Debug.LogWarning($"{name}: Unknown node type {data.Type} at index {nodeIndex}");
                    return null;
            }
        }

        static string Label(string prefix, string nodeName, Object asset = null)
        {
            string suffix = !string.IsNullOrEmpty(nodeName) ? nodeName
                          : asset != null                   ? asset.name
                          : null;
            return suffix != null ? $"{prefix} ({suffix})" : prefix;
        }
    }
}
