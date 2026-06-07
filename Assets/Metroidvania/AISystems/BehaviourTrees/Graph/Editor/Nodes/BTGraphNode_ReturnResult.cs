using System;
using Unity.GraphToolkit.Editor;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_ReturnResult : BTGraphNodeBase
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineInPort(context);
            DefineOutPort(context);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("NodeName").WithDisplayName("Name").WithDefaultValue("").Build();
            context.AddOption<string>("ResultStatus")
                   .WithDisplayName("Result Status")
                   .WithDefaultValue("Succeeded")
                   .Build();
        }
    }
}
