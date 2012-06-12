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

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for SharedAccessKeysEditor.xaml
    /// </summary>
    public partial class SharedAccessKeysEditor : TabDocument
    {
        private SharedAccessKeysViewModel _SharedAccessKeys;
        public SharedAccessKeysViewModel SharedAccessKeys
        {
            get { return _SharedAccessKeys; }
            set
            {
                if (_SharedAccessKeys != value)
                {
                    _SharedAccessKeys = value;
                    RaisePropertyChanged("SharedAccessKeys");
                }
            }
        }

        public SharedAccessKeysEditor()
            : this(null)
        {

        }

        public SharedAccessKeysEditor(SharedAccessKeysNodeViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            if (viewModel != null)
            {
                Skill.DataModels.AI.SharedAccessKeys sa = viewModel.LoadData() as Skill.DataModels.AI.SharedAccessKeys;
                if (sa != null)
                    Data = SharedAccessKeys = new SharedAccessKeysViewModel(sa);
            }
        }


    }
}
