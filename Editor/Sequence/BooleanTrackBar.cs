using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class BooleanTrackBar : DiscreteTrackBar<bool>
    {
        protected override DiscreteTrackBar<bool>.DiscreteKeyView CreateNewEvent(IPropertyKey<bool> key) { return new BooleanKeyView(this, key); }
        protected override IPropertyKey<bool> CreateNewKey() { return new BooleanKey(); }
        protected override IPropertyKey<bool>[] CreateKeyArray(int arraySize) { return new BooleanKey[arraySize]; }

        private BooleanTrack _BooleanTrack;
        public BooleanTrackBar(BooleanTrack track)
            : base(track)
        {
            _BooleanTrack = track;
        }

        protected override bool IsEqual(bool v1, bool v2) { return v1 == v2; }

        class BooleanKeyView : DiscreteKeyView
        {
            public override string Title { get { return "Boolean Key"; } }
            public BooleanKeyView(BooleanTrackBar trackbar, IPropertyKey<bool> key)
                : base(trackbar, key)
            {
            }

            protected override Texture GetIcon()
            {
                if (Key.ValueKey)
                    return Resources.UITextures.Keyframe;
                else
                    return Resources.UITextures.KeyframeEmpty;
            }
        }

    }
}