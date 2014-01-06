using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Skill.Fbx;
using System.Collections.ObjectModel;

namespace Skill.Studio.Tools.HToMesh.ViewModel
{
    public class ExportPageViewModel : WizardPageViewModelBase<RawFile>
    {
        public override string DisplayName { get { return "Export"; } }
        public override bool IsValid { get { return true; } }

        private bool _IsLodAvailable;
        public bool IsLodAvailable
        {
            get { return _IsLodAvailable; }
            set
            {
                if (_IsLodAvailable != value)
                {
                    _IsLodAvailable = value;
                    OnPropertyChanged("IsLodAvailable");
                    if (_IsLodAvailable == false)
                        SelectedLod = 0;
                }
            }
        }

        private int _SelectedLod;
        public int SelectedLod
        {
            get { return _SelectedLod; }
            set
            {
                if (_SelectedLod != value)
                {
                    _SelectedLod = value;
                    OnPropertyChanged("SelectedLod");
                }
            }
        }

        public int[] Lods { get; private set; }

        public ExportPageViewModel(RawFile file)
            : base(file)
        {
            MinHeight = 0;
            MaxHeight = 255;
            ScaleX = ScaleY = ScaleZ = 1;
            InverseY = false;
        }

        private bool IsPowerOf2(int num)
        {
            return num == 8 || num == 16 || num == 32 || num == 64 || num == 128 || num == 256 || num == 512 || num == 1024 || num == 2048;
        }
        private void RefreshLod()
        {
            if (Data.SplitSize.Width == Data.SplitSize.Height)
                IsLodAvailable = IsPowerOf2(Data.SplitSize.Width);
            else
                IsLodAvailable = false;

            if (IsLodAvailable)
            {
                List<int> lodlist = new List<int>();
                lodlist.Add(0);

                int lodCounter = 1;
                int size = 8;
                while (size < Data.SplitSize.Width)
                {
                    lodlist.Add(lodCounter++);
                    size *= 2;
                }
                Lods = lodlist.ToArray();
            }
            else
            {
                Lods = new int[] { 0 };
                SelectedLod = 0;

            }
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            RefreshLod();
        }

        #region Commands

        private RelayCommand _ExportCommand;
        public ICommand ExportCommand
        {
            get
            {
                if (_ExportCommand == null)
                    _ExportCommand = new RelayCommand(
                        () => this.Export(),
                        () => this.CanExport);
                return _ExportCommand;
            }
        }
        bool CanExport { get { return true; } }
        void Export()
        {
            Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
            save.Filter = "fbx|*.fbx";
            if (save.ShowDialog() == true)
            {
                TerrainScene scene = new Skill.Fbx.TerrainScene("Terrain", Data.HeightsDouble, Data.Size, Data.SplitSize, _SelectedLod);
                scene.Scale = new Vector3() { X = ScaleX, Y = ScaleY, Z = ScaleZ };
                scene.MinHeight = this.MinHeight;
                scene.MaxHeight = this.MaxHeight;
                scene.InverseY = this.InverseY;

                CreatePatchParams parameters = new CreatePatchParams();

                foreach (var p in Data.Patches)
                {
                    if (p.Selected)
                    {
                        parameters.IndexI = p.Row;
                        parameters.IndexJ = p.Column;
                        parameters.PatchName = string.Format("Patch{0}_{1}", p.Row, p.Column);
                        scene.AddPatch(parameters);
                    }
                }

                scene.Build();

                if (System.IO.File.Exists(save.FileName))
                    System.IO.File.Delete(save.FileName);
                scene.Save(save.FileName, _SaveOptions);
                scene.Destroy();
                scene = null;
            }
        }
        #endregion


        private SaveSceneOptions _SaveOptions;
        public SaveSceneOptions SaveOptions
        {
            get { return _SaveOptions; }
            set
            {
                _SaveOptions = value;
                OnPropertyChanged("SaveOptions");
            }
        }

        private bool _InverseY;
        public bool InverseY
        {
            get { return _InverseY; }
            set
            {
                if (_InverseY != value)
                {
                    _InverseY = value;
                    OnPropertyChanged("InverseY");
                }
            }
        }

        private float _MinHeight;
        public float MinHeight
        {
            get { return _MinHeight; }
            set
            {
                if (_MinHeight != value)
                {
                    _MinHeight = value;
                    OnPropertyChanged("MinHeight");
                }
            }
        }

        private float _MaxHeight;
        public float MaxHeight
        {
            get { return _MaxHeight; }
            set
            {
                if (_MaxHeight != value)
                {
                    _MaxHeight = value;
                    OnPropertyChanged("MaxHeight");
                }
            }
        }

        private float _ScaleX;
        public float ScaleX
        {
            get { return _ScaleX; }
            set
            {
                if (_ScaleX != value)
                {
                    _ScaleX = value;
                    OnPropertyChanged("ScaleX");
                }
            }
        }
        private float _ScaleY;
        public float ScaleY
        {
            get { return _ScaleY; }
            set
            {
                if (_ScaleY != value)
                {
                    _ScaleY = value;
                    OnPropertyChanged("ScaleY");
                }
            }
        }
        private float _ScaleZ;
        public float ScaleZ
        {
            get { return _ScaleZ; }
            set
            {
                if (_ScaleZ != value)
                {
                    _ScaleZ = value;
                    OnPropertyChanged("ScaleZ");
                }
            }
        }
    }
}
