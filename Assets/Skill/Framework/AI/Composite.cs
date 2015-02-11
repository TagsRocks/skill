using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{

    #region BehaviorContainer
    /// <summary>
    /// Helper class that contains Behavior node and apropriate parameters
    /// </summary>
    public class BehaviorContainer
    {
        /// <summary>
        /// Parameters of behavior at this position of tree
        /// </summary>
        public BehaviorParameterCollection Parameters { get; private set; }
        /// <summary>
        /// Behavior node
        /// </summary>
        public Behavior Behavior { get; private set; }

        /// <summary>
        /// Last ecexution result of behavior - (setter is for internal use - do not change this manually)
        /// </summary>
        public BehaviorResult Result { get; set; }

        /// <summary>
        /// parent of Behavior node
        /// </summary>
        public Behavior Parent { get; private set; }

        /// <summary>
        /// Create an instance of BehaviorContainer
        /// </summary>
        /// <param name="behavior">Behavior node</param>
        /// <param name="parameters">Parameters of behavior at this position of tree</param>
        public BehaviorContainer(Behavior parent, Behavior behavior, BehaviorParameterCollection parameters)
        {
            this.Parent = parent;
            if (behavior == null)
                throw new ArgumentNullException("Behavior is null.");
            this.Result = BehaviorResult.Failure;
            this.Behavior = behavior;
            this.Parameters = parameters;
        }

        public void Execute(BehaviorTreeStatus status)
        {
            status.RegisterForExecution(this); // register in execution sequence
            Result = Behavior.Execute(status);
        }
    }
    #endregion

    #region Composite
    /// <summary>
    /// Defines base class for composit behaviors
    /// </summary>
    public abstract class Composite : Behavior, IEnumerable<BehaviorContainer>
    {

        private List<BehaviorContainer> _Children; // list of children

        /// <summary>
        /// CompositeType
        /// </summary>
        public abstract CompositeType CompositeType { get; }
        /// <summary>
        /// Index of child that already running and needs to update next frame
        /// </summary>
        public int RunningChildIndex { get; protected set; }

        /// <summary>
        /// Access children by index
        /// </summary>
        /// <param name="index">Index of children</param>
        /// <returns>Child at specified index</returns>
        public BehaviorContainer this[int index] { get { return _Children[index]; } } // list class check for exceptions

        /// <summary>
        /// Create an instance of Composite
        /// </summary>
        /// <param name="name">Name of behavior node</param>
        public Composite(string name)
            : base(name, BehaviorType.Composite)
        {
            _Children = new List<BehaviorContainer>();
        }

        /// <summary>
        /// Add child. Remember to add children in correct priority
        /// </summary>
        /// <param name="child">Child behavior node</param>
        /// <param name="parameters">optional parameters for behavior</param>
        public virtual void Add(Behavior child, BehaviorParameterCollection parameters)
        {
            if (child != null)
            {
                _Children.Add(new BehaviorContainer(this, child, parameters));
            }
        }

        /// <summary>
        /// Add child. Remember to add children in correct priority
        /// </summary>
        /// <param name="child">Child behavior node</param>        
        public virtual void Add(Behavior child)
        {
            this.Add(child, null);
        }

        /// <summary>
        /// remove all children
        /// </summary>
        public virtual void RemoveAll()
        {
            _Children.Clear();
        }

        /// <summary>
        /// Retrieves count of children
        /// </summary>
        public int ChildCount { get { return _Children.Count; } }

        /// <summary>
        /// Remove specified child
        /// </summary>
        /// <param name="child">child to remove</param>
        /// <returns></returns>
        public virtual bool Remove(Behavior child)
        {
            if (child != null)
            {
                BehaviorContainer toRemove = null;
                foreach (var bctr in this)
                {
                    if (bctr.Behavior == child)
                    {
                        toRemove = bctr;
                        break;
                    }
                }
                if (toRemove != null)
                    return _Children.Remove(toRemove);
            }
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through children
        /// </summary>
        /// <returns>enumerator that iterates through children</returns>
        public IEnumerator<BehaviorContainer> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through children
        /// </summary>
        /// <returns>enumerator that iterates through children</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Children as System.Collections.IEnumerable).GetEnumerator();
        }

        /// <summary>
        /// Reset behavior. For internal use. when a branch with higher priority executed, let nodes in previous branch reset
        /// </summary>        
        /// <param name="status">Status of BehaviorTree</param>                
        public override void ResetBehavior(BehaviorTreeStatus status)
        {
            if (Result == BehaviorResult.Running)
            {
                if (LastUpdateId != status.UpdateId)
                    RunningChildIndex = -1;
                foreach (var child in this)
                {
                    if (child != null)
                        child.Behavior.ResetBehavior(status);
                }
            }
            base.ResetBehavior(status);
        }

        /// <summary>
        /// Checks whether behavior2 is one of next siblings of behavior1
        /// </summary>
        /// <param name="behavior1">firsy behavior</param>
        /// <param name="behavior2">next behavior</param>
        /// <returns>True if behavior2 is one of next siblings of behavior1, otherwise false</returns>
        public bool IsInSequenceChild(Behavior behavior1, Behavior behavior2)
        {
            bool found1 = false;
            foreach (var ct in _Children)
            {
                if (!found1)
                {
                    if (ct.Behavior == behavior1)
                        found1 = true;
                }
                else if (ct.Behavior == behavior2)
                    return true;
            }
            return false;
        }
    }
    #endregion
}
