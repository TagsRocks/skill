using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.IO;
using System.Collections.ObjectModel;

namespace Skill.Studio.IO
{
    public class SaveClassViewModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public SaveClass Model { get; private set; }

        [Browsable(false)]
        public SaveGameViewModel SaveGame { get; protected set; }

        [Browsable(false)]
        public ObservableCollection<SavePropertyViewModel> Properties { get; private set; }

        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                if (value == null) value = "";
                if (Model.Name != value)
                {
                    if (SaveGame.Editor.History != null)
                    {
                        SaveGame.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    }
                    Model.Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public SaveClassViewModel(SaveGameViewModel saveGame, SaveClass model)
        {
            this.SaveGame = saveGame;
            this.Model = model;
            this.Properties = new ObservableCollection<SavePropertyViewModel>();
            if (Model.Properties != null)
            {
                foreach (var item in Model.Properties)
                {
                    this.Properties.Add(CreateProperty(item));
                }
            }
        }

        public SavePropertyViewModel CreateProperty(SaveProperty property)
        {
            switch (property.Type)
            {
                case PropertyType.Primitive:
                    return new PrimitivePropertyViewModel(this, (PrimitiveProperty)property);
                case PropertyType.Class:
                    return new ClassPropertyViewModel(this, (ClassProperty)property);
                default:
                    return null;
            }
        }

        public SavePropertyViewModel CreateProperty(PropertyType type)
        {
            SaveProperty p = SaveClass.CreateProperty(type);
            p.Name = GetValidPropertyName(p.Name);
            SavePropertyViewModel pVM = CreateProperty(p);
            return pVM;
        }

        public void AddProperty(PropertyType type)
        {
            SavePropertyViewModel newP = CreateProperty(type);
            this.Properties.Add(newP);
            SaveGame.History.Insert(new AddSavePropertyUnDoRedo(newP, this, this.Properties.Count - 1));
        }

        public void RemovePropertyAt(int index)
        {
            if (index >= 0 && index < this.Properties.Count)
            {
                SavePropertyViewModel p = this.Properties[index];
                this.Properties.RemoveAt(index);
                SaveGame.History.Insert(new AddSavePropertyUnDoRedo(p, this, index, true));
            }
        }

        private string GetValidPropertyName(string initialName)
        {
            string name = initialName;
            int i = 1;

            while (this.Properties.Count(p => p.Name == name) > 0)
            {
                name = initialName + i;
                i++;
            }

            return name;
        }

        public virtual void CommiteChanges()
        {
            Model.Properties = new SaveProperty[this.Properties.Count];
            for (int i = 0; i < this.Properties.Count; i++)
            {
                Model.Properties[i] = this.Properties[i].Model;
            }
        }

        [Browsable(false)]
        public string DisplayName { get { return string.Format("class {0}", Name); } }


        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region UnDoRedo helper classes
        class AddSavePropertyUnDoRedo : IUnDoRedoCommand
        {
            int _Index;
            SavePropertyViewModel _NewProperty;
            SaveClassViewModel _Class;
            bool _Reverse;

            public AddSavePropertyUnDoRedo(SavePropertyViewModel newProperty, SaveClassViewModel cl, int index, bool reverse = false)
            {
                this._NewProperty = newProperty;
                this._Class = cl;
                this._Reverse = reverse;
                this._Index = index;
            }

            public void Undo()
            {
                if (_Reverse)
                    _Class.Properties.Insert(_Index, _NewProperty);
                else
                    _Class.Properties.Remove(_NewProperty);
            }

            public void Redo()
            {
                if (_Reverse)
                    _Class.Properties.Remove(_NewProperty);
                else
                    _Class.Properties.Insert(_Index, _NewProperty);
            }
        }
        #endregion

    }
}
