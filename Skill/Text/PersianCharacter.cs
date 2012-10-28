using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Text
{
    /// <summary>
    /// Defines all forms of a persian character
    /// </summary>
    public class PersianCharacter
    {
        // variables
        private char[] _Characters;

        /// <summary> Gets or sets Character in Initial form </summary>
        public char InitialForm { get { return _Characters[0]; } set { _Characters[0] = value; } }
        /// <summary> Gets or sets Character in Medial form </summary>
        public char MedialForm { get { return _Characters[1]; } set { _Characters[1] = value; } }
        /// <summary> Gets or sets Character in Final form </summary>
        public char FinalForm { get { return _Characters[2]; } set { _Characters[2] = value; } }
        /// <summary> Gets or sets Character in Isolated form </summary>
        public char IsolatedForm { get { return _Characters[3]; } set { _Characters[3] = value; } }

        /// <summary> Whether this persian character can stick to previous character.</summary>
        /// <remarks>
        /// usually four form characters can stick to previous and next character, two form characters can stick to previous and not to next character.
        /// </remarks>
        public bool CanStickToPrevious { get; set; }
        /// <summary> Whether this persian character can stick to next character.</summary>
        /// <remarks>
        /// usually four form characters can stick to previous and next character, two form characters can stick to previous and not to next character.
        /// </remarks>
        public bool CanStickToNext { get; set; }

        /// <summary>
        /// Is this character left to right ( like numerics )
        /// </summary>
        public bool LeftToRight { get; set; }

        /// <summary> Gets or sets Character in specified form </summary>
        /// <param name="form">Form of persian character</param>
        /// <returns>Character in specified form.</returns>
        public char this[PersianCharacterForm form]
        {
            get { return _Characters[(int)form]; }
            set { _Characters[(int)form] = value; }
        }

        /// <summary>
        /// Create a PersianCharacter
        /// </summary>
        /// <param name="initial">Character in Initial form</param>
        /// <param name="medial">Character in Medial form</param>
        /// <param name="final">Character in Final form </param>
        /// <param name="isolated">Character in Isolated form</param>
        /// <param name="canStickToPrevious">can stick to previous character</param>
        /// <param name="canStickToNext">can stick to next character</param>
        public PersianCharacter(char initial, char medial, char final, char isolated, bool canStickToPrevious = true, bool canStickToNext = true)
        {
            this.LeftToRight = false;
            this._Characters = new char[4];
            SetData(initial, medial, final, isolated, canStickToPrevious, canStickToNext);
        }

        /// <summary>
        /// Create a PersianCharacter
        /// </summary>        
        /// <param name="final">Character in Final (also Initial and Medial) form </param>
        /// <param name="isolated">Character in Isolated form</param>
        /// <param name="canStickToPrevious">can stick to previous character</param>
        /// <param name="canStickToNext">can stick to next character</param>
        public PersianCharacter(char final, char isolated, bool canStickToPrevious = true, bool canStickToNext = false)
            : this(isolated, isolated, final, isolated, canStickToPrevious, canStickToNext)
        {

        }

        /// <summary>
        /// Create a PersianCharacter
        /// </summary>                
        /// <param name="isolated">Character in Isolated ( also Initial, Medial and Final) form</param>        
        public PersianCharacter(char isolated)
            : this(isolated, isolated, isolated, isolated, false, false)
        {

        }

        /// <summary>
        /// Set all properties in one call
        /// </summary>
        /// <param name="initial">Character in Initial form</param>
        /// <param name="medial">Character in Medial form</param>
        /// <param name="final">Character in Final form </param>
        /// <param name="isolated">Character in Isolated form</param>
        /// <param name="canStickToPrevious">can stick to previous character</param>
        /// <param name="canStickToNext">can stick to next character</param>
        public void SetData(char initial, char medial, char final, char isolated, bool canStickToPrevious = true, bool canStickToNext = true)
        {
            this._Characters[0] = initial;
            this._Characters[1] = medial;
            this._Characters[2] = final;
            this._Characters[3] = isolated;
            this.CanStickToPrevious = canStickToPrevious;
            this.CanStickToNext = canStickToNext;
        }

        /// <summary>
        /// Set all properties in one call
        /// </summary>        
        /// <param name="final">Character in Final (also Initial and Medial) form </param>
        /// <param name="isolated">Character in Isolated form</param>
        /// <param name="canStickToPrevious">can stick to previous character</param>
        /// <param name="canStickToNext">can stick to next character</param>
        public void SetData(char final, char isolated, bool canStickToPrevious = true, bool canStickToNext = false)
        {
            SetData(isolated, isolated, final, isolated, canStickToPrevious, canStickToNext);
        }

        /// <summary>
        /// Set all properties in one call
        /// </summary>                
        /// <param name="isolated">Character in Isolated ( also Initial, Medial and Final) form</param>  
        public void SetData(char isolated)
        {
            SetData(isolated, isolated, isolated, isolated, false, false);
        }

        /// <summary>
        /// Whether specified character is any form of persian character
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns>True if specified character is one form of persian character, otherwise false.</returns>
        public bool Contains(char c)
        {
            return _Characters[0] == c || _Characters[1] == c || _Characters[2] == c || _Characters[3] == c;
        }
    }
}
