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

            if (e.Args != null && e.Args.Length > 0)            
            {
                foreach (var arg in e.Args)
                {
                    if (!string.IsNullOrEmpty(arg))
                    {
                        if (System.IO.File.Exists(arg))
                        {                            
                            if (System.IO.Path.GetExtension(arg) == Project.Extension)
                            {
                                Skill.Editor.MainWindow.ProjectAddressToOpen = arg;
                                break;
                            }
                        }
                    }
                }
            }

            base.OnStartup(e);            
        }
    }
}
