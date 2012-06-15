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

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for AnimNodeSequenceSelector.xaml
    /// </summary>
    public partial class AnimNodeSequenceClipSelectorControl : UserControl, INotifyPropertyChanged
    {

        private AnimNodeSequenceViewModel _AnimNodeSequence;
        public AnimNodeSequenceViewModel AnimNodeSequence
        {
            get { return _AnimNodeSequence; }
            set
            {
                if (_AnimNodeSequence != value)
                {
                    _AnimNodeSequence = value;
                    InitialData();
                    OnPropertyChanged("AnimNodeSequence");
                }
            }
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
                    if (_SkinMesh != null)
                    {
                        _CmbAnimations.ItemsSource = _SkinMesh.Animations;
                        _TvBones.ItemsSource = new BoneViewModel[] { _SkinMesh.Root };
                    }
                    else
                    {
                        _CmbAnimations.ItemsSource = null;
                        _TvBones.ItemsSource = null;
                    }

                    OnPropertyChanged("SkinMesh");
                }
            }
        }

        private void InitialData()
        {
            if (_AnimNodeSequence != null)
            {
                SkinMesh = _AnimNodeSequence.Tree.SkinMesh;
                bool found = false;
                if (SkinMesh != null)
                {
                    LoadBonesChecked(SkinMesh.Root);
                    for (int i = 0; i < SkinMesh.Animations.Count; i++)
                    {
                        if (SkinMesh.Animations[i].Name == _AnimNodeSequence.AnimationName)
                        {
                            _CmbAnimations.SelectedIndex = i;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    _CmbAnimations.SelectedIndex = -1;
                }
            }
            else
            {
                SkinMesh = null;
            }
        }

        private void LoadBonesChecked(BoneViewModel bone)
        {
            bone.IsChecked = _AnimNodeSequence.MixingTransforms.Contains(bone.Address);
            foreach (BoneViewModel b in bone)
            {
                LoadBonesChecked(b);
            }
        }

        public AnimNodeSequenceClipSelectorControl()
        {
            InitializeComponent();
        }


        private void CmbAnimations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_AnimNodeSequence != null)
            {
                AnimationClipViewModel clip = _CmbAnimations.SelectedItem as AnimationClipViewModel;
                if (clip != null)
                {
                    _AnimNodeSequence.AnimationName = clip.Name;
                    ((Skill.DataModels.Animation.AnimNodeSequence)_AnimNodeSequence.Model).Length = clip.Length;
                }
            }
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

        private void Bone_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (_AnimNodeSequence != null)
            {
                CheckBox checkBox = sender as CheckBox;
                if (checkBox != null)
                {
                    string address = checkBox.Tag as string;
                    if (address != null)
                    {
                        if (checkBox.IsChecked == true)
                        {
                            if (!_AnimNodeSequence.MixingTransforms.Contains(address))
                                _AnimNodeSequence.MixingTransforms.Add(address);
                        }
                        else
                        {
                            _AnimNodeSequence.MixingTransforms.Remove(address);
                        }
                        _AnimNodeSequence.Tree.Editor.SetChanged(true);
                    }
                }
            }
        }


    }
}
