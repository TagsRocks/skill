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
                    if (OwnerClass.SaveData.Editor.History != null)
                    {
                        OwnerClass.SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    }
                    Model.Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public bool IsArray
        {
            get
            {
                return Model.IsArray;
            }
            set
            {
                if (Model.IsArray != value)
                {
                    if (OwnerClass.SaveData.Editor.History != null)
                    {
                        OwnerClass.SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "IsArray", value, !value));
                    }
                    Model.IsArray = value;
                    OnPropertyChanged("IsArray");
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
                    if (OwnerClass.SaveData.Editor.History != null)
                    {
                        OwnerClass.SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Comment", value, Model.Comment));
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
                    if (OwnerClass.SaveData.Editor.History != null)
                    {
                        OwnerClass.SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "PrimitiveType", value, ((PrimitiveProperty)Model).PrimitiveType));
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
                if (IsArray)
                    return string.Format("public {0}[] {1}", PrimitiveType, Name);
                else
                    return string.Format("public {0} {1}", PrimitiveType, Name);
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
                    if (OwnerClass.SaveData.Editor.History != null)
                    {
                        OwnerClass.SaveData.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ClassName", value, ((ClassProperty)Model).ClassName));
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
                if (IsArray)
                    return string.Format("public class {0}[] {1}", ClassName, Name);
                else
                    return string.Format("public class {0} {1}", ClassName, Name);

            }
        }
    }
}
