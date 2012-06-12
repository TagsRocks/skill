using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for AnimNodeInputsEditor.xaml
    /// </summary>
    public partial class AnimNodeInputsEditor : Window, INotifyPropertyChanged
    {
        private AnimNodeViewModel _Node;
        private ObservableCollection<ConnectorViewModel> _Connectors;

        private bool _CanRemove;
        public bool CanRemove
        {
            get { return _CanRemove; }
            set
            {
                if (_CanRemove != value)
                {
                    _CanRemove = value;
                    OnPropertyChanged("CanRemove");
                }
            }
        }

        public AnimNodeInputsEditor()
        {
            InitializeComponent();
        }

        public AnimNodeInputsEditor(AnimNodeViewModel node)
        {
            InitializeComponent();
            this._Node = node;
            this._Connectors = new ObservableCollection<ConnectorViewModel>();

            foreach (var item in this._Node.Inputs)
            {
                ConnectorViewModel c = new ConnectorViewModel(_Node, new DataModels.Animation.Connector() { Index = item.Index, Name = item.Name, Type = item.Type });
                this._Connectors.Add(c);
            }
            this._LbInputs.ItemsSource = this._Connectors;
            CheckCanRemove();
        }

        private void CheckCanRemove() { CanRemove = this._Connectors.Count > 2; }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this._Connectors.Count; i++)
            {
                if (i < this._Node.Inputs.Count)
                {
                    this._Node.Inputs[i].Name = this._Connectors[i].Name;
                }
                else if (i == this._Node.Inputs.Count)
                {
                    this._Node.Inputs.Add(this._Connectors[i]);
                }
            }

            this._Node.Tree.History.IsEnable = false;
            while (this._Node.Inputs.Count > this._Connectors.Count)
            {
                int index = this._Node.Inputs.Count - 1;
                this._Node.Inputs.RemoveAt(index);
                this._Node.Tree.RemoveConnectionsTo(this._Node, index);
            }
            this._Node.Tree.History.IsEnable = true;

            this._Node.Tree.Editor.SetChanged(true);
            Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (CanRemove)
                _Connectors.RemoveAt(_Connectors.Count - 1);
            CheckCanRemove();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConnectorViewModel c = new ConnectorViewModel(_Node, new DataModels.Animation.Connector()
            {
                Index = this._Connectors.Count,
                Name = string.Format("Input{0}", this._Connectors.Count + 1),
                Type = DataModels.Animation.ConnectorType.Input
            });
            this._Connectors.Add(c);
            CheckCanRemove();
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
