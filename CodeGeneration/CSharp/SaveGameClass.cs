using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.IO;

namespace Skill.CodeGeneration.CSharp
{
    class SaveDataClass : SaveClassClass
    {
        private SaveData _SaveData;

        public SaveDataClass(SaveData saveGame)
            : base(saveGame)
        {
            this._SaveData = saveGame;
            CreateClasses();
        }

        private void CreateClasses()
        {
            foreach (var item in _SaveData.Classes)
            {
                SaveClassClass saveClass = new SaveClassClass(item);                
                Add(saveClass);
            }
        }
    }
}
