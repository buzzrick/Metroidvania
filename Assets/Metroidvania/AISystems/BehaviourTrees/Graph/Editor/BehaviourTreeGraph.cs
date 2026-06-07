using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    internal class BehaviourTreeGraph : Unity.GraphToolkit.Editor.Graph
    {
        internal const string AssetExtension = "btgraph";

        [MenuItem("Assets/Create/BehaviourTree/BehaviourTree Graph")]
        static void CreateAsset()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<BehaviourTreeGraph>("New BehaviourTree");
        }
    }
}
