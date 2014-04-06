using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class ColorTrackBar : PropertyTrackBar<Color>
    {
        protected override PropertyTrackBar<Color>.PropertyTimeLineEvent CreateNewEvent(IPropertyKey<Color> key) { return new ColorKeyView(this, key); }
        protected override IPropertyKey<Color> CreateNewKey() { return new ColorKey(); }
        protected override IPropertyKey<Color>[] CreateKeyArray(int arraySize) { return new ColorKey[arraySize]; }

        public ColorTrackBar(ColorTrack track)
            : base(track)
        {
            this.Height = 20;
        }

        protected override void EvaluateNewKey(IPropertyKey<Color> newKey, IPropertyKey<Color> previousKey)
        {
            if (previousKey != null)
                newKey.ValueKey = previousKey.ValueKey;
            else
                newKey.ValueKey = ((PropertyTrack<Color>)Track).DefaultValue;
        }

        class ColorKeyView : PropertyTimeLineEvent
        {
            public override double Duration { get { return 0.01f; } set { } }
            public override string Title { get { return "Color Event"; } }
            public override float MinWidth { get { return 26; } }
            public override float MaxWidth { get { return MinWidth; } }

            private ColorKey _ColorKey;
            public ColorKeyView(ColorTrackBar trackbar, IPropertyKey<Color> key)
                : base(trackbar, key)
            {
                _ColorKey = (ColorKey)key;
                DragStyle = new GUIStyle();
            }
            protected override void Render()
            {
                UnityEditor.EditorGUIUtility.DrawColorSwatch(RenderArea, _ColorKey.Value);
                base.Render();
            }
        }


    }
}