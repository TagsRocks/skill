using UnityEngine;
using System.Collections;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public class IntegerTrackBar : DiscreteTrackBar<int>
    {
        protected override DiscreteTrackBar<int>.DiscreteKeyView CreateNewEvent(IPropertyKey<int> key) { return new IntegerKeyView(this, key); }
        protected override IPropertyKey<int> CreateNewKey() { return new IntegerKey(); }
        protected override IPropertyKey<int>[] CreateKeyArray(int arraySize) { return new IntegerKey[arraySize]; }

        public IntegerTrackBar(IntegerTrack track)
            : base(track)
        {
        }

        protected override bool IsEqual(int v1, int v2) { return v1 == v2; }

        class IntegerKeyView : DiscreteKeyView
        {
            public override string Title { get { return "Integer Key"; } }
            public IntegerKeyView(IntegerTrackBar trackbar, IPropertyKey<int> key)
                : base(trackbar, key)
            {
            }
        }


    }
}