using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// encapsulate child controls in a Foldout
    /// </summary>
    public class FolderView : Panel
    {
        /// <summary> Offset of childs relative to header (default : 16) </summary>
        public float ChildOffset { get; set; }

        private Skill.Editor.UI.Foldout _Foldout;
        /// <summary> Foldout header </summary>
        public Skill.Editor.UI.Foldout Foldout { get { return _Foldout; } }

        /// <summary>
        /// Create a FolderView
        /// </summary>
        public FolderView()
        {
            this._Foldout = new Skill.Editor.UI.Foldout() { Parent = this, ToggleOnLabelClick = false };
            this._Foldout.StateChanged += Foldout_StateChanged;
            this.ChildOffset = 16;
            this.Height = 16;
            this._UpdateLayoutHeight = true;
            this.Margin = new Thickness(2, 2, 1, 1);
        }

        private void Foldout_StateChanged(object sender, System.EventArgs e)
        {
            OnLayoutChanged();
        }

        /// <summary> OnLayoutChanged </summary>
        protected override void OnLayoutChanged()
        {
            _UpdateLayoutHeight = true;
            base.OnLayoutChanged();
        }

        /// <summary> UpdateLayout </summary>
        public override void UpdateLayout()
        {
            Rect rect = RenderArea;

            // set RenderArea of foldout
            rect.height = this.Height * this.ScaleFactor;
            this._Foldout.RenderArea = rect;


            if (_Foldout.IsOpen)
            {
                rect.x += ChildOffset;
                rect.y += rect.height + this.Margin.Bottom * this.ScaleFactor;

                foreach (var c in Controls)
                {
                    if (c != null)
                    {
                        c.ScaleFactor = this.ScaleFactor;
                        Rect cRect = rect;

                        var margin = c.Margin;

                        cRect.x += margin.Left * this.ScaleFactor;
                        cRect.width -= margin.Right * this.ScaleFactor;
                        cRect.y += margin.Top * this.ScaleFactor;
                        cRect.height = c.LayoutHeight * this.ScaleFactor;

                        c.RenderArea = cRect;

                        rect.y += cRect.height + margin.Bottom * this.ScaleFactor;
                    }
                }
            }
        }

        private float _LayoutHeight;
        private bool _UpdateLayoutHeight;
        public override float LayoutHeight
        {
            get
            {
                if (Visibility == Skill.Framework.UI.Visibility.Collapsed)
                {
                    return 0;
                }
                else
                {
                    if (_UpdateLayoutHeight)
                    {
                        _LayoutHeight = this.Height;
                        if (_Foldout.IsOpen)
                        {
                            foreach (var c in Controls)
                            {
                                if (c != null)
                                    _LayoutHeight += c.LayoutHeight + c.Margin.Vertical;
                            }
                        }
                        _UpdateLayoutHeight = false;
                    }
                    return _LayoutHeight;
                }
            }
        }

        /// <summary>
        /// Return the first control that contains specified point
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Control at specified point</returns>
        public override BaseControl GetControlAtPoint(Vector2 point)
        {
            if (_Foldout.IsOpen)
            {
                return base.GetControlAtPoint(point);
            }
            else
            {
                Rect ra = RenderArea;
                ra.height = Height;
                if (ra.Contains(point))
                    return this;
            }
            return null;
        }

        /// <summary> Render </summary>
        protected override void Render()
        {
            this._Foldout.OnGUI();
            if (_Foldout.IsOpen)
                base.Render();
        }
    }
}
