using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Text
{
    /// <summary>
    /// Defines forms that each persian character can take in a word
    /// </summary>
    public enum PersianCharacterForm
    {
        /// <summary> Character is first character of word  </summary>
        Initial = 0,
        /// <summary> Character is between and stick to beside characters in word  </summary>
        Medial = 1,
        /// <summary> Character is end character of word  </summary>
        Final = 2,
        /// <summary> Character is whole word (is alone) </summary>
        Isolated = 3
    }
}
