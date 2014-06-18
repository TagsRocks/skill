using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class IntegerTrackBar : PropertyTrackBar<int>
    {
        protected override PropertyTrackBar<int>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<int> key) { return new IntegerKeyView(this, key); }
        protected override IPropertyKey<int> CreateNewKey() { return new IntegerKey(); }
        protected override IPropertyKey<int>[] CreateKeyArray(int arraySize) { return new IntegerKey[arraySize]; }

        public IntegerTrackBar(IntegerTrack track)
            : base(track)
        {
            //this.Height = 22;
        }

        protected override void EvaluateNewKey(IPropertyKey<int> newKey, IPropertyKey<int> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<int>)Track).DefaultValue;
        }

        class IntegerKeyView : PropertyTimeLineEvent
        {
            public override double Duration { get { return 0.1f; } set { } }
            public override string Title { get { return "Integer Event"; } }


            private float _MinWidth;
            private int _PreValue;
            public override float MinWidth
            {
                get
                {
                    if (_MinWidth < 1f || _PreValue != PropertyKey.ValueKey)
                    {
                        _PreValue = PropertyKey.ValueKey;
                        GUIStyle labelStyle = "Label";
                        GUIContent content = new GUIContent() { text = PropertyKey.ValueKey.ToString() };
                        _MinWidth = labelStyle.CalcSize(content).x;
                    }
                    return _MinWidth;

                }
            }
            public override float MaxWidth { get { return MinWidth; } }

            private Skill.Framework.UI.Label _LblState;

            public IntegerKeyView(IntegerTrackBar trackbar, IPropertyKey<int> key)
                : base(trackbar, key)
            {
                _LblState = new Skill.Framework.UI.Label() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10 };
                Controls.Add(_LblState);
                DragStyle = new GUIStyle();
            }

            protected override void Render()
            {
                _LblState.Text = PropertyKey.ValueKey.ToString();
                base.Render();
            }
        }


    }
}