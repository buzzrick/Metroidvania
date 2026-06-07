using System;
using Buzzrick.AISystems.BehaviourTree.Graph;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_Service : BTGraphNodeBase
    {
        [SerializeField] public BTServiceSO Service;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineInPort(context);
            // No output — services don't have children
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("NodeName").WithDisplayName("Name").WithDefaultValue("").Build();
        }
    }
}
