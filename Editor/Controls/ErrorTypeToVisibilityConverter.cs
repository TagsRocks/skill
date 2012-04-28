using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Skill.Editor.Controls
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class ErrorTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ErrorType)
            {
                ErrorType et = ErrorType.Error;
                if (parameter != null)
                    Enum.TryParse<ErrorType>(parameter.ToString(), out et);

                return ((ErrorType)value == et) ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
