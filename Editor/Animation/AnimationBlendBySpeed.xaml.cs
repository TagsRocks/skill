﻿using System;
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
using System.Collections;
using System.Collections.ObjectModel;

namespace Skill.Editor.Animation
{

    public class AnimationBlendBySpeed : AnimationChildrenEditable
    {
        public AnimationBlendBySpeed()
            : base("AnimationBlendBySpeed")
        {
            Constraints = new Constraints();
            this.BlendUpTime = BlendTime;
            this.BlendDownTime = BlendTime;
        }


        public override AnimationNodeType NodeType { get { return AnimationNodeType.BlendBySpeed; } }

        public float BlendUpTime { get; set; }

        public float BlendDownTime { get; set; }

        public float BlendUpDelay { get; set; }

        public float BlendDownDelay { get; set; }

        public Constraints Constraints { get; private set; }

        protected override void WriteAttributes(XElement e)
        {

            XElement data = new XElement("BlendBySpeedData");
            data.SetAttributeValue("BlendUpTime", this.BlendUpTime);
            data.SetAttributeValue("BlendDownTime", this.BlendDownTime);
            data.SetAttributeValue("BlendUpDelay", this.BlendUpDelay);
            data.SetAttributeValue("BlendDownDelay", this.BlendDownDelay);

            XElement constraints = new XElement("Constraints");
            constraints.SetAttributeValue("Count", Constraints.Count);
            foreach (var item in Constraints)
            {
                XElement f = new XElement("Float");
                f.Value = item.Property.ToString();
                constraints.Add(f);
            }
            data.Add(constraints);
            e.Add(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("BlendBySpeedData");
            if (data != null)
            {
                this.BlendUpTime = data.GetAttributeValueAsFloat("BlendUpTime", 0.3f);
                this.BlendDownTime = data.GetAttributeValueAsFloat("BlendDownTime", 0.3f);
                this.BlendUpDelay = data.GetAttributeValueAsFloat("BlendUpDelay", 0.0f);
                this.BlendDownDelay = data.GetAttributeValueAsFloat("BlendDownDelay", 0.0f);

                XElement constraints = data.FindChildByName("Constraints");
                if (constraints != null)
                {
                    int count = constraints.GetAttributeValueAsInt("Count", 0);
                    Constraints.SetCount(count);
                    int i = 0;
                    foreach (var element in constraints.Elements().Where(p => p.Name == "Float"))
                    {
                        Constraints[i].Property = float.Parse(element.Value);
                        i++;
                    }
                }

            }
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimationNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimationBlendBySpeed s = other as AnimationBlendBySpeed;
            this.BlendDownDelay = s.BlendDownDelay;
            this.BlendDownTime = s.BlendDownTime;
            this.BlendUpDelay = s.BlendUpDelay;
            this.BlendUpTime = s.BlendUpTime;
            this.ChildrenNames = s.ChildrenNames.Clone() as string[];
            this.Constraints.SetCount(s.Constraints.Count);
            for (int i = 0; i < s.Constraints.Count; i++)
            {
                this.Constraints[i].Property = s.Constraints[i].Property;
            }
        }

        public override AnimationNode Clone()
        {
            AnimationBlendBySpeed node = new AnimationBlendBySpeed();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimationBlendBySpeedProperties : AnimationChildrenEditableProperties, IFloatCollectionContainer, IAnimConnectorContainer
    {
        public AnimationBlendBySpeedProperties(AnimationBlendBySpeedViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Description("How fast to blend when going up an index.")]
        public float BlendUpTime
        {
            get { return ((AnimationBlendBySpeed)ViewModel.Model).BlendUpTime; }
            set
            {
                if (((AnimationBlendBySpeed)ViewModel.Model).BlendUpTime != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendUpTime", value, ((AnimationBlendBySpeed)ViewModel.Model).BlendUpTime));
                    }
                    ((AnimationBlendBySpeed)ViewModel.Model).BlendUpTime = value;
                }
            }

        }

        [Description("How fast to blend when going down an index.")]
        public float BlendDownTime
        {
            get { return ((AnimationBlendBySpeed)ViewModel.Model).BlendDownTime; }
            set
            {
                if (((AnimationBlendBySpeed)ViewModel.Model).BlendDownTime != value)
                {

                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendDownTime", value, ((AnimationBlendBySpeed)ViewModel.Model).BlendDownTime));
                    }
                    ((AnimationBlendBySpeed)ViewModel.Model).BlendDownTime = value;
                }
            }
        }

        [Description("Time delay before blending up an index.")]
        public float BlendUpDelay
        {
            get { return ((AnimationBlendBySpeed)ViewModel.Model).BlendUpDelay; }
            set
            {
                if (((AnimationBlendBySpeed)ViewModel.Model).BlendUpDelay != value)
                {

                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendUpDelay", value, ((AnimationBlendBySpeed)ViewModel.Model).BlendUpDelay));
                    }
                    ((AnimationBlendBySpeed)ViewModel.Model).BlendUpDelay = value;
                }
            }
        }

