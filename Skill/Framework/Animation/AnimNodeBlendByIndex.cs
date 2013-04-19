using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// Try to keep single child active at time
    /// </summary>
    public class AnimNodeBlendByIndex : AnimNodeSingleLayer
    {
        private bool _SwitchOneShot;
        private TimeWatch _SwitchTimer;
        private int _PreChildIndex;
        private int _SelectedChildIndex;

        /// <summary>
        /// Get or set selected child by index
        /// </summary>
        public int SelectedChildIndex
        {
            get { return _SelectedChildIndex; }
            set
            {
                if (value < 0) value = 0;
                else if (value >= ChildCount) value = ChildCount - 1;
                _SelectedChildIndex = value;
                DisableSwitch();
            }
        }

        private void DisableSwitch()
        {
            _PreChildIndex = _SelectedChildIndex;
            _SwitchOneShot = false;
            _SwitchTimer.End();
        }

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnBecameRelevant(AnimationTreeState state)
        {
            DisableSwitch();
            base.OnBecameRelevant(state);
        }
        /// <summary>
        /// call CeaseRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnCeaseRelevant(AnimationTreeState state)
        {
            DisableSwitch();
            base.OnCeaseRelevant(state);
        }

        /// <summary>
        /// Get or set selected child by name
        /// </summary>
        public string SelectedChildName
        {
            get
            {
                if (_SelectedChildIndex < ChildCount)
                {
                    if (this[_SelectedChildIndex] != null)
                        return this[_SelectedChildIndex].Name;
                }
                return string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        AnimNode childNode = this[i];
                        if (childNode != null)
                        {
                            if (childNode.Name == value)
                            {
                                _SelectedChildIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrives selected child node
        /// </summary>
        public AnimNode SelectedChildNode { get { return this[_SelectedChildIndex]; } }

        /// <summary>
        /// Retrieves lenght of active sub branch
        /// </summary>
        public override float Length
        {
            get
            {
                if (SelectedChildNode != null)
                    return SelectedChildNode.Length;
                return 0;
            }
        }

        /// <summary>
        /// Create new instance of AnimNodeBlendByIndex
        /// </summary>
        /// <param name="childCount">number of children</param>
        public AnimNodeBlendByIndex(int childCount)
            : base(childCount)
        {

        }
        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>        
        protected override void CalcBlendWeights(ref BlendWeight[] blendWeights)
        {
            float blendRate = BlendRate;
            for (int i = 0; i < blendWeights.Length; i++)
            {
                float f = blendWeights[i].Weight;
                if (i == _SelectedChildIndex)
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
        /// Update
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        internal override void Update(AnimationTreeState state)
        {
            if (_SwitchTimer.IsEnabled)
            {
                if (_SwitchTimer.IsOver)
                {
                    CancelSwitchOneShot();
                }
            }
            else if (_SwitchOneShot)
            {
                base.Update(state);// update to make sure lenght of child is valid
                if (SelectedChildNode != null)
                    _SwitchTimer.Begin(SelectedChildNode.Length - BlendTime);

                _SwitchOneShot = false;
                return;// avoid update twice
            }
            base.Update(state);
        }

        /// <summary>        
        /// For example you can play reload one shot
        /// </summary>
        /// <param name="switchIndex"> switch node by index (index is between '0' - 'ChildCount -1' ) </param>
        public void SwitchOneShot(int switchIndex)
        {
            if (_SwitchTimer.IsEnabled) return;
            _PreChildIndex = _SelectedChildIndex;
            _SelectedChildIndex = switchIndex;
            _SwitchOneShot = true;
        }

        /// <summary>
        /// Cancel SwitchOneShot operation
        /// </summary>
        public void CancelSwitchOneShot()
        {
            if (_SwitchTimer.IsEnabled)
            {
                _SelectedChildIndex = _PreChildIndex;
                DisableSwitch();
            }
        }
    }
}
