using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Skill.Editor.Validation
{
    /// <summary>
    /// Defines a validator for name of variables (behaviors, animations , ...)
    /// </summary>
    class VariableNameValidator : ValidationRule
    {
        /// <summary>
        /// Check whether given name is valid
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <returns>True if valid, otherwise false</returns>
        public static bool IsValid(string name)
        {
            if (name == null) return false;
            return _Regex.Match(name).Success;
        }

        /// <summary> Regular expression to check names </summary>
        private static Regex _Regex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.IgnoreCase);

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null || !_Regex.Match(value.ToString()).Success)
            {
                return new ValidationResult(false, "The value is not a valid name");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
