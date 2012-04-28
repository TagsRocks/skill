using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public abstract class AccessLimit
    {
        public string AccessKey { get; private set; }

        public AccessLimit(string accessKey)
        {
            this.AccessKey = accessKey;
        }

        public abstract bool Lock();
        public abstract void Unlock();
    }

    public class CountAccessLimit : AccessLimit
    {
        private int _CurrentAccess;
        public int MaxAccessCount { get; set; }

        public CountAccessLimit(string accessKey, int maxAccessCount)
            : base(accessKey)
        {
            this.MaxAccessCount = maxAccessCount;
            this._CurrentAccess = 0;
        }

        public override bool Lock()
        {
            if (this._CurrentAccess >= MaxAccessCount)
                return false;
            this._CurrentAccess++;
            return true;
        }

        public override void Unlock()
        {
            if (--this._CurrentAccess < 0) this._CurrentAccess = 0;
        }
    }

    public class AccessLimitDecorator : Decorator
    {
        private static Dictionary<string, AccessLimit> _Limits = new Dictionary<string, AccessLimit>();

        public static void AddLimit(AccessLimit item)
        {
            if (item != null)
            {
                if (!_Limits.ContainsKey(item.AccessKey))
                {
                    _Limits.Add(item.AccessKey, item);
                }
            }
        }
        public static bool RemoveLimit(AccessLimit item)
        {
            if (item != null)
            {
                return _Limits.Remove(item.AccessKey);
            }
            return false;
        }
        public static bool RemoveLimit(string accessKey)
        {
            if (!string.IsNullOrEmpty(accessKey))
            {
                return _Limits.Remove(accessKey);
            }
            return false;
        }

        public string AccessKey { get; private set; }

        public AccessLimitDecorator(string name, string accessKey)
            : base(name, null)
        {
            AccessKey = accessKey;
        }

        private bool HandleAccess(object userData)
        {
            return true;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (Child != null)
            {
                AccessLimit limit = null;
                _Limits.TryGetValue(AccessKey, out limit);
                if (limit != null)
                    if (limit.Lock())
                    {
                        result = base.Behave(state);
                        limit.Unlock();
                    }
            }
            return result;
        }
    }
}
