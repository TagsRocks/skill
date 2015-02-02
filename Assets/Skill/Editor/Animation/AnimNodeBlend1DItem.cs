using UnityEngine;
using System.Collections;

namespace Skill.Editor.Animation
{
    public class AnimNodeBlend1DItem : AnimNodeBlendBaseItem
    {
        public AnimNodeBlend1DItem(AnimNodeBlend1DData data)
            : base(data)
        {
            ValidateThresholds(data.Inputs.Length);
        }

        private int GetThresholdsCount()
        {
            return ((AnimNodeBlend1DData)Data).Thresholds.Length;
        }
        private void ValidateThresholds(int count)
        {
            float[] preConstraints = ((AnimNodeBlend1DData)Data).Thresholds;
            if (preConstraints == null)
            {
                preConstraints = new float[count];
                ((AnimNodeBlend1DData)Data).Thresholds = preConstraints;
            }
            if (preConstraints.Length == count) return;

            if (preConstraints.Length > count)
            {
                ((AnimNodeBlend1DData)Data).Thresholds = new float[count];
                for (int i = 0; i < count; i++)
                    ((AnimNodeBlend1DData)Data).Thresholds[i] = preConstraints[i];
            }
            else if (preConstraints.Length < count)
            {
                ((AnimNodeBlend1DData)Data).Thresholds = new float[count];
                for (int i = 0; i < preConstraints.Length; i++)
                    ((AnimNodeBlend1DData)Data).Thresholds[i] = preConstraints[i];

                for (int i = preConstraints.Length; i < ((AnimNodeBlend1DData)Data).Thresholds.Length; i++)
                    ((AnimNodeBlend1DData)Data).Thresholds[i] = preConstraints[preConstraints.Length - 1];
            }
        }
        private void AddThreshold()
        {
            float[] preConstraints = ((AnimNodeBlend1DData)Data).Thresholds;
            ((AnimNodeBlend1DData)Data).Thresholds = new float[preConstraints.Length + 1];
            for (int i = 0; i < preConstraints.Length; i++)
                ((AnimNodeBlend1DData)Data).Thresholds[i] = preConstraints[i];
            ((AnimNodeBlend1DData)Data).Thresholds[preConstraints.Length] = preConstraints[preConstraints.Length - 1] + 1;

        }
        private void RemoveThreshold(int index)
        {
            float[] preConstraints = ((AnimNodeBlend1DData)Data).Thresholds;

            ((AnimNodeBlend1DData)Data).Thresholds = new float[preConstraints.Length - 1];

            int counter = 0;
            for (int i = 0; i < preConstraints.Length; i++)
            {
                if (i == index) continue;
                ((AnimNodeBlend1DData)Data).Thresholds[counter] = preConstraints[i];
                counter++;
            }

        }
        private float GetThreshold(int index)
        {
            return ((AnimNodeBlend1DData)Data).Thresholds[index];
        }
        private void SetThreshold(int index, float value)
        {
            if (index > 0) value = Mathf.Max(GetThreshold(index - 1), value);
            if (index < GetThresholdsCount() - 1) value = Mathf.Min(GetThreshold(index + 1), value);
            ((AnimNodeBlend1DData)Data).Thresholds[index] = value;
        }

        public override void ChangeParameterName(string oldParamName, string newParamName)
        {
            if (((AnimNodeBlend1DData)Data).Parameter == oldParamName)
                ((AnimNodeBlend1DData)Data).Parameter = newParamName;
            base.ChangeParameterName(oldParamName, newParamName);
        }

        #region IProperties members

        protected override AnimNodeItem.ItemProperties CreateProperties() { return new Blend1DItemProperties(this); }

        class Blend1DInputConnectorManager : InputConnectorManager
        {
            public Blend1DInputConnectorManager(AnimNodeBlend1DItem item) :
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

                header.Controls.Add(new Skill.Framework.UI.Box() { Column = 0, ColumnSpan = 6, Style = (GUIStyle)"RL Header" });
                header.Controls.Add(new Skill.Framework.UI.Label() { Column = 1, Text = "Name" });
                header.Controls.Add(new Skill.Framework.UI.Label() { Column = 2, Text = "Threshold" });

                return header;
            }

            protected override InputItem CreateItem()
            {
                return new InputItem1D((AnimNodeBlend1DItem)this.Item);
            }

            protected override void OnConnectorAdd()
            {
                ((AnimNodeBlend1DItem)Item).AddThreshold();
                base.OnConnectorAdd();
            }

            protected override void OnConnectorRemove(int index)
            {
                ((AnimNodeBlend1DItem)Item).RemoveThreshold(index);
                base.OnConnectorRemove(index);
            }

            class InputItem1D : InputItem
            {
                private bool _Ignore;
                private Skill.Editor.UI.FloatField _ThresoldFields;
                private Skill.Editor.UI.TextField _TxtName;

                protected override void RefreshValue()
                {
                    if (this._ThresoldFields == null) return;
                    _Ignore = true;
                    this._ThresoldFields.Value = ((AnimNodeBlend1DItem)Item).GetThreshold(Index);
                    this._TxtName.Text = Item.GetInputName(Index);
                    _Ignore = false;
                }


                public InputItem1D(AnimNodeBlend1DItem item)
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

                    this._ThresoldFields = new UI.FloatField() { Column = 3, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                    this.Controls.Add(this._ThresoldFields);
                    this._ThresoldFields.ValueChanged += _ThresoldFields_ValueChanged;
                }

                void _TxtName_TextChanged(object sender, System.EventArgs e)
                {
                    if (_Ignore) return;
                    Item.SetInputName(Index, _TxtName.Text);
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(Item);
                    RefreshValue();
                }

                void _ThresoldFields_ValueChanged(object sender, System.EventArgs e)
                {
                    if (_Ignore) return;
                    ((AnimNodeBlend1DItem)Item).SetThreshold(Index, _ThresoldFields.Value);
                    RefreshValue();
                }
            }
        }

        protected class Blend1DItemProperties : ItemProperties
        {
            private Blend1DInputConnectorManager _ConnectorManager;
            private Blend1DParameterSelector _ParameterSelector;
            public Blend1DItemProperties(AnimNodeBlend1DItem item)
                : base(item)
            {
            }

            protected override void CreateCustomFileds()
            {
                AnimNodeBlend1DItem item = base.Object as AnimNodeBlend1DItem;                

                _ParameterSelector = new Blend1DParameterSelector(item);
                Controls.Add(_ParameterSelector);

                _ConnectorManager = new Blend1DInputConnectorManager(item);
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

            class Blend1DParameterSelector : ParameterSelector
            {
                public Blend1DParameterSelector(AnimNodeBlend1DItem item)
                    : base(item, AnimationTreeParameterType.Float, 1)
                {
                }

                protected override void SetParameter(int parameterIndex, string value)
                {
                    ((AnimNodeBlend1DData)((AnimNodeBlend1DItem)Item).Data).Parameter = value;
                }

                protected override string GetParameter(int parameterIndex)
                {
                    return ((AnimNodeBlend1DData)((AnimNodeBlend1DItem)Item).Data).Parameter;
                }
            }
        }

        #endregion
    }
}