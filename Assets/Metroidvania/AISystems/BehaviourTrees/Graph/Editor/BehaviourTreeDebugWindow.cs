using System.Collections.Generic;
using Buzzrick.AISystems.BehaviourTree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    public class BehaviourTreeDebugWindow : EditorWindow
    {
        const float NodeW   = 160f;
        const float NodeH   = 46f;
        const float HGap    = 28f;
        const float VGap    = 60f;
        const float Padding = 24f;

        static readonly Color ColUnknown    = new(0.22f, 0.22f, 0.22f);
        static readonly Color ColInProgress = new(0f,    0.50f, 0.68f);
        static readonly Color ColSucceeded  = new(0.10f, 0.48f, 0.15f);
        static readonly Color ColFailed     = new(0.52f, 0.10f, 0.10f);
        static readonly Color ColEdge       = new(0.50f, 0.50f, 0.50f);

        BehaviourTree _trackedTree;
        Label         _headerLabel;
        VisualElement _canvas;
        VisualElement _edgeLayer;
        int           _updateTick;

        readonly Dictionary<BTNodeBase, Vector2>              _positions = new();
        readonly List<(BTNodeBase from, BTNodeBase to)>       _edgeList  = new();

        [MenuItem("Window/BehaviourTree Debugger")]
        static void Open() => GetWindow<BehaviourTreeDebugWindow>("BT Debugger");

        void CreateGUI()
        {
            _headerLabel = new Label("No BehaviourTree selected.");
            _headerLabel.style.paddingLeft        = 8;
            _headerLabel.style.paddingTop         = 5;
            _headerLabel.style.paddingBottom      = 5;
            _headerLabel.style.color              = new StyleColor(new Color(0.65f, 0.65f, 0.65f));
            _headerLabel.style.fontSize           = 11;
            _headerLabel.style.borderBottomWidth  = 1;
            _headerLabel.style.borderBottomColor  = new StyleColor(new Color(0.18f, 0.18f, 0.18f));
            rootVisualElement.Add(_headerLabel);

            var scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            scrollView.style.flexGrow = 1;
            rootVisualElement.Add(scrollView);

            _canvas = new VisualElement();
            _canvas.style.position = Position.Relative;
            scrollView.Add(_canvas);

            _edgeLayer = new VisualElement();
            _edgeLayer.style.position  = Position.Absolute;
            _edgeLayer.style.left      = 0;
            _edgeLayer.style.top       = 0;
            _edgeLayer.pickingMode     = PickingMode.Ignore;
            _edgeLayer.generateVisualContent += PaintEdges;
            _canvas.Add(_edgeLayer);
        }

        void OnEnable()
        {
            EditorApplication.update   += OnEditorUpdate;
            Selection.selectionChanged += OnSelectionChanged;
        }

        void OnDisable()
        {
            EditorApplication.update   -= OnEditorUpdate;
            Selection.selectionChanged -= OnSelectionChanged;
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
            if (++_updateTick < 6) return;
            _updateTick = 0;
            Refresh();
        }

        // ── Layout & rendering ───────────────────────────────────────────────

        void Refresh()
        {
            if (_canvas == null) return;

            _canvas.Clear();
            _canvas.Add(_edgeLayer);   // always bottom layer

            _positions.Clear();
            _edgeList.Clear();

            if (_trackedTree == null || _trackedTree.RootNode == null)
            {
                if (_headerLabel != null)
                    _headerLabel.text = "No BehaviourTree selected — pick a GameObject with a BehaviourTree component.";
                _canvas.style.width  = 1;
                _canvas.style.height = 1;
                _edgeLayer.MarkDirtyRepaint();
                return;
            }

            if (_headerLabel != null)
                _headerLabel.text = $"Tracking:  {_trackedTree.gameObject.name}";

            float rootCx = Padding + CalcSubtreeWidth(_trackedTree.RootNode) / 2f;
            LayoutNode(_trackedTree.RootNode, rootCx, 0, null);

            float maxX = 0f, maxY = 0f;
            foreach (var pos in _positions.Values)
            {
                maxX = Mathf.Max(maxX, pos.x + NodeW / 2f);
                maxY = Mathf.Max(maxY, pos.y + NodeH);
            }
            float w = maxX + Padding;
            float h = maxY + Padding;

            _canvas.style.width     = w;
            _canvas.style.height    = h;
            _edgeLayer.style.width  = w;
            _edgeLayer.style.height = h;
            _edgeLayer.MarkDirtyRepaint();

            foreach (var (node, pos) in _positions)
                _canvas.Add(MakeNodeBox(node, pos));
        }

        float CalcSubtreeWidth(BTNodeBase node)
        {
            var ch = node.GetChildren();
            if (ch.Count == 0) return NodeW;
            float total = 0f;
            foreach (var c in ch) total += CalcSubtreeWidth(c) + HGap;
            return total - HGap;
        }

        void LayoutNode(BTNodeBase node, float cx, int depth, BTNodeBase parentNode)
        {
            _positions[node] = new Vector2(cx, Padding + depth * (NodeH + VGap));

            if (parentNode != null)
                _edgeList.Add((parentNode, node));

            var children = node.GetChildren();
            if (children.Count == 0) return;

            float[] widths = new float[children.Count];
            float totalW = 0f;
            for (int i = 0; i < children.Count; i++)
            {
                widths[i] = CalcSubtreeWidth(children[i]);
                totalW += widths[i];
            }
            totalW += HGap * (children.Count - 1);

            float x = cx - totalW / 2f;
            for (int i = 0; i < children.Count; i++)
            {
                LayoutNode(children[i], x + widths[i] / 2f, depth + 1, node);
                x += widths[i] + HGap;
            }
        }

        VisualElement MakeNodeBox(BTNodeBase node, Vector2 pos)
        {
            var box = new VisualElement();
            box.style.position        = Position.Absolute;
            box.style.left            = pos.x - NodeW / 2f;
            box.style.top             = pos.y;
            box.style.width           = NodeW;
            box.style.height          = NodeH;
            box.style.backgroundColor = new StyleColor(StatusColor(node.LastStatus));
            SetRadius(box, 5f);

            // thin border to distinguish nodes on similar backgrounds
            box.style.borderTopWidth    = box.style.borderBottomWidth =
            box.style.borderLeftWidth   = box.style.borderRightWidth  = 1f;
            box.style.borderTopColor    = box.style.borderBottomColor =
            box.style.borderLeftColor   = box.style.borderRightColor  =
                new StyleColor(new Color(0f, 0f, 0f, 0.35f));

            var lbl = new Label($"{node.Name}  [{node.LastStatus}]");
            lbl.style.color          = new StyleColor(Color.white);
            lbl.style.fontSize       = 11;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.whiteSpace     = WhiteSpace.Normal;
            lbl.style.width          = NodeW;
            lbl.style.height         = NodeH;
            box.Add(lbl);

            return box;
        }

        static void SetRadius(VisualElement el, float r)
        {
            el.style.borderTopLeftRadius     = r;
            el.style.borderTopRightRadius    = r;
            el.style.borderBottomLeftRadius  = r;
            el.style.borderBottomRightRadius = r;
        }

        // ── Edge painting (Painter2D) ────────────────────────────────────────

        void PaintEdges(MeshGenerationContext mgc)
        {
            if (_edgeList.Count == 0) return;

            var p       = mgc.painter2D;
            p.strokeColor = ColEdge;
            p.lineWidth   = 1.5f;

            foreach (var (from, to) in _edgeList)
            {
                if (!_positions.TryGetValue(from, out var a)) continue;
                if (!_positions.TryGetValue(to,   out var b)) continue;

                var start = new Vector2(a.x, a.y + NodeH);      // bottom-center of parent
                var end   = new Vector2(b.x, b.y);              // top-center of child
                var c1    = new Vector2(start.x, start.y + VGap * 0.4f);
                var c2    = new Vector2(end.x,   end.y   - VGap * 0.4f);

                p.BeginPath();
                p.MoveTo(start);
                p.BezierCurveTo(c1, c2, end);
                p.Stroke();
            }
        }

        static Color StatusColor(BehaviourTree.ENodeStatus status) => status switch
        {
            BehaviourTree.ENodeStatus.InProgress => ColInProgress,
            BehaviourTree.ENodeStatus.Succeeded  => ColSucceeded,
            BehaviourTree.ENodeStatus.Failed     => ColFailed,
            _                                    => ColUnknown,
        };
    }
}
