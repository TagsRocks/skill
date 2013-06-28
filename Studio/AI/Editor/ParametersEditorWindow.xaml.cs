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
using Skill.DataModels.AI;

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for ParametersEditor.xaml
    /// </summary>
    public partial class ParametersEditorWindow : Window, INotifyPropertyChanged
    {
        private ParameterCollectionViewModel _Parameters;

        private bool _HasSelected;
        public bool HasSelected
        {
            get { return _HasSelected; }
            set
            {
                if (_HasSelected != value)
                {
                    _HasSelected = value;
                    OnPropertyChanged("HasSelected");
                }
            }
        }

        public ParametersEditorWindow()
            : this(null)
        {
        }
        public ParametersEditorWindow(ParameterCollectionViewModel parameters)
        {
            InitializeComponent();
            if (parameters != null)
            {
                _Parameters = parameters;
                _LbParameters.ItemsSource = _Parameters.Parameters;
            }
        }

        private string GetValidParameterName(string pName)
        {
            int i = 0;
            string name = pName + i;
            while (_Parameters.Count(p => p.Name == name) > 0)
            {
                i++;
                name = pName + i;
            }
            return name;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_CmbParameters.SelectedItem != null)
            {
                ParameterType pType = (ParameterType)((ComboBoxItem)_CmbParameters.SelectedItem).Tag;
                Parameter p = new Parameter() { Name = GetValidParameterName("Parameter"), Type = pType };

                switch (pType)
                {
                    case ParameterType.Float:
                    case ParameterType.Int:
                        p.Value = "0";
                        break;
                    case ParameterType.Bool:
                        p.Value = "False";
                        break;
                    case ParameterType.String:
                        p.Value = "";
                        break;
                }

                _Parameters.Add(new ParameterViewModel(p));
                _LbParameters.SelectedIndex = _LbParameters.Items.Count - 1;
            }
        }

        private void LbParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelected = _LbParameters.SelectedItem != null;
        }

        private void LbParameters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelected();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelected();
        }

        private void DeleteSelected()
        {
            if (_LbParameters.SelectedItem != null)
                _Parameters.Remove(_LbParameters.SelectedItem as ParameterViewModel);
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
