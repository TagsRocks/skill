using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
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
            if (maxAccessCount < 1) maxAccessCount = 1; // at least on decorator can hold the key at time
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
        private bool _Lock;// state of lock

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
        /// <param name="accessKey">Shared AccessKey</param>
        public AccessLimitDecorator(string name, AccessKey accessKey)
            : base(name, null)
        {
            if (accessKey == null)
                throw new ArgumentNullException("Accesskey is null.");
            AccessKey = accessKey;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <returns>Result</returns>
        protected override BehaviorResult Behave(BehaviorState state)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (Child != null)
            {
                if (AccessKey != null)
                {
                    if (!_Lock)// if do not access to key
                        _Lock = AccessKey.Lock();// try to lock key
                    if (_Lock)// if success, execute child
                    {
                        result = base.Behave(state);
                    }
                    if (_Lock && result != BehaviorResult.Running)// if finish job, unlock key
                    {
                        AccessKey.Unlock();
                        _Lock = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// when a branch with more periority be valid let this nod to unlock key
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        public override void Reset(BehaviorState state)
        {
            if (_Lock)
                AccessKey.Unlock();
            base.Reset(state);
        }
    }
    #endregion
}
