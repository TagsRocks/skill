using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Skill.Studio.Controls
{
    /// <summary>
    /// Interaction logic for ThreeStateImageButton.xaml
    /// </summary>
    public partial class ThreeStateImageButton : Button, INotifyPropertyChanged
    {

        private BitmapImage _ImgNormal;
        public BitmapImage ImgNormal
        {
            get { return _ImgNormal; }
            set
            {
                if (_ImgNormal != value)
                {
                    _ImgNormal = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ImgNormal"));
                }
            }
        }

        private BitmapImage _ImgPressed;
        public BitmapImage ImgPressed
        {
            get { return _ImgPressed; }
            set
            {
                if (_ImgPressed != value)
                {
                    _ImgPressed = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ImgPressed"));
                }
            }
        }

        private BitmapImage _ImgDisabled;
        public BitmapImage ImgDisabled
        {
            get { return _ImgDisabled; }
            set
            {
                if (_ImgDisabled != value)
                {
                    _ImgDisabled = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ImgDisabled"));
                }
            }
        }

        public ThreeStateImageButton()
        {
            InitializeComponent();            
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        #endregion // INotifyPropertyChanged Members
    }
}
