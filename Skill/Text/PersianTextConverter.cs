using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Text
{
    /// <summary>
    /// Convert a text contains of none persian characters to equivalent persian characters
    /// </summary>
    public class PersianTextConverter : ITextConverter
    {
        class CharInfo
        {
            public char SourceChar;
            public PersianCharacter Character;
            public PersianCharacterForm Form;
        }


        /// <summary>
        /// IPersianCharacterMap provided for this converter
        /// </summary>
        public IPersianCharacterMap CharacterMap { get; private set; }

        /// <summary>
        /// Maximum length of text ( for better performance)
        /// </summary>
        public int MaxLength { get; private set; }

        /// <summary>
        /// Retrieves last converted text by this converter
        /// </summary>
        public string LastConvertedText { get; private set; }

        /// <summary>
        /// if your TextField is right to left set this parameter to true
        /// </summary>
        /// <remarks>
        /// Unity currently does not support right to left TextField, so to convert text in LTR format to RTL persian format
        /// we have to reverse text.
        /// </remarks>
        public bool RightToLeft { get; private set; }

        private CharInfo[] _SourceChars;

        /// <summary>
        /// Create a PersianTextConverter
        /// </summary>
        /// <param name="characterMap">Character mapping information for persian language</param>
        /// <param name="maxLength">Maximum length of text ( for better performance)</param>
        public PersianTextConverter(IPersianCharacterMap characterMap, int maxLength = 100)
        {
            this.CharacterMap = characterMap;
            if (this.CharacterMap == null)
                throw new ArgumentNullException("Invalid IPersianCharacterMap for PersianTextConverter");

            this.RightToLeft = false;
            this.MaxLength = 0;
            EnsureCharSize(Math.Max(10, maxLength));
        }

        private void EnsureCharSize(int textLength)
        {
            if (this.MaxLength < textLength)
            {
                this.MaxLength = textLength;
                this._SourceChars = new CharInfo[this.MaxLength];
                for (int i = 0; i < this.MaxLength; i++)
                    this._SourceChars[i] = new CharInfo() { Character = null, Form = PersianCharacterForm.Isolated };
            }
        }


        /// <summary>
        /// Convert specified text to equivalent persian text
        /// </summary>
        /// <param name="text">Text to convert</param>
        /// <returns>Converted text</returns>
        public string Converter(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                LastConvertedText = string.Empty;
            }
            else
            {
                // make sure that we alocated enough space for text
                EnsureCharSize(text.Length);

                // find maps (if exist)
                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo cf = _SourceChars[i];
                    cf.SourceChar = text[i];
                    if (!CharacterMap.Map.TryGetValue(cf.SourceChar, out cf.Character))
                        cf.Character = null;
                    cf.Form = PersianCharacterForm.Isolated;
                }

                // calc forms of each character
                for (int i = 0; i < text.Length; i++)
                {
                    PersianCharacter prePc = null;
                    PersianCharacter nextPc = null;
                    PersianCharacterForm form = PersianCharacterForm.Isolated;

                    if (i > 0) prePc = _SourceChars[i - 1].Character;
                    if (i < text.Length - 1) nextPc = _SourceChars[i + 1].Character;

                    if (prePc == null)
                    {
                        if (nextPc != null && nextPc.CanStickToPrevious)
                            form = PersianCharacterForm.Initial;
                    }
                    else if (nextPc == null)
                    {
                        if (prePc != null && prePc.CanStickToNext)
                            form = PersianCharacterForm.Final;
                    }
                    else
                    {
                        if (nextPc.CanStickToPrevious && prePc.CanStickToNext)
                            form = PersianCharacterForm.Medial;
                        else if (prePc.CanStickToNext)
                            form = PersianCharacterForm.Final;
                        else if (nextPc.CanStickToPrevious)
                            form = PersianCharacterForm.Initial;
                    }

                    _SourceChars[i].Form = form;
                }

                // build text from end to start
                StringBuilder result = new StringBuilder();

                if (RightToLeft)
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        CharInfo cf = _SourceChars[i];
                        if (cf != null && cf.Character != null)
                            result.Append(cf.Character[cf.Form]);
                        else
                            result.Append(cf.SourceChar);
                    }
                }
                else // reverse text
                {
                    for (int i = text.Length - 1; i >= 0; i--)
                    {
                        CharInfo cf = _SourceChars[i];
                        if (cf != null && cf.Character != null)
                            result.Append(cf.Character[cf.Form]);
                        else
                            result.Append(cf.SourceChar);
                    }
                }

                LastConvertedText = result.ToString();
            }
            return LastConvertedText;
        }
    }
}
