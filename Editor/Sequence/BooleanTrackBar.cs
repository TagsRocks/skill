using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class BooleanTrackBar : PropertyTrackBar<bool>
    {
        protected override PropertyTrackBar<bool>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<bool> key) { return new BooleanKeyView(this, key); }
        protected override IPropertyKey<bool> CreateNewKey() { return new BooleanKey(); }
        protected override IPropertyKey<bool>[] CreateKeyArray(int arraySize) { return new BooleanKey[arraySize]; }

        public BooleanTrackBar(BooleanTrack track)
            : base(track)
        {
            this.Height = 22;
        }

        protected override void EvaluateNewKey(IPropertyKey<bool> newKey, IPropertyKey<bool> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<bool>)Track).DefaultValue;
        }

        class BooleanKeyView : PropertyTimeLineEvent
        {
            public override double Duration { get { return 0.1f; } set { } }
            public override string Title { get { return "Boolean Event"; } }

            public override float MinWidth { get { return 20; } }
            public override float MaxWidth { get { return MinWidth; } }

            private Skill.Framework.UI.Image _ImgState;

            public BooleanKeyView(BooleanTrackBar trackbar, IPropertyKey<bool> key)
                : base(trackbar, key)
            {
                _ImgState = new Skill.Framework.UI.Image() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Center, VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, Width = 16, Height = 16 };
                Controls.Add(_ImgState);
                DragStyle = new GUIStyle();
            }

            protected override void Render()
            {
                if (PropertyKey.ValueKey)
                    _ImgState.Texture = MatineeResources.Textures.Checkbox_Checked;
                else
                    _ImgState.Texture = MatineeResources.Textures.Checkbox_Unchecked;
                base.Render();
            }            
        }



        
    }

}