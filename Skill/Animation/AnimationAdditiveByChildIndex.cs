using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationAdditiveByChildIndex : AnimationBlendBase
    {
        //private float _AdditiveWeight;
        private int _PlayingIndex;
        private bool _StartPlay;
        private float _StartPlayTime;
        private float _Length;

        public AnimationNode Normal { get { return this[0]; } set { this[0] = value; } }

        public void SetChild(int childIndex, AnimationSequence child)
        {
            this[childIndex + 1] = child;
        }
        public AnimationSequence GetChild(int childIndex)
        {
            return this[childIndex + 1] as AnimationSequence;
        }

        public void Play(int childIndex)
        {
            int index = childIndex + 1;
            if (childIndex >= ChildCount - 1) return;
            if (_PlayingIndex == index) return;
            _PlayingIndex = index;
            AnimationSequence playing = this[_PlayingIndex] as AnimationSequence;
            _Length = playing.Length / playing.Speed;
            _StartPlay = true;
        }
        public void Play(int childIndex, float length)
        {
            int index = childIndex + 1;
            if (childIndex >= ChildCount - 1) return;
            if (_PlayingIndex == index) return;
            _PlayingIndex = index;
            _Length = length;
            _StartPlay = true;
        }

        public AnimationAdditiveByChildIndex(int childCount)
            : base(childCount + 1)
        {

        }

        protected override void Updating()
        {
            if (_PlayingIndex > 0)
            {
                if (_StartPlay)
                    _StartPlayTime = UnityEngine.Time.time;

                if (UnityEngine.Time.time >= _StartPlayTime + _Length)
                {
                    _PlayingIndex = -1;
                }
                else
                {
                    //AnimationSequence playing = this[_PlayingIndex] as AnimationSequence;                    
                }
                _StartPlay = false;
            }

            base.Updating();
        }

        protected override void CalcBlendWeights(ref float[] blendWeights)
        {
            blendWeights[0] = 1;
            for (int i = 1; i < ChildCount; i++)
            {
                if (i == _PlayingIndex) blendWeights[i] = 1; else blendWeights[i] = 0;
            }
        }
    }
}
