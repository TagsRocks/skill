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
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation.Editor
{
        
    public class AnimationTreeRootProperties : AnimNodeProperties
    {
        public AnimationTreeRootProperties(AnimationTreeRootViewModel viewmodel)
            : base(viewmodel)
        {

        }

        //[Description("Profiles in c# string format. contains atleast one {0} to replace with AnimNodeSequence AnimationName")]
        //public List<string> Profiles { get { return ((AnimationTreeRoot)ViewModel.Model).Profiles; } }

        [Browsable(false)]
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        [Description("Edit profiles")]
        [System.ComponentModel.Editor(typeof(EditProfilesButton), typeof(EditProfilesButton))]
        public ObservableCollection<AnimationTreeProfileViewModel> Profiles
        {
            get
            {                
                return ((AnimationTreeRootViewModel)ViewModel).Profiles;
            }
        }
    }

    /// <summary>
    /// Interaction logic for AnimationTreeRoot.xaml
    /// </summary>
    public partial class AnimationTreeRootViewModel : AnimNodeViewModel
    {
        private ObservableCollection<AnimationTreeProfileViewModel> _Profiles;
        public ObservableCollection<AnimationTreeProfileViewModel> Profiles { get { return _Profiles; } }

        public AnimationTreeRootViewModel()
            : this(new AnimationTreeRoot())
        {
        }
        public AnimationTreeRootViewModel(AnimationTreeRoot model)
            : base(model)
        {
            InitializeComponent();
            this._Profiles = new ObservableCollection<AnimationTreeProfileViewModel>();
            foreach (var item in model.Profiles)
            {
                this.Profiles.Add(new AnimationTreeProfileViewModel(_Profiles, item));
            }
            this.Properties = new AnimationTreeRootProperties(this);
        }

        public override void CommiteChangesToModel()
        {
            base.CommiteChangesToModel();
            AnimationTreeProfile[] profiles = new AnimationTreeProfile[_Profiles.Count];
            for (int i = 0; i < _Profiles.Count; i++)
            {
                profiles[i] = _Profiles[i].Model;
            }
            ((AnimationTreeRoot)Model).Profiles = profiles;
        }
    }
}
