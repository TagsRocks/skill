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
using System.Collections.ObjectModel;

namespace Skill.Studio
{
    /// <summary>
    /// Interaction logic for GifBrowser.xaml
    /// </summary>
    public partial class GifBrowser : Window, INotifyPropertyChanged
    {
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

        /// <summary> local path of selected animation </summary>
        public string Animation { get; private set; }

        public int FrameRate
        {
            get { return _Preview.FrameRate; }
            set
            {
                if (_Preview.FrameRate != value)
                {
                    _Preview.FrameRate = value;
                    OnPropertyChanged("FrameRate");
                }
            }
        }

        #region Constructor

        public GifBrowser()
            : this("")
        {
        }
        public GifBrowser(string animation)
        {
            Sets = new ObservableCollection<string>();
            Animations = new ObservableCollection<string>();
            InitializeComponent();
            LoadAnimationSets();

            if (!string.IsNullOrEmpty(animation))
            {
                string set = System.IO.Path.GetDirectoryName(animation);
                string anim = System.IO.Path.GetFileName(animation);

                int setIndex = Sets.IndexOf(set);
                if (setIndex >= 0)
                {
                    _LbSets.SelectedIndex = setIndex;

                    int animIndex = Animations.IndexOf(anim);
                    if (animIndex >= 0)
                        _LbAnimations.SelectedIndex = animIndex;
                }
            }
        }

        private void LoadAnimationSets()
        {
            string dir = MainWindow.Instance.Project.GifAnimationDirectory;
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            string[] sets = System.IO.Directory.GetDirectories(dir);
            foreach (var s in sets)
                Sets.Add(System.IO.Path.GetFileNameWithoutExtension(s));

            if (Sets.Count > 0)
                _LbSets.SelectedIndex = 0;

        }
        #endregion

        #region Done
        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion


        #region Sets

        public ObservableCollection<string> Sets { get; private set; }

        private string SelectedSet { get { return HasSelectedSet ? Sets[_LbSets.SelectedIndex] : ""; } }

        private bool _HasSelectedSet;
        public bool HasSelectedSet
        {
            get { return _HasSelectedSet; }
            set
            {
                if (_HasSelectedSet != value)
                {
                    _HasSelectedSet = value;
                    OnPropertyChanged("HasSelectedSet");
                }
            }
        }

        private void BtnAddSet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_TxtNewSet.Text))
            {
                System.Windows.MessageBox.Show("Please enter valid name for set.");
                return;
            }
            if (Sets.Contains(_TxtNewSet.Text))
            {
                System.Windows.MessageBox.Show("The set with same name already exist.");
                return;
            }

            string newSetDir = MainWindow.Instance.Project.GetAnimationPath(_TxtNewSet.Text);

            if (!System.IO.Directory.Exists(newSetDir))
                System.IO.Directory.CreateDirectory(newSetDir);

            Sets.Add(_TxtNewSet.Text);
            _LbSets.SelectedIndex = Sets.Count - 1;
        }

        private void BtnRemoveSet_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show(string.Format("Are you sure?\nAll animations of set : {0} will be deleted.", SelectedSet), "Delete Animation Set", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.Yes)
            {
                int index = _LbSets.SelectedIndex;
                try
                {
                    _Preview.Unload();
                    string dir = MainWindow.Instance.Project.GetAnimationPath(Sets[index]);
                    if (System.IO.Directory.Exists(dir))
                        System.IO.Directory.Delete(dir, true);

                    Sets.RemoveAt(index);
                    if (Sets.Count > index)
                        _LbSets.SelectedIndex = index;
                    else
                        _LbSets.SelectedIndex = Sets.Count - 1;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }

        private void LbSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelectedSet = _LbSets.SelectedIndex >= 0;
            if (_LbSets.SelectedIndex >= 0)
                LoadAnimations(SelectedSet);
            else
                Animations.Clear();
        }

        private void LoadAnimations(string setName)
        {
            Animations.Clear();
            string[] gifFiles = System.IO.Directory.GetFiles(MainWindow.Instance.Project.GetAnimationPath(setName), "*.gif", System.IO.SearchOption.TopDirectoryOnly);

            foreach (var g in gifFiles)
                Animations.Add(System.IO.Path.GetFileName(g));

            if (Animations.Count > 0)
                _LbAnimations.SelectedIndex = 0;
        }

        private void LbSets_KeyDown(object sender, KeyEventArgs e)
        {

        }
        #endregion

        #region Animations

        public ObservableCollection<string> Animations { get; private set; }

        private string SelectedAnimation { get { return HasSelectedAnimation ? Animations[_LbAnimations.SelectedIndex] : ""; } }

        private bool _HasSelectedAnimation;
        public bool HasSelectedAnimation
        {
            get { return _HasSelectedAnimation; }
            set
            {
                if (_HasSelectedAnimation != value)
                {
                    _HasSelectedAnimation = value;
                    OnPropertyChanged("HasSelectedAnimation");
                }
            }
        }

        private void BtnAddAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (HasSelectedSet)
            {
                Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
                open.Multiselect = true;
                open.Filter = "Gif Animation|*.gif";
                if (open.ShowDialog() == true)
                {
                    foreach (var item in open.FileNames)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            string filename = System.IO.Path.GetFileName(item);
                            string newPath = MainWindow.Instance.Project.GetAnimationPath(System.IO.Path.Combine(SelectedSet, filename));
                            System.IO.File.Copy(item, newPath, true);
                            if (!Animations.Contains(filename))
                                Animations.Add(filename);
                        }
                    }
                }
            }
        }

        private void BtnRemoveAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (HasSelectedAnimation)
            {
                string anim = SelectedAnimation;
                string filename = MainWindow.Instance.Project.GetAnimationPath(System.IO.Path.Combine(SelectedSet, anim));
                if (System.IO.File.Exists(filename))
                {

                    try
                    {
                        _Preview.Unload();
                        int index = _LbAnimations.SelectedIndex;
                        _LbAnimations.SelectedIndex = index - 1;
                        System.IO.File.Delete(filename);
                        Animations.Remove(anim);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        throw;
                    }

                }
            }
        }

        private void LbAnimations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelectedAnimation = HasSelectedSet && _LbAnimations.SelectedIndex >= 0;
            if (HasSelectedAnimation)
            {
                string localPath = System.IO.Path.Combine(SelectedSet, SelectedAnimation);
                string gifSource = MainWindow.Instance.Project.GetAnimationPath(localPath);
                if (System.IO.File.Exists(gifSource))
                {
                    _Preview.GifSource = gifSource;
                    Animation = localPath;
                }
                else
                {
                    _Preview.GifSource = "";
                    Animation = "";
                }
            }
            else
            {
                _Preview.GifSource = "";
                Animation = "";
            }
        }

        private void LbAnimations_KeyDown(object sender, KeyEventArgs e)
        {

        }
        #endregion

        private void BtnSelectNone_Click(object sender, RoutedEventArgs e)
        {
            _LbSets.SelectedIndex = -1;
            _LbAnimations.SelectedIndex = -1;
        }
    }
}
