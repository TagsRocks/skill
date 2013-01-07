using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.Text
{
    /// <summary>
    /// Convert text to other format(in other language)
    /// </summary>
    public interface ITextConverter
    {
        /// <summary>
        /// Convert specified text
        /// </summary>
        /// <param name="source">Source text to convert</param>
        /// <returns>Converted text</returns>
        string Convert(string source);
    }
}
