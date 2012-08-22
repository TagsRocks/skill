using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Skill.Studio.AI.Editor
{
   public class BehaviorBorder : Border
    {
        public BehaviorViewModel ViewModel
        {
            get { return (BehaviorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(BehaviorViewModel), typeof(BehaviorBorder), new UIPropertyMetadata(null, ViewModel_Changed));
        

        static void ViewModel_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BehaviorBorder bb = d as BehaviorBorder;
            if (bb != null)
            {
                BehaviorViewModel bvm = e.NewValue as BehaviorViewModel;
                if (bvm != null)
                {
                    bvm.Border = bb;
                }
            }
        }
    }
}
