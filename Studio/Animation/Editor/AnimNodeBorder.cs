using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Skill.Studio.Animation.Editor
{
    public class AnimNodeBorder : Border
    {
        public AnimNodeViewModel ViewModel
        {
            get { return (AnimNodeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AnimNodeViewModel), typeof(AnimNodeBorder), new UIPropertyMetadata(null, ViewModel_Changed));

        static void ViewModel_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimNodeBorder ab = d as AnimNodeBorder;
            if (ab != null)
            {
                AnimNodeViewModel an = e.NewValue as AnimNodeViewModel;
                if (an != null)
                {
                    an.Border = ab;
                }
            }
        }
    }
}
