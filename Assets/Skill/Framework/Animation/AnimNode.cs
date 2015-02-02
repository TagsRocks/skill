using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
        
    /// <summary>
    /// Base class for all AnimNodes in AnimationTree
    /// </summary>
    public abstract class AnimNode : IEnumerable<AnimNode>
    {

        private AnimNode[] _Children;// childrens
        private float _PreWeight; // previous amount of weight
        private bool _ForceUpdate;
        

        /// <summary>
        /// Weight of node (0.0f - 1.0f)
        /// </summary>
        public BlendWeight BlendWeight { get; private set; }
        
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

        /// <summary> Last relevant time </summary> 
        public float RelevantTime { get; private set; }

        /// <summary>
        /// This node is considered 'relevant' - that is, has >0 weight in the final blend.
        /// </summary> 
        public bool IsRelevant { get { return BlendWeight.Weight > 0; } }

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
        public System.EventHandler BecameRelevant;

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected virtual void OnBecameRelevant()
        {
            RelevantTime = UnityEngine.Time.time;
            IsJustBecameRelevant = true;
            if (BecameRelevant != null) BecameRelevant(this,EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when AnimNode cease Relevant
        /// </summary>
        public System.EventHandler CeaseRelevant;
        /// <summary>
        /// call CeaseRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected virtual void OnCeaseRelevant()
        {
            IsJustCeaseRelevant = true;
            if (CeaseRelevant != null) CeaseRelevant(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create new instance of AnimNode
        /// </summary>
        /// <param name="childCount">Number of childrent</param>
        protected AnimNode(int childCount)
        {
            this.BlendWeight = new Animation.BlendWeight();
            this.Parent = null;
            this.Index = -1;
            this._Children = new AnimNode[childCount];            
            this.IsJustBecameRelevant = false;
            this.IsJustCeaseRelevant = false;
        }

        /// <summary>
        /// Update AnimationNode
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        internal virtual void Update()
        {
            if (BeginUpdate())
            {                            
                Blend();
                if (ChildCount > 0)
                {
                    foreach (var item in _Children)
                    {
                        if (item != null)
                            item.Update();
                    }
                }
            }
        }

        /// <summary>
        /// Perform pre required actions before update
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        /// <returns>true if update needed, otherwise false</returns>
        protected virtual bool BeginUpdate()
        {
            IsJustBecameRelevant = false;
            IsJustCeaseRelevant = false;            

            if (_PreWeight == 0 && BlendWeight.Weight > 0) OnBecameRelevant();
            else if (_PreWeight > 0 && BlendWeight.Weight == 0) OnCeaseRelevant();
            _PreWeight = BlendWeight.Weight;

            if (BlendWeight.Weight == 0 && !IsJustCeaseRelevant)
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
