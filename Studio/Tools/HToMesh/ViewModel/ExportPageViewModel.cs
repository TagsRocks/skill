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

        public ExportPageViewModel(RawFile file)
            : base(file)
        {
            MinHeight = 0;
            MaxHeight = 255;
            ScaleX = ScaleY = ScaleZ = 1;
            _InverseY = false;
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
                TerrainScene scene = new Skill.Fbx.TerrainScene("Terrain", Data.HeightsDouble, Data.Size, Data.SplitSize);
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
