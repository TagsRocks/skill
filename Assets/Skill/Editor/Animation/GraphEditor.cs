using UnityEngine;
using System.Collections;
using System.Linq;
using Skill.Framework.UI;
using System.Collections.Generic;

namespace Skill.Editor.Animation
{
    public class GraphEditor : UI.Extended.ConnectionHost, IEnumerable<AnimNodeItem>
    {
        private AnimationTreeEditorWindow _Editor;
        private Skill.Framework.UI.Box _Background;
        private Skill.Framework.UI.Grid _MainPanel;
        private Skill.Editor.UI.Extended.ZoomPanel _Panel;
        private Skill.Editor.UI.MultiSelector<AnimNodeItem> _MultiSelector;

        private Skill.Framework.UI.Grid _ToolbarPanel;
        private Skill.Framework.UI.Box _ToolbarBg;

        private UniformGrid _ToolbarAlignButtons;
        private Button _BtnAlignLeft;
        private Button _BtnAlignRight;
        private Button _BtnAlignTop;
        private Button _BtnAlignBottom;
        private Button _BtnAlignCenteredHorizontal;
        private Button _BtnAlignCenteredVertical;

        public Skill.Editor.UI.SelectableCollection<AnimNodeItem> Selection { get; private set; }
        public GraphEditor(AnimationTreeEditorWindow editor)
        {
            this._Editor = editor;

            this.Selection = new UI.SelectableCollection<AnimNodeItem>();
            this.Selection.SelectionChanged += Selection_SelectionChanged;

            this._MainPanel = new Grid();
            this._MainPanel.RowDefinitions.Add(20, GridUnitType.Pixel);
            this._MainPanel.RowDefinitions.Add(1, GridUnitType.Star);
            this.Controls.Add(_MainPanel);

            _Background = new Box() { Row = 1 };
            this._MainPanel.Controls.Add(_Background);

            _Panel = new UI.Extended.ZoomPanel() { Row = 1 };
            this._MainPanel.Controls.Add(_Panel);

            this._MultiSelector = new UI.MultiSelector<AnimNodeItem>(this, Selection) { Row = 1 };
            this._MainPanel.Controls.Add(_MultiSelector);

            this.WantsMouseEvents = true;
            this._MainPanel.ContextMenu = new GraphContextMenu(this);

            _ToolbarPanel = new Grid() { Row = 0 };
            _ToolbarPanel.ColumnDefinitions.Add(180, GridUnitType.Pixel);
            _ToolbarPanel.ColumnDefinitions.Add(1, GridUnitType.Star);
            this._MainPanel.Controls.Add(_ToolbarPanel);

            _ToolbarBg = new Box() { Column = 1 };
            this._ToolbarPanel.Controls.Add(_ToolbarBg);

            _ToolbarAlignButtons = new UniformGrid() { Rows = 1, Columns = 6 };
            this._ToolbarPanel.Controls.Add(_ToolbarAlignButtons);

            _BtnAlignLeft = new Button() { Column = 0 };
            _BtnAlignRight = new Button() { Column = 1 };
            _BtnAlignTop = new Button() { Column = 2 };
            _BtnAlignBottom = new Button() { Column = 3 };
            _BtnAlignCenteredHorizontal = new Button() { Column = 4 };
            _BtnAlignCenteredVertical = new Button() { Column = 5 };


            _BtnAlignLeft.Content.tooltip = "align nodes top";
            _BtnAlignRight.Content.tooltip = "align nodes bottom";
            _BtnAlignTop.Content.tooltip = "align nodes left";
            _BtnAlignBottom.Content.tooltip = "align nodes right";
            _BtnAlignCenteredHorizontal.Content.tooltip = "align nodes centered horizontal";
            _BtnAlignCenteredVertical.Content.tooltip = "align nodes centered vertical";


            _ToolbarAlignButtons.Controls.Add(_BtnAlignLeft);
            _ToolbarAlignButtons.Controls.Add(_BtnAlignRight);
            _ToolbarAlignButtons.Controls.Add(_BtnAlignTop);
            _ToolbarAlignButtons.Controls.Add(_BtnAlignBottom);
            _ToolbarAlignButtons.Controls.Add(_BtnAlignCenteredHorizontal);
            _ToolbarAlignButtons.Controls.Add(_BtnAlignCenteredVertical);

            _BtnAlignLeft.Click += _BtnAlignLeft_Click;
            _BtnAlignRight.Click += _BtnAlignRight_Click;
            _BtnAlignTop.Click += _BtnAlignTop_Click;
            _BtnAlignBottom.Click += _BtnAlignBottom_Click;
            _BtnAlignCenteredHorizontal.Click += _BtnAlignCenteredHorizontal_Click;
            _BtnAlignCenteredVertical.Click += _BtnAlignCenteredVertical_Click;

            EnableToolbar();
        }

