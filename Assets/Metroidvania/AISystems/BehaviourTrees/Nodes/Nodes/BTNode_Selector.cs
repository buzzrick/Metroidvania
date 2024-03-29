namespace Buzzrick.AISystems.BehaviourTree
{
    public class BTNode_Selector : BTNodeBase
    {
        protected override bool ContinueEvaluatingIfChildFailed()
        {
            return true;
        }

        protected override bool ContinueEvaluatingIfChildSucceeded()
        {
            return false;
        }

        protected override void OnTickedAllChildren()
        {
            LastStatus = Children[^1].LastStatus;
        }
    }
}