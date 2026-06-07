namespace Buzzrick.AISystems.BehaviourTree
{
    public class BTNode_Parallel : BTNodeBase
    {
        protected override bool OnTick(float deltaTime)
        {
            if (!DecoratorsPermitRunning)
            {
                LastStatus = BehaviourTree.ENodeStatus.Failed;
                return true;
            }

            if (Children.Count == 0)
            {
                LastStatus = BehaviourTree.ENodeStatus.Failed;
                return false;
            }

            TickServices(deltaTime);

            // First child is primary — its status drives this node's result
            var primary = Children[0];
            bool primaryWasEnabled = primary.DecoratorsPermitRunning;
            bool primaryIsEnabled  = primary.EvaluateDecorators();
            if (!primaryWasEnabled && primaryIsEnabled)
                primary.Reset();

            bool tickedAnyNodes = false;
            if (primaryIsEnabled)
                tickedAnyNodes |= primary.Tick(deltaTime);

            // Remaining children always run regardless of the primary's result
            for (int i = 1; i < Children.Count; i++)
            {
                var child = Children[i];
                bool wasEnabled = child.DecoratorsPermitRunning;
                bool isEnabled  = child.EvaluateDecorators();
                if (!wasEnabled && isEnabled)
                    child.Reset();
                if (isEnabled)
                    child.Tick(deltaTime);
            }

            LastStatus = primaryIsEnabled ? primary.LastStatus : BehaviourTree.ENodeStatus.Failed;

            return tickedAnyNodes;
        }
    }
}
