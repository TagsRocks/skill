using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Skill.Studio.Tools.HToMesh.ViewModel
{
    public class OpenPageViewModel : WizardPageViewModelBase<RawFile>
    {
        public override string DisplayName { get { return "Open file"; } }

        private bool _IsValid;
        public override bool IsValid { get { return _IsValid; } }

        private int _Width;
        public int Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    OnPropertyChanged("Width");
                }
            }
        }

        private int _Height;
        public int Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    OnPropertyChanged("Height");
                }
            }
        }

        public OpenPageViewModel(RawFile file)
            : base(file)
        {

        }

        private int _SelectedPixelFormat;
        public int SelectedPixelFormat
        {
            get { return _SelectedPixelFormat; }
            set
            {
                if (_SelectedPixelFormat != value)
                {
                    _SelectedPixelFormat = value;
                    OnPropertyChanged("SelectedPixelFormat");
                }
            }
        }

        private RelayCommand _OpenCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (_OpenCommand == null)
                    _OpenCommand = new RelayCommand(
                        () => this.Open(),
                        () => this.CanOpen);
                return _OpenCommand;
            }
        }

        bool CanOpen { get { return true; } }
        void Open()
        {
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
            open.Filter = "raw|*.raw";
            if (open.ShowDialog() == true)
            {
                try
                {
                    Data.Open(open.FileName, SelectedPixelFormat == 0 ? RawPixelFormat.Bit8 : RawPixelFormat.Bit16);
                    _IsValid = Data.Image != null;
                    if (Data.Image != null)
                    {
                        Width = Data.Size.Width;
                        Height = Data.Size.Height;
                        _IsValid = true;
                    }
                    else
                    {
                        Width = 0;
                        Height = 0;
                        _IsValid = false;
                    }

                }
                catch (Exception ex)
                {
                    _IsValid = false;
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }




        private RelayCommand _CreateCommand;
        public ICommand CreateCommand
        {
            get
            {
                if (_CreateCommand == null)
                    _CreateCommand = new RelayCommand(
                        () => this.CreateImage(),
                        () => this.CanCreateImage);
                return _CreateCommand;
            }
        }

        bool CanCreateImage { get { return Width > 0 && Height > 0; } }
        void CreateImage()
        {
            if (CanCreateImage)
            {
                Data.TryCreateImage(new Skill.Fbx.Dimension() { Width = Width, Height = Height });
                _IsValid = Data.Image != null;
            }
        }
    }
}
