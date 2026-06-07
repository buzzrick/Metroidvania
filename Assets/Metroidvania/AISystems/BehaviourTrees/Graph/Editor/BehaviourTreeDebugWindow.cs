using System.Collections.Generic;
using Buzzrick.AISystems.BehaviourTree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    public class BehaviourTreeDebugWindow : EditorWindow
    {
        static readonly Color ColorUnknown    = new(0.3f, 0.3f, 0.3f);
        static readonly Color ColorInProgress = new(0f,   0.7f, 0.8f);
        static readonly Color ColorSucceeded  = new(0f,   0.6f, 0.2f);
        static readonly Color ColorFailed     = new(0.7f, 0.1f, 0.1f);

        BehaviourTree _trackedTree;
        ScrollView    _scrollView;
        VisualElement _treeContainer;
        int           _updateCounter;

        [MenuItem("Window/BehaviourTree Debugger")]
        static void Open() => GetWindow<BehaviourTreeDebugWindow>("BT Debugger");

        void CreateGUI()
        {
            _scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            _scrollView.style.flexGrow = 1;
            rootVisualElement.Add(_scrollView);

            _treeContainer = new VisualElement();
            _treeContainer.style.paddingLeft   = 8;
            _treeContainer.style.paddingTop    = 8;
            _treeContainer.style.paddingBottom = 8;
            _scrollView.Add(_treeContainer);
        }

        void OnEnable()
        {
            EditorApplication.update       += OnEditorUpdate;
            Selection.selectionChanged     += OnSelectionChanged;
        }

        void OnDisable()
        {
            EditorApplication.update       -= OnEditorUpdate;
            Selection.selectionChanged     -= OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            _trackedTree = null;
            if (Selection.activeGameObject != null)
                _trackedTree = Selection.activeGameObject.GetComponent<BehaviourTree>();
            Refresh();
        }

        void OnEditorUpdate()
        {
            if (!EditorApplication.isPlaying) return;
            _updateCounter++;
            if (_updateCounter < 6) return;  // ~10fps at 60fps editor
            _updateCounter = 0;
            Refresh();
        }

        void Refresh()
        {
            if (_treeContainer == null) return;
            _treeContainer.Clear();

            if (_trackedTree == null)
            {
                var label = new Label("Select a GameObject with a BehaviourTree component.");
                label.style.color = new StyleColor(Color.grey);
                _treeContainer.Add(label);
                return;
            }

            BuildVisualTree(_trackedTree.RootNode, _treeContainer, 0);
        }

        static void BuildVisualTree(BTNodeBase node, VisualElement parent, int depth)
        {
            if (node == null) return;

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginTop     = 2;
            parent.Add(row);

            // Indentation spacer
            if (depth > 0)
            {
                var spacer = new VisualElement();
                spacer.style.width = depth * 20;
                row.Add(spacer);
            }

            // Node box
            var box = new VisualElement();
            box.style.backgroundColor = new StyleColor(StatusColor(node.LastStatus));
            box.style.borderTopLeftRadius     = 4;
            box.style.borderTopRightRadius    = 4;
            box.style.borderBottomLeftRadius  = 4;
            box.style.borderBottomRightRadius = 4;
            box.style.paddingLeft   = 8;
            box.style.paddingRight  = 8;
            box.style.paddingTop    = 4;
            box.style.paddingBottom = 4;
            box.style.minWidth      = 140;
            row.Add(box);

            var label = new Label($"{node.Name}  [{node.LastStatus}]");
            label.style.color    = new StyleColor(Color.white);
            label.style.fontSize = 12;
            box.Add(label);

            // Recurse into children
            foreach (var child in node.GetChildren())
                BuildVisualTree(child, parent, depth + 1);
        }

        static Color StatusColor(BehaviourTree.ENodeStatus status) => status switch
        {
            BehaviourTree.ENodeStatus.InProgress => ColorInProgress,
            BehaviourTree.ENodeStatus.Succeeded  => ColorSucceeded,
            BehaviourTree.ENodeStatus.Failed     => ColorFailed,
            _                                    => ColorUnknown,
        };
    }
}
