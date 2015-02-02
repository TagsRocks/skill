using UnityEngine;
using System.Collections;

namespace Skill.Editor.Animation
{
    public class AnimNodeBlendByIndexItem : AnimNodeBlendBaseItem
    {
        public AnimNodeBlendByIndexItem(AnimNodeBlendByIndexData data)
            : base(data)
        {

        }

        [Skill.Framework.ExposeProperty(11, "BlendTime", "Blend Time of animation node between indices")]
        public float BlendTime
        {
            get { return ((AnimNodeBlendByIndexData)Data).BlendTime; }
            set
            {
                if (value < 0) value = 0;
                ((AnimNodeBlendByIndexData)Data).BlendTime = value;
            }
        }

        [Skill.Framework.ExposeProperty(12, "Enum", "name of enumerator based on input names.")]
        public string Enum
        {
            get { return ((AnimNodeBlendByIndexData)Data).Enum; }
            set
            {
                if (value == null) value = string.Empty;
                ((AnimNodeBlendByIndexData)Data).Enum = value;
            }
        }

        public override void ChangeParameterName(string oldParamName, string newParamName)
        {
            if (((AnimNodeBlendByIndexData)Data).Parameter == oldParamName)
                ((AnimNodeBlendByIndexData)Data).Parameter = newParamName;
            base.ChangeParameterName(oldParamName, newParamName);
        }



        #region IProperties members

        protected override AnimNodeItem.ItemProperties CreateProperties() { return new BlendByIndexProperties(this); }

        class BlendByIndexInputConnectorManager : InputConnectorManager
        {
            public BlendByIndexInputConnectorManager(AnimNodeBlendByIndexItem item) :
                base(item)
            {
            }

            protected override Framework.UI.Grid CreateHeader()
            {
                Framework.UI.Grid header = new Framework.UI.Grid();
                header.ColumnDefinitions.Add(4, Framework.UI.GridUnitType.Pixel);
                header.ColumnDefinitions.Add(2, Framework.UI.GridUnitType.Star);
                header.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
                header.ColumnDefinitions.Add(4, Framework.UI.GridUnitType.Pixel);

                header.Controls.Add(new Skill.Framework.UI.Box() { Column = 0, ColumnSpan = 4, Style = (GUIStyle)"RL Header" });
                header.Controls.Add(new Skill.Framework.UI.Label() { Column = 1, Text = "Name" });
                header.Controls.Add(new Skill.Framework.UI.Label() { Column = 2, Text = "Index" });

                return header;
            }

            protected override InputItem CreateItem()
            {
                return new InputItemIndex((AnimNodeBlendByIndexItem)this.Item);
            }

            class InputItemIndex : InputItem
            {
                private bool _Ignore;
                private Skill.Framework.UI.Label _LblIndex;
                private Skill.Editor.UI.TextField _TxtName;

                protected override void RefreshValue()
                {
                    if (this._TxtName == null) return;
                    _Ignore = true;
                    this._TxtName.Text = Item.GetInputName(Index);
                    this._LblIndex.Text = Index.ToString();
                    _Ignore = false;
                }


                public InputItemIndex(AnimNodeBlendByIndexItem item)
                    : base(item)
                {
                    this.Height = 22;
                    this.Margin = new Framework.UI.Thickness(0, 0, 17, 0);
                    this.ColumnDefinitions.Add(8, Framework.UI.GridUnitType.Pixel);
                    this.ColumnDefinitions.Add(2, Framework.UI.GridUnitType.Star);
                    this.ColumnDefinitions.Add(8, Framework.UI.GridUnitType.Pixel);
                    this.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
                    this.ColumnDefinitions.Add(8, Framework.UI.GridUnitType.Pixel);

                    this._TxtName = new UI.TextField() { Column = 1, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                    this.Controls.Add(this._TxtName);
                    this._TxtName.TextChanged += _TxtName_TextChanged;

                    this._LblIndex = new Framework.UI.Label() { Column = 3, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                    this.Controls.Add(this._LblIndex);
                }

                void _TxtName_TextChanged(object sender, System.EventArgs e)
                {
                    if (_Ignore) return;
                    Item.SetInputName(Index, _TxtName.Text);
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(Item);
                    RefreshValue();
                }

            }
        }

        protected class BlendByIndexProperties : ItemProperties
        {
            private BlendByIndexInputConnectorManager _ConnectorManager;
            private BlendByIndexParameterSelector _ParameterSelector;
            public BlendByIndexProperties(AnimNodeBlendByIndexItem item)
                : base(item)
            {
            }

            protected override void CreateCustomFileds()
            {
                AnimNodeBlendByIndexItem item = base.Object as AnimNodeBlendByIndexItem;
                _ParameterSelector = new BlendByIndexParameterSelector(item);
                Controls.Add(_ParameterSelector);

                _ConnectorManager = new BlendByIndexInputConnectorManager(item);
                Controls.Add(_ConnectorManager);
                base.CreateCustomFileds();
            }

            protected override void RefreshData()
            {
                base.RefreshData();
                AnimNodeItem item = base.Object as AnimNodeItem;
                Skill.Editor.Animation.AnimationTreeEditorWindow window = (Skill.Editor.Animation.AnimationTreeEditorWindow)((Skill.Editor.UI.EditorFrame)item.OwnerFrame).Owner;
                _ParameterSelector.Rebuild(window.Tree.Parameters);
            }

            class BlendByIndexParameterSelector : ParameterSelector
            {
                public BlendByIndexParameterSelector(AnimNodeBlendByIndexItem item)
                    : base(item, AnimationTreeParameterType.Integer, 1)
                {
                }

                protected override void SetParameter(int parameterIndex, string value)
                {
                    ((AnimNodeBlendByIndexData)((AnimNodeBlendByIndexItem)Item).Data).Parameter = value;
                }

                protected override string GetParameter(int parameterIndex)
                {
                    return ((AnimNodeBlendByIndexData)((AnimNodeBlendByIndexItem)Item).Data).Parameter;
                }
            }
        }

        #endregion
    }
}
