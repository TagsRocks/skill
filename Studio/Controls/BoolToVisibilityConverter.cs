using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Skill.Studio.Controls
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool inverse = false;
                if (parameter != null)
                    bool.TryParse(parameter.ToString(), out inverse);

                if (inverse)
                    return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
                else
                    return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;

            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
