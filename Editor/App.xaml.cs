using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Skill.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //string theme = Skill.Editor.Properties.Settings.Default.Theme;
            //if (string.IsNullOrEmpty(theme))
            //{
            //    theme = Skill.Editor.Themes.ThemeManager.GetThemes()[0];
            //}
            //Skill.Editor.Themes.ThemeManager.ApplyTheme(this, theme);

            AvalonDock.ThemeFactory.ChangeTheme(new Uri("/AvalonDock.Themes;component/themes/dev2010.xaml", UriKind.RelativeOrAbsolute));
            base.OnStartup(e);
        }
    }
}
