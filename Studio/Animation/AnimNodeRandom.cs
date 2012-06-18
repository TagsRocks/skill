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
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.ComponentModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation
{
    [DisplayName("AnimNodeRandom")]
    public class AnimNodeRandomViewModel : AnimNodeViewModel
    {

        public AnimNodeRandomViewModel(AnimationTreeViewModel tree, AnimNodeRandom model)
            : base(tree, model)
        {
        }

        [Browsable(false)]
        public override string ImageName { get { return Editor.AnimationImages.Random; } }

        [Description("Edit Connectors")]
        [Editor(typeof(Editor.AnimNodeRandomInputsPropertyEditor), typeof(Editor.AnimNodeRandomInputsPropertyEditor))]
        public string Connectors
        {
            get { return "Connectors"; }
        }

        [Browsable(false)]
        public override System.Windows.Media.Brush ContentBrush { get { return Editor.StaticBrushes.AnimRandomContnetBrush; } }
    }
}
