using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public enum WeightChangeMode
    {
        NoChange,
        Increased,
        Decreased,
    }

    public abstract class AnimationNode : IEnumerable<AnimationNode>
    {
        private AnimationNode[] _Children;
        private float _PreWeight;
        private float _Weight;

        public abstract float Length { get; }

        public virtual float Weight
        {
            get { return _Weight; }
            internal set
            {
                _Weight = value;
                if (_Weight < 0) _Weight = 0; else if (_Weight > 1) _Weight = 1;
            }
        }

        public WeightChangeMode WeightChange { get; private set; }

        // drived class should call it in the begin of update method
        private void CheckForWeightEvents()
        {
            if (_Weight > _PreWeight)
                WeightChange = WeightChangeMode.Increased;
            else if (_Weight < _PreWeight)
                WeightChange = WeightChangeMode.Decreased;
            else
                WeightChange = WeightChangeMode.NoChange;

            if (_PreWeight == 0 && _Weight > 0) OnBecameRelevant();
            else if (_PreWeight > 0 && _Weight == 0) OnCeaseRelevant();
            _PreWeight = _Weight;
        }

        public int ChildCount
        {
            get { return _Children.Length; }
        }

        /// <summary> Index of node in parent children </summary>
        public int Index { get; private set; }
        /// <summary> parent node</summary>
        public AnimationNode Parent { get; private set; }

        // This node is considered 'relevant' - that is, has >0 weight in the final blend.
        public bool IsRelevant { get { return Weight > 0; } }
        // set to TRUE when this node became relevant this round of updates. Will be set to false on the next tick.
        public bool IsJustBecameRelevant { get; private set; }
        public bool IsJustCeaseRelevant { get; private set; }
        //public AnimationNode Parent { get; internal set; }
        public string Name { get; set; }

        // Parent node is requesting a blend out. Give node a chance to delay that.
        //public virtual bool CanBlendOutFrom { get { return true; } }
        // parent node is requesting a blend in. Give node a chance to delay that.
        //public virtual bool CanBlendTo { get { return true; } }

        private float _BlendTime;
        public virtual float BlendTime
        {
            get { return _BlendTime; }
            set
            {
                _BlendTime = value;
                //foreach (var item in this)
                //{
                //    if (item != null)
                //        item.BlendTime = value;
                //}
            }
        }

        public EventHandler BecameRelevant;
        protected virtual void OnBecameRelevant()
        {
            IsJustBecameRelevant = true;            
            if (BecameRelevant != null) BecameRelevant(this, EventArgs.Empty);
        }

        public EventHandler CeaseRelevant;
        protected virtual void OnCeaseRelevant()
        {
            IsJustCeaseRelevant = true;
            if (CeaseRelevant != null) CeaseRelevant(this, EventArgs.Empty);
        }

        public AnimationNode this[int index]
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

        protected AnimationNode(int childCount)
        {
            Index = -1;
            _Children = new AnimationNode[childCount];
            BlendTime = 0.3f;
            IsJustBecameRelevant = false;
            IsJustCeaseRelevant = false;
        }

        public void Update()
        {
            if (BeginUpdate())
            {
                Updating();
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

        protected virtual bool BeginUpdate()
        {
            IsJustBecameRelevant = false;
            IsJustCeaseRelevant = false;
            CheckForWeightEvents();
            if (Weight == 0 && !IsJustCeaseRelevant)
                return false;
            return true;
        }

        protected abstract void Updating();

        public abstract void SetLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer);

        public virtual void CollectInfo(UnityEngine.Animation animationComponent)
        {
            foreach (var item in this)
            {
                if (item != null)
                    item.CollectInfo(animationComponent);
            }
        }

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

        public virtual void SetBlendTime(float blendTime, bool applyToChildren)
        {
            if (applyToChildren) BlendTime = blendTime;
            else _BlendTime = blendTime;
        }

        public IEnumerator<AnimationNode> GetEnumerator()
        {
            return new _Enumerator(_Children.GetEnumerator());
        }

        protected float BlendRate { get { if (_BlendTime > 0) return UnityEngine.Time.deltaTime / _BlendTime; else return 1; } }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        private class _Enumerator : IEnumerator<AnimationNode>
        {
            private System.Collections.IEnumerator _Enum;
            public _Enumerator(System.Collections.IEnumerator baseEnum)
            {
                _Enum = baseEnum;
            }

            public AnimationNode Current
            {
                get { return _Enum.Current as AnimationNode; }
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

        public override string ToString() { return Name; }
    }
}
