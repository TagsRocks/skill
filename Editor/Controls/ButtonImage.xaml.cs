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

namespace Skill.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ButtonImage.xaml
    /// </summary>
    public partial class ButtonImage : UserControl
    {



        public ImageSource NormalImage
        {
            get { return (ImageSource)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NormalImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalImageProperty =
            DependencyProperty.Register("NormalImage", typeof(ImageSource), typeof(ButtonImage), new FrameworkPropertyMetadata(null));



        public ImageSource DisableImage
        {
            get { return (ImageSource)GetValue(DisableImageProperty); }
            set { SetValue(DisableImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisableImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisableImageProperty =
            DependencyProperty.Register("DisableImage", typeof(ImageSource), typeof(ButtonImage), new FrameworkPropertyMetadata(null));





        public ButtonImage()
        {
            InitializeComponent();
        }
    }
}
