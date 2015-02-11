using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    /// <summary>
    /// Defines base class for all behavior nodes in BehaviorTree
    /// </summary>
    public abstract class Behavior
    {
        /// <summary>
        /// Name of Behavior
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Type of Behavior
        /// </summary>
        public BehaviorType Type { get; private set; }
        /// <summary>
        /// Last ecexution result of behavior - (setter is for internal use - do not change this manually)
        /// </summary>
        public BehaviorResult Result { get; set; }

        /// <summary>
        /// Behavior of node when is child of a ConcurrentSelector
        /// </summary>
        public ConcurrencyMode Concurrency { get; set; }

        /// <summary>
        /// Wheight or chance of behavior when is behavior is child of a RandomSelector
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Is loaf of tree? 
        /// </summary>
        public virtual bool IsLeaf { get { return false; } }

        /// <summary>
        /// User object data
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Create an instance of Behavior
        /// </summary>
        /// <param name="name">Name of Behavior</param>
        /// <param name="behaviorType">Type of behavior (specified by subclass)</param>
        protected Behavior(string name, BehaviorType behaviorType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid Behavior name");
            this.Name = name;
            this.Type = behaviorType;
            this.Weight = 1;
        }

        /// <summary>
        /// Last update id
        /// </summary>
        public uint LastUpdateId { get; private set; }

        /// <summary>
        /// handle execution of behavior and call appropriate events
        /// </summary>
        /// <param name="status">Status of BehaviorTree</param>
        /// <returns>Result of execution</returns>
        public BehaviorResult Execute(BehaviorTreeStatus status)
        {
            LastUpdateId = status.UpdateId;            
            try
            {
                Result = Behave(status);// let subclass behave
            }
            catch (Exception e)
            {
                status.Exception = e;// store exception
                Result = BehaviorResult.Failure; // set result to failure
            }            
            return Result;
        }



        /// <summary>
        /// Let subclass implement behaveior
        /// </summary>
        /// <param name="status">Status of BehaviorTre</param>
        /// <returns>Result of behavior</returns>
        protected abstract BehaviorResult Behave(BehaviorTreeStatus status);

        /// <summary>
        /// Represent Behavior as string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString() { return Name; }


        /// <summary>
        /// Reset behavior. For internal use. when a branch with higher priority executed, let nodes in previous branch reset (internal use)
        /// </summary>
        /// <param name="status">Status of BehaviorTree</param>                
        public virtual void ResetBehavior(BehaviorTreeStatus status)
        {
            if (Result == BehaviorResult.Running && LastUpdateId != status.UpdateId)
                Result = BehaviorResult.Failure;
        }

    }
}