        void _BtnAlignCenteredVertical_Click(object sender, System.EventArgs e)
        {
            AlignCenteredVertical();
        }

        void _BtnAlignCenteredHorizontal_Click(object sender, System.EventArgs e)
        {
            AlignCenteredHorizontal();
        }

        void _BtnAlignBottom_Click(object sender, System.EventArgs e)
        {
            AlignBottom();
        }

        void _BtnAlignTop_Click(object sender, System.EventArgs e)
        {
            AlignTop();
        }

        void _BtnAlignRight_Click(object sender, System.EventArgs e)
        {
            AlignRight();
        }

        void _BtnAlignLeft_Click(object sender, System.EventArgs e)
        {
            AlignLeft();
        }

        private void EnableToolbar()
        {
            _ToolbarAlignButtons.IsEnabled = Selection.Count > 1;
        }
        void Selection_SelectionChanged(object sender, System.EventArgs e)
        {
            _DragStarted = false;
            Skill.Editor.UI.ISelectable selected = this.Selection.SelectedItem;
            if (selected != null)
            {
                Skill.Editor.UI.Extended.InspectorProperties.Select((AnimNodeItem)selected);
            }
            else
            {
                var s = Skill.Editor.UI.Extended.InspectorProperties.GetSelected();
                if (s != null && s is AnimNodeItem)
                    Skill.Editor.UI.Extended.InspectorProperties.Select(null);
            }

            EnableToolbar();
        }

        protected override void BeginRender()
        {
            LineDrawer.ClipArea = this._Panel.RenderArea;
            base.BeginRender();
        }

        protected override void EndRender()
        {
            LineDrawer.ClipArea = null;
            base.EndRender();
        }

        public void DeselectInspector()
        {
            if (Skill.Editor.UI.Extended.InspectorProperties.GetSelected() is AnimNodeItem)
                Skill.Editor.UI.Extended.InspectorProperties.Select(null);
        }

