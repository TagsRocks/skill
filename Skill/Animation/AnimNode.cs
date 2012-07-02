using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    #region WeightChangeMode
    /// <summary>
    /// Defines weight state of AnimNode
    /// </summary>
    public enum WeightChangeMode
    {
        /// <summary>
        /// No changes occurs between this and previous update
        /// </summary>
        NoChange,
        /// <summary>
        /// The value of Weight increased depend on previous update
        /// </summary>
        Increased,
        /// <summary>
        /// The value of Weight decreased depend on previous update
        /// </summary>
        Decreased,
    }
    #endregion


    #region AnimNodeEventHandler
    /// <summary>
    /// Represents the method that will handle events of AnimNpde
    /// </summary>
    /// <param name="sender">The actual AnimNode that this even belongs to</param>
    /// <param name="state">State of AnimationTree</param>
    public delegate void AnimNodeEventHandler(AnimNode sender, AnimationTreeState state);
    #endregion

    /// <summary>
    /// Base class for all AnimNodes in AnimationTree
    /// </summary>
    public abstract class AnimNode : IEnumerable<AnimNode>
    {

        private AnimNode[] _Children;// childrens
        private float _PreWeight; // previous amount of weight
        private float _Weight; // last setted amount of weight        

        /// <summary>
        /// Get or set weight of node (0.0f - 1.0f)
        /// </summary>
        public virtual float Weight
        {
            get { return _Weight; }
            internal set
            {
                _Weight = value;
                if (_Weight < 0) _Weight = 0; else if (_Weight > 1) _Weight = 1;
            }
        }

        /// <summary>
        /// Retrieves weight state of node depend on previous frame
        /// </summary>
        public WeightChangeMode WeightChange { get; private set; }

        /// <summary>
        /// Retrieves number of children
        /// </summary>
        public int ChildCount { get { return _Children.Length; } }

        /// <summary>
        /// Index of node in parent children array
        /// </summary>
        public int Index { get; private set; }

        /// <summary> parent node</summary>
        public AnimNode Parent { get; private set; }

        /// <summary>
        /// This node is considered 'relevant' - that is, has >0 weight in the final blend.
        /// </summary> 
        public bool IsRelevant { get { return Weight > 0; } }

        /// <summary>
        /// set to true when this node became relevant this round of updates. Will be set to false on the next tick.
        /// </summary>
        public bool IsJustBecameRelevant { get; private set; }

        /// <summary>
        /// set to true when this node cease relevant this round of updates. Will be set to false on the next tick.
        /// </summary>
        public bool IsJustCeaseRelevant { get; private set; }

        /// <summary>
        /// Get of set name of AnimNode
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent node is requesting a blend out. Give node a chance to delay that.
        /// </summary>
        public virtual bool CanBlendOutFrom { get { return true; } }

        /// <summary>
        /// parent node is requesting a blend in. Give node a chance to delay that.
        /// </summary>
        public virtual bool CanBlendTo { get { return true; } }

        /// <summary>
        /// Get or set chilc AnimNodes by index
        /// </summary>
        /// <param name="index">Index of child ( 0 - ChildCount )</param>
        /// <returns>AnimNode at specified index</returns>
        public AnimNode this[int index]
        {
            get { return _Children[index]; }
            set
            {
                _Children[index] = value;
                if (value != null)
                {
                    value.Index = index;
                    value.Parent = this;
                }
            }
        }

        /// <summary>
        /// Occurs when AnimNode became Relevant
        /// </summary>
        public AnimNodeEventHandler BecameRelevant;

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected virtual void OnBecameRelevant(AnimationTreeState state)
        {
            IsJustBecameRelevant = true;
            if (BecameRelevant != null) BecameRelevant(this, state);
        }

        /// <summary>
        /// Occurs when AnimNode cease Relevant
        /// </summary>
        public AnimNodeEventHandler CeaseRelevant;
        /// <summary>
        /// call CeaseRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected virtual void OnCeaseRelevant(AnimationTreeState state)
        {
            IsJustCeaseRelevant = true;
            if (CeaseRelevant != null) CeaseRelevant(this, state);
        }

        /// <summary>
        /// Create new instance of AnimNode
        /// </summary>
        /// <param name="childCount">Number of childrent</param>
        protected AnimNode(int childCount)
        {
            Parent = null;
            Index = -1;
            _Children = new AnimNode[childCount];
            IsJustBecameRelevant = false;
            IsJustCeaseRelevant = false;
        }

        /// <summary>
        /// Update AnimationNode
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        public void Update(AnimationTreeState state)
        {
            if (BeginUpdate(state))
            {
                Blend();
                if (ChildCount > 0)
                {
                    foreach (var item in _Children)
                    {
                        if (item != null)
                            item.Update(state);
                    }
                }
            }
        }

        /// <summary>
        /// Perform pre required actions before update
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        /// <returns>true if update needed, otherwise false</returns>
        protected virtual bool BeginUpdate(AnimationTreeState state)
        {
            IsJustBecameRelevant = false;
            IsJustCeaseRelevant = false;

            if (_Weight > _PreWeight)
                WeightChange = WeightChangeMode.Increased;
            else if (_Weight < _PreWeight)
                WeightChange = WeightChangeMode.Decreased;
            else
                WeightChange = WeightChangeMode.NoChange;

            if (_PreWeight == 0 && _Weight > 0) OnBecameRelevant(state);
            else if (_PreWeight > 0 && _Weight == 0) OnCeaseRelevant(state);
            _PreWeight = _Weight;

            if (Weight == 0 && !IsJustCeaseRelevant)
                return false;
            return true;
        }

        /// <summary>
        /// subclasses override this method to update weight
        /// </summary>
        protected abstract void Blend();

        /// <summary>
        /// Retrieves lenght of active sub branch
        /// </summary>
        public abstract float Length { get; }        


        /// <summary>
        /// Allow each node to get apropriate AnimationLayer
        /// </summary>
        /// <param name="manager">LayerManager to create layer</param>
        /// <param name="parentSuggestLayer">AnimationLayer suggested by parent. (layer of child at index 0)</param>
        public abstract void SelectLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer);

        /// <summary>
        /// Initialize and collect information from animationComponent
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation</param>
        public virtual void Initialize(UnityEngine.Animation animationComponent)
        {
            foreach (var item in this)
            {
                if (item != null)
                    item.Initialize(animationComponent);
            }
        }

        /// <summary>
        /// Destroy hierarchy of Children
        /// </summary>
        public virtual void Destroy()
        {
            if (_Children != null)
            {
                foreach (var item in _Children)
                {
                    if (item != null)
                        item.Destroy();
                }
            }
            _Children = null;
        }

        /// <summary>
        ///  Returns an System.Collections.IEnumerator for the children AnimNodes.
        /// </summary>
        /// <returns>Returns an System.Collections.IEnumerator for the children AnimNodes.</returns>
        public IEnumerator<AnimNode> GetEnumerator()
        {
            return new _ChildrenEnumerator(_Children.GetEnumerator());
        }

        /// <summary>
        ///  Returns an System.Collections.IEnumerator for the children AnimNodes.
        /// </summary>
        /// <returns>Returns an System.Collections.IEnumerator for the children AnimNodes.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        /// <summary>
        /// custom enumerator to enumerate children of AnimNode
        /// </summary>
        private class _ChildrenEnumerator : IEnumerator<AnimNode>
        {
            private System.Collections.IEnumerator _Enum;


            public _ChildrenEnumerator(System.Collections.IEnumerator baseEnum)
            {
                _Enum = baseEnum;
            }

            public AnimNode Current
            {
                get { return _Enum.Current as AnimNode; }
            }

            public void Dispose()
            {
                _Enum = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _Enum.Current; }
            }

            public bool MoveNext()
            {
                return _Enum.MoveNext();
            }

            public void Reset()
            {
                _Enum.Reset();
            }
        }

        /// <summary>
        /// Represent AnimNode as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
                return Name;
            return base.ToString();
        }


        /// <summary>
        /// update format of all AnimNodeSequences in tree
        /// </summary>
        /// <param name="format">Format</param>        
        internal virtual void SetFormat(string format)
        {
            foreach (var child in this)
            {
                if (child != null)
                {
                    child.SetFormat(format);
                }
            }
        }
    }
}
