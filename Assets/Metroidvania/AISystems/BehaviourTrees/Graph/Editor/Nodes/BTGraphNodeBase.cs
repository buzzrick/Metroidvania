using System;
using Unity.GraphToolkit.Editor;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal abstract class BTGraphNodeBase : Node
    {
        internal const string PORT_IN  = "ParentIn";
        internal const string PORT_OUT = "ChildrenOut";

        protected static void DefineInPort(IPortDefinitionContext ctx)
        {
            ctx.AddInputPort<BTExecutionFlow>(PORT_IN)
               .WithDisplayName("")
               .WithConnectorUI(PortConnectorUI.Arrowhead)
               .Build();
        }

        protected static void DefineOutPort(IPortDefinitionContext ctx)
        {
            ctx.AddOutputPort<BTExecutionFlow>(PORT_OUT)
               .WithDisplayName("Children")
               .WithConnectorUI(PortConnectorUI.Arrowhead)
               .Build();
        }
    }
}