        private AnimNodeItem Find(AnimNodeData data)
        {
            foreach (var item in this._Panel.Controls)
            {
                if (item is AnimNodeItem)
                {
                    if (((AnimNodeItem)item).Data == data)
                        return (AnimNodeItem)item;
                }
            }
            return null;
        }
        public void Rebuild()
        {
            Clear();
            if (_Editor.Tree != null)
            {
                foreach (var node in _Editor.Tree)
                {
                    AnimNodeItem item = CreateItem(node);
                    _Panel.Controls.Add(item);
                }

                if (_Editor.Tree.Connections != null)
                {
                    foreach (ConnectionData connection in _Editor.Tree.Connections)
                    {
                        AnimNodeItem startNode = Find(connection.Start);
                        AnimNodeItem endNode = Find(connection.End);
                        if (startNode != null && endNode != null)
                        {
                            Skill.Editor.UI.Extended.IConnector startConnector = startNode.OutConnector;
                            Skill.Editor.UI.Extended.IConnector endConnector = endNode.GetInputConnector(connection.EndConnectorIndex);

                            if (startConnector != null && endConnector != null)
                            {
                                Skill.Editor.UI.Extended.Connection c = new UI.Extended.Connection(startConnector, endConnector);
                                this.Controls.Add(c);
                            }
                        }
                    }
                }

                _Panel.ZoomFactor = _Editor.Tree.Zoom;
                _Panel.PanPosition = new Vector2(_Editor.Tree.PanX, _Editor.Tree.PanY);
            }
        }
        private AnimNodeItem CreateItem(AnimNodeData data)
        {
            AnimNodeItem item = null;
            switch (data.NodeType)
            {
                case AnimNodeType.Sequence:
                    item = new AnimNodeSequenceItem((AnimNodeSequenceData)data) { };
                    break;
                case AnimNodeType.Override:
                    item = new AnimNodeOverrideItem((AnimNodeOverrideData)data);
                    break;
                case AnimNodeType.Blend1D:
                    item = new AnimNodeBlend1DItem((AnimNodeBlend1DData)data);
                    break;
                case AnimNodeType.Blend2D:
                    item = new AnimNodeBlend2DItem((AnimNodeBlend2DData)data);
                    break;
                case AnimNodeType.Additive:
                    item = new AnimNodeAdditiveBlendingItem((AnimNodeAdditiveBlendingData)data);
                    break;
                case AnimNodeType.BlendByIndex:
                    item = new AnimNodeBlendByIndexItem((AnimNodeBlendByIndexData)data);
                    break;
                case AnimNodeType.Root:
                    item = new AnimationTreeRootItem((AnimationTreeRootData)data);
                    break;
                //case AnimNodeType.SubTree:
                //    item = new AnimNodeSubTreeItem((AnimNodeSubTreeData)data);
                //    break;                
            }
            return item;
        }
        public void Save()
        {
            _Editor.Tree.Clear();

            foreach (var item in this._Panel.Controls)
            {
                if (item is AnimNodeItem)
                {
                    ((AnimNodeItem)item).Save();
                    _Editor.Tree.Add(((AnimNodeItem)item).Data);
                }
            }

            List<Skill.Editor.UI.Extended.Connection> connections = new List<UI.Extended.Connection>();
            foreach (var item in this.Controls)
            {
                if (item is Skill.Editor.UI.Extended.Connection)
                    connections.Add((Skill.Editor.UI.Extended.Connection)item);
            }

            _Editor.Tree.Connections = new ConnectionData[connections.Count];
            for (int i = 0; i < connections.Count; i++)
            {
                Skill.Editor.UI.Extended.Connection c = connections[i];
                AnimNodeItem start = (AnimNodeItem)c.Start.UserData;
                AnimNodeItem end = (AnimNodeItem)c.End.UserData;

                if (c.Start.ConnectorType == UI.Extended.ConnectorType.Output)
                    _Editor.Tree.Connections[i] = new ConnectionData(start.Data, end.Data, end.GetConnectorIndex(c.End));
                else
                    _Editor.Tree.Connections[i] = new ConnectionData(end.Data, start.Data, start.GetConnectorIndex(c.Start));

            }
            _Editor.Tree.Zoom = this._Panel.ZoomFactor;
            _Editor.Tree.PanX = this._Panel.PanPosition.x;
            _Editor.Tree.PanY = this._Panel.PanPosition.y;
        }
        public void Clear()
        {
            this._Panel.Controls.Clear();
            this.Controls.Clear();
            this.Controls.Add(_MainPanel);
        }
        public void RefreshStyles()
        {
            _Background.Style = Skill.Editor.Resources.Styles.BackgroundShadow;

            _BtnAlignLeft.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnAlignRight.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnAlignTop.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnAlignBottom.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnAlignCenteredHorizontal.Style = Skill.Editor.Resources.Styles.ToolbarButton;
            _BtnAlignCenteredVertical.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _ToolbarBg.Style = Skill.Editor.Resources.Styles.ToolbarButton;

            _BtnAlignLeft.Content.image = Skill.Editor.Resources.UITextures.AlignLeft;
            _BtnAlignRight.Content.image = Skill.Editor.Resources.UITextures.AlignRight;
            _BtnAlignTop.Content.image = Skill.Editor.Resources.UITextures.AlignTop;
            _BtnAlignBottom.Content.image = Skill.Editor.Resources.UITextures.AlignBottom;
            _BtnAlignCenteredHorizontal.Content.image = Skill.Editor.Resources.UITextures.AlignCenteredHorizontal;
            _BtnAlignCenteredVertical.Content.image = Skill.Editor.Resources.UITextures.AlignCenteredVertical;
        }
        protected override bool CanConnect(UI.Extended.IConnector from, UI.Extended.IConnector to)
        {
            if (from.UserData != to.UserData)
                return true;
            return false;
        }
        private void New(AnimNodeType type, Vector2 position)
        {            
            position -= _Panel.PanPosition;
            position /= _Panel.ZoomFactor;
            position.x -= this.RenderArea.x;
            position.y -= this.RenderArea.y;

            AnimNodeData data = AnimationTreeData.CreateNode(type);
            if (data != null)
            {
                data.Name = GetUniqueName(string.Format("new{0}", type));                                
                data.X = position.x;
                data.Y = position.y;

                AnimNodeItem item = CreateItem(data);
                _Panel.Controls.Add(item);
            }
        }

