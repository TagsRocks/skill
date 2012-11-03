using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// place children nodes in different layers.
    /// </summary>
    /// <remarks>
    /// for example :
    /// almost in reload AnimationClips only hand bones involved and other bones do not have keys.
    /// you can use this node to override hand animations so the lower body can play another animation
    /// be sure that lower body bones do not have any keys in AnimationClip or set MixingTransforms for bones
    /// Another usage of this nodeis when you want to play an IdleBreak animation sometimes
    /// </remarks>
    public class AnimNodeOverride : AnimNodeMultilayer
    {
        private bool _OverrideOneShot;
        private TimeWatch _Timer;
        private TimeWatch _OverrideTimer;
        private int _OverrideIndex;

        /// <summary>
        /// Normal node
        /// </summary>
        public AnimNode NormalNode { get { return this[0]; } set { this[0] = value; } }
        /// <summary>
        /// Override Node (higher layer)
        /// </summary>
        public AnimNode OverrideNode { get { return this[_OverrideIndex]; } }

        /// <summary>
        /// Get or set active overriding node by index (index is between '1' - 'ChildCount -1' )
        /// </summary>
        public int OverrideIndex
        {
            get { return _OverrideIndex; }
            set
            {
                _OverrideIndex = value;
                if (_OverrideIndex < 1)
                {
                    IsOverriding = false;
                    _OverrideIndex = 1;
                }
                else if (_OverrideIndex >= ChildCount)
                {
                    IsOverriding = false;
                    _OverrideIndex = ChildCount - 1;
                }
            }
        }

        private bool _Overriding;
        /// <summary>
        /// Whether overriding brach enable?
        /// </summary>
        public bool IsOverriding { get { return _Overriding; } set { if (!value)DisableOverride(); else _Overriding = value; } }

        /// <summary>
        /// if true, node automatically override one shot at period time
        /// </summary>
        /// <remarks>
        /// can be used on IdleBreak, set idle to NormalNode and idlebreak(could be an AnimNodeRandom) to OverrideNode.
        /// remember to disable IsOverriding when actor is not idle
        /// </remarks>
        public float OverridePeriod { get; set; }


        /// <summary>
        /// Lenght of active branch
        /// </summary>
        public override float Length
        {
            get
            {
                if (IsOverriding && OverrideNode != null)
                {
                    return OverrideNode.Length;
                }
                else if (NormalNode != null)
                {
                    return NormalNode.Length;
                }
                return 0;
            }
        }

        /// <summary>
        /// Create new instance of AnimNodeOverride
        /// </summary>
        /// <param name="childCount">number of children</param>
        public AnimNodeOverride(int childCount = 2)
            : base(childCount)
        {
            _OverrideIndex = 1;
        }

        private void DisableOverride()
        {
            _Overriding = false;
            _OverrideTimer.End();
            _Timer.End();
        }

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnBecameRelevant(AnimationTreeState state)
        {
            DisableOverride();
            base.OnBecameRelevant(state);
        }
        /// <summary>
        /// call CeaseRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnCeaseRelevant(AnimationTreeState state)
        {
            DisableOverride();
            base.OnCeaseRelevant(state);
        }


        /// <summary>
        /// Update
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        internal override void Update(AnimationTreeState state)
        {
            if (_OverrideTimer.Enabled)
            {
                if (_OverrideTimer.IsOver)
                {
                    DisableOverride();
                }
            }
            if (_OverrideOneShot)
            {
                IsOverriding = true;
                base.Update(state);// update to make sure lenght of child is valid
                if (OverrideNode != null)
                {
                    _OverrideTimer.Begin(OverrideNode.Length);
                    _Timer.End();
                    _OverrideOneShot = false;
                }
                else
                    IsOverriding = false;
                return;// avoid update twice
            }
            else if (OverridePeriod > 0 && !_OverrideTimer.Enabled)
            {
                if (_Timer.Enabled)
                {
                    if (_Timer.IsOver)
                    {
                        IsOverriding = true;
                        base.Update(state);// update to make sure lenght of child is valid
                        if (OverrideNode != null)
                        {
                            _OverrideTimer.Begin(OverrideNode.Length);
                            _Timer.End();
                            return;// avoid update twice
                        }
                        else
                            IsOverriding = false;
                    }
                }
                else
                    _Timer.Begin(OverridePeriod);
            }

            base.Update(state);
        }
        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>        
        protected override void CalcBlendWeights(ref BlendWeight[] blendWeights)
        {
            blendWeights[0].SetBoth(1);
            float blendRate = BlendRate;
            for (int i = 1; i < blendWeights.Length; i++)
            {
                float f = blendWeights[i].Weight;
                if (IsOverriding && i == _OverrideIndex)
                {
                    f += blendRate;
                    if (f > 1)
                        f = 1;
                }
                else
                {
                    f -= blendRate;
                    if (f < 0)
                        f = 0;
                }

                blendWeights[i].SetBoth(f);
            }
        }

        /// <summary>
        /// Used when OverridePeriod is zero
        /// For example you can play reload one shot
        /// </summary>
        public void OverrideOneShot()
        {
            if (OverrideNode != null)
            {
                if (_OverrideTimer.Enabled)
                {
                    return;
                }
                else if (!_OverrideOneShot)
                {
                    _OverrideOneShot = true;
                    IsOverriding = true;
                }
            }
        }

        /// <summary>
        /// Used when OverridePeriod is zero
        /// For example you can play reload one shot
        /// </summary>
        /// <param name="overrideIndex"> overriding node by index (index is between '1' - 'ChildCount -1' ) </param>
        public void OverrideOneShot(int overrideIndex)
        {
            OverrideIndex = overrideIndex;
            OverrideOneShot();
        }
    }
}
