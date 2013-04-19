using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{

    #region AccessKey
    /// <summary>
    /// Defines base class for keys that used by AccessLimitDecorator
    /// </summary>
    /// <remarks>
    /// Contains a string key. Each BehaviorTree has it's own set of keys that shared among all AccessLimitDecorators
    /// Whenever a AccessLimitDecorator try to execute first must get access to key, otherwise fails.
    /// </remarks>
    public abstract class AccessKey
    {
        /// <summary> The unique Accesskey in BehaviorTree.</summary>
        public string Key { get; private set; }

        /// <summary>
        /// Creates an insance of AccessKey
        /// </summary>
        /// <param name="accessKey">The Unique access key in BehaviorTree.</param>
        public AccessKey(string accessKey)
        {
            if (string.IsNullOrEmpty(accessKey))
                throw new ArgumentException("you must provide valid key");
            this.Key = accessKey;
        }

        /// <summary>
        /// Implemented by subclass to defines how to lock key and if success returns true, otherwise false.
        /// </summary>
        /// <returns>True for success, false for fail</returns>
        public abstract bool Lock();

        /// <summary>
        /// Implemented by subclass. this method called by AccessLimitDecorators after finish his work and free key to use by another AccessLimitDecorator.
        /// </summary>
        public abstract void Unlock();
    }
    #endregion

    #region CounterLimitAccessKey
    /// <summary>
    /// Only constant number of AccessLimitDecorators can access this key at the same time.
    /// </summary>
    public class CounterLimitAccessKey : AccessKey
    {
        private int _CurrentAccess;// current number of AccessLimitDecorators that access this key

        /// <summary>
        /// Defines maximum number of AccessLimitDecorators to access this key
        /// </summary>
        public int MaxAccessCount { get; set; }

        /// <summary>
        /// Create an instance of CounterLimitAccessKey
        /// </summary>
        /// <param name="accessKey">The Unique access key in BehaviorTree.</param>
        /// <param name="maxAccessCount">Maximum number of AccessLimitDecorators to access this key</param>
        public CounterLimitAccessKey(string accessKey, int maxAccessCount)
            : base(accessKey)
        {
            if (maxAccessCount < 1) maxAccessCount = 1; // at least one decorator can access the key at time
            this.MaxAccessCount = maxAccessCount;
            this._CurrentAccess = 0;
        }

        /// <summary>
        /// Defines how to lock key and if success returns true, otherwise false.
        /// </summary>
        /// <returns>True for success, false for fail</returns>
        public override bool Lock()
        {
            if (this._CurrentAccess >= MaxAccessCount)// reach limit count
                return false;
            this._CurrentAccess++;
            return true;
        }

        /// <summary>
        /// This method called by AccessLimitDecorators after finish his work and free key to use by another AccessLimitDecorator.
        /// </summary>
        public override void Unlock()
        {
            if (--this._CurrentAccess < 0) this._CurrentAccess = 0;
        }
    }
    #endregion

    #region TimeLimitAccessKey
    /// <summary>
    /// Only first request accepted after TimeInterval and lock untile next TimeInterval
    /// </summary>
    public class TimeLimitAccessKey : AccessKey
    {
        private bool _Lock;
        private TimeWatch _TimeTW;

        /// <summary>
        /// Defines time interval between access to key
        /// </summary>
        public float TimeInterval { get; set; }

        /// <summary>
        /// Create an instance of TimeLimitAccessKey
        /// </summary>
        /// <param name="accessKey">The Unique access key in BehaviorTree.</param>
        /// <param name="timeInterval">time interval between access to key</param>
        public TimeLimitAccessKey(string accessKey, float timeInterval)
            : base(accessKey)
        {
            this.TimeInterval = timeInterval;
            if (this.TimeInterval < 0.1f) this.TimeInterval = 0.1f;
            _Lock = false;
            _TimeTW.Begin(TimeInterval);
        }

        /// <summary>
        /// Defines how to lock key and if success returns true, otherwise false.
        /// </summary>
        /// <returns>True for success, false for fail</returns>
        public override bool Lock()
        {
            if (!_Lock)
            {
                if (_TimeTW.IsEnabled)
                {
                    if (_TimeTW.IsOver)
                    {
                        _TimeTW.End();
                        _Lock = true;
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    _Lock = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method called by AccessLimitDecorators after finish his work and free key to use by another AccessLimitDecorator.
        /// </summary>
        public override void Unlock()
        {
            _TimeTW.Begin(TimeInterval);
            _Lock = false;
        }
    }
    #endregion

    #region AccessLimitDecorator
    /// <summary>
    /// Limit execution of child node on access key.
    /// </summary>
    /// <remarks>
    /// Each BehaviorTree has set of AccessKey that shared among all AccessLimitDecorators in that BehaviorTree.
    /// Whenever a AccessLimitDecorator try to execute, first check AccessKey and continue if get access, otherwise returns BehaviorResul.Failure
    /// </remarks>
    public class AccessLimitDecorator : Decorator
    {
        private bool _Lock;// status of lock

        /// <summary>
        /// Shared AccessKey
        /// </summary>
        public AccessKey AccessKey { get; private set; }

        /// <summary>
        /// Type of Decorator
        /// </summary>
        public override DecoratorType DecoratorType { get { return AI.DecoratorType.AccessLimit; } }

        /// <summary>
        /// Create an instance of AccessLimitDecorator
        /// </summary>
        /// <param name="name">Name of behavior node</param>
        /// <param name="handler">user provided function to handle execution of Decorator</param>
        /// <param name="accessKey">Shared AccessKey</param>
        public AccessLimitDecorator(string name, DecoratorHandler handler, AccessKey accessKey)
            : base(name, handler)
        {
            if (accessKey == null)
                throw new ArgumentNullException("Accesskey is null.");
            AccessKey = accessKey;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="status">Status of BehaviorTree</param>
        /// <returns>Result</returns>
        //protected override BehaviorResult Behave(BehaviorStatus status)
        //{
        //    BehaviorResult result = BehaviorResult.Failure;
        //    if (Child != null)
        //    {
        //        if (AccessKey != null)
        //        {
        //            if (!_Lock)// if do not access to key
        //                _Lock = AccessKey.Lock();// try to lock key
        //            if (_Lock)// if success, execute child
        //            {
        //                status.Parameters = Child.Parameters;
        //                result = Child.Behavior.Trace(status);
        //            }
        //            if (_Lock && result != BehaviorResult.Running)// if finish job, unlock key
        //            {
        //                AccessKey.Unlock();
        //                _Lock = false;
        //            }
        //        }
        //    }
        //    if (NeverFail && result == BehaviorResult.Failure)
        //        result = BehaviorResult.Success;
        //    return result;
        //}

        protected override BehaviorResult TraceChild(BehaviorTreeStatus status)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (AccessKey != null)
            {
                if (!_Lock)// if do not access to key
                    _Lock = AccessKey.Lock();// try to lock key
                if (_Lock)// if success, execute child
                {
                    result = base.TraceChild(status);
                }
                if (_Lock && result != BehaviorResult.Running)// if finish job, unlock key
                {
                    AccessKey.Unlock();
                    _Lock = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Reset behavior. For internal use. when a branch with higher priority executed, let nodes in previous branch reset
        /// </summary>        
        /// <param name="status">Status of BehaviorTree</param>                
        public override void ResetBehavior(BehaviorTreeStatus status)
        {
            if (_Lock && Result == BehaviorResult.Running)
            {
                if (LastUpdateId != status.UpdateId)
                {
                    AccessKey.Unlock();
                    _Lock = false;
                }
            }
            base.ResetBehavior(status);
        }
    }
    #endregion
}
