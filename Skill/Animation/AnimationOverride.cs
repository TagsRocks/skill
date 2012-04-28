using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationOverride : AnimationBlendBase
    {
        private TimeWatch _Timer;
        private TimeWatch _OverrideTimer;
        private int _OverrideIndex;

        public AnimationNode Normal { get { return this[0]; } set { this[0] = value; } }
        public AnimationNode Override { get { return this[_OverrideIndex]; } }

        public int OverrideIndex { get { return _OverrideIndex; } set { _OverrideIndex = value; if (_OverrideIndex < 1) _OverrideIndex = 1; else if (_OverrideIndex >= ChildCount) _OverrideIndex = ChildCount - 1; } }

        public bool Overriding { get; set; }

        public float OverridePeriod { get; set; }

        public AnimationOverride(int childCount = 2)
            : base(childCount)
        {
            _OverrideIndex = 1;
        }

        protected override void OnBecameRelevant()
        {
            _OverrideTimer.End();
            _Timer.End();
            Overriding = false;
            base.OnBecameRelevant();
        }

        protected override void OnCeaseRelevant()
        {
            _OverrideTimer.End();
            _Timer.End();
            Overriding = false;
            base.OnCeaseRelevant();
        }

        protected override void Updating()
        {
            if (_OverrideTimer.Enabled)
            {
                if (_OverrideTimer.IsOver)
                {
                    _OverrideTimer.End();
                    Overriding = false;
                }
            }
            if (OverridePeriod > 0 && !_OverrideTimer.Enabled)
            {
                if (_Timer.Enabled)
                {
                    if (_Timer.IsOver)
                    {
                        base.Updating();// update to make sure lenght of child is valid
                        if (Override != null)
                        {
                            _OverrideTimer.Begin(Override.Length);
                            Overriding = true;
                            _Timer.End();
                            return;// avoid update twice
                        }
                    }
                }
                else
                    _Timer.Begin(OverridePeriod);

            }

            base.Updating();
        }

        protected override void CalcBlendWeights(ref float[] blendWeights)
        {
            blendWeights[0] = 1;

            for (int i = 1; i < blendWeights.Length; i++)
            {
                if (i == _OverrideIndex)
                    blendWeights[i] = Overriding ? 1 : 0;
                else
                    blendWeights[i] = 0;
            }
        }

        public override void SetBlendTime(float blendTime, bool applyToChildren)
        {
            base.SetBlendTime(blendTime, applyToChildren);
            if (!applyToChildren) Override.BlendTime = blendTime;
        }


        /// <summary>
        /// Used when OverridePeriod is zero
        /// For example you can play reload one shot
        /// </summary>
        public void OverrideOneShot()
        {
            if (Override != null)
            {
                if (_OverrideTimer.Enabled)
                    return;
                else
                {
                    _OverrideTimer.Begin(Override.Length);
                    Overriding = true;
                    _Timer.End();
                }
            }
        }


        public override void SetLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            AnimationLayer layer = parentSuggestLayer;
            if (ChildCount > 0)
            {
                if (this[0] != null)
                    this[0].SetLayer(manager, layer);
            }
            layer = manager.NewLayer(LayerMode.BlendAll);
            for (int i = 1; i < ChildCount; i++)
            {
                var child = this[i];
                if (child != null)
                {
                    child.SetLayer(manager, layer);
                }
            }
        }
    }
}
