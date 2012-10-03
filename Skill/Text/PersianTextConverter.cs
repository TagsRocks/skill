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
    /// this version of converter does not care about changes of text and use light calculation to convert text.
    /// use this class when you want convert static texts, or when your TextField is RightToLeft (currently unity does not support RTL TextFields)
    /// </remarks>
    public class PersianTextConverter : ITextConverter
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


        /// <summary>
        /// if converter is LTR This is reversed text to save.
        /// </summary>
        /// <remarks>
        /// When we set text of TextField for first time the converter does not know that this text reversed before
        /// so (if converter is LTR) it will reverse it and the result in TextFiled gets wrong.
        /// </remarks>
        public string TextToSave
        {
            get
            {
                if (!string.IsNullOrEmpty(LastConvertedText))
                {
                    if (RightToLeft) // the text is correct and we do not need to reverse it
                    {
                        return LastConvertedText;
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();
                        // reverse text for save
                        for (int i = LastConvertedText.Length - 1; i >= 0; i--)
                        {
                            builder.Append(LastConvertedText[i]);
                        }
                        return builder.ToString();
                    }
                }
                else
                    return string.Empty;
            }
        }

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
        public string Convert(string text)
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
                    PersianCharacter currentPc = _SourceChars[i].Character;
                    PersianCharacter prePc = null;
                    PersianCharacter nextPc = null;
                    PersianCharacterForm form = PersianCharacterForm.Isolated;

                    if (currentPc != null)
                    {
                        if (i > 0) prePc = _SourceChars[i - 1].Character;
                        if (i < text.Length - 1) nextPc = _SourceChars[i + 1].Character;

                        if (prePc == null)
                        {
                            if (nextPc != null && nextPc.CanStickToPrevious && currentPc.CanStickToNext)
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
                            {
                                if (currentPc.CanStickToNext)
                                    form = PersianCharacterForm.Medial;
                                else
                                    form = PersianCharacterForm.Final;
                            }
                            else if (prePc.CanStickToNext)
                                form = PersianCharacterForm.Final;
                            else if (nextPc.CanStickToPrevious && currentPc.CanStickToNext)
                                form = PersianCharacterForm.Initial;
                        }
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


                string temp = result.ToString();

                temp = temp.Replace("\uFE8E\uFEDF", "\uFEFB");
                temp = temp.Replace("\uFE8E\uFEE0", "\uFEFC");
                temp = temp.Replace("\uFEEA\uFEE0\uFEDF\uFE8D", "\uFDF2");

                LastConvertedText = temp;
            }
            return LastConvertedText;
        }
    }
}