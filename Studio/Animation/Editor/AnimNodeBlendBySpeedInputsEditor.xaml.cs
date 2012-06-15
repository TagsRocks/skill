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

    public class ConstraintViewModel : INotifyPropertyChanged
    {

        private float _Value;
        public float Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged("Value");
                    OnPropertyChanged("Text");
                }
            }
        }

        public string Text
        {
            get { return _Value.ToString("F"); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Value = 0;
                }
                else
                {
                    float f;
                    if (float.TryParse(value, out f))
                    {
                        Value = f;
                    }
                }
            }
        }

        public int Index { get; private set; }

        public ConstraintViewModel(int index)
        {
            this.Index = index;
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

    /// <summary>
    /// Interaction logic for AnimNodeRandomInputsEditor.xaml
    /// </summary>
    public partial class AnimNodeBlendBySpeedInputsEditor : Window, INotifyPropertyChanged
    {
        private AnimNodeBlendBySpeedViewModel _Node;
        private ObservableCollection<ConnectorViewModel> _Connectors;
        private ObservableCollection<ConstraintViewModel> _Constraints;

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

        public AnimNodeBlendBySpeedInputsEditor()
        {
            InitializeComponent();
        }

        public AnimNodeBlendBySpeedInputsEditor(AnimNodeBlendBySpeedViewModel node)
        {
            InitializeComponent();
            this._Node = node;
            this._Connectors = new ObservableCollection<ConnectorViewModel>();
            this._Constraints = new ObservableCollection<ConstraintViewModel>();


            Skill.DataModels.Animation.AnimNodeBlendBySpeed model = (Skill.DataModels.Animation.AnimNodeBlendBySpeed)_Node.Model;

            int i = 0;
            for (i = 0; i < this._Node.Inputs.Count; i++)
            {
                ConnectorViewModel c = new ConnectorViewModel(_Node, new DataModels.Animation.Connector() { Index = i, Name = this._Node.Inputs[i].Name });
                this._Connectors.Add(c);
                this._Constraints.Add(new ConstraintViewModel(i) { Value = model.Constraints[i] });
            }
            this._Constraints.Add(new ConstraintViewModel(i) { Value = model.Constraints[i] });
            this._LbInputs.ItemsSource = this._Connectors;
            this._LbConstraints.ItemsSource = this._Constraints;
            CheckCanRemove();
        }

        private void CheckCanRemove() { CanRemove = this._Connectors.Count > 2; }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this._Constraints.Count - 1; i++)
            {
                if (this._Constraints[i].Value >= this._Constraints[i + 1].Value)
                {
                    System.Windows.MessageBox.Show("Constraints must be ascendant ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

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

            Skill.DataModels.Animation.AnimNodeBlendBySpeed model = (Skill.DataModels.Animation.AnimNodeBlendBySpeed)_Node.Model;
            model.Constraints = new float[this._Constraints.Count];
            for (int i = 0; i < this._Constraints.Count; i++)
            {
                model.Constraints[i] = this._Constraints[i].Value;
            }

            this._Node.Tree.Editor.SetChanged(true);
            Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (CanRemove)
            {
                _Connectors.RemoveAt(_Connectors.Count - 1);
                _Constraints.RemoveAt(_Constraints.Count - 1);
            }
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

            this._Constraints.Add(new ConstraintViewModel(this._Constraints.Count) { Value = this._Constraints[this._Constraints.Count - 1].Value + 1 });
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
