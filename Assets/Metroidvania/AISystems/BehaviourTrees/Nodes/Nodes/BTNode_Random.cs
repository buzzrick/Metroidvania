using System.Collections;
using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree
{
    /// <summary>
    /// 
    /// </summary>
    public class BTNode_Random : BTNodeBase
    {
        private BTNodeBase _currentNode;

        protected override bool OnTick(float deltaTime)
        {
            //  if we don't have a current node, pick one at random
            if (_currentNode == null)
            {
                if (!SelectRandomNode())
                    return false;
            }

            //  tick the current node
            bool result = _currentNode.Tick(deltaTime);
            LastStatus = _currentNode.LastStatus;

            if (!result
                || _currentNode.LastStatus != BehaviourTree.ENodeStatus.InProgress)
            {
                //  if the current node failed, reset the randomiser
                _currentNode = null;
            }
            return result;
        }

        public override void Reset()
        {
            base.Reset();
            _currentNode = null;
        }

        private bool SelectRandomNode()
        {
            if (Children.Count == 0)
            {
                LastStatus = BehaviourTree.ENodeStatus.Failed;
                return false;
            }

            _currentNode = Children[Random.Range(0, Children.Count)];
            _currentNode.Reset();
            return true;
        }
    }
}