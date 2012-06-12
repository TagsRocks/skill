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
    public class SaveGameNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.SaveGame; } }

        public SaveGameNode()
            : base("NewSaveGame")
        {

        }
    }
    #endregion

    public class SaveGameNodeViewModel : EntityNodeViewModel
    {
        public SaveGameNodeViewModel(EntityNodeViewModel parent, SaveGameNode save)
            : base(parent, save)
        {
        }

        public override string ImageName { get { return Images.SaveGame; } }

        public override void New()
        {
            SaveGame saveGame = new SaveGame();
            saveGame.Name = Name;
            SaveData(saveGame);
        }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            SaveGameNode cloneModel = new SaveGameNode();
            cloneModel.Name = Name;
            SaveGameNodeViewModel sg = new SaveGameNodeViewModel(copyParent, cloneModel);
            return sg;
        }


        public override void SaveData(object data)
        {
            if (data != null)
            {
                SaveGame saveGame = data as SaveGame;
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
        }

        public override object LoadData()
        {
            string filename = AbsolutePath;
            if (System.IO.File.Exists(filename))
            {
                SaveGame saveGame = new SaveGame();
                DataFile data = new DataFile(filename);
                saveGame.Load(data.Root);
                return saveGame;
            }
            return null;
        }
    }
}
