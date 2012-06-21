using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation
{
    [DisplayName("Connector")]
    public class ConnectorViewModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public ConnectorType Type
        {
            get { return Model.Type; }
            set
            {
                if (Model.Type != value)
                {
                    Model.Type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        [Browsable(false)]
        public int Index
        {
            get { return Model.Index; }
            set
            {
                if (Model.Index != value)
                {
                    Model.Index = value;
                    OnPropertyChanged("Index");
                }
            }
        }
      
        [Description("Name of Connector")]
        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (Model.Name != value)
                {
                    if (AnimNode.Tree.Editor.History != null)
                    {
                        AnimNode.Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    }
                    Model.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public AnimNodeViewModel AnimNode { get; private set; }

        private Editor.AnimConnector _Connector;
        [Browsable(false)]
        public Editor.AnimConnector Connector { get { return _Connector; }
            set
            {
                if (_Connector != value)
                {
                    _Connector = value;
                    OnPropertyChanged("Connector");
                }
            }
        }

        [Browsable(false)]
        public Connector Model { get; private set; }

        public ConnectorViewModel(AnimNodeViewModel animNode, Connector model)
        {
            this.AnimNode = animNode;
            this.Model = model;
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
}
