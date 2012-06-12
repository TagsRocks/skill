using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
{

    public class SharedAccessKeys : IXElement
    {
        #region Properties
        /// <summary> Collection of AccessKeys  </summary>
        public System.Collections.Generic.Dictionary<string, AccessKey> Keys { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create new instance of SharedAccessKeys
        /// </summary>
        public SharedAccessKeys()
        {
            this.Keys = new Dictionary<string, AccessKey>();
        }
        #endregion


        #region Add & Remove
        public bool AddAccessKey(AccessKey key)
        {
            if (!string.IsNullOrEmpty(key.Key))
            {
                Keys.Add(key.Key, key);
                return true;
            }
            return false;
        }
        public bool RemoveAccessKey(AccessKey key)
        {
            return Keys.Remove(key.Key);
        }
        #endregion

        #region Save
        public XElement ToXElement()
        {
            XElement accessKeys = new XElement("AccessKeys");
            accessKeys.SetAttributeValue("Count", Keys.Count);
            foreach (var item in Keys)
            {
                XElement n = item.Value.ToXElement();
                accessKeys.Add(n);
            }
            return accessKeys;
        }
        #endregion

        #region Load
        public void Load(XElement e)
        {
            int count = e.GetAttributeValueAsInt("Count", 0);
            Keys.Clear();
            foreach (var item in e.Elements())
            {
                AccessKey ak = CreateAccessKeyFrom(item);
                if (ak != null)
                {
                    ak.Load(item);
                    AddAccessKey(ak);
                }
            }

        }

        private static AccessKey CreateAccessKeyFrom(XElement node)
        {
            AccessKey result = null;
            AccessKeyType accessKeyType = AccessKeyType.TimeLimit;
            bool isCorrect = false;
            try
            {
                accessKeyType = (AccessKeyType)Enum.Parse(typeof(AccessKeyType), node.Name.ToString(), false);
                isCorrect = true;
            }
            catch (Exception)
            {
                isCorrect = false;
            }
            if (isCorrect)
            {
                switch (accessKeyType)
                {
                    case AccessKeyType.CounterLimit:
                        result = new CounterLimitAccessKey();
                        break;
                    case AccessKeyType.TimeLimit:
                        result = new TimeLimitAccessKey();
                        break;
                }
            }
            return result;
        }
        #endregion
    }
}
