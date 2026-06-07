using System;
using Buzzrick.AISystems.BehaviourTree.Graph;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_Action : BTGraphNodeBase
    {
        [SerializeField] public BTActionSO Action;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineInPort(context);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("NodeName").WithDisplayName("Name").WithDefaultValue("").Build();
        }
    }
}
