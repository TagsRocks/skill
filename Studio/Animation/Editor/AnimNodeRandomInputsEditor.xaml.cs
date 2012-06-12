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
    public class ChanceViewModel : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

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
                }
            }
        }

        public int Index { get; set; }

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
    public partial class AnimNodeRandomInputsEditor : Window, INotifyPropertyChanged
    {
        private AnimNodeRandomViewModel _Node;
        private ObservableCollection<ChanceViewModel> _Chances;

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

        public AnimNodeRandomInputsEditor()
        {
            InitializeComponent();
        }

        public AnimNodeRandomInputsEditor(AnimNodeRandomViewModel node)
        {
            InitializeComponent();
            this._Node = node;
            this._Chances = new ObservableCollection<ChanceViewModel>();

            Skill.DataModels.Animation.AnimNodeRandom model = (Skill.DataModels.Animation.AnimNodeRandom)this._Node.Model;

            for (int i = 0; i < this._Node.Inputs.Count; i++)
            {
                ChanceViewModel c = new ChanceViewModel();
                c.Name = this._Node.Inputs[i].Name;
                c.Value = model.Chances.Length >= this._Node.Inputs.Count ? model.Chances[i] : 1;
                c.Index = i;
                this._Chances.Add(c);
            }
            this._LbInputs.ItemsSource = this._Chances;
            CheckCanRemove();
        }

        private void CheckCanRemove() { CanRemove = this._Chances.Count > 2; }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            float[] newChances = new float[this._Chances.Count];

            for (int i = 0; i < this._Chances.Count; i++)
            {
                newChances[i] = this._Chances[i].Value;
                if (i < this._Node.Inputs.Count)
                {
                    this._Node.Inputs[i].Name = this._Chances[i].Name;
                }
                else if (i == this._Node.Inputs.Count)
                {
                    ConnectorViewModel connector = new ConnectorViewModel(this._Node, new DataModels.Animation.Connector()
                    {
                        Index = i,
                        Name = string.Format("Input{0}", i + 1),
                        Type = DataModels.Animation.ConnectorType.Input
                    });
                    this._Node.Inputs.Add(connector);
                }
            }

            this._Node.Tree.History.IsEnable = false;
            while (this._Node.Inputs.Count > this._Chances.Count)
            {
                int index = this._Node.Inputs.Count - 1;
                this._Node.Inputs.RemoveAt(index);
                this._Node.Tree.RemoveConnectionsTo(this._Node, index);
            }
            this._Node.Tree.History.IsEnable = true;

            Skill.DataModels.Animation.AnimNodeRandom model = (Skill.DataModels.Animation.AnimNodeRandom)this._Node.Model;
            model.Chances = newChances;

            this._Node.Tree.Editor.SetChanged(true);
            Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (CanRemove)
                _Chances.RemoveAt(_Chances.Count - 1);
            CheckCanRemove();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ChanceViewModel c = new ChanceViewModel()
            {
                Name = string.Format("Input{0}", this._Chances.Count + 1),
                Value = 1,
                Index = this._Chances.Count
            };
            this._Chances.Add(c);
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
