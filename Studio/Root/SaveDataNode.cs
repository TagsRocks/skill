using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.IO;
using System.Xml.Linq;
using Skill.DataModels;

namespace Skill.Studio
{

    #region SaveDataNode
    public class SaveDataNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.SaveData; } }

        public SaveDataNode()
            : base("NewSaveData")
        {

        }
    }
    #endregion

    public class SaveDataNodeViewModel : EntityNodeViewModel
    {
        public SaveDataNodeViewModel(EntityNodeViewModel parent, SaveDataNode save)
            : base(parent, save)
        {
        }

        public override string ImageName { get { return Images.SaveData; } }

        public override void New()
        {
            SaveData saveGame = new SaveData();
            saveGame.Name = Name;
            SaveData(saveGame);
        }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            SaveDataNode cloneModel = new SaveDataNode();
            cloneModel.Name = Name;
            SaveDataNodeViewModel sg = new SaveDataNodeViewModel(copyParent, cloneModel);
            return sg;
        }


        public override void SaveData(object data)
        {
            if (data != null)
            {
                SaveData saveGame = data as SaveData;
                if (saveGame != null)
                {
                    saveGame.Name = this.Name;

                    string filename = AbsolutePath;
                    Project.CreateDirectory(filename);

                    DataFile datafile = new DataFile();
                    datafile.Document.Add(saveGame.ToXElement());

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
                SaveData saveGame = new SaveData();
                DataFile data = new DataFile(filename);
                saveGame.Load(data.Root);

                saveGame.Name = System.IO.Path.GetFileNameWithoutExtension(filename);

                return saveGame;
            }
            return null;
        }
    }
}
