using System.Collections.Generic;
using System.IO;
using Buzzrick.AISystems.BehaviourTree.Graph;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [ScriptedImporter(1, BehaviourTreeGraph.AssetExtension)]
    internal class BehaviourTreeGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<BehaviourTreeGraph>(ctx.assetPath);
            if (graph == null)
            {
                ctx.LogImportError("Failed to load BehaviourTreeGraph asset.");
                return;
            }

            var runtimeData = ScriptableObject.CreateInstance<BehaviourTreeRuntimeData>();
            runtimeData.name = Path.GetFileNameWithoutExtension(ctx.assetPath);

            // Map input ports → owning nodes for reverse connection lookup
            var portToNode = new Dictionary<IPort, Node>();
            foreach (var iNode in graph.GetNodes())
            {
                if (iNode is not Node node) continue;
                foreach (var port in node.GetInputPorts())
                    portToNode[port] = node;
            }

            // Find root node
            BTGraphNode_Root rootNode = null;
            foreach (var iNode in graph.GetNodes())
            {
                if (iNode is BTGraphNode_Root r) { rootNode = r; break; }
            }

            if (rootNode == null)
            {
                ctx.LogImportWarning($"{ctx.assetPath}: No Root node found. Graph will produce an empty tree.");
                ctx.AddObjectToAsset("RuntimeData", runtimeData);
                ctx.SetMainObject(runtimeData);
                return;
            }

            // BFS to build ordered node list and assign stable indices
            var nodeList  = new List<Node>();
            var nodeIndex = new Dictionary<Node, int>();
            var queue     = new Queue<Node>();
            queue.Enqueue(rootNode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (nodeIndex.ContainsKey(current)) continue;

                nodeIndex[current] = nodeList.Count;
                nodeList.Add(current);

                if (current is not BTGraphNode_Action and not BTGraphNode_Service)
                {
                    var childrenPort = current.GetOutputPortByName(BTGraphNodeBase.PORT_OUT);
                    if (childrenPort?.isConnected == true)
                    {
                        var connected = new List<IPort>();
                        childrenPort.GetConnectedPorts(connected);
                        foreach (var p in connected)
                        {
                            if (portToNode.TryGetValue(p, out var child))
                                queue.Enqueue(child);
                        }
                    }
                }
            }

            runtimeData.RootNodeIndex = nodeIndex.TryGetValue(rootNode, out int ri) ? ri : 0;

            foreach (var node in nodeList)
                runtimeData.Nodes.Add(BuildRuntimeData(node, nodeIndex, portToNode));

            ctx.AddObjectToAsset("RuntimeData", runtimeData);
            ctx.SetMainObject(runtimeData);
        }

        static BTRuntimeNodeData BuildRuntimeData(Node node, Dictionary<Node, int> nodeIndex,
                                                  Dictionary<IPort, Node> portToNode)
        {
            var data = new BTRuntimeNodeData();
            data.NodeName = GetStringOption(node, "NodeName");

            switch (node)
            {
                case BTGraphNode_Root:
                    data.Type = EBTNodeType.Root;
                    data.ChildrenIndices = GetChildIndices(node, BTGraphNodeBase.PORT_OUT, nodeIndex, portToNode);
                    break;

                case BTGraphNode_Sequence:
                    data.Type = EBTNodeType.Sequence;
                    data.ChildrenIndices = GetChildIndices(node, BTGraphNodeBase.PORT_OUT, nodeIndex, portToNode);
                    break;

                case BTGraphNode_Selector:
                    data.Type = EBTNodeType.Selector;
                    data.ChildrenIndices = GetChildIndices(node, BTGraphNodeBase.PORT_OUT, nodeIndex, portToNode);
                    break;

                case BTGraphNode_Random:
                    data.Type = EBTNodeType.Random;
                    data.ChildrenIndices = GetChildIndices(node, BTGraphNodeBase.PORT_OUT, nodeIndex, portToNode);
                    break;

                case BTGraphNode_Parallel:
                    data.Type = EBTNodeType.Parallel;
                    data.ChildrenIndices = GetChildIndices(node, BTGraphNodeBase.PORT_OUT, nodeIndex, portToNode);
                    break;

                case BTGraphNode_Action act:
                    data.Type   = EBTNodeType.Action;
                    data.Action = act.Action;
                    break;

                case BTGraphNode_Decorator dec:
                    data.Type      = EBTNodeType.Decorator;
                    data.Condition = dec.Condition;
                    data.ChildrenIndices = GetChildIndices(node, BTGraphNodeBase.PORT_OUT, nodeIndex, portToNode);
                    break;

                case BTGraphNode_Service svc:
                    data.Type    = EBTNodeType.Service;
                    data.Service = svc.Service;
                    break;

                case BTGraphNode_ReturnResult:
                    data.Type = EBTNodeType.ReturnResult;
                    data.ChildrenIndices = GetChildIndices(node, BTGraphNodeBase.PORT_OUT, nodeIndex, portToNode);
                    string statusStr = GetStringOption(node, "ResultStatus");
                    data.ResultStatus = System.Enum.TryParse<BehaviourTree.ENodeStatus>(statusStr, out var status)
                        ? status : BehaviourTree.ENodeStatus.Succeeded;
                    break;
            }

            return data;
        }

        static List<int> GetChildIndices(Node node, string portName, Dictionary<Node, int> nodeIndex,
                                         Dictionary<IPort, Node> portToNode)
        {
            var result = new List<int>();
            var port = node.GetOutputPortByName(portName);
            if (port == null || !port.isConnected) return result;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            foreach (var p in connected)
            {
                if (portToNode.TryGetValue(p, out var child) && nodeIndex.TryGetValue(child, out int idx))
                    result.Add(idx);
            }
            return result;
        }

        static string GetStringOption(Node node, string optionName)
        {
            var opt = node.GetNodeOptionByName(optionName);
            if (opt == null) return "";
            return opt.TryGetValue<string>(out string value) ? (value ?? "") : "";
        }
    }
}
