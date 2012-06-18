using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using Skill.DataModels;

namespace Skill.Studio
{
    #region SharedAccessKeysNode
    public class SharedAccessKeysNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.SharedAccessKeys; } }

        public SharedAccessKeysNode()
            : base("NewSharedAccessKeys")
        {

        }
    }
    #endregion

    public class SharedAccessKeysNodeViewModel : EntityNodeViewModel
    {
        public SharedAccessKeysNodeViewModel(EntityNodeViewModel parent, SharedAccessKeysNode anim)
            : base(parent, anim)
        {            
        }

        public override string ImageName { get { return Images.SharedAccessKeys; } }

        public override void New()
        {
            SharedAccessKeys sharedAccessKeys = new SharedAccessKeys();
            SaveData(sharedAccessKeys);
        }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            SharedAccessKeysNode cloneNode = new SharedAccessKeysNode();
            cloneNode.Name = Name;
            SharedAccessKeysNodeViewModel cloneNodeVM = new SharedAccessKeysNodeViewModel(copyParent, cloneNode);
            return cloneNodeVM;
        }


        public override void SaveData(object data)
        {
            if (data != null)
            {
                SharedAccessKeys sa = data as SharedAccessKeys;
                if (sa != null)
                {
                    sa.Name = this.Name;

                    string filename = AbsolutePath;
                    Project.CreateDirectory(filename);

                    DataFile datafile = new DataFile();
                    datafile.Document.Add(sa.ToXElement());
                    datafile.Save(filename);
                }
            }
            base.SaveData(data);
        }

        public override object LoadData()
        {
            string filename = AbsolutePath;
            if (System.IO.File.Exists(filename))
            {
                SharedAccessKeys sharedAccessKeys = new SharedAccessKeys();
                DataFile data = new DataFile(filename);
                sharedAccessKeys.Load(data.Root);
                return sharedAccessKeys;
            }
            return null;
        }
    }
}
