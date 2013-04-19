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

namespace Skill.Studio.Controls
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class PropertyGrid : UserControl
    {
        public PropertyGrid()
        {
            InitializeComponent();
        }

        public void SetViewModel(object viewModel)
        {
            _PropertyGrid.SelectedObject = viewModel;
        }
    }
}
