using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Skill.Studio.Tools.HToMesh.ViewModel
{
    public class SplitPageViewModel : WizardPageViewModelBase<RawFile>
    {
        private int _RowCount;
        public int RowCount
        {
            get { return _RowCount; }
            set
            {
                if (_RowCount != value)
                {
                    _RowCount = value;
                    OnPropertyChanged("RowCount");
                }
            }
        }

        private int _ColumnCount;
        public int ColumnCount
        {
            get { return _ColumnCount; }
            set
            {
                if (_ColumnCount != value)
                {
                    _ColumnCount = value;
                    OnPropertyChanged("ColumnCount");
                }
            }
        }

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

                    for (int i = 0; i < SplitSizes.Count; i++)
                    {
                        if (SplitSizes[i] == _Width && SplitSizes[i] == _Height)
                        {
                            SelectedSplitSize = i;
                            return;
                        }
                    }
                    SelectedSplitSize = -1;
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

                    for (int i = 0; i < SplitSizes.Count; i++)
                    {
                        if (SplitSizes[i] == _Width && SplitSizes[i] == _Height)
                        {
                            SelectedSplitSize = i;
                            return;
                        }
                    }
                    SelectedSplitSize = -1;
                }
            }
        }

        public ObservableCollection<int> SplitSizes { get; private set; }

        public override string DisplayName { get { return "Split"; } }

        private bool _IsValid;
        public override bool IsValid { get { return _IsValid; } }

        public SplitPageViewModel(RawFile file)
            : base(file)
        {
            SplitSizes = new ObservableCollection<int>();
            ColumnCount = RowCount = 1;
        }

        private int _SelectedSplitSize;
        public int SelectedSplitSize
        {
            get { return _SelectedSplitSize; }
            set
            {
                if (_SelectedSplitSize != value)
                {
                    _SelectedSplitSize = value;
                    if (_SelectedSplitSize >= 0)
                        Width = Height = SplitSizes[_SelectedSplitSize];
                    OnPropertyChanged("SelectedSplitSize");
                }
            }
        }

        private RelayCommand _SplitCommand;
        public ICommand SplitCommand
        {
            get
            {
                if (_SplitCommand == null)
                    _SplitCommand = new RelayCommand(
                        () => this.Split(),
                        () => this.CanSplit);
                return _SplitCommand;
            }
        }
        bool CanSplit { get { return Width > 0 && Height > 0 && Width < Data.Size.Width && Height < Data.Size.Height; } }
        void Split()
        {
            try
            {
                if (CanSplit)
                {
                    Data.Split(new Skill.Fbx.Dimension() { Width = this.Width, Height = this.Height });
                    RowCount = Data.Size.Height / this.Height;
                    ColumnCount = Data.Size.Width / this.Width;
                    _IsValid = true;
                }
                else
                    _IsValid = false;

            }
            catch (Exception ex)
            {
                _IsValid = false;
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        private RelayCommand _SelectAllCommand;
        public ICommand SelectAllCommand
        {
            get
            {
                if (_SelectAllCommand == null)
                    _SelectAllCommand = new RelayCommand(
                        () => this.SelectAll(),
                        () => this.CanSelectAll);
                return _SelectAllCommand;
            }
        }
        bool CanSelectAll { get { return Data.Patches.Count > 0; } }
        void SelectAll()
        {
            foreach (var p in Data.Patches)            
                p.Selected = true;            
        }

        private RelayCommand _SelectNoneCommand;
        public ICommand SelectNoneCommand
        {
            get
            {
                if (_SelectNoneCommand == null)
                    _SelectNoneCommand = new RelayCommand(
                        () => this.SelectNone(),
                        () => this.CanSelectNone);
                return _SelectNoneCommand;
            }
        }
        bool CanSelectNone { get { return Data.Patches.Count > 0; } }
        void SelectNone()
        {
            foreach (var p in Data.Patches)
                p.Selected = false;
        }

        protected override void OnEnter()
        {

            if (SplitSizes.Count == 0)
            {
                int[] availableSplitSizes = Data.GetAvailableSplitSize();
                foreach (int sp in availableSplitSizes)
                {
                    SplitSizes.Add(sp);
                }
                SelectedSplitSize = -1;
            }

            base.OnEnter();
        }
    }
}
