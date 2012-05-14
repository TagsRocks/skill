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
using Skill.Studio.Diagram;
using System.Xml.Linq;
using System.ComponentModel;

namespace Skill.Studio.Animation
{
    public enum WrapMode
    {
        Default = 0,
        Once = 1,
        Loop = 2,
        PingPong = 4,
        ClampForever = 8,
    }

    public class AnimNodeSequence : AnimNode
    {

        public AnimNodeSequence()
            : base("AnimNodeSequence")
        {
            this.Speed = 1;
            this.WrapMode = WrapMode.Default;
            IsPublic = false;
            UseTreeProfile = true;
            Synchronize = false;
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.Sequence; } }

        public string AnimationName { get; set; }

        public bool UseTreeProfile { get; set; }

        public bool Synchronize { get; set; }

        /// <summary> Speed at which the animation will be played back.Default is 1.0 </summary>        
        public float Speed { get; set; }

        public WrapMode WrapMode { get; set; }

        protected override void WriteAttributes(XElement e)
        {
            XElement data = new XElement("SequenceData");

            data.SetAttributeValue("AnimationName", string.IsNullOrEmpty(this.AnimationName) ? "" : this.AnimationName);
            data.SetAttributeValue("Speed", this.Speed);
            data.SetAttributeValue("WrapMode", (int)this.WrapMode);
            data.SetAttributeValue("UseTreeProfile", this.UseTreeProfile);
            data.SetAttributeValue("Synchronize", this.Synchronize);

            e.Add(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("SequenceData");
            if (data != null)
            {
                this.AnimationName = data.GetAttributeValueAsString("AnimationName", "");
                this.Speed = data.GetAttributeValueAsFloat("Speed", 1);
                this.WrapMode = (WrapMode)data.GetAttributeValueAsInt("WrapMode", (int)WrapMode.Default);
                this.UseTreeProfile = data.GetAttributeValueAsBoolean("UseTreeProfile", true);
                this.Synchronize = data.GetAttributeValueAsBoolean("Synchronize", false);
            }
            base.ReadAttributes(e);
        }

        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeSequence sequence = other as AnimNodeSequence;
            this.AnimationName = sequence.AnimationName;
            this.Speed = sequence.Speed;
            this.WrapMode = sequence.WrapMode;
            this.UseTreeProfile = sequence.UseTreeProfile;
        }

        public override AnimNode Clone()
        {
            AnimNodeSequence node = new AnimNodeSequence();
            node.CopyFrom(this);
            return node;
        }
    }


    public class AnimNodeSequenceProperties : AnimNodeProperties
    {
        public AnimNodeSequenceProperties(AnimNodeSequenceViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Description("Name of AnimationClip")]
        public string AnimationName
        {
            get { return ((AnimNodeSequence)ViewModel.Model).AnimationName; }
            set
            {
                if (((AnimNodeSequence)ViewModel.Model).AnimationName != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "AnimationName", value, ((AnimNodeSequence)ViewModel.Model).AnimationName));
                    }
                    ((AnimNodeSequence)ViewModel.Model).AnimationName = value;
                    ((AnimNodeSequenceViewModel)ViewModel).AnimationName = value;
                }
            }
        }

        [Description("Speed at which the animation will be played back.Default is 1.0")]
        public float Speed
        {
            get { return ((AnimNodeSequence)ViewModel.Model).Speed; }
            set
            {
                if (((AnimNodeSequence)ViewModel.Model).Speed != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Speed", value, ((AnimNodeSequence)ViewModel.Model).Speed));
                    }
                    ((AnimNodeSequence)ViewModel.Model).Speed = value;
                }
            }
        }

        [Description("if false, do not take effect by AnimationTree profiles and always use AnimationName")]
        public bool UseTreeProfile
        {
            get { return ((AnimNodeSequence)ViewModel.Model).UseTreeProfile; }
            set
            {
                if (((AnimNodeSequence)ViewModel.Model).UseTreeProfile != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "UseTreeProfile", value, ((AnimNodeSequence)ViewModel.Model).UseTreeProfile));
                    }
                    ((AnimNodeSequence)ViewModel.Model).UseTreeProfile = value;
                }
            }
        }

        [Description("Synchronize animations with other animations in same Layer?")]
        public bool Synchronize
        {
            get { return ((AnimNodeSequence)ViewModel.Model).Synchronize; }
            set
            {
                if (((AnimNodeSequence)ViewModel.Model).Synchronize != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Synchronize", value, ((AnimNodeSequence)ViewModel.Model).Synchronize));
                    }
                    ((AnimNodeSequence)ViewModel.Model).Synchronize = value;
                }
            }
        }

        [Browsable(false)]
        public override float BlendTime
        {
            get
            {
                return base.BlendTime;
            }
            set
            {
                base.BlendTime = value;
            }
        }

        [Description("WrapMode")]
        public WrapMode WrapMode
        {
            get { return ((AnimNodeSequence)ViewModel.Model).WrapMode; }
            set
            {
                if (((AnimNodeSequence)ViewModel.Model).WrapMode != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "WrapMode", value, ((AnimNodeSequence)ViewModel.Model).WrapMode));
                    }
                    ((AnimNodeSequence)ViewModel.Model).WrapMode = value;
                }
            }
        }

        [Description("Whether code generator create a public property for this node")]
        public bool IsPublic
        {
            get { return ((AnimNodeSequence)ViewModel.Model).IsPublic; }
            set
            {
                if (((AnimNodeSequence)ViewModel.Model).IsPublic != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "IsPublic", value, ((AnimNodeSequence)ViewModel.Model).IsPublic));
                    }
                    ((AnimNodeSequence)ViewModel.Model).IsPublic = value;
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeSequence.xaml
    /// </summary>
    public partial class AnimNodeSequenceViewModel : AnimNodeViewModel
    {
        public override Skill.Studio.Diagram.Connector Out { get { return _Out; } }

        private string _AnimationName;
        public string AnimationName
        {
            get
            {
                return _AnimationName;
            }
            set
            {
                if (_AnimationName != value)
                {
                    _AnimationName = value;
                    OnPropertyChanged("AnimationName");
                }
            }
        }


        public AnimNodeSequenceViewModel()
            : this(new AnimNodeSequence())
        {
        }

        public AnimNodeSequenceViewModel(AnimNodeSequence model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimNodeSequenceProperties(this);
            AnimationName = model.AnimationName;
        }


    }
}
