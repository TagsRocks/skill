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
using Skill.Fbx;

namespace Skill.Studio.Tools
{
    /// <summary>
    /// Interaction logic for SaveOption.xaml
    /// </summary>
    public partial class SaveOptionControl : UserControl, INotifyPropertyChanged
    {
        private SaveSceneOptions _SaveOptions;

        public SaveSceneOptions SaveOptions
        {
            get { return (SaveSceneOptions)GetValue(SaveOptionsProperty); }
            set { SetValue(SaveOptionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveOptions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveOptionsProperty =
            DependencyProperty.Register("SaveOptions", typeof(SaveSceneOptions), typeof(SaveOptionControl));

        public SaveOptionControl()
        {
            Versions = new string[] { "2013", "2012", "2011", "2010", "2009", "2006" };
            Formats = new string[] { "Binary", "Ascii" };
            Axises = new string[] { "YUp", "ZUp" };
            Units = new string[] { "Inches", "Feet", " Yards", "Miles", "Millimeters", "Centimeters", "Meters", "Kilometers" };

            _SaveOptions = new SaveSceneOptions();
            //_Axis = FbxAxis.YUp;
            _SaveOptions.FileFormat = Skill.Fbx.FbxFileFormat.Binary;
            _SaveOptions.SmoothingGroups = false;
            _SaveOptions.SmoothMesh = false;
            _SaveOptions.TangentsAndBinormals = false;
            //_Units = FbxSystemUnits.Meters;
            _SaveOptions.Version = FbxVersion.V2012;
            _SaveOptions.Axis = FbxAxis.YUp;
            _SaveOptions.Units = FbxSystemUnits.Meters;

            InitializeComponent();

            SelectedVersion = 1;
            SelectedFormat = 0;
            SelectedAxis = 0;
            SelectedUnit = 6;
        }

        public string[] Versions { get; private set; }
        public string[] Formats { get; private set; }
        public string[] Axises { get; private set; }
        public string[] Units { get; private set; }


        private int _SelectedFormat;
        public int SelectedFormat
        {
            get { return _SelectedFormat; }
            set
            {
                if (_SelectedFormat != value)
                {
                    _SelectedFormat = value;
                    OnPropertyChanged("SelectedFormat");
                    ApplySaveOptions();
                }
            }
        }

        private int _SelectedVersion;
        public int SelectedVersion
        {
            get { return _SelectedVersion; }
            set
            {
                if (_SelectedVersion != value)
                {
                    _SelectedVersion = value;
                    OnPropertyChanged("SelectedVersion");
                    ApplySaveOptions();
                }
            }
        }

        private int _SelectedAxis;
        public int SelectedAxis
        {
            get { return _SelectedAxis; }
            set
            {
                if (_SelectedAxis != value)
                {
                    _SelectedAxis = value;
                    OnPropertyChanged("SelectedAxis");
                    ApplySaveOptions();
                }
            }
        }

        private int _SelectedUnit;
        public int SelectedUnit
        {
            get { return _SelectedUnit; }
            set
            {
                if (_SelectedUnit != value)
                {
                    _SelectedUnit = value;
                    OnPropertyChanged("SelectedUnit");
                    ApplySaveOptions();
                }
            }
        }

        public bool SmoothingGroups
        {
            get { return _SaveOptions.SmoothingGroups; }
            set
            {
                if (_SaveOptions.SmoothingGroups != value)
                {
                    _SaveOptions.SmoothingGroups = value;
                    OnPropertyChanged("SmoothingGroups");
                    ApplySaveOptions();
                }
            }
        }
        public bool TangentsAndBinormals
        {
            get { return _SaveOptions.TangentsAndBinormals; }
            set
            {
                if (_SaveOptions.TangentsAndBinormals != value)
                {
                    _SaveOptions.TangentsAndBinormals = value;
                    OnPropertyChanged("TangentsAndBinormals");
                    ApplySaveOptions();
                }
            }
        }
        public bool SmoothMesh
        {
            get { return _SaveOptions.SmoothMesh; }
            set
            {
                if (_SaveOptions.SmoothMesh != value)
                {
                    _SaveOptions.SmoothMesh = value;
                    OnPropertyChanged("SmoothMesh");
                    ApplySaveOptions();
                }
            }
        }

        private void ApplySaveOptions()
        {
            _SaveOptions.Version = (FbxVersion)_SelectedVersion;
            _SaveOptions.FileFormat = (FbxFileFormat)_SelectedFormat;
            _SaveOptions.Axis = (FbxAxis)_SelectedAxis;
            _SaveOptions.Units = (FbxSystemUnits)(_SelectedUnit + 1);
            SaveOptions = _SaveOptions;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
