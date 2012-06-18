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
using System.Windows.Shapes;

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for AnimNodeSequenceClipSelector.xaml
    /// </summary>
    public partial class AnimNodeSequenceClipSelector : Window
    {
        private AnimNodeSequenceViewModel _Node;

        public AnimNodeSequenceClipSelector()
        {
            InitializeComponent();
        }
        public AnimNodeSequenceClipSelector(AnimNodeSequenceViewModel node)
        {
            InitializeComponent();
            this._Node = node;
            _Selector.AnimNodeSequence = this._Node;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _Selector.AnimNodeSequence = null;
            base.OnClosing(e);
        }
    }
}