        internal void Remove(AnimNodeItem item)
        {
            _Panel.Controls.Remove(item);
            item.RemoveAllConnections();
            List<Skill.Editor.UI.Extended.Connection> connectionList = new List<UI.Extended.Connection>();
            foreach (var c in Controls)
            {
                if (c is Skill.Editor.UI.Extended.Connection)
                {
                    Skill.Editor.UI.Extended.Connection connection = (Skill.Editor.UI.Extended.Connection)c;
                    if (connection.Start.UserData == item || connection.End.UserData == item)
                        connectionList.Add(connection);
                }
            }

            foreach (var c in connectionList)
                c.Break();
            connectionList.Clear();
        }

        private string GetUniqueName(string name)
        {
            int i = 1;
            string newName = name;
            while (_Editor.Tree.Where(b => b.Name == newName).Count() > 0)
                newName = name + i++;
            return newName;
        }

        private Vector2 _DeltaDrag;
        private bool _DragStarted;
        internal void ItemDrag(AnimNodeItem node, Vector2 delta)
        {
            if (Selection.Contains(node))
            {
                if (!_DragStarted)
                {
                    foreach (var item in Selection)
                    {
                        item.StartDrag.x = item.X;
                        item.StartDrag.y = item.Y;
                    }
                    _DeltaDrag = Vector3.zero;
                    _DragStarted = true;
                }
                _DeltaDrag += delta;
                delta = _DeltaDrag;

                foreach (var item in Selection)
                {
                    item.X = item.StartDrag.x + delta.x;
                    item.Y = item.StartDrag.y + delta.y;
                }
            }
            //else
            //{
            //    Selection.Select(node);
            //}
        }
        internal void ChangeParameterName(string oldParamName, string newParamName)
        {
            foreach (var item in this._Panel.Controls)
            {
                if (item is AnimNodeItem)
                {
                    ((AnimNodeItem)item).ChangeParameterName(oldParamName, newParamName);
                }
            }
        }


        #region ContextMenu

