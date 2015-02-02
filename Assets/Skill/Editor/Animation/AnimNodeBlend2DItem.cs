using UnityEngine;
using System.Collections;

namespace Skill.Editor.Animation
{
    public class AnimNodeBlend2DItem : AnimNodeBlendBaseItem
    {

        public AnimNodeBlend2DItem(AnimNodeBlend2DData data)
            : base(data)
        {
            ValidateThresholds(data.Inputs.Length);
        }

        public override void ChangeParameterName(string oldParamName, string newParamName)
        {
            if (((AnimNodeBlend2DData)Data).Parameter1 == oldParamName)
                ((AnimNodeBlend2DData)Data).Parameter1 = newParamName;
            else if (((AnimNodeBlend2DData)Data).Parameter2 == oldParamName)
                ((AnimNodeBlend2DData)Data).Parameter2 = newParamName;
            base.ChangeParameterName(oldParamName, newParamName);
        }

        private int GetThresholdsCount()
        {
            return ((AnimNodeBlend2DData)Data).Thresholds.Length;
        }
        private void ValidateThresholds(int count)
        {
            Vector2Data[] preConstraints = ((AnimNodeBlend2DData)Data).Thresholds;
            if (preConstraints == null)
            {
                preConstraints = new Vector2Data[count];
                ((AnimNodeBlend2DData)Data).Thresholds = preConstraints;
            }
            if (preConstraints.Length == count) return;

            if (preConstraints.Length > count)
            {
                ((AnimNodeBlend2DData)Data).Thresholds = new Vector2Data[count];
                for (int i = 0; i < count; i++)
                    ((AnimNodeBlend2DData)Data).Thresholds[i] = preConstraints[i];
            }
            else if (preConstraints.Length < count)
            {
                ((AnimNodeBlend2DData)Data).Thresholds = new Vector2Data[count];
                for (int i = 0; i < preConstraints.Length; i++)
                    ((AnimNodeBlend2DData)Data).Thresholds[i] = preConstraints[i];

                for (int i = preConstraints.Length; i < ((AnimNodeBlend2DData)Data).Thresholds.Length; i++)
                    ((AnimNodeBlend2DData)Data).Thresholds[i] = preConstraints[preConstraints.Length - 1];
            }
        }
        private void AddThreshold()
        {
            Vector2Data[] preConstraints = ((AnimNodeBlend2DData)Data).Thresholds;
            ((AnimNodeBlend2DData)Data).Thresholds = new Vector2Data[preConstraints.Length + 1];
            for (int i = 0; i < preConstraints.Length; i++)
                ((AnimNodeBlend2DData)Data).Thresholds[i] = preConstraints[i];
            ((AnimNodeBlend2DData)Data).Thresholds[preConstraints.Length] = preConstraints[preConstraints.Length - 1];

        }
        private void RemoveThreshold(int index)
        {
            Vector2Data[] preConstraints = ((AnimNodeBlend2DData)Data).Thresholds;

            ((AnimNodeBlend2DData)Data).Thresholds = new Vector2Data[preConstraints.Length - 1];

            int counter = 0;
            for (int i = 0; i < preConstraints.Length; i++)
            {
                if (i == index) continue;
                ((AnimNodeBlend2DData)Data).Thresholds[counter] = preConstraints[i];
                counter++;
            }

        }
        private float GetThresholdX(int index)
        {
            return ((AnimNodeBlend2DData)Data).Thresholds[index].X;
        }
        private float GetThresholdY(int index)
        {
            return ((AnimNodeBlend2DData)Data).Thresholds[index].Y;
        }

        private void SetThresholdX(int index, float value)
        {
            ((AnimNodeBlend2DData)Data).Thresholds[index].X = value;
        }
        private void SetThresholdY(int index, float value)
        {
            ((AnimNodeBlend2DData)Data).Thresholds[index].Y = value;
        }



        #region IProperties members

        protected override AnimNodeItem.ItemProperties CreateProperties() { return new Blend2DItemProperties(this); }

        class Blend2DInputConnectorManager : InputConnectorManager
        {
            public Blend2DInputConnectorManager(AnimNodeBlend2DItem item) :
                base(item)
            {
            }

            protected override Framework.UI.Grid CreateHeader()
            {
                Framework.UI.Grid header = new Framework.UI.Grid();
                header.ColumnDefinitions.Add(4, Framework.UI.GridUnitType.Pixel);
                header.ColumnDefinitions.Add(2, Framework.UI.GridUnitType.Star);
                header.ColumnDefinitions.Add(4, Framework.UI.GridUnitType.Pixel);
                header.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
                header.ColumnDefinitions.Add(4, Framework.UI.GridUnitType.Pixel);
                header.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
                header.ColumnDefinitions.Add(4, Framework.UI.GridUnitType.Pixel);

                header.Controls.Add(new Skill.Framework.UI.Box() { Column = 0, ColumnSpan = 6, Style = (GUIStyle)"RL Header" });
                header.Controls.Add(new Skill.Framework.UI.Label() { Column = 1, Text = "Name" });
                header.Controls.Add(new Skill.Framework.UI.Label() { Column = 3, Text = "Pos X" });
                header.Controls.Add(new Skill.Framework.UI.Label() { Column = 5, Text = "Pos Y" });

                return header;
            }

