using System.Collections.Generic;
using Buzzrick.AISystems.BehaviourTree;
using Metroidvania.AISystems.Blackboard;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Buzzrick.AISystems.BehaviourTree.Graph.Editor
{
    public class BehaviourTreeDebugWindow : EditorWindow
    {
        const float NodeW    = 160f;
        const float NodeH    = 46f;
        const float HGap     = 28f;
        const float VGap     = 60f;
        const float Padding  = 24f;
        const float BBWidth  = 230f;

        static readonly Color ColUnknown    = new(0.22f, 0.22f, 0.22f);
        static readonly Color ColInProgress = new(0f,    0.50f, 0.68f);
        static readonly Color ColSucceeded  = new(0.10f, 0.48f, 0.15f);
        static readonly Color ColFailed     = new(0.52f, 0.10f, 0.10f);
        static readonly Color ColEdge       = new(0.50f, 0.50f, 0.50f);

        BehaviourTree           _trackedTree;
        IBlackboardDebugProvider _debugProvider;
        Label                   _headerLabel;
        VisualElement           _canvas;
        VisualElement           _edgeLayer;
        VisualElement           _bbContent;
        int                     _updateTick;

        readonly Dictionary<BTNodeBase, Vector2>        _positions = new();
        readonly List<(BTNodeBase from, BTNodeBase to)> _edgeList  = new();

        [MenuItem("Window/BehaviourTree Debugger")]
        static void Open() => GetWindow<BehaviourTreeDebugWindow>("BT Debugger");

        void CreateGUI()
        {
            _headerLabel = new Label("No BehaviourTree selected.");
            _headerLabel.style.paddingLeft       = 8;
            _headerLabel.style.paddingTop        = 5;
            _headerLabel.style.paddingBottom     = 5;
            _headerLabel.style.color             = new StyleColor(new Color(0.65f, 0.65f, 0.65f));
            _headerLabel.style.fontSize          = 11;
            _headerLabel.style.borderBottomWidth = 1;
            _headerLabel.style.borderBottomColor = new StyleColor(new Color(0.18f, 0.18f, 0.18f));
            rootVisualElement.Add(_headerLabel);

            var mainRow = new VisualElement();
            mainRow.style.flexDirection = FlexDirection.Row;
            mainRow.style.flexGrow      = 1;
            rootVisualElement.Add(mainRow);

            // ── Tree panel ──────────────────────────────────────────────────
            var treeScroll = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            treeScroll.style.flexGrow = 1;
            mainRow.Add(treeScroll);

            _canvas = new VisualElement();
            _canvas.style.position = Position.Relative;
            treeScroll.Add(_canvas);

            _edgeLayer = new VisualElement();
            _edgeLayer.style.position  = Position.Absolute;
            _edgeLayer.style.left      = 0;
            _edgeLayer.style.top       = 0;
            _edgeLayer.pickingMode     = PickingMode.Ignore;
            _edgeLayer.generateVisualContent += PaintEdges;
            _canvas.Add(_edgeLayer);

            // ── Divider ─────────────────────────────────────────────────────
            var divider = new VisualElement();
            divider.style.width           = 1;
            divider.style.flexShrink      = 0;
            divider.style.backgroundColor = new StyleColor(new Color(0.18f, 0.18f, 0.18f));
            mainRow.Add(divider);

            // ── Blackboard panel ────────────────────────────────────────────
            var bbPanel = new VisualElement();
            bbPanel.style.width     = BBWidth;
            bbPanel.style.flexShrink = 0;
            mainRow.Add(bbPanel);

            var bbTitle = new Label("Blackboard");
            bbTitle.style.paddingLeft        = 8;
            bbTitle.style.paddingTop         = 5;
            bbTitle.style.paddingBottom      = 5;
            bbTitle.style.fontSize           = 11;
            bbTitle.style.color              = new StyleColor(new Color(0.65f, 0.65f, 0.65f));
            bbTitle.style.borderBottomWidth  = 1;
            bbTitle.style.borderBottomColor  = new StyleColor(new Color(0.18f, 0.18f, 0.18f));
            bbPanel.Add(bbTitle);

            var bbScroll = new ScrollView(ScrollViewMode.Vertical);
            bbScroll.style.flexGrow = 1;
            bbPanel.Add(bbScroll);

            _bbContent = new VisualElement();
            _bbContent.style.paddingTop    = 4;
            _bbContent.style.paddingBottom = 4;
            bbScroll.Add(_bbContent);
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
            _trackedTree   = null;
            _debugProvider = null;

            if (Selection.activeGameObject != null)
            {
                _trackedTree = Selection.activeGameObject.GetComponent<BehaviourTree>()
                            ?? Selection.activeGameObject.GetComponentInChildren<BehaviourTree>();

                _debugProvider = Selection.activeGameObject.GetComponent<IBlackboardDebugProvider>()
                              ?? Selection.activeGameObject.GetComponentInChildren<IBlackboardDebugProvider>();
            }

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
            _canvas.Add(_edgeLayer);

            _positions.Clear();
            _edgeList.Clear();

            if (_trackedTree == null || _trackedTree.RootNode == null)
            {
                if (_headerLabel != null)
                    _headerLabel.text = "No BehaviourTree selected — pick a GameObject with a BehaviourTree component.";
                _canvas.style.width  = 1;
                _canvas.style.height = 1;
                _edgeLayer.MarkDirtyRepaint();
                RefreshBlackboard();
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

            RefreshBlackboard();
        }

        void RefreshBlackboard()
        {
            if (_bbContent == null) return;
            _bbContent.Clear();

            if (_debugProvider == null)
            {
                var lbl = new Label("No IBlackboardDebugProvider\nfound on selected object.");
                lbl.style.color      = new StyleColor(new Color(0.5f, 0.5f, 0.5f));
                lbl.style.fontSize   = 10;
                lbl.style.whiteSpace = WhiteSpace.Normal;
                lbl.style.paddingLeft = 8;
                lbl.style.paddingTop  = 6;
                _bbContent.Add(lbl);
                return;
            }

            int rowIndex = 0;
            foreach (var (key, value) in _debugProvider.GetBlackboardDebugEntries())
            {
                var row = new VisualElement();
                row.style.flexDirection   = FlexDirection.Row;
                row.style.paddingLeft     = 6;
                row.style.paddingRight    = 6;
                row.style.paddingTop      = 2;
                row.style.paddingBottom   = 2;
                row.style.backgroundColor = rowIndex % 2 == 0
                    ? new StyleColor(new Color(0f, 0f, 0f, 0f))
                    : new StyleColor(new Color(0f, 0f, 0f, 0.12f));
                rowIndex++;

                var keyLbl = new Label(FormatKey(key));
                keyLbl.style.color     = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
                keyLbl.style.fontSize  = 10;
                keyLbl.style.flexGrow  = 1;
                keyLbl.style.flexShrink = 1;
                keyLbl.style.overflow  = Overflow.Hidden;
                row.Add(keyLbl);

                var valLbl = new Label(value);
                valLbl.style.color        = new StyleColor(Color.white);
                valLbl.style.fontSize     = 10;
                valLbl.style.flexShrink   = 0;
                valLbl.style.unityTextAlign = TextAnchor.MiddleRight;
                valLbl.style.marginLeft   = 4;
                row.Add(valLbl);

                _bbContent.Add(row);
            }
        }

        // ── Tree layout helpers ──────────────────────────────────────────────

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

            box.style.borderTopWidth    = box.style.borderBottomWidth =
            box.style.borderLeftWidth   = box.style.borderRightWidth  = 1f;
            box.style.borderTopColor    = box.style.borderBottomColor =
            box.style.borderLeftColor   = box.style.borderRightColor  =
                new StyleColor(new Color(0f, 0f, 0f, 0.35f));

            var nameLbl = new Label(node.Name);
            nameLbl.style.color          = new StyleColor(Color.white);
            nameLbl.style.fontSize       = 11;
            nameLbl.style.unityTextAlign = TextAnchor.UpperCenter;
            nameLbl.style.whiteSpace     = WhiteSpace.Normal;
            nameLbl.style.width          = NodeW;
            nameLbl.style.paddingTop     = 4;
            box.Add(nameLbl);

            var statusLbl = new Label($"[{node.LastStatus}]");
            statusLbl.style.color           = new StyleColor(new Color(1f, 1f, 1f, 0.6f));
            statusLbl.style.fontSize        = 9;
            statusLbl.style.unityTextAlign  = TextAnchor.LowerCenter;
            statusLbl.style.width           = NodeW;
            statusLbl.style.paddingBottom   = 3;
            statusLbl.style.flexGrow        = 1;
            box.Add(statusLbl);

            return box;
        }

        static void SetRadius(VisualElement el, float r)
        {
            el.style.borderTopLeftRadius     = r;
            el.style.borderTopRightRadius    = r;
            el.style.borderBottomLeftRadius  = r;
            el.style.borderBottomRightRadius = r;
        }

        static string FormatKey(string raw) =>
            raw.StartsWith("Key: ") ? raw[5..] : raw;

        // ── Edge painting (Painter2D) ────────────────────────────────────────

        void PaintEdges(MeshGenerationContext mgc)
        {
            if (_edgeList.Count == 0) return;

            var p = mgc.painter2D;
            p.strokeColor = ColEdge;
            p.lineWidth   = 1.5f;

            foreach (var (from, to) in _edgeList)
            {
                if (!_positions.TryGetValue(from, out var a)) continue;
                if (!_positions.TryGetValue(to,   out var b)) continue;

                var start = new Vector2(a.x, a.y + NodeH);
                var end   = new Vector2(b.x, b.y);
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
