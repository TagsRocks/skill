using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.IO;

namespace Skill.Editor.CodeGeneration
{
    /// <summary>
    /// Generate C# code for SaveData
    /// </summary>
    class SaveDataClass : ClassDataClass
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
                ClassDataClass saveClass = new ClassDataClass(item);                
                Add(saveClass);
            }
        }        
    }
}
