using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.AI;
using System.Collections.ObjectModel;

namespace Skill.Studio.AI
{
    #region ParameterViewModel
    public class ParameterViewModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public Parameter Model { get; private set; }

        /// <summary> Name of parameter </summary>
        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        /// <summary> Type of parameter </summary>
        public ParameterType Type
        {
            get { return Model.Type; }
            set
            {
                if (Model.Type != value)
                {
                    Model.Type = value;
                    switch (Model.Type)
                    {
                        case ParameterType.Int:
                            this.Value = 0.ToString();
                            break;
                        case ParameterType.Bool:
                            this.Value = false.ToString();
                            break;
                        case ParameterType.Float:
                            this.Value = 0.0f.ToString();
                            break;
                        case ParameterType.String:
                            this.Value = "";
                            break;
                        default:
                            break;
                    }
                    OnPropertyChanged("Type");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        /// <summary> Value of parameter </summary>
        public string Value
        {
            get
            {
                return Model.Value;
            }
            set
            {
                if (Model.Value != value)
                {
                    bool valid = false;
                    switch (Model.Type)
                    {
                        case ParameterType.Int:
                            int i;
                            if (valid = int.TryParse(value, out i))
                                Model.Value = value;
                            break;
                        case ParameterType.Bool:
                            bool b;
                            if (valid = bool.TryParse(value, out b))
                                Model.Value = value;
                            break;
                        case ParameterType.Float:
                            float f;
                            if (valid = float.TryParse(value, out f))
                                Model.Value = value;
                            break;
                        case ParameterType.String:
                            valid = true;
                            if (value == null)
                                value = string.Empty;
                            Model.Value = value;
                            break;
                    }
                    if (valid)
                        OnPropertyChanged("Value");
                }
            }
        }

        [Browsable(false)]
        public string DisplayName
        {
            get
            {
                return string.Format("{0}({1})", Name, Type);
            }
        }

        public ParameterViewModel(Parameter model)
        {
            this.Model = model;
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Name, Value);
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion // INotifyPropertyChanged Members
    }
    #endregion

    #region ParameterCollectionViewModel
    public class ParameterCollectionViewModel : ICollection<ParameterViewModel>
    {
        public ParameterCollection Model { get; private set; }

        public ObservableCollection<ParameterViewModel> Parameters { get; private set; }

        public ParameterCollectionViewModel(ParameterCollection collection)
        {
            this.Model = collection;
            this.Parameters = new ObservableCollection<ParameterViewModel>();

            foreach (var item in this.Model)
            {
                this.Parameters.Add(new ParameterViewModel(item));
            }
        }

        public void Add(ParameterViewModel item)
        {
            Parameters.Add(item);
            Model.Add(item.Model);
        }

        public void Clear()
        {
            Parameters.Clear();
            Model.Clear();
        }

        public bool Contains(ParameterViewModel item)
        {
            return Parameters.Contains(item);
        }

        public void CopyTo(ParameterViewModel[] array, int arrayIndex)
        {
            Parameters.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Parameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ParameterViewModel item)
        {
            if (Parameters.Remove(item))
                return Model.Remove(item.Model);
            return false;
        }

        public IEnumerator<ParameterViewModel> GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (Parameters as System.Collections.IEnumerable).GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append('(');

            for (int i = 0; i < Count; i++)
            {
                ParameterViewModel item = Parameters[i];
                builder.Append(item.ToString());
                if (i < Count - 1)
                    builder.Append(", ");
            }

            builder.Append(')');

            return builder.ToString();
        }
    }
    #endregion
}