        class GraphContextMenu : Skill.Editor.UI.ContextMenu
        {
            private GraphEditor _Editor;
            public GraphContextMenu(GraphEditor editor)
            {
                this._Editor = editor;

                Skill.Editor.UI.MenuItem munNew = new Skill.Editor.UI.MenuItem("New");
                this.Add(munNew);

                Skill.Editor.UI.MenuItem munSequence = new Skill.Editor.UI.MenuItem("Sequence") { UserData = AnimNodeType.Sequence };
                munSequence.Click += Mnu_Click;
                munNew.Add(munSequence);

                Skill.Editor.UI.MenuItem munSpeed = new Skill.Editor.UI.MenuItem("Blend1D") { UserData = AnimNodeType.Blend1D };
                munSpeed.Click += Mnu_Click;
                munNew.Add(munSpeed);

                Skill.Editor.UI.MenuItem mun4Directional = new Skill.Editor.UI.MenuItem("Blend2D") { UserData = AnimNodeType.Blend2D };
                mun4Directional.Click += Mnu_Click;
                munNew.Add(mun4Directional);

                Skill.Editor.UI.MenuItem munIndex = new Skill.Editor.UI.MenuItem("BlendByIndex") { UserData = AnimNodeType.BlendByIndex };
                munIndex.Click += Mnu_Click;
                munNew.Add(munIndex);

                munNew.AddSeparator();

                Skill.Editor.UI.MenuItem munAdditive = new Skill.Editor.UI.MenuItem("Additive") { UserData = AnimNodeType.Additive };
                munAdditive.Click += Mnu_Click;
                munNew.Add(munAdditive);

                Skill.Editor.UI.MenuItem munOverride = new Skill.Editor.UI.MenuItem("Override") { UserData = AnimNodeType.Override };
                munOverride.Click += Mnu_Click;
                munNew.Add(munOverride);                

                //munNew.AddSeparator();

                //Skill.Editor.UI.MenuItem munSubTree = new Skill.Editor.UI.MenuItem("SubTree") { UserData = AnimNodeType.SubTree };
                //munSubTree.Click += Mnu_Click;
                //munNew.Add(munSubTree);


            }

            void Mnu_Click(object sender, System.EventArgs e)
            {
                Skill.Editor.UI.MenuItem munItem = (Skill.Editor.UI.MenuItem)sender;
                if (munItem != null)
                    _Editor.New((AnimNodeType)munItem.UserData, this.Position);
            }
        }

        #endregion


