﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// Generate c# code for SharedAccessKeys
    /// </summary>
    class SharedAccessKeysClass : Class
    {
        private SharedAccessKeys _Keys;

        /// <summary>
        /// Create a BehaviorTreeClass
        /// </summary>
        /// <param name="keys">SharedAccessKeys model</param>
        public SharedAccessKeysClass(SharedAccessKeys keys)
            : base(keys.Name)
        {
            this.IsPublic = true;
            this.IsStatic = true;
            this.IsPartial = false;
            this._Keys = keys;
            CreateAccessKeys();
        }


        private void CreateAccessKeys()
        {
            if (_Keys.Keys != null)
            {
                StringBuilder staticConstructorBody = new StringBuilder();
                foreach (var ak in _Keys.Keys)
                {
                    switch (ak.Type)
                    {
                        case AccessKeyType.CounterLimit:
                            this.Add(new Variable("Skill.Framework.AI.CounterLimitAccessKey", ak.Key) { IsStatic = true });
                            this.Add(new Property("Skill.Framework.AI.CounterLimitAccessKey", ak.Key, Variable.GetName(ak.Key), false) { IsStatic = true });
                            staticConstructorBody.AppendLine(string.Format("{0} = new Skill.Framework.AI.CounterLimitAccessKey(\"{1}\",{2});", Variable.GetName(ak.Key), ak.Key, ((CounterLimitAccessKey)ak).MaxAccessCount));
                            break;
                        case AccessKeyType.TimeLimit:
                            this.Add(new Variable("Skill.Framework.AI.TimeLimitAccessKey", ak.Key) { IsStatic = true });
                            this.Add(new Property("Skill.Framework.AI.TimeLimitAccessKey", ak.Key, Variable.GetName(ak.Key), false) { IsStatic = true });
                            staticConstructorBody.AppendLine(string.Format("{0} = new Skill.Framework.AI.TimeLimitAccessKey(\"{1}\",{2}f);", Variable.GetName(ak.Key), ak.Key, ((TimeLimitAccessKey)ak).TimeInterval.ToString("F")));
                            break;
                    }
                }

                Method staticConstructor = new Method(string.Empty, _Keys.Name, staticConstructorBody.ToString()) { IsStatic = true, Modifiers = Modifiers.None };
                Add(staticConstructor);
            }
        }
    }
}
