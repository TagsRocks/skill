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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Skill.Studio.Tools.HToMesh
{
    /// <summary>
    /// Interaction logic for Patch.xaml
    /// </summary>
    public partial class Patch : UserControl, INotifyPropertyChanged
    {
        public Patch()
        {
            InitializeComponent();
        }        

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected != value)
                {
                    _Selected = value;
                    BorderVisibility = value ? Visibility.Visible : System.Windows.Visibility.Hidden; 
                    OnPropertyChanged("Selected");
                }
            }
        }

        private Visibility _BorderVisibility;
        public Visibility BorderVisibility
        {
            get { return _BorderVisibility; }
            set
            {
                if (_BorderVisibility != value)
                {
                    _BorderVisibility = value;
                    OnPropertyChanged("BorderVisibility");
                }
            }
        }

        private int _Row;
        public int Row
        {
            get { return _Row; }
            set
            {
                if (_Row != value)
                {
                    _Row = value;
                    Grid.SetRow(this, value);
                    OnPropertyChanged("Row");
                }
            }
        }

        private int _Column;
        public int Column
        {
            get { return _Column; }
            set
            {
                if (_Column != value)
                {
                    _Column = value;
                    Grid.SetColumn(this, value);
                    OnPropertyChanged("Column");
                }
            }
        }

        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void Img_MouseClick(object sender, MouseButtonEventArgs e)
        {
            Selected = !Selected;
        } 
        #endregion
    }
}
