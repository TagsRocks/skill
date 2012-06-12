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
using System.Collections;
using System.Collections.ObjectModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation
{

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
        public Constraints(float[] constraints)
        {

        }

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

    public class AnimNodeBlendBySpeedViewModel : AnimNodeViewModel
    {
        public AnimNodeBlendBySpeedViewModel(AnimationTreeViewModel tree, AnimNodeBlendBySpeed model)
            : base(tree, model)
        {
        }

        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.BlendBySpeed; } }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimBlendContnetBrush; } }

        [Description("How fast to blend when going up an index.")]
        public float BlendUpTime
        {
            get { return ((AnimNodeBlendBySpeed)Model).BlendUpTime; }
            set
            {
                if (((AnimNodeBlendBySpeed)Model).BlendUpTime != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendUpTime", value, ((AnimNodeBlendBySpeed)Model).BlendUpTime));
                    }
                    ((AnimNodeBlendBySpeed)Model).BlendUpTime = value;
                }
            }

        }

        [Description("How fast to blend when going down an index.")]
        public float BlendDownTime
        {
            get { return ((AnimNodeBlendBySpeed)Model).BlendDownTime; }
            set
            {
                if (((AnimNodeBlendBySpeed)Model).BlendDownTime != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendDownTime", value, ((AnimNodeBlendBySpeed)Model).BlendDownTime));
                    }
                    ((AnimNodeBlendBySpeed)Model).BlendDownTime = value;
                }
            }
        }

        [Description("Time delay before blending up an index.")]
        public float BlendUpDelay
        {
            get { return ((AnimNodeBlendBySpeed)Model).BlendUpDelay; }
            set
            {
                if (((AnimNodeBlendBySpeed)Model).BlendUpDelay != value)
                {

                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendUpDelay", value, ((AnimNodeBlendBySpeed)Model).BlendUpDelay));
                    }
                    ((AnimNodeBlendBySpeed)Model).BlendUpDelay = value;
                }
            }
        }

        [Description("Time delay before blending down an index.")]
        public float BlendDownDelay
        {
            get { return ((AnimNodeBlendBySpeed)Model).BlendDownDelay; }
            set
            {
                if (((AnimNodeBlendBySpeed)Model).BlendDownDelay != value)
                {

                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendDownDelay", value, ((AnimNodeBlendBySpeed)Model).BlendDownDelay));
                    }
                    ((AnimNodeBlendBySpeed)Model).BlendDownDelay = value;
                }
            }
        }

        [Description("Where abouts in the constraint bounds should the blend start blending down.")]
        public float BlendDownPercent
        {
            get { return ((AnimNodeBlendBySpeed)Model).BlendDownPercent; }
            set
            {
                if (((AnimNodeBlendBySpeed)Model).BlendDownPercent != value)
                {

                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendDownPercent", value, ((AnimNodeBlendBySpeed)Model).BlendDownPercent));
                    }
                    ((AnimNodeBlendBySpeed)Model).BlendDownPercent = value;
                }
            }
        }


        //[Description("minimum and maximum value of constraints for each child(index 0 , 1 are constraints for child index 0 and so on ...)")]
        //[System.ComponentModel.Editor(typeof(FloatCollectionEditor), typeof(FloatCollectionEditor))]
        //public Constraints Constraints
        //{
        //    get { return new Constraints(((AnimNodeBlendBySpeed)ViewModel.Model).Constraints); }
        //}

        //[Browsable(false)]
        //public ObservableCollection<FloatContainer> FloatCollection
        //{
        //    get { return Constraints; }
        //}
    }
}
