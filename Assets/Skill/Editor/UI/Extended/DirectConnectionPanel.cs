using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;

namespace Skill.Editor.UI.Extended
{
    public abstract class DirectConnectionPanel : Panel
    {
        private bool _IsDetecting;
        private BaseControl _StartControl;
        private Vector2 _EndPreviewConnection;

        /// <summary> Preview connection color (default : red) </summary>
        public Color PreviewColor { get; set; }
        /// <summary> Preview connection thickness (default : 3) </summary>
        
        public bool InteractiveCreateConnection { get; set; }

        /// <summary> Occurs when a connection created by mouse drag </summary>
        public event DirectConnectionEventHandler CreateConnection;
        /// <summary> Occurs when a connection created by mouse drag </summary>
        /// <param name="newConnection">New connection</param>
        protected virtual void OnCreateConnection(DirectConnection newConnection)
        {
            if (CreateConnection != null) CreateConnection(this, new DirectConnectionEventArgs(newConnection));
        }

        /// <summary>
        /// Create a ConnectionHost
        /// </summary>
        public DirectConnectionPanel()
        {
            this.PreviewColor = new Color(1.0f, 0.6f, 0.6f, 1.0f);            
            this.InteractiveCreateConnection = true;
        }

        public bool BeginConnectionDetect(BaseControl startConnector)
        {
            if (InteractiveCreateConnection && !_IsDetecting)
            {
                Frame of = OwnerFrame;
                if (of != null)
                    _IsDetecting = OwnerFrame.RegisterPrecedenceEvent(this);
                if (_IsDetecting)
                {
                    _StartControl = startConnector;
                    this._EndPreviewConnection = startConnector.RenderArea.center;
                }
                return true;
            }
            return false;
        }

        protected abstract IEnumerable<BaseControl> GetConnectableControls();
        protected abstract bool CanConnect(BaseControl control1, BaseControl control2);
        protected abstract DirectConnection CreateNewConnection(BaseControl start, BaseControl end);

        /// <summary>
        /// HandleEvent
        /// </summary>
        /// <param name="e">Event</param>
        public override void HandleEvent(Event e)
        {
            if (_IsDetecting && Parent != null && e != null)
            {
                this._EndPreviewConnection = e.mousePosition;
                Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
                if ((e.type == EventType.MouseDown) && e.button == 0)
                {
                    Frame of = OwnerFrame;
                    if (of != null)
                    {
                        of.UnregisterPrecedenceEvent(this);
                        _IsDetecting = false;

                        Vector2 mousePosition = e.mousePosition;

                        foreach (var c in GetConnectableControls())
                        {
                            if (c.RenderArea.Contains(mousePosition) && CanConnect(_StartControl, c))
                            {
                                DirectConnection newConnection = CreateNewConnection(_StartControl, c);
                                OnCreateConnection(newConnection);
                                Controls.Add(newConnection);
                                break;
                            }
                        }
                    }
                    e.Use();
                }
                else
                {
                    e.Use();
                }
            }
            else
                base.HandleEvent(e);
        }

        /// <summary> Render </summary>
        protected override void Render()
        {
            base.Render();
            if (_IsDetecting)
            {
                DirectConnection.DrawConnection(this._StartControl.RenderArea.center, this._EndPreviewConnection, this.PreviewColor);
            }

        }


        protected override void UpdateLayout()
        {

            Rect rect = RenderAreaShrinksByPadding;
            if (rect.xMax < rect.xMin) rect.xMax = rect.xMin;
            if (rect.yMax < rect.yMin) rect.yMax = rect.yMin;

            foreach (var c in Controls)
            {
                c.ScaleFactor = this.ScaleFactor;
                if (!(c is DirectConnection)) // do like canvas
                {
                    Rect cRect = new Rect();
                    cRect.x = rect.x + (c.X + c.Margin.Left) * this.ScaleFactor;
                    cRect.y = rect.y + (c.Y + c.Margin.Top) * this.ScaleFactor;
                    cRect.width = c.LayoutWidth * this.ScaleFactor;
                    cRect.height = c.LayoutHeight * this.ScaleFactor;
                    c.RenderArea = cRect;
                }
            }
        }
    }
}