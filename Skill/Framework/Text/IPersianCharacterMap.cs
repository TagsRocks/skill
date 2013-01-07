using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.Text
{
    /// <summary>
    /// Defines character mapping information for persian language
    /// </summary>
    public interface IPersianCharacterMap
    {
        /// <summary>
        /// maps each none persian chracter to equivalent persian character
        /// </summary>
        /// <param name="c">The character to remap to persian character</param>
        /// <returns>if there is a map for given character returns a PersianCharacter, otherwise null.</returns>
        PersianCharacter GetMappedCharacter(char c);
    }
}
