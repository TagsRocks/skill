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

namespace Skill.Studio.Animation
{

    

    public class AnimationTreeRoot : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Root; } }

        public AnimationTreeRoot()
            : base("AnimationTree")
        {
            Profiles = new AnimationTreeProfile[0];
        }

        public AnimationTreeProfile[] Profiles { get; set; }

        public override AnimNode Clone()
        {
            AnimationTreeRoot node = new AnimationTreeRoot();
            node.CopyFrom(this);
            return node;
        }

        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimationTreeRoot root = other as AnimationTreeRoot;

            if (root.Profiles != null)
            {
                this.Profiles = new AnimationTreeProfile[root.Profiles.Length];
                for (int i = 0; i < root.Profiles.Length; i++)
                {
                    this.Profiles[i] = new AnimationTreeProfile() { Name = root.Profiles[i].Name, Format = root.Profiles[i].Format };
                }
            }
            else
                Profiles = new AnimationTreeProfile[0];
        }


        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            XElement data = new XElement("RootData");

            XElement profiles = new XElement("Profiles");
            if (Profiles != null)
            {
                profiles.SetAttributeValue("Count", Profiles.Length);
                foreach (var item in Profiles)
                {
                    XElement p = item.ToXElement();
                    profiles.Add(p);
                }
            }
            data.Add(profiles);
            e.Add(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("RootData");
            if (data != null)
            {
                XElement profiles = data.FindChildByName("Profiles");
                if (profiles != null)
                {
                    int count = profiles.GetAttributeValueAsInt("Count", 0);
                    this.Profiles = new AnimationTreeProfile[count];
                    int i = 0;
                    foreach (var element in profiles.Elements().Where(p => p.Name == "Profile"))
                    {
                        AnimationTreeProfile p = new AnimationTreeProfile();
                        p.Load(element);
                        this.Profiles[i++] = p;
                    }
                }
            }
            base.ReadAttributes(e);
        }
    }

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
