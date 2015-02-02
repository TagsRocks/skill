using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.AI
{
    /// <summary>
    /// Defines Acceess keys to share between BehaviorTrees
    /// </summary>
    public class SharedAccessKeysData : IXmlElementSerializable
    {
        #region Properties
        /// <summary> Collection of AccessKeys  </summary>
        public AccessKeyData[] Keys { get; set; }
        /// <summary> Name of file </summary>
        public string Name { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create new instance of SharedAccessKeys
        /// </summary>
        public SharedAccessKeysData()
        {
            this.Name = "AccessKeys";
        }
        #endregion

        #region Save
        public XmlElement ToXmlElement()
        {
            XmlElement accessKeys = new XmlElement("AccessKeys");
            accessKeys.SetAttributeValue("Name", Name);

            if (this.Keys != null)
            {
                accessKeys.SetAttributeValue("Count", Keys.Length);
                foreach (var item in Keys)
                {
                    XmlElement n = item.ToXmlElement();
                    accessKeys.AppendChild(n);
                }
            }
            return accessKeys;
        }
        #endregion

        #region Load
        public void Load(XmlElement e)
        {
            int count = e.GetAttributeValueAsInt("Count", 0);
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            Keys = new AccessKeyData[count];
            int index = 0;
            foreach (var item in e)
            {
                AccessKeyData ak = CreateAccessKeyFrom(item);
                if (ak != null)
                {
                    ak.Load(item);
                    Keys[index++] = ak;
                }
            }

        }

        private static AccessKeyData CreateAccessKeyFrom(XmlElement node)
        {
            AccessKeyData result = null;
            Skill.Framework.AI.AccessKeyType accessKeyType = Skill.Framework.AI.AccessKeyType.TimeLimit;
            bool isCorrect = false;
            try
            {
                accessKeyType = (Skill.Framework.AI.AccessKeyType)Enum.Parse(typeof(Skill.Framework.AI.AccessKeyType), node.Name.ToString(), false);
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
                    case Skill.Framework.AI.AccessKeyType.CounterLimit:
                        result = new CounterLimitAccessKeyData();
                        break;
                    case Skill.Framework.AI.AccessKeyType.TimeLimit:
                        result = new TimeLimitAccessKeyData();
                        break;
                }
            }
            return result;
        }
        #endregion
    }
}
