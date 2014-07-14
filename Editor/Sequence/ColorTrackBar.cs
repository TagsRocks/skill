using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class ColorTrackBar : DiscreteTrackBar<Color>
    {
        protected override DiscreteTrackBar<Color>.DiscreteKeyView CreateNewEvent(IPropertyKey<Color> key) { return new ColorKeyView(this, key); }
        protected override IPropertyKey<Color> CreateNewKey() { return new ColorKey(); }
        protected override IPropertyKey<Color>[] CreateKeyArray(int arraySize) { return new ColorKey[arraySize]; }

        private ColorTrack _ColorTrack;
        public ColorTrackBar(ColorTrack track)
            : base(track)
        {
            _ColorTrack = track;
        }

        protected override bool IsEqual(Color v1, Color v2) { return v1 == v2; }

        class ColorKeyView : DiscreteKeyView
        {
            public override string Title { get { return "Color Key"; } }

            private ColorKey _ColorKey;

            public ColorKeyView(ColorTrackBar trackbar, IPropertyKey<Color> key)
                : base(trackbar, key)
            {
                _ColorKey = (ColorKey)key;
            }

            protected override Color GetIconColor()
            {
                Color color = _ColorKey.Value;
                color.a = Mathf.Max(0.2f, color.a);
                return color;
            }
        }
    }
}