using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Skill.Studio.Validation
{
    /// <summary>
    /// Defines validation for a directory
    /// </summary>
    class LocationValidator : ValidationRule
    {
        /// <summary>
        /// Whether given location can be a valid directory
        /// </summary>
        /// <param name="location">address of directry to check</param>
        /// <returns>True if valid, otherwise false</returns>
        public static bool IsValid(string location)
        {
            bool valid = true;
            try
            {
                System.IO.Path.GetFullPath(location);
            }
            catch
            {
                valid = false;
            }
            return valid;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null || !IsValid(value.ToString()))
            {
                return new ValidationResult(false, "The value is not a valid location");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
