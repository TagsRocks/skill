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
using System.Xml.Linq;
using System.ComponentModel;

namespace Skill.Editor.Animation
{
    public enum WrapMode
    {
        Default = 0,
        Once = 1,        
        Loop = 2,
        PingPong = 4,
        ClampForever = 8,
    }

    public class AnimationSequence : AnimationNode
    {

        public AnimationSequence()
            : base("AnimationSequence")
        {
            this.Speed = 1;
            this.WrapMode = WrapMode.Default;
            IsPublic = false;
        }

        /// <summary> Whether code generator create a public property for this node</summary>
        public bool IsPublic { get; set; }

        public override AnimationNodeType NodeType { get { return AnimationNodeType.Sequence; } }

        public string AnimationName { get; set; }

        public bool UseProfile { get; set; }

        /// <summary> Speed at which the animation will be played back.Default is 1.0 </summary>        
        public float Speed { get; set; }

        public WrapMode WrapMode { get; set; }

        protected override void WriteAttributes(XElement e)
        {
            XElement data = new XElement("SequenceData");

            data.SetAttributeValue("AnimationName", string.IsNullOrEmpty(this.AnimationName) ? "" : this.AnimationName);
            data.SetAttributeValue("Speed", this.Speed);
            data.SetAttributeValue("WrapMode", (int)this.WrapMode);
            data.SetAttributeValue("IsPublic", this.IsPublic);
            data.SetAttributeValue("UseProfile", this.UseProfile);

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
                this.IsPublic = data.GetAttributeValueAsBoolean("IsPublic", false);
                this.UseProfile = data.GetAttributeValueAsBoolean("UseProfile", true);
            }
            base.ReadAttributes(e);
        }

        public override void CopyFrom(AnimationNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimationSequence sequence = other as AnimationSequence;
            this.AnimationName = sequence.AnimationName;
            this.Speed = sequence.Speed;
            this.WrapMode = sequence.WrapMode;
        }

        public override AnimationNode Clone()
        {
            AnimationSequence node = new AnimationSequence();
            node.CopyFrom(this);
            return node;
        }
    }


    public class AnimationSequenceProperties : AnimationNodeProperties
    {
        public AnimationSequenceProperties(AnimationSequenceViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Description("Name of AnimationClip")]
        public string AnimationName
        {
            get { return ((AnimationSequence)ViewModel.Model).AnimationName; }
            set
            {
                if (((AnimationSequence)ViewModel.Model).AnimationName != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "AnimationName", value, ((AnimationSequence)ViewModel.Model).AnimationName));
                    }
                    ((AnimationSequence)ViewModel.Model).AnimationName = value;
                    ((AnimationSequenceViewModel)ViewModel).AnimationName = value;
                }
            }
        }

        [Description("Speed at which the animation will be played back.Default is 1.0")]
        public float Speed
        {
            get { return ((AnimationSequence)ViewModel.Model).Speed; }
            set
            {
                if (((AnimationSequence)ViewModel.Model).Speed != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Speed", value, ((AnimationSequence)ViewModel.Model).Speed));
                    }
                    ((AnimationSequence)ViewModel.Model).Speed = value;
                }
            }
        }

        [Description("if false, do not take effect by AnimationTree profiles and always use AnimationName")]
        public bool UseProfile
        {
            get { return ((AnimationSequence)ViewModel.Model).UseProfile; }
            set
            {
                if (((AnimationSequence)ViewModel.Model).UseProfile != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "UseProfile", value, ((AnimationSequence)ViewModel.Model).UseProfile));
                    }
                    ((AnimationSequence)ViewModel.Model).UseProfile = value;
                }
            }
        }

        [Description("WrapMode")]
        public WrapMode WrapMode
        {
            get { return ((AnimationSequence)ViewModel.Model).WrapMode; }
            set
            {
                if (((AnimationSequence)ViewModel.Model).WrapMode != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "WrapMode", value, ((AnimationSequence)ViewModel.Model).WrapMode));
                    }
                    ((AnimationSequence)ViewModel.Model).WrapMode = value;
                }
            }
        }

        [Description("Whether code generator create a public property for this node")]
        public bool IsPublic
        {
            get { return ((AnimationSequence)ViewModel.Model).IsPublic; }
            set
            {
                if (((AnimationSequence)ViewModel.Model).IsPublic != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "IsPublic", value, ((AnimationSequence)ViewModel.Model).IsPublic));
                    }
                    ((AnimationSequence)ViewModel.Model).IsPublic = value;
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for AnimationSequence.xaml
    /// </summary>
    public partial class AnimationSequenceViewModel : AnimationNodeViewModel
    {
        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

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


        public AnimationSequenceViewModel()
            : this(new AnimationSequence())
        {
        }

        public AnimationSequenceViewModel(AnimationSequence model)
            : base(model)
        {
            InitializeComponent();
            this.Properties = new AnimationSequenceProperties(this);
            AnimationName = model.AnimationName;
        }


    }
}