        public IEnumerator<AnimNodeItem> GetEnumerator()
        {
            foreach (var item in _Panel.Controls)
            {
                if (item is AnimNodeItem)
                {
                    yield return (AnimNodeItem)item;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in _Panel.Controls)
            {
                if (item is AnimNodeItem)
                {
                    yield return (AnimNodeItem)item;
                }
            }
        }




        #region Alignment

        private void AlignTop()
        {
            if (Selection.Count > 1)
            {
                List<AlignmentNodeData> dataList = new List<AlignmentNodeData>(); // history data

                float minY = float.MaxValue;
                foreach (AnimNodeItem node in Selection)
                    minY = Mathf.Min(minY, node.Y);


                foreach (AnimNodeItem node in Selection)
                {
                    AlignmentNodeData data = new AlignmentNodeData(); // create for history
                    data.Node = node;
                    data.PreY = node.Y;
                    node.Y = minY;
                    data.NewY = node.Y;
                    dataList.Add(data);
                }

                //History.Insert(new AlignmentUndoRedo(dataList));
            }
        }

        private void AlignBottom()
        {
            if (Selection.Count > 1)
            {
                List<AlignmentNodeData> dataList = new List<AlignmentNodeData>();
                float maxY = 0;
                foreach (AnimNodeItem node in Selection)
                    maxY = Mathf.Max(maxY, node.Y + node.Height);

                foreach (AnimNodeItem node in Selection)
                {
                    AlignmentNodeData data = new AlignmentNodeData();
                    data.Node = node;
                    data.PreY = node.Y;
                    node.Y = maxY - node.Height;
                    data.NewY = node.Y;
                    dataList.Add(data);
                }
                //History.Insert(new AlignmentUndoRedo(dataList));
            }
        }

        private void AlignLeft()
        {
            if (Selection.Count > 1)
            {
                List<AlignmentNodeData> dataList = new List<AlignmentNodeData>();
                float minX = float.MaxValue;
                foreach (AnimNodeItem node in Selection)
                    minX = Mathf.Min(minX, node.X);

                foreach (AnimNodeItem node in Selection)
                {
                    AlignmentNodeData data = new AlignmentNodeData();
                    data.Node = node;
                    data.PreX = node.X;
                    node.X = minX;
                    data.NewX = node.X;
                    dataList.Add(data);
                }
                //History.Insert(new AlignmentUndoRedo(dataList));
            }
        }

        private void AlignRight()
        {
            if (Selection.Count > 1)
            {
                List<AlignmentNodeData> dataList = new List<AlignmentNodeData>();
                float maxX = 0;
                foreach (AnimNodeItem node in Selection)
                    maxX = Mathf.Max(maxX, node.X + node.Width);

                foreach (AnimNodeItem node in Selection)
                {
                    AlignmentNodeData data = new AlignmentNodeData();
                    data.Node = node;
                    data.PreX = node.X;
                    node.X = maxX - node.Width;
                    data.NewX = node.X;
                    dataList.Add(data);
                }
                //History.Insert(new AlignmentUndoRedo(dataList));
            }
        }

        private void AlignCenteredHorizontal()
        {
            if (Selection.Count > 1)
            {
                List<AlignmentNodeData> dataList = new List<AlignmentNodeData>();
                float minX = float.MaxValue;
                float maxX = 0;
                foreach (AnimNodeItem node in Selection)
                {
                    maxX = Mathf.Max(maxX, node.X + node.Width);
                    minX = Mathf.Min(minX, node.X);
                }

                float middleX = (maxX + minX) * 0.5f;

                foreach (AnimNodeItem node in Selection)
                {
                    AlignmentNodeData data = new AlignmentNodeData();
                    data.Node = node;
                    data.PreX = node.X;
                    node.X = middleX - (node.Width * 0.5f);
                    data.NewX = node.X;
                    dataList.Add(data);
                }
                //History.Insert(new AlignmentUndoRedo(dataList));

            }

        }

        private void AlignCenteredVertical()
        {
            if (Selection.Count > 1)
            {
                List<AlignmentNodeData> dataList = new List<AlignmentNodeData>();
                float minY = float.MaxValue;
                float maxY = 0;
                foreach (AnimNodeItem node in Selection)
                {
                    maxY = Mathf.Max(maxY, node.Y + node.Height);
                    minY = Mathf.Min(minY, node.Y);
                }

                float middleY = (maxY + minY) * 0.5f;

                foreach (AnimNodeItem node in Selection)
                {
                    AlignmentNodeData data = new AlignmentNodeData();
                    data.Node = node;
                    data.PreY = node.Y;
                    node.Y = middleY - (node.Height * 0.5f);
                    data.NewY = node.Y;
                    dataList.Add(data);
                }
                //History.Insert(new AlignmentUndoRedo(dataList));

            }
        }

        class AlignmentNodeData
        {
            public AnimNodeItem Node;
            public double PreX, PreY;
            public double NewX, NewY;

            public AlignmentNodeData()
            {
                PreX = PreY = NewX = NewY = -1;
            }
        }

        //class AlignmentUndoRedo : IUnDoRedoCommand
        //{
        //    private List<AlignmentNodeData> _Data;

        //    public AlignmentUndoRedo(List<AlignmentNodeData> data)
        //    {
        //        _Data = data;
        //    }

        //    public void Undo()
        //    {
        //        foreach (var item in _Data)
        //        {
        //            if (item.PreX > 0)
        //                item.Node.X = item.PreX;
        //            if (item.PreY > 0)
        //                item.Node.Y = item.PreY;
        //        }
        //    }

        //    public void Redo()
        //    {
        //        foreach (var item in _Data)
        //        {
        //            if (item.NewX > 0)
        //                item.Node.X = item.NewX;
        //            if (item.NewY > 0)
        //                item.Node.Y = item.NewY;
        //        }
        //    }
        //}

        #endregion
        
    }
}
