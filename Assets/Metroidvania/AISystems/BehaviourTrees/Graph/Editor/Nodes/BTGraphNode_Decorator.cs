using System;
using Buzzrick.AISystems.BehaviourTree.Graph;
using Unity.GraphToolkit.Editor;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_Decorator : BTGraphNodeBase
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineInPort(context);
            DefineOutPort(context);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("NodeName").WithDisplayName("Name").WithDefaultValue("").Build();
            context.AddOption<BTConditionSO>("Condition").WithDisplayName("Condition").Build();
        }
    }
}
