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
using Skill.Studio.Tools.HToMesh.ViewModel;

namespace Skill.Studio.Tools
{
    /// <summary>
    /// Interaction logic for HeightmapToMesh.xaml
    /// </summary>
    public partial class HeightmapToMesh : Window
    {
        private TSWizardViewModel _WizardViewModel;

        public HeightmapToMesh()
        {
            InitializeComponent();
            _WizardViewModel = new TSWizardViewModel();
            _WizardViewModel.RequestClose += this.OnViewModelRequestClose;
            base.DataContext = _WizardViewModel;
        }

        void OnViewModelRequestClose(object sender, EventArgs e)
        {
            base.DialogResult = this._WizardViewModel.Data != null;
        }

        protected override void OnClosed(EventArgs e)
        {
            _WizardViewModel.Dispose();
            base.OnClosed(e);
        }
    }
}
