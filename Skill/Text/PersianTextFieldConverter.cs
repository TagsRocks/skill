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
    /// so use this class when your text is dynamic.
    /// As unity does not support right to left TextField(yet), this algorithm is not perfect and have some bugs. if you search, you can find more persian algorithms
    /// each has it's own cons and pros. it is on you to use this method.
    /// in this algorithm you can write text from begin to end without problem, but if you want to edit text and add some character in middle it goes wrong.
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
        /// Gets last converted text by this converter and set it when you want to continue editing text.
        /// for example when load profile name and you want edit it again.
        /// </summary>
        public string LastConvertedText { get; set; }

        private CharInfo[] _SourceChars;
        private CharInfo[] _RepositionedChars;

        /// <summary>
        /// Whether convert لا and الله to one equivalent character. (default false)
        /// </summary>
        public bool ConvertLigature { get; set; }

        /// <summary>
        /// This is reversed text to save.
        /// </summary>
        /// <remarks>
        /// When we set text of TextField for first time the converter does not know that this text reversed before
        /// so it will reverse it and the result in TextFiled gets wrong.
        /// </remarks>
        public string TextToSave
        {
            get
            {
                if (!string.IsNullOrEmpty(LastConvertedText))
                {
                    StringBuilder builder = new StringBuilder();
                    // reverse text for save
                    for (int i = LastConvertedText.Length - 1; i >= 0; i--)
                    {
                        builder.Append(LastConvertedText[i]);
                    }

                    return builder.ToString();
                }
                else
                    return string.Empty;
            }
        }

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

            this.ConvertLigature = false;
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
                int startoldIndex = 0;
                int lastoldIndex = LastConvertedText.Length - 1;

                if (LastConvertedText.Length == text.Length + 1) // characters removed
                {
                    int index = LastConvertedText.IndexOf(text);
                    if (index >= 0)
                    {
                        if (index == 0)
                        {
                            PersianCharacter pc = CharacterMap.GetMappedCharacter(LastConvertedText[LastConvertedText.Length - 1]);
                            if (pc != null && !pc.LeftToRight)
                            {
                                // we have to remove from first because text is reversed
                                startoldIndex = LastConvertedText.Length - text.Length;
                            }
                            else
                                lastoldIndex = text.Length - 1;
                        }
                        else
                            lastoldIndex = text.Length - 1;
                        text = LastConvertedText.Substring(startoldIndex, text.Length);
                    }
                }

                bool findDiff = false;
                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo cf = _SourceChars[i];
                    cf.SourceChar = text[i];
                    cf.Character = CharacterMap.GetMappedCharacter(cf.SourceChar);                    
                    cf.Form = PersianCharacterForm.Isolated;
                    _RepositionedChars[i] = null;
                    cf.IsReversed = false;

                    // start from begin and search for missmatch
                    if (!findDiff && startoldIndex < LastConvertedText.Length)
                    {
                        if (cf.SourceChar == LastConvertedText[startoldIndex])
                        {
                            startoldIndex++;
                            cf.IsReversed = true;
                        }
                        else
                            findDiff = true;
                    }

                }

                if (findDiff) // if we found missmatch start from last index to find last missmatch
                {
                    for (int i = text.Length - 1; i >= 0; i--)
                    {
                        CharInfo cf = _SourceChars[i];
                        if (lastoldIndex >= startoldIndex)
                        {
                            if (cf.SourceChar == LastConvertedText[lastoldIndex])
                            {
                                lastoldIndex--;
                                cf.IsReversed = true;
                            }
                            else
                                break;
                        }
                    }
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

                // place Left To Right characters to correct place

                // find one left to right character
                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo cf = _RepositionedChars[i];
                    if (!cf.IsReversed && (cf.Character == null || cf.Character.LeftToRight))
                    {
                        // find surrounding ltf chars
                        int startIndex = i;
                        int endIndex = i;

                        //find most previous valid ltf index
                        for (int k = i - 1; k >= 0; k--)
                        {
                            CharInfo cf2 = _RepositionedChars[k];
                            if (cf2.IsReversed && (cf2.Character == null || cf2.Character.LeftToRight))
                            {
                                startIndex = k;
                            }
                            else
                                break;
                        }

                        //find most next valid ltf index
                        for (int k = i + 1; k < text.Length; k++)
                        {
                            CharInfo cf2 = _RepositionedChars[k];
                            if (cf2.IsReversed && (cf2.Character == null || cf2.Character.LeftToRight))
                            {
                                endIndex = k;
                            }
                            else
                                break;
                        }

                        int index = endIndex - (i - startIndex);
                        int sign = Math.Sign(index - i);

                        for (int k = i; k != index; k += sign)
                            _RepositionedChars[k] = _RepositionedChars[k + sign];

                        _RepositionedChars[index] = cf;
                        cf.IsReversed = true;
                    }
                }

                // calc forms of each character
                for (int i = 0; i < text.Length; i++)
                {
                    PersianCharacter currentPc = _RepositionedChars[i].Character;
                    PersianCharacter prePc = null;
                    PersianCharacter nextPc = null;
                    PersianCharacterForm form = PersianCharacterForm.Isolated;

                    if (currentPc != null)
                    {
                        if (i > 0) nextPc = _RepositionedChars[i - 1].Character;
                        if (i < text.Length - 1) prePc = _RepositionedChars[i + 1].Character;

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

                if (ConvertLigature)
                {
                    result.Replace("\uFE8E\uFEDF", "\uFEFB");
                    result.Replace("\uFE8E\uFEE0", "\uFEFC");
                    result.Replace("\uFEEA\uFEE0\uFEDF\uFE8D", "\uFDF2");
                }
                LastConvertedText = result.ToString();
            }
            return LastConvertedText;
        }
    }
}
