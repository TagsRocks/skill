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
using Skill.Editor.Diagram;
using System.ComponentModel;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace Skill.Editor.Animation
{

    public class AnimationAimOffsetProfile : IXElement, INotifyPropertyChanged
    {
        public static string ElementName = "AimOffsetProfile";

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        string _CenterCenter;
        public string CenterCenter
        {
            get { return _CenterCenter; }
            set
            {
                if (_CenterCenter != value)
                {
                    _CenterCenter = value;
                    OnPropertyChanged("CenterCenter");
                }
            }
        }

        string _CenterUp;
        public string CenterUp
        {
            get { return _CenterUp; }
            set
            {
                if (_CenterUp != value)
                {
                    _CenterUp = value;
                    OnPropertyChanged("CenterUp");
                }
            }
        }

        string _CenterDown;
        public string CenterDown
        {
            get { return _CenterDown; }
            set
            {
                if (_CenterDown != value)
                {
                    _CenterDown = value;
                    OnPropertyChanged("CenterDown");
                }
            }
        }

        string _LeftCenter;
        public string LeftCenter
        {
            get { return _LeftCenter; }
            set
            {
                if (_LeftCenter != value)
                {
                    _LeftCenter = value;
                    OnPropertyChanged("LeftCenter");
                }
            }
        }

        string _LeftUp;
        public string LeftUp
        {
            get { return _LeftUp; }
            set
            {
                if (_LeftUp != value)
                {
                    _LeftUp = value;
                    OnPropertyChanged("LeftUp");
                }
            }
        }

        string _LeftDown;
        public string LeftDown
        {
            get { return _LeftDown; }
            set
            {
                if (_LeftDown != value)
                {
                    _LeftDown = value;
                    OnPropertyChanged("LeftDown");
                }
            }
        }

        string _RightCenter;
        public string RightCenter
        {
            get { return _RightCenter; }
            set
            {
                if (_RightCenter != value)
                {
                    _RightCenter = value;
                    OnPropertyChanged("RightCenter");
                }
            }
        }

        string _RightUp;
        public string RightUp
        {
            get { return _RightUp; }
            set
            {
                if (_RightUp != value)
                {
                    _RightUp = value;
                    OnPropertyChanged("RightUp");
                }
            }
        }

        string _RightDown;
        public string RightDown
        {
            get { return _RightDown; }
            set
            {
                if (_RightDown != value)
                {
                    _RightDown = value;
                    OnPropertyChanged("RightDown");
                }
            }
        }


        public XElement ToXElement()
        {
            XElement e = new XElement(ElementName);
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("CenterCenter", CenterCenter);
            e.SetAttributeValue("CenterUp", CenterUp);
            e.SetAttributeValue("CenterDown", CenterDown);
            e.SetAttributeValue("LeftCenter", LeftCenter);
            e.SetAttributeValue("LeftUp", LeftUp);
            e.SetAttributeValue("LeftDown", LeftDown);
            e.SetAttributeValue("RightCenter", RightCenter);
            e.SetAttributeValue("RightUp", RightUp);
            e.SetAttributeValue("RightDown", RightDown);
            return e;
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "");
            this.CenterCenter = e.GetAttributeValueAsString("CenterCenter", "");
            this.CenterUp = e.GetAttributeValueAsString("CenterUp", "");
            this.CenterDown = e.GetAttributeValueAsString("CenterDown", "");
            this.LeftCenter = e.GetAttributeValueAsString("LeftCenter", "");
            this.LeftUp = e.GetAttributeValueAsString("LeftUp", "");
            this.LeftDown = e.GetAttributeValueAsString("LeftDown", "");
            this.RightCenter = e.GetAttributeValueAsString("RightCenter", "");
            this.RightUp = e.GetAttributeValueAsString("RightUp", "");
            this.RightDown = e.GetAttributeValueAsString("RightDown", "");
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

        public AnimationAimOffsetProfile Clone()
        {
            return new AnimationAimOffsetProfile()
            {
                _CenterCenter = this._CenterCenter,
                _CenterDown = this._CenterDown,
                _CenterUp = this._CenterUp,
                _LeftCenter = this._LeftCenter,
                _LeftDown = this._LeftDown,
                _LeftUp = this._LeftUp,
                _Name = this._Name,
                _RightCenter = this._RightCenter,
                _RightDown = this._RightDown,
                _RightUp = this._RightUp,
            };
        }
    }


    public class AnimationAimOffset : AnimationNode
    {
        public override AnimationNodeType NodeType { get { return AnimationNodeType.AimOffset; } }

        public bool ProfileChanged { get; set; }

        private AnimationAimOffsetProfile[] _Profiles;
        public AnimationAimOffsetProfile[] Profiles
        {
            get
            {
                if (this._Profiles == null)
                    this._Profiles = new AnimationAimOffsetProfile[0];
                return this._Profiles;
            }
            set
            {
                this._Profiles = value;
            }
        }

        public AnimationAimOffset()
            : base("AnimationAimOffset")
        {
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("AimOffsetData");
            if (data != null)
            {
                this.ProfileChanged = data.GetAttributeValueAsBoolean("ProfileChanged", false);

                XElement profiles = data.FindChildByName("Profiles");
                if (profiles != null)
                {
                    int count = profiles.GetAttributeValueAsInt("Count", 0);
                    this.Profiles = new AnimationAimOffsetProfile[count];
                    int i = 0;
                    foreach (var item in profiles.Elements().Where(p => p.Name == AnimationAimOffsetProfile.ElementName))
                    {
                        this.Profiles[i] = new AnimationAimOffsetProfile();
                        this.Profiles[i].Load(item);
                        i++;
                    }
                }
            }
            base.ReadAttributes(e);
        }
        protected override void WriteAttributes(XElement e)
        {
            XElement data = new XElement("AimOffsetData");

            data.SetAttributeValue("ProfileChanged", this.ProfileChanged);

            if (this.Profiles != null)
            {
                XElement profiles = new XElement("Profiles");

                profiles.SetAttributeValue("Count", this.Profiles.Length);
                for (int i = 0; i < this.Profiles.Length; i++)
                {
                    profiles.Add(this.Profiles[i].ToXElement());
                }
                data.Add(profiles);
            }
            e.Add(data);
            base.WriteAttributes(e);
        }

        public override AnimationNode Clone()
        {
            AnimationAimOffset node = new AnimationAimOffset();
            node.CopyFrom(this);
            return node;
        }

        public override void CopyFrom(AnimationNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimationAimOffset offset = other as AnimationAimOffset;
            this.ProfileChanged = offset.ProfileChanged;
            this.Profiles = new AnimationAimOffsetProfile[offset.Profiles.Length];
            for (int i = 0; i < offset.Profiles.Length; i++)
            {
                this.Profiles[i] = offset.Profiles[i].Clone();
            }
        }
    }

    public class AnimationAimOffsetProperties : AnimationNodeProperties
    {
        public AnimationAimOffsetProperties(AnimationAimOffsetViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Category("Events")]
        [Description("If true code generator create an method and hook it to ProfileChanged event")]
        public bool ProfileChanged
        {
            get { return ((AnimationAimOffset)ViewModel.Model).ProfileChanged; }
            set
            {
                if (((AnimationAimOffset)ViewModel.Model).ProfileChanged != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ProfileChanged", value, ((AnimationAimOffset)ViewModel.Model).ProfileChanged));
                    }
                    ((AnimationAimOffset)ViewModel.Model).ProfileChanged = value;
                }
            }
        }

        [Description("Edit profiles( like 'Rifle', 'Pistol', ...)")]
        [System.ComponentModel.Editor(typeof(EditProfilesButton), typeof(EditProfilesButton))]
        public ObservableCollection<AnimationAimOffsetProfile> Profiles
        {
            get
            {
                if (((AnimationAimOffsetViewModel)ViewModel).Profiles.Count == 0)
                {
                    if (((AnimationAimOffset)ViewModel.Model).Profiles != null && ((AnimationAimOffset)ViewModel.Model).Profiles.Length > 0)
                    {
                        for (int i = 0; i < ((AnimationAimOffset)ViewModel.Model).Profiles.Length; i++)
                        {
                            ((AnimationAimOffsetViewModel)ViewModel).Profiles.Add(((AnimationAimOffset)ViewModel.Model).Profiles[i]);
                        }
                    }
                }

                return ((AnimationAimOffsetViewModel)ViewModel).Profiles;
            }
        }
    }

    /// <summary>
    /// Interaction logic for AnimationAimOffset.xaml
    /// </summary>
    public partial class AnimationAimOffsetViewModel : AnimationNodeViewModel
    {
        public ObservableCollection<AnimationAimOffsetProfile> Profiles { get; private set; }

        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        public AnimationAimOffsetViewModel()
            : this(new AnimationAimOffset())
        {
        }
        public AnimationAimOffsetViewModel(AnimationAimOffset model)
            : base(model)
        {
            InitializeComponent();
            Profiles = new ObservableCollection<AnimationAimOffsetProfile>();
            if (model.Profiles != null)
                for (int i = 0; i < model.Profiles.Length; i++)
                {
                    Profiles.Add(model.Profiles[i]);
                }
            this.Properties = new AnimationAimOffsetProperties(this);
        }

        public override void CommiteChangesToModel()
        {
            ((AnimationAimOffset)Model).Profiles = this.Profiles.ToArray();
            base.CommiteChangesToModel();
        }
    }
}
