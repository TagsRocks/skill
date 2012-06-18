using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.CodeGeneration.CSharp
{
    class SharedAccessKeysClass : Class
    {
        private SharedAccessKeys _Keys;

        /// <summary>
        /// Create a BehaviorTreeClass
        /// </summary>
        /// <param name="tree">BehaviorTree model</param>
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
            if (_Keys.Keys.Count > 0)
            {
                StringBuilder staticConstructorBody = new StringBuilder();
                foreach (var ak in _Keys.Keys)
                {
                    switch (ak.Value.Type)
                    {
                        case AccessKeyType.CounterLimit:
                            this.Add(new Variable("Skill.AI.CounterLimitAccessKey", ak.Key) { IsStatic = true });
                            this.Add(new Property("Skill.AI.CounterLimitAccessKey", ak.Key, Variable.GetName(ak.Key), false) { IsStatic = true });
                            staticConstructorBody.AppendLine(string.Format("{0} = new Skill.AI.CounterLimitAccessKey(\"{1}\",{2});", Variable.GetName(ak.Key), ak.Key, ((CounterLimitAccessKey)ak.Value).MaxAccessCount));
                            break;
                        case AccessKeyType.TimeLimit:
                            this.Add(new Variable("Skill.AI.TimeLimitAccessKey", ak.Key) { IsStatic = true });
                            this.Add(new Property("Skill.AI.TimeLimitAccessKey", ak.Key, Variable.GetName(ak.Key), false) { IsStatic = true });
                            staticConstructorBody.AppendLine(string.Format("{0} = new Skill.AI.TimeLimitAccessKey(\"{1}\",{2});", Variable.GetName(ak.Key), ak.Key, ((TimeLimitAccessKey)ak.Value).TimeInterval));
                            break;
                    }
                }

                Method staticConstructor = new Method(string.Empty, _Keys.Name, staticConstructorBody.ToString()) { IsStatic = true, Modifiers = Modifiers.None };
                Add(staticConstructor);
            }
        }
    }
}
