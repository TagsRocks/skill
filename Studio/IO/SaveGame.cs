using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Skill.DataModels.IO;

namespace Skill.Studio.IO
{
    public class SaveGameViewModel : SaveClassViewModel, IDataViewModel
    {
        [Browsable(false)]
        public ObservableCollection<SaveClassViewModel> Classes { get; private set; }

        [Browsable(false)]
        public Editor.SaveGameEditor Editor { get; set; }

        public SaveGameViewModel(SaveGame model)
            : base(null, model)
        {
            base.SaveGame = this;
            this.Classes = new ObservableCollection<SaveClassViewModel>();
            if (model.Classes != null)
            {
                foreach (var item in model.Classes)
                {
                    this.Classes.Add(new SaveClassViewModel(this, item));
                }
            }
        }

        public void AddClass()
        {
            SaveClass c = new SaveClass();
            c.Name = GetValidClassName(c.Name);

            SaveClassViewModel cVM = new SaveClassViewModel(this, c);
            this.Classes.Add(cVM);
            SaveGame.History.Insert(new AddSaveClassUnDoRedo(cVM, this, this.Classes.Count - 1));
        }

        public void RemoveClassAt(int index)
        {
            if (index >= 0 && index < this.Classes.Count)
            {
                SaveClassViewModel cl = this.Classes[index];
                this.Classes.RemoveAt(index);
                SaveGame.History.Insert(new AddSaveClassUnDoRedo(cl, this, index, true));
            }
        }

        private string GetValidClassName(string initialName)
        {
            string name = initialName;
            int i = 1;

            while (this.Classes.Count(c => c.Name == name) > 0)
            {
                name = initialName + i;
                i++;
            }

            return name;
        }

        public override void CommiteChanges()
        {
            ((SaveGame)Model).Classes = new SaveClass[this.Classes.Count];
            for (int i = 0; i < this.Classes.Count; i++)
            {
                this.Classes[i].CommiteChanges();
                ((SaveGame)Model).Classes[i] = this.Classes[i].Model;
            }
            base.CommiteChanges();
        }

        #region IDataViewModel members
        public void NotifyEntityChange(EntityType type, string previousPath, string newPath)
        {
        }

        public UnDoRedo History { get; set; }

        public object GetDataModel()
        {
            return Model;
        }
        #endregion


        #region UnDoRedo helper classes
        class AddSaveClassUnDoRedo : IUnDoRedoCommand
        {
            int _Index;
            SaveClassViewModel _NewClass;
            SaveGameViewModel _SaveGame;
            bool _Reverse;

            public AddSaveClassUnDoRedo(SaveClassViewModel newClass, SaveGameViewModel sg, int index, bool reverse = false)
            {
                this._NewClass = newClass;
                this._SaveGame = sg;
                this._Reverse = reverse;
                this._Index = index;
            }

            public void Undo()
            {
                if (_Reverse)
                    _SaveGame.Classes.Insert(_Index, _NewClass);
                else
                    _SaveGame.Classes.Remove(_NewClass);
            }

            public void Redo()
            {
                if (_Reverse)
                    _SaveGame.Classes.Remove(_NewClass);
                else
                    _SaveGame.Classes.Insert(_Index, _NewClass);
            }
        }
        #endregion
    }
}
