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
using Skill.DataModels.Animation;
using System.Xml.Linq;

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for SkinMeshEditor.xaml
    /// </summary>
    public partial class SkinMeshEditor : TabDocument
    {

        #region Properties
        private bool _CanAddClip;
        public bool CanAddClip
        {
            get { return _CanAddClip; }
            set
            {
                if (_CanAddClip != value)
                {
                    _CanAddClip = value;
                    RaisePropertyChanged("CanAddClip");
                }
            }
        }
        #endregion

        #region SelectedItem
        /// <summary>
        /// Get selected Bone
        /// </summary>
        /// <returns>selected Bone</returns>
        private BoneViewModel GetSelectedItem()
        {
            if (_BoneTree.SelectedItem != null)
            {
                if (_BoneTree.SelectedItem is BoneViewModel)
                {
                    return (BoneViewModel)_BoneTree.SelectedItem;
                }
            }
            return null;
        }

        private SkinMeshViewModel _SkinMesh;
        public SkinMeshViewModel SkinMesh
        {
            get { return _SkinMesh; }
            set
            {
                if (_SkinMesh != value)
                {
                    _SkinMesh = value;
                    RaisePropertyChanged("SkinMesh");
                }
            }
        }

        #endregion

        #region Constructor
        public SkinMeshEditor()
            : this(null)
        {
        }
        public SkinMeshEditor(SkinMeshNodeViewModel vm)
            : base(vm)
        {
            InitializeComponent();
            if (vm != null)
            {
                SkinMesh sk = vm.LoadData() as SkinMesh;
                Data = SkinMesh = new SkinMeshViewModel(sk);
            }

            History.RedoChange += new UnDoRedoChangeEventHandler(History_RedoChange);
            History.UndoChange += new UnDoRedoChangeEventHandler(History_UndoChange);
        }

        // hook events of History
        void History_RedoChange(UnDoRedo sender, UnDoRedoChangeEventArgs e)
        {
            ResetCut();
        }

        void History_UndoChange(UnDoRedo sender, UnDoRedoChangeEventArgs e)
        {
            ResetCut();
        }
        #endregion

        #region Selected item changed

        private void BoneTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ApplicationCommands.Properties.Execute(_BoneTree.SelectedItem, null);
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
            }
        }
        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ResetCut();
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                DeleteBone();
                e.Handled = true;
            }
        }

        private void DeleteBone()
        {
            var selected = GetSelectedItem();
            if (selected != null)
            {
                if (selected.Parent != null)
                {
                    if (selected.Parent is BoneViewModel)
                    {
                        ((BoneViewModel)selected.Parent).RemoveBone(selected);
                        ResetCut();
                    }
                }
            }
        }
        #endregion

        #region Edit

        private void CopyBoneToClipboard(Bone b)
        {
            XDocument document = new XDocument();
            document.Add(b.ToXElement());
            Clipboard.Clear();
            Clipboard.SetData(Skill.DataModels.SkillDataFormats.Bone, document.ToString());
        }

        private Bone GetBoneFromClipboard()
        {
            if (Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.Bone))
            {
                string text = Clipboard.GetData(Skill.DataModels.SkillDataFormats.Bone) as string;
                if (text != null)
                {
                    try
                    {
                        XDocument document = XDocument.Parse(text);
                        Bone bone = new Bone();
                        bone.Load(document.Elements().First());
                        return bone;
                    }
                    catch (Exception)
                    {

                        return null;
                    }


                }
            }
            return null;
        }

        BoneViewModel _CuttedBone;
        private void ResetCut()
        {
            if (_CuttedBone != null)
                _CuttedBone.Cutting = false;
            _CuttedBone = null;
        }

        void CutCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var selected = GetSelectedItem();
            e.CanExecute = selected != null && selected.Parent != null;
            e.Handled = true;

        }
        void CutCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            var selected = GetSelectedItem();
            if (selected != null)
            {
                ResetCut();
                _CuttedBone = selected;
                _CuttedBone.Cutting = true;
                CopyBoneToClipboard(selected.Model);
            }
            e.Handled = true;
        }

        void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetSelectedItem() != null;
            e.Handled = true;
        }

        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            var selected = GetSelectedItem();
            if (selected != null)
            {
                ResetCut();
                CopyBoneToClipboard(selected.Model);
            }
            e.Handled = true;
        }

        void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(Skill.DataModels.SkillDataFormats.Bone);
            e.Handled = true;
        }
        void PasteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            Bone pasteBone = GetBoneFromClipboard();

            if (pasteBone != null)
            {
                var selected = GetSelectedItem();
                if (selected != null)
                {
                    try
                    {
                        if (_CuttedBone != null && _CuttedBone.Model.Name == pasteBone.Name) // paste as cut                                                    
                        {
                            ((BoneViewModel)_CuttedBone.Parent).RemoveBone(_CuttedBone);
                        }
                        ResetCut();
                        BoneViewModel bvm = new BoneViewModel(SkinMesh, pasteBone);
                        var newVM = selected.AddBone(bvm, false);
                        if (newVM != null)
                        {
                            newVM.IsSelected = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Instance.ShowError(ex.Message);
                    }
                    ResetCut();
                }
            }

            e.Handled = true;
        }

        void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetSelectedItem() != null;
            e.Handled = true;
        }
        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            DeleteBone();
            e.Handled = true;
        }
        #endregion

        #region New Bone
        private void Mnu_NewBone_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedItem();
            if (selected != null)
            {
                var newVM = selected.AddBone();
                if (newVM != null)
                {
                    SkinMesh.CreateNewName(newVM);
                    newVM.IsSelected = true;
                }
            }
        }
        #endregion

        #region Skeleton from Clipboard
        private void BtnCreateSkeletonFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Clipboard.ContainsData(DataFormats.Text))
            {
                string text = Clipboard.GetText();
                if (text != null)
                {
                    try
                    {
                        XDocument document = XDocument.Parse(text);
                        Bone bone = new Bone();
                        bone.Load(document.Elements().First());

                        BoneViewModel newBvm = new BoneViewModel(SkinMesh, bone);
                        SkinMesh.ReplaceRoot(newBvm);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        #endregion       

        #region Animation from Clipboard
        private void BtnAddClipsFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Clipboard.ContainsData(DataFormats.Text))
            {
                string text = Clipboard.GetText();
                if (text != null)
                {
                    try
                    {
                        List<AnimationClipViewModel> animations = new List<AnimationClipViewModel>();
                        XDocument document = XDocument.Parse(text);
                        XElement animationsElemet = document.Elements().First();
                        if (animationsElemet.Name == "Animations")
                        {
                            foreach (XElement element in animationsElemet.Elements())
                            {
                                AnimationClip clip = new AnimationClip();
                                clip.Load(element);
                                if (SkinMesh.Animations.Count(a => a.Name == clip.Name) == 0)
                                    animations.Add(new AnimationClipViewModel(SkinMesh, clip));
                            }
                            if (animations.Count > 0)
                            {
                                SkinMesh.AddAnimations(animations.ToArray());
                            }
                            else
                                System.Windows.MessageBox.Show("no animation");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        #endregion

        #region Add & Remove AnimationClip
        private void BtnAddClip_Click(object sender, RoutedEventArgs e)
        {
            AnimationClipViewModel clipVM = new AnimationClipViewModel(this.SkinMesh, new AnimationClip() { Name = "Animation", WrapMode = WrapMode.Default });
            SkinMesh.Animations.Add(clipVM);
            History.Insert(new AddAnimationClipUnDoRedo(clipVM, SkinMesh, SkinMesh.Animations.Count - 1));
        }

        private void BtnRemoveClip_Click(object sender, RoutedEventArgs e)
        {
            if (_LbAnimations.SelectedItem != null)
            {
                AnimationClipViewModel clipVM = _LbAnimations.SelectedItem as AnimationClipViewModel;
                if (clipVM != null)
                {
                    int index = SkinMesh.Animations.IndexOf(clipVM);
                    SkinMesh.Animations.Remove(clipVM);
                    History.Insert(new AddAnimationClipUnDoRedo(clipVM, SkinMesh, index, true));
                }
            }
        }

        private void LbAnimations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CanAddClip = _LbAnimations.SelectedItem != null;
            ApplicationCommands.Properties.Execute(_LbAnimations.SelectedItem, null);
        }


        private void BtnClearClip_Click(object sender, RoutedEventArgs e)
        {
            AnimationClipViewModel[] anims = SkinMesh.Animations.ToArray();
            SkinMesh.Animations.Clear();

            if (anims != null && anims.Length > 0)
                History.Insert(new AddAnimationsUnDoRedo(anims, SkinMesh, true));

        }

        #endregion
    }
}
