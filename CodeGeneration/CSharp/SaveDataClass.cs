using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.IO;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// Generate C# code for SaveData
    /// </summary>
    class SaveDataClass : SaveClassClass
    {
        private SaveData _SaveData;

        /// <summary>
        /// Create an instance of SaveData
        /// </summary>
        /// <param name="saveData">SaveData model</param>
        public SaveDataClass(SaveData saveData)
            : base(saveData)
        {
            this._SaveData = saveData;
            CreateClasses();
        }

        /// <summary>
        /// Create internal classes
        /// </summary>
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
