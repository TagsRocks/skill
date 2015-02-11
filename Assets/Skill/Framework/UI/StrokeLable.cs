using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// An extended lable to visualize stroke color by rendering four additional label at left, right, top and bottom side of main label
    /// </summary>
    public class StrokeLable : Label
    {
        // Variables
        private bool _Changed;
        private GUIStyle _StrokeStyle;
        private Label _LblLeft;
        private Label _LblDown;
        private Label _LblUp;
        private Label _LblRight;

        /// <summary> Parent </summary>
        public override IControl Parent { get { return base.Parent; } set { base.Parent = _LblLeft.Parent = _LblDown.Parent = _LblUp.Parent = _LblRight.Parent = value; } }

        private Color _StrokeColor;
        /// <summary> Gets or set Stroke Color </summary>
        public Color StrokeColor { get { return _StrokeColor; } set { _StrokeColor = value; } }

        private Thickness _StrokeThickness;
        /// <summary> Gets of sets Stroke Thickness </summary>
        public Thickness StrokeThickness { get { return _StrokeThickness; } set { _StrokeThickness = value; } }

        private int _StrokeFont;
        /// <summary> it is possible to change the font used as stroke </summary>
        public int StrokeFont { get { return _StrokeFont; } set { _StrokeFont = value; } }

        /// <summary> Style of label </summary>
        public override GUIStyle Style { get { return base.Style; } set { base.Style = value; _Changed = true; } }

        /// <summary>
        /// Render aread chnged
        /// </summary>
        protected override void OnRenderAreaChanged()
        {
            Rect rect = RenderArea;
            rect.x -= StrokeThickness.Left;
            _LblLeft.RenderArea = rect;

            rect = RenderArea;
            rect.x += StrokeThickness.Right;
            _LblRight.RenderArea = rect;

            rect = RenderArea;
            rect.y -= StrokeThickness.Top;
            _LblUp.RenderArea = rect;

            rect = RenderArea;
            rect.y += StrokeThickness.Bottom;
            _LblDown.RenderArea = rect;

            base.OnRenderAreaChanged();
        }

        /// <summary>
        /// Create a StrokeLable
        /// </summary>
        public StrokeLable()
        {
            this.StrokeFont = 0;
            this.StrokeThickness = new Thickness(1);
            this.StrokeColor = Color.black;

            _LblLeft = new Label();
            _LblDown = new Label();
            _LblUp = new Label();
            _LblRight = new Label();
        }

        /// <summary>
        /// Render StrokeLable
        /// </summary>
        protected override void Render()
        {
            ApplyChange();

            SetContent(_LblLeft);
            SetContent(_LblDown);
            SetContent(_LblUp);
            SetContent(_LblRight);

            // first render lowe labels as stroke
            _LblLeft.OnGUI();
            _LblUp.OnGUI();
            _LblRight.OnGUI();
            _LblDown.OnGUI();

            // then render main label over the stroke labels
            base.Render();
        }

        private void SetContent(Label lbl)
        {
            lbl.Content.image = Content.image;
            lbl.Content.text = Content.text;
        }

        // apply changes to stroke labels
        private void ApplyChange()
        {
            if (Style != null)
            {
                if (_StrokeStyle == null || _Changed)
                {
                    _StrokeStyle = new GUIStyle(Style);
                    _LblLeft.Style = _StrokeStyle;
                    _LblDown.Style = _StrokeStyle;
                    _LblUp.Style = _StrokeStyle;
                    _LblRight.Style = _StrokeStyle;
                    _Changed = false;
                }
                _StrokeStyle.normal.textColor = StrokeColor;
                _StrokeStyle.hover.textColor = StrokeColor;
                _StrokeStyle.fontSize = Style.fontSize + StrokeFont;
            }
            else
            {
                _StrokeStyle = null;
            }
        }
    }
}
