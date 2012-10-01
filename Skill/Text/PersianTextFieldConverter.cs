using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Text
{
    /// <summary>
    /// Convert a text contains of none persian characters to equivalent persian characters.
    /// </summary>
    /// <remarks>
    /// this version of converter use more calculation to convert text and should use for left to right TextField.
    /// so use this class when your text is dynamic and 
    /// </remarks>
    public class PersianTextFieldConverter : ITextConverter
    {        
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
        private CharInfo[] _RepositionedChars;

        /// <summary>
        /// Create a PersianTextConverter
        /// </summary>
        /// <param name="characterMap">Character mapping information for persian language</param>
        /// <param name="maxLength">Maximum length of text ( for better performance)</param>
        public PersianTextFieldConverter(IPersianCharacterMap characterMap, int maxLength = 100)
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
                this._RepositionedChars = new CharInfo[this.MaxLength];
                for (int i = 0; i < this.MaxLength; i++)
                    this._SourceChars[i] = new CharInfo() { Character = null, Form = PersianCharacterForm.Isolated };
            }
        }


        /// <summary>
        /// Convert specified text to equivalent persian text
        /// </summary>
        /// <param name="text">Text to convert</param>
        /// <returns>Converted text</returns>
        public string Convert(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                LastConvertedText = string.Empty;
            }
            else if (LastConvertedText != null && LastConvertedText.Equals(text, StringComparison.Ordinal))
            {
                return LastConvertedText;
            }
            else
            {
                // make sure that we alocated enough space for text
                EnsureCharSize(text.Length);

                if (LastConvertedText == null) LastConvertedText = string.Empty;
                int oldIndex = 0;

                if (LastConvertedText.Length > text.Length) // characters removed
                {
                    int index = LastConvertedText.IndexOf(text);
                    if (index >= 0)
                    {
                        if (index == 0) // we have to remove from first because text is reversed
                            oldIndex = LastConvertedText.Length - text.Length;
                        text = LastConvertedText.Substring(oldIndex, text.Length);
                    }
                }

                // find maps (if exist)
                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo cf = _SourceChars[i];
                    cf.SourceChar = text[i];
                    if (!CharacterMap.Map.TryGetValue(cf.SourceChar, out cf.Character))
                        cf.Character = null;
                    cf.Form = PersianCharacterForm.Isolated;
                    _RepositionedChars[i] = null;

                    // check for new characters in text
                    if (!RightToLeft && oldIndex < LastConvertedText.Length)
                    {
                        char oldChar = LastConvertedText[oldIndex];
                        if (cf.SourceChar == oldChar || (cf.Character != null && cf.Character.Contains(oldChar)) || cf.SourceChar != ' ')
                        {
                            cf.IsReversed = true;
                            oldIndex++;
                        }
                        else
                            cf.IsReversed = false;
                    }
                    else
                        cf.IsReversed = false;
                }


                // find none persian characters and place them in correct position
                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo cf = _SourceChars[i];
                    if (!cf.IsReversed)
                    {
                        _RepositionedChars[text.Length - i - 1] = cf;
                    }
                }

                // place persian characters ( that we found them in previous text change) in correct position
                int j = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (_RepositionedChars[i] == null)
                    {
                        for (; j < text.Length; j++)
                        {
                            if (_SourceChars[j].IsReversed)
                            {
                                _RepositionedChars[i] = _SourceChars[j];
                                j++;
                                break;
                            }
                        }
                    }
                }



                // calc forms of each character
                for (int i = 0; i < text.Length; i++)
                {
                    PersianCharacter prePc = null;
                    PersianCharacter nextPc = null;
                    PersianCharacterForm form = PersianCharacterForm.Isolated;

                    if (i > 0) nextPc = _RepositionedChars[i - 1].Character;
                    if (i < text.Length - 1) prePc = _RepositionedChars[i + 1].Character;

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

                    _RepositionedChars[i].Form = form;
                }

                // build text from end to start
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo cf = _RepositionedChars[i];
                    if (cf.Character != null)
                        result.Append(cf.Character[cf.Form]);
                    else
                        result.Append(cf.SourceChar);
                }

                LastConvertedText = result.ToString();
            }
            return LastConvertedText;
        }
    }
}
