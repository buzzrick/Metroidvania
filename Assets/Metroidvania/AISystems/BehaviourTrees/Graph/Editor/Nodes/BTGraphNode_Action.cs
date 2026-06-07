using System;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_Action : BTGraphNodeBase
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineInPort(context);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>("NodeName").WithDisplayName("Name").WithDefaultValue("").Build();
            context.AddOption<BTActionSO>("Action").WithDisplayName("Action").Build();
        }
    }
}