            protected override InputItem CreateItem()
            {
                return new InputItem2D((AnimNodeBlend2DItem)this.Item);
            }

            protected override void OnConnectorAdd()
            {
                ((AnimNodeBlend2DItem)Item).AddThreshold();
                base.OnConnectorAdd();
            }

            protected override void OnConnectorRemove(int index)
            {
                ((AnimNodeBlend2DItem)Item).RemoveThreshold(index);
                base.OnConnectorRemove(index);
            }

            class InputItem2D : InputItem
            {
                private bool _Ignore;
                private Skill.Editor.UI.FloatField _ThresoldFieldX;
                private Skill.Editor.UI.FloatField _ThresoldFieldY;
                private Skill.Editor.UI.TextField _TxtName;

                protected override void RefreshValue()
                {
                    if (this._ThresoldFieldX == null) return;
                    _Ignore = true;
                    this._ThresoldFieldX.Value = ((AnimNodeBlend2DItem)Item).GetThresholdX(Index);
                    this._ThresoldFieldY.Value = ((AnimNodeBlend2DItem)Item).GetThresholdY(Index);
                    this._TxtName.Text = Item.GetInputName(Index);
                    _Ignore = false;
                }


                public InputItem2D(AnimNodeBlend2DItem item)
                    : base(item)
                {
                    this.Height = 22;
                    this.Margin = new Framework.UI.Thickness(0, 0, 17, 0);
                    this.ColumnDefinitions.Add(8, Framework.UI.GridUnitType.Pixel);
                    this.ColumnDefinitions.Add(2, Framework.UI.GridUnitType.Star);
                    this.ColumnDefinitions.Add(8, Framework.UI.GridUnitType.Pixel);
                    this.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
                    this.ColumnDefinitions.Add(8, Framework.UI.GridUnitType.Pixel);
                    this.ColumnDefinitions.Add(1, Framework.UI.GridUnitType.Star);
                    this.ColumnDefinitions.Add(8, Framework.UI.GridUnitType.Pixel);

                    this._TxtName = new UI.TextField() { Column = 1, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                    this.Controls.Add(this._TxtName);
                    this._TxtName.TextChanged += _TxtName_TextChanged;

                    this._ThresoldFieldX = new UI.FloatField() { Column = 3, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                    this.Controls.Add(this._ThresoldFieldX);
                    this._ThresoldFieldX.ValueChanged += _ThresoldFieldX_ValueChanged;

                    this._ThresoldFieldY = new UI.FloatField() { Column = 5, VerticalAlignment = Framework.UI.VerticalAlignment.Center };
                    this.Controls.Add(this._ThresoldFieldY);
                    this._ThresoldFieldY.ValueChanged += _ThresoldFieldY_ValueChanged;
                }

                void _TxtName_TextChanged(object sender, System.EventArgs e)
                {
                    if (_Ignore) return;
                    Item.SetInputName(Index, _TxtName.Text);
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(Item);
                    RefreshValue();
                }

                void _ThresoldFieldX_ValueChanged(object sender, System.EventArgs e)
                {
                    if (_Ignore) return;
                    ((AnimNodeBlend2DItem)Item).SetThresholdX(Index, _ThresoldFieldX.Value);
                    RefreshValue();
                }

                void _ThresoldFieldY_ValueChanged(object sender, System.EventArgs e)
                {
                    if (_Ignore) return;
                    ((AnimNodeBlend2DItem)Item).SetThresholdY(Index, _ThresoldFieldY.Value);
                    RefreshValue();
                }
            }
        }

        protected class Blend2DItemProperties : ItemProperties
        {
            private Blend2DInputConnectorManager _ConnectorManager;
            private Blend2DParameterSelector _ParameterSelector;
            public Blend2DItemProperties(AnimNodeBlend2DItem item)
                : base(item)
            {
            }

            protected override void CreateCustomFileds()
            {

                AnimNodeBlend2DItem item = base.Object as AnimNodeBlend2DItem;

                _ParameterSelector = new Blend2DParameterSelector(item);
                Controls.Add(_ParameterSelector);

                _ConnectorManager = new Blend2DInputConnectorManager(item);
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
        }


        class Blend2DParameterSelector : ParameterSelector
        {

            public Blend2DParameterSelector(AnimNodeBlend2DItem item)
                : base(item, AnimationTreeParameterType.Float, 2)
            {

            }

            protected override void SetParameter(int parameterIndex, string value)
            {
                if (parameterIndex == 0)
                    ((AnimNodeBlend2DData)((AnimNodeBlend2DItem)Item).Data).Parameter1 = value;
                else
                    ((AnimNodeBlend2DData)((AnimNodeBlend2DItem)Item).Data).Parameter2 = value;
            }

            protected override string GetParameter(int parameterIndex)
            {
                if (parameterIndex == 0)
                    return ((AnimNodeBlend2DData)((AnimNodeBlend2DItem)Item).Data).Parameter1;
                else
                    return ((AnimNodeBlend2DData)((AnimNodeBlend2DItem)Item).Data).Parameter2;
            }
        }

        #endregion



    }
}