using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Skill.Studio.Themes
{
    public static class ThemeManager
    {
        private static ResourceDictionary _CurrentTheme = null;

        public static ResourceDictionary GetThemeResourceDictionary(string theme)
        {
            if (!string.IsNullOrEmpty(theme))
            {                
                string packUri = String.Format("/Themes/{0}.xaml", theme);
                return Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
            }
            return null;
        }

        public static string[] GetThemes()
        {
            string[] themes = new string[] 
            { 
                "Default",
            };
            return themes;
        }

        public static void ApplyTheme(this Application app, string theme)
        {
            ResourceDictionary dictionary = ThemeManager.GetThemeResourceDictionary(theme);
            if (dictionary != null)
            {
                if (_CurrentTheme != null)
                    app.Resources.MergedDictionaries.Remove(_CurrentTheme);
                app.Resources.MergedDictionaries.Add(dictionary);
                _CurrentTheme = dictionary;
            }
        }

        public static void ApplyTheme(this ContentControl control, string theme)
        {
            ResourceDictionary dictionary = ThemeManager.GetThemeResourceDictionary(theme);

            if (dictionary != null)
            {
                if (_CurrentTheme != null)
                    control.Resources.MergedDictionaries.Remove(_CurrentTheme);
                control.Resources.MergedDictionaries.Add(dictionary);
                _CurrentTheme = dictionary;
            }
        }

        #region Theme

        /// <summary>
        /// Theme Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.RegisterAttached("Theme", typeof(string), typeof(ThemeManager),
                new FrameworkPropertyMetadata((string)string.Empty,
                    new PropertyChangedCallback(OnThemeChanged)));

        /// <summary>
        /// Gets the Theme property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static string GetTheme(DependencyObject d)
        {
            return (string)d.GetValue(ThemeProperty);
        }

        /// <summary>
        /// Sets the Theme property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetTheme(DependencyObject d, string value)
        {
            d.SetValue(ThemeProperty, value);
        }

        /// <summary>
        /// Handles changes to the Theme property.
        /// </summary>
        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string theme = e.NewValue as string;
            if (theme == string.Empty)
                return;

            ContentControl control = d as ContentControl;
            if (control != null)
            {
                control.ApplyTheme(theme);
            }
        }

        #endregion



    }
}
