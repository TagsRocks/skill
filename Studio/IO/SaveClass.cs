using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.IO;
using System.Collections.ObjectModel;

namespace Skill.Studio.IO
{
    [DisplayName("Class")]
    public class SaveClassViewModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public SaveClass Model { get; private set; }

        [Browsable(false)]
        public SaveDataViewModel SaveData { get; protected set; }

        [Browsable(false)]
        public ObservableCollection<SavePropertyViewModel> Properties { get; private set; }

        [Description("Name of class")]
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
                    string preName = Model.Name;
                    if (SaveData.Editor.History != null)
                    {
                        SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    }
                    Model.Name = value;
                    OnPropertyChanged("Name", preName, value);
                    OnPropertyChanged("CodeString");
                }
            }
        }

        [Description("is struct?")]
        public bool IsStruct
        {
            get
            {
                return Model.IsStruct;
            }
            set
            {
                if (Model.IsStruct != value)
                {
                    SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "IsStruct", value, Model.IsStruct));
                    Model.IsStruct = value;
                    OnPropertyChanged("IsStruct", !value, value);
                    OnPropertyChanged("CodeString");
                }
            }
        }

        [DefaultValue("")]
        [Description("Comment of class")]
        public string Comment
        {
            get
            {
                return Model.Comment;
            }
            set
            {
                if (value == null) value = "";
                if (Model.Comment != value)
                {
                    if (SaveData.Editor.History != null)
                    {
                        SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Comment", value, Model.Comment));
                    }
                    Model.Comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }

        public SaveClassViewModel(SaveDataViewModel saveGame, SaveClass model)
        {
            this.SaveData = saveGame;
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
            SaveData.History.Insert(new AddSavePropertyUnDoRedo(newP, this, this.Properties.Count - 1));
        }
        public void RemovePropertyAt(int index)
        {
            if (index >= 0 && index < this.Properties.Count)
            {
                SavePropertyViewModel p = this.Properties[index];
                this.Properties.RemoveAt(index);
                SaveData.History.Insert(new AddSavePropertyUnDoRedo(p, this, index, true));
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
        public string CodeString { get { return string.Format("{0} {1}", IsStruct ? "struct" : "class", Name); } }


        public void NotifyChangeClassName(string oldName, string newName)
        {
            foreach (var p in Properties)
            {
                if (p is ClassPropertyViewModel)
                {
                    ClassPropertyViewModel cp = p as ClassPropertyViewModel;
                    if (cp.ClassName == oldName)
                    {
                        cp.ClassName = newName;
                    }
                }
            }
        }

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
        protected void OnPropertyChanged(string name, object preValue, object newValue)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new ObjectPropertyChangedEventArgs(name, preValue, newValue));
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
