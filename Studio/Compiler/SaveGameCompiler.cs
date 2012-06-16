using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.IO;

namespace Skill.Studio.Compiler
{
    public class SaveGameCompiler : DataCompiler
    {
        private SaveGame _SaveGame;

        public SaveGameCompiler(ICollection<CompileError> errors)
            : base(EntityType.SaveGame, errors)
        {
        }

        protected override void Compile()
        {
            this._SaveGame = Node.SavedData as SaveGame;
            if (this._SaveGame == null) return;
            CheckForErrors();
            CheckForWarnings();
        }

        #region Check For Errors

        private void CheckForErrors()
        {
            CheckForErrors(_SaveGame);
            List<string> nameList = new List<string>(_SaveGame.Classes.Length);
            foreach (var cl in _SaveGame.Classes)
            {
                CheckForErrors(cl);


                if (!nameList.Contains(cl.Name))
                {
                    int count = _SaveGame.Classes.Count(c => c.Name == cl.Name);
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
                    if (_SaveGame.Classes.Count(c => c.Name == ((ClassProperty)p).ClassName) <= 0)
                        AddError(string.Format("The property {0} of class {1} has invalid ClassName.", p.Name, cl.Name));
                }
            }
        }
        #endregion

        #region Check For Warnings

        private void CheckForWarnings()
        {
            CheckForNoPropertyWarning(_SaveGame);
            foreach (var cl in _SaveGame.Classes)
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
