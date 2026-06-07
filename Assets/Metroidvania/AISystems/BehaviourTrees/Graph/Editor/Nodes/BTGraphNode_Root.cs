using System;
using Unity.GraphToolkit.Editor;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    internal class BTGraphNode_Root : BTGraphNodeBase
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineOutPort(context);
        }
    }
}
