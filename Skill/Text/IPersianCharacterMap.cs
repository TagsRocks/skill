using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Text
{
    /// <summary>
    /// Defines character mapping information for persian language
    /// </summary>
    public interface IPersianCharacterMap
    {
        /// <summary>
        /// A dictionary that maps each word to equivalent persian character
        /// </summary>
        Dictionary<char, PersianCharacter> Map { get; }
    }
}
