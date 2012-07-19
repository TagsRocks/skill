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
using System.ComponentModel;

namespace Skill.Studio
{
    /// <summary>
    /// Interaction logic for PluginWindow.xaml
    /// </summary>
    public partial class PluginWindow : Window
    {
        private int _SelectedPlugin;

        public PluginWindow()
        {
            InitializeComponent();

            PluginInfo[] plugins = PluginManager.FindPlugins();
            _SelectedPlugin = 0;
            for (int i = 0; i < plugins.Length; i++)
            {
                _LbPlugins.Items.Add(plugins[i]);
                if (plugins[i].Equals(PluginManager.SelectedPlugin))
                    _SelectedPlugin = i;
            }
            _LbPlugins.SelectedIndex = _SelectedPlugin;

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
        }

        private void Save()
        {
            if (_LbPlugins.SelectedItem != null && _SelectedPlugin != _LbPlugins.SelectedIndex)
            {
                PluginInfo newPlugin = _LbPlugins.SelectedItem as PluginInfo;
                if (newPlugin != null)
                {
                    PluginManager.SelectedPlugin = newPlugin;
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                Save();
                Close();
            }
        }
    }
}
