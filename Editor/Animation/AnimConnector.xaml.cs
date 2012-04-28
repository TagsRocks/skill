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
using System.ComponentModel;

namespace Skill.Editor.Animation
{

    public class AnimConnectorProperties
    {
        public AnimConnector Connector { get; private set; }

        public string Text
        {
            get { return this.Connector.Text; }
            set { this.Connector.Text = value; }
        }

        public AnimConnectorProperties(AnimConnector connector)
        {
            this.Connector = connector;
        }
    }

    /// <summary>
    /// Interaction logic for AnimConnector.xaml
    /// </summary>
    public partial class AnimConnector : UserControl, INotifyPropertyChanged
    {

        public Skill.Editor.Diagram.Connector Connector { get { return _Connector; } }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public AnimConnectorProperties Properties { get; private set; }



        public AnimConnector()
        {
            InitializeComponent();
            Properties = new AnimConnectorProperties(this);
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void Destroy()
        {
            _Connector.Disconnect();
        }

        #endregion
    }
}
