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

namespace Skill.Studio.Animation
{

    public class AnimNodeConnectorProperties
    {
        public AnimNodeConnector Connector { get; private set; }

        public string Text
        {
            get { return this.Connector.Text; }
            set { this.Connector.Text = value; }
        }

        public AnimNodeConnectorProperties(AnimNodeConnector connector)
        {
            this.Connector = connector;
        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeConnector.xaml
    /// </summary>
    public partial class AnimNodeConnector : UserControl, INotifyPropertyChanged
    {

        public Skill.Studio.Diagram.Connector Connector { get { return _Connector; } }

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

        public AnimNodeConnectorProperties Properties { get; private set; }



        public AnimNodeConnector()
        {
            InitializeComponent();
            Properties = new AnimNodeConnectorProperties(this);
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
