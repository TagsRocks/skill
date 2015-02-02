using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;
using UnityEngine;
using System.Linq;

namespace Skill.Editor.AI
{
    public static class SharedAccessKeysCompiler
    {
        private static bool _ErrorFound;
        private static SharedAccessKeysData _Data;

        public static bool Compile(SharedAccessKeysData data)
        {
            _ErrorFound = false;
            _Data = data;

            CheckForErrors();

            _Data = null;
            return !_ErrorFound;
        }               

        private static void CheckForErrors()
        {
            if (_Data.Keys != null)
            {
                List<string> nameList = new List<string>(_Data.Keys.Length);
                foreach (var item in _Data.Keys)
                {
                    if (string.IsNullOrEmpty(item.Key))
                    {
                        Debug.LogError("There is an AccessKey with empty name.");
                        _ErrorFound = true;
                    }
                    else
                    {
                        if (!nameList.Contains(item.Key))
                        {
                            int count = _Data.Keys.Count(c => c.Key == item.Key);
                            if (count > 1)
                            {
                                Debug.LogError(string.Format("There are {0} AccessKeys with same name ({1}).", count, item.Key));
                                _ErrorFound = true;
                            }
                            nameList.Add(item.Key);
                        }


                        switch (item.Type)
                        {
                            case Skill.Framework.AI.AccessKeyType.CounterLimit:
                                if (((CounterLimitAccessKeyData)item).MaxAccessCount <= 0)
                                {
                                    Debug.LogError(string.Format("Invalid 'MaxAccessCount' of AccessKey {0} (must be greater than 0).", item.Key));
                                    _ErrorFound = true;
                                }
                                break;
                            case Skill.Framework.AI.AccessKeyType.TimeLimit:
                                if (((TimeLimitAccessKeyData)item).TimeInterval <= 0)
                                {
                                    Debug.LogError(string.Format("Invalid 'TimeInterval' of AccessKey {0} (must be greater than 0).", item.Key));
                                    _ErrorFound = true;
                                }
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
