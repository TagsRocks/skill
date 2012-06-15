using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.IO;

namespace Skill.Studio.IO
{
    public abstract class SavePropertyViewModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public SaveProperty Model { get; private set; }
        [Browsable(false)]
        public SaveClassViewModel OwnerClass { get; private set; }

        public SavePropertyViewModel(SaveClassViewModel ownerClass, SaveProperty model)
        {
            this.OwnerClass = ownerClass;
            this.Model = model;
        }

        public PropertyType Type { get { return Model.Type; } }

        [Browsable(false)]
        public abstract string DisplayName { get; }

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
                    if (OwnerClass.SaveGame.Editor.History != null)
                    {
                        OwnerClass.SaveGame.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    }
                    Model.Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public CollectionType CollectionType
        {
            get
            {
                return Model.CollectionType;
            }
            set
            {
                if (Model.CollectionType != value)
                {
                    if (OwnerClass.SaveGame.Editor.History != null)
                    {
                        OwnerClass.SaveGame.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "CollectionType", value, Model.CollectionType));
                    }
                    Model.CollectionType = value;
                    OnPropertyChanged("CollectionType");
                    if (value == DataModels.IO.CollectionType.Array)
                    {
                        if (ArrayLength <= 0)
                        {
                            Model.ArrayLength = 1;
                            OnPropertyChanged("ArrayLength");
                        }
                    }
                    else if (value == DataModels.IO.CollectionType.List)
                    {
                        Model.ArrayLength = 0;
                        OnPropertyChanged("ArrayLength");
                    }
                    OnPropertyChanged("DisplayName");
                }
            }
        }


        public int ArrayLength
        {
            get
            {
                return Model.ArrayLength;
            }
            set
            {
                if (Model.ArrayLength != value)
                {
                    if (OwnerClass.SaveGame.Editor.History != null)
                    {
                        OwnerClass.SaveGame.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ArrayLength", value, Model.ArrayLength));
                    }
                    Model.ArrayLength = value;
                    OnPropertyChanged("ArrayLength");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

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
                    if (OwnerClass.SaveGame.Editor.History != null)
                    {
                        OwnerClass.SaveGame.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Comment", value, Model.Comment));
                    }
                    Model.Comment = value;
                    OnPropertyChanged("Comment");
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

        #endregion
    }



    public class PrimitivePropertyViewModel : SavePropertyViewModel
    {
        public PrimitivePropertyViewModel(SaveClassViewModel ownerClass, PrimitiveProperty model)
            : base(ownerClass, model)
        {

        }

        public Skill.DataModels.PrimitiveType PrimitiveType
        {
            get
            {
                return ((PrimitiveProperty)Model).PrimitiveType;
            }
            set
            {
                if (((PrimitiveProperty)Model).PrimitiveType != value)
                {
                    if (OwnerClass.SaveGame.Editor.History != null)
                    {
                        OwnerClass.SaveGame.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "PrimitiveType", value, ((PrimitiveProperty)Model).PrimitiveType));
                    }
                    ((PrimitiveProperty)Model).PrimitiveType = value;
                    OnPropertyChanged("PrimitiveType");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public override string DisplayName
        {
            get
            {
                switch (CollectionType)
                {
                    case CollectionType.None:
                        return string.Format("{0} : {1}", Name, PrimitiveType);
                    case CollectionType.List:
                        return string.Format("{0} : List<{1}>", Name, PrimitiveType);
                    case CollectionType.Array:
                        return string.Format("{0} : {1}[{2}]", Name, PrimitiveType, ArrayLength);
                }
                return Name;
            }
        }
    }

    public class ClassPropertyViewModel : SavePropertyViewModel
    {

        public ClassPropertyViewModel(SaveClassViewModel ownerClass, ClassProperty model)
            : base(ownerClass, model)
        {

        }

        [Description("Name of class")]
        [Editor(typeof(Editor.ClassNamePropertyEditor), typeof(Editor.ClassNamePropertyEditor))]
        public string ClassName
        {
            get
            {
                return ((ClassProperty)Model).ClassName;
            }
            set
            {
                if (value == null) value = "";
                if (((ClassProperty)Model).ClassName != value)
                {
                    if (OwnerClass.SaveGame.Editor.History != null)
                    {
                        OwnerClass.SaveGame.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ClassName", value, ((ClassProperty)Model).ClassName));
                    }
                    ((ClassProperty)Model).ClassName = value;
                    OnPropertyChanged("ClassName");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public override string DisplayName
        {
            get
            {
                switch (CollectionType)
                {
                    case CollectionType.None:
                        return string.Format("{0} : class {1}", Name, ClassName);
                    case CollectionType.List:
                        return string.Format("{0} : List<class {1}>", Name, ClassName);
                    case CollectionType.Array:
                        return string.Format("{0} : class {1}[{2}]", Name, ClassName, ArrayLength);
                }
                return Name;
            }
        }
    }
}
