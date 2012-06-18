using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.IO;

namespace Skill.Studio.Compiler
{
    public class SaveDataCompiler : DataCompiler
    {
        private SaveData _SaveData;

        public SaveDataCompiler(ICollection<CompileError> errors)
            : base(EntityType.SaveData, errors)
        {
        }

        protected override void Compile()
        {
            this._SaveData = Node.SavedData as SaveData;
            if (this._SaveData == null) return;
            CheckForErrors();
            CheckForWarnings();
        }

        #region Check For Errors

        private void CheckForErrors()
        {
            CheckForErrors(_SaveData);
            List<string> nameList = new List<string>(_SaveData.Classes.Length);
            foreach (var cl in _SaveData.Classes)
            {
                CheckForErrors(cl);


                if (!nameList.Contains(cl.Name))
                {
                    int count = _SaveData.Classes.Count(c => c.Name == cl.Name);
                    if (count > 1)
                        AddError(string.Format("There are {0} Class with same name ({1}).", count, cl.Name));
                    nameList.Add(cl.Name);
                }

            }
            nameList.Clear();
        }

        private void CheckForErrors(SaveClass cl)
        {
            if (string.IsNullOrEmpty(cl.Name))
            {
                AddError("There is a class with empty name.");
            }
            if (cl.Properties == null) return;
            foreach (var p in cl.Properties)
            {
                if (string.IsNullOrEmpty(p.Name))
                {
                    AddError("There is a property with empty name.");
                }
                else
                {
                    int count = cl.Properties.Count(c => c.Name == p.Name);
                    if (count > 1)
                        AddError(string.Format("There are {0} Property in Class {1} with same name ({2}).", count, cl.Name, p.Name));
                }

                if (p.Type == PropertyType.Class)
                {
                    if (_SaveData.Classes.Count(c => c.Name == ((ClassProperty)p).ClassName) <= 0)
                        AddError(string.Format("The property {0} of class {1} has invalid ClassName.", p.Name, cl.Name));
                }
            }
        }
        #endregion

        #region Check For Warnings

        private void CheckForWarnings()
        {
            CheckForNoPropertyWarning(_SaveData);
            foreach (var cl in _SaveData.Classes)
            {
                CheckForNoPropertyWarning(cl);
            }
        }

        private void CheckForNoPropertyWarning(SaveClass cl)
        {
            if (cl.Properties == null || cl.Properties.Length == 0)
            {
                AddWarning(string.Format("The Class {0} has not any Property.", cl.Name));
            }
        }

        #endregion
    }
}