        [Description("Time delay before blending down an index.")]
        public float BlendDownDelay
        {
            get { return ((AnimationBlendBySpeed)ViewModel.Model).BlendDownDelay; }
            set
            {
                if (((AnimationBlendBySpeed)ViewModel.Model).BlendDownDelay != value)
                {

                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendDownDelay", value, ((AnimationBlendBySpeed)ViewModel.Model).BlendDownDelay));
                    }
                    ((AnimationBlendBySpeed)ViewModel.Model).BlendDownDelay = value;
                }
            }
        }


        [Description("minimum and maximum value of constraints for each child(index 0 , 1 are constraints for child index 0 and so on ...)")]
        [System.ComponentModel.Editor(typeof(FloatCollectionEditor), typeof(FloatCollectionEditor))]
        public Constraints Constraints
        {
            get { return ((AnimationBlendBySpeed)ViewModel.Model).Constraints; }
        }

        [Browsable(false)]
        public ObservableCollection<FloatContainer> FloatCollection
        {
            get { return Constraints; }
        }
    }

    public class FloatContainer : INotifyPropertyChanged
    {
        private float _Property;
        public float Property
        {
            get { return _Property; }
            set
            {
                if (_Property != value)
                {
                    _Property = value;
                    OnPropertyChanged("Property");
                }
            }
        }

        public FloatContainer(float value)
        {
            Property = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Constraints : ObservableCollection<FloatContainer>
    {
        public Constraints()
        {
            SetCount(3);
        }

        public void SetCount(int count)
        {
            if (count < 3) count = 3;
            float v = 0;
            if (Count > 0)
                v = this[Count - 1].Property + 1;

            if (Count < count)
            {
                while (Count < count)
                {
                    Add(new FloatContainer(v));
                    v++;
                }
            }
            else if (Count > count)
            {
                while (Count > count)
                {
                    int index = Count - 1;
                    RemoveAt(index);
                }
            }

        }
    }


    /// <summary>
    /// Interaction logic for AnimationBlendBySpeed.xaml
    /// </summary>
    public partial class AnimationBlendBySpeedViewModel : AnimationChildrenEditableViewModel
    {

        protected override StackPanel ConnectorPanel { get { return _ConnectorPanel; } }

        public override Skill.Editor.Diagram.Connector Out { get { return _Out; } }

        public override int ChildCount
        {
            get
            {
                return base.ChildCount;
            }
            set
            {
                base.ChildCount = value;
                ((AnimationBlendBySpeed)Model).Constraints.SetCount(base.ChildCount + 1);
            }
        }                

        public AnimationBlendBySpeedViewModel()
            : this(new AnimationBlendBySpeed())
        {
        }

        public AnimationBlendBySpeedViewModel(AnimationBlendBySpeed model)
            : base(model)
        {
            InitializeComponent();
            InitializeChildNames();
            this.Properties = new AnimationBlendBySpeedProperties(this);
        }        
    }
}
