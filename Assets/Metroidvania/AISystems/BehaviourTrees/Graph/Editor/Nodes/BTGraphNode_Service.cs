using System;
using Buzzrick.AISystems.BehaviourTree.Graph;
using Unity.GraphToolkit.Editor;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_Service : BTGraphNodeBase
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineInPort(context);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("NodeName").WithDisplayName("Name").WithDefaultValue("").Build();
            context.AddOption<BTServiceSO>("Service").WithDisplayName("Service").Build();
        }
    }
}
