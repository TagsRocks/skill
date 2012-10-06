using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.IO;

namespace Skill.Studio.IO
{
    [DisplayName("Property")]
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
        public abstract string CodeString { get; }

        [Description("Name of property in class definition")]
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
                    OnPropertyChanged("CodeString");
                }
            }
        }

        [DefaultValue(false)]
        [Description("Whether this property is a single value or array of values")]
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
                    OnPropertyChanged("CodeString");
                }
            }
        }

        [DefaultValue("")]
        [Description("User comment for property")]
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


    [DisplayName("Primitive Property")]
    public class PrimitivePropertyViewModel : SavePropertyViewModel
    {
        public PrimitivePropertyViewModel(SaveClassViewModel ownerClass, PrimitiveProperty model)
            : base(ownerClass, model)
        {

        }

        [Description("Choose type of property")]
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
                    OnPropertyChanged("CodeString");
                }
            }
        }

        private static string GetPrimitiveTypeString(Skill.DataModels.PrimitiveType pt)
        {
            switch (pt)
            {
                case Skill.DataModels.PrimitiveType.Int:
                case Skill.DataModels.PrimitiveType.Float:
                case Skill.DataModels.PrimitiveType.Bool:
                case Skill.DataModels.PrimitiveType.String:
                    return pt.ToString().ToLower();
                default:
                    return pt.ToString();
            }
        }

        public override string CodeString
        {
            get
            {
                if (IsArray)
                    return string.Format("public {0}[] {1}", GetPrimitiveTypeString(PrimitiveType), Name);
                else
                    return string.Format("public {0} {1}", GetPrimitiveTypeString(PrimitiveType), Name);
            }
        }
    }

    [DisplayName("Class Property")]
    public class ClassPropertyViewModel : SavePropertyViewModel
    {
        public const string InvalidClass = "Not_Set";

        public ClassPropertyViewModel(SaveClassViewModel ownerClass, ClassProperty model)
            : base(ownerClass, model)
        {

        }

        [Description("Name of class that this property refer to.")]
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
                    OnPropertyChanged("CodeString");
                }
            }
        }

        [Browsable(false)]
        public override string CodeString
        {
            get
            {
                string cName = string.IsNullOrEmpty(ClassName) ? InvalidClass : ClassName;

                if (IsArray)
                    return string.Format("public {0}[] {1}", cName, Name);
                else
                    return string.Format("public {0} {1}", cName, Name);

            }
        }
    }
}
