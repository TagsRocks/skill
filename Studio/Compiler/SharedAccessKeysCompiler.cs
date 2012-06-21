using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.Studio.Compiler
{
    public class SharedAccessKeysCompiler : DataCompiler
    {
        private SharedAccessKeys _SharedAccessKeys;

        public SharedAccessKeysCompiler(ICollection<CompileError> errors)
            : base(EntityType.SharedAccessKeys, errors)
        {
        }

        protected override void Compile()
        {
            this._SharedAccessKeys = Node.SavedData as SharedAccessKeys;
            if (this._SharedAccessKeys == null) return;
            CheckForErrors();
        }

        public void Compile(SharedAccessKeys sharedAccessKeys)
        {
            this._SharedAccessKeys = sharedAccessKeys;
            if (this._SharedAccessKeys == null) return;
            CheckForErrors();
        }

        private void CheckForErrors()
        {
            if (_SharedAccessKeys.Keys != null)
            {
                List<string> nameList = new List<string>(_SharedAccessKeys.Keys.Length);
                foreach (var item in _SharedAccessKeys.Keys)
                {
                    if (string.IsNullOrEmpty(item.Key))
                        AddError("There is an AccessKey with empty name.");
                    else
                    {
                        if (!nameList.Contains(item.Key))
                        {
                            int count = _SharedAccessKeys.Keys.Count(c => c.Key == item.Key);
                            if (count > 1)
                                AddError(string.Format("There are {0} AccessKeys with same name ({1}).", count, item.Key));
                            nameList.Add(item.Key);
                        }


                        switch (item.Type)
                        {
                            case AccessKeyType.CounterLimit:
                                if (((CounterLimitAccessKey)item).MaxAccessCount <= 0)
                                    AddError(string.Format("Invalid 'MaxAccessCount' of AccessKey {0} (must be greater than 0).", item.Key));
                                break;
                            case AccessKeyType.TimeLimit:
                                if (((TimeLimitAccessKey)item).TimeInterval <= 0)
                                    AddError(string.Format("Invalid 'TimeInterval' of AccessKey {0} (must be greater than 0).", item.Key));
                                break;
                            default:
                                break;
                        }
                    }
                }
                nameList.Clear();
            }
        }
    }
}
