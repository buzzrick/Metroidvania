using UnityEngine;

namespace Buzzrick.AISystems.BehaviourTree
{
    // Borrowed from :
    // https://www.youtube.com/watch?v=b6kvr10uWsg&ab_channel=IainMcManus
    // https://www.youtube.com/watch?v=LL0DtWwIO9A&ab_channel=IainMcManus
    // https://github.com/GameDevEducation/UnityAITutorial_BehaviourTrees/tree/Part-1-Behaviour-Tree-Base
    // https://github.com/GameDevEducation/UnityAITutorial_BehaviourTrees/tree/Part-2-Decorators-and-Services
    // https://github.com/GameDevEducation/UnityAITutorial_BehaviourTrees/tree/Part-3-Parallel-Node

    /// <summary>
    /// </summary>
    public class BehaviourTree : MonoBehaviour
    {
        public enum ENodeStatus
        {
            Unknown,
            InProgress,
            Failed,
            Succeeded
        }

        public BTNodeBase RootNode { get; private set; } = new BTNodeBase("ROOT");

        private void Start()
        {
            RootNode.Reset();
        }

        // Update is called once per frame
        private void Update()
        {
            RootNode.Tick(Time.deltaTime);
        }

        public string GetDebugText()
        {
            return RootNode.GetDebugText();
        }
    }
}