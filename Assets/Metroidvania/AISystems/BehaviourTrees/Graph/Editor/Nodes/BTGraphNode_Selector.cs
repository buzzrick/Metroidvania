using System;
using Unity.GraphToolkit.Editor;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_Selector : BTGraphNodeBase
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineInPort(context);
            DefineOutPort(context);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("NodeName").WithDisplayName("Name").WithDefaultValue("").Build();
        }
    }
}
