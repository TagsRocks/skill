using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Text
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
        /// if your TextField is right to left set this parameter to true
        /// </summary>
        /// <remarks>
        /// Unity currently does not support right to left TextField, so to convert text in LTR format to RTL persian format
        /// we have to reverse text.
        /// </remarks>
        public bool RightToLeft { get; private set; }

        /// <summary>
        /// Whether convert لا and الله to one equivalent character. (default true)
        /// </summary>
        public bool ConvertLigature { get; set; }

        private CharInfo[] _SourceChars;
        private List<Ligature> _Ligatures;

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

            this.ConvertLigature = true;
            this.RightToLeft = false;
            this.MaxLength = 0;
            EnsureCharSize(Math.Max(10, maxLength));
            this._Ligatures = new List<Ligature>();
            AddDefaultLigatures();
        }

        private void AddDefaultLigatures()
        {
            AddLigature("\uFE8E\uFEDF", "\uFEFB");
            AddLigature("\uFE8E\uFEE0", "\uFEFC");
            AddLigature("\uFEEA\uFEE0\uFEDF\uFE8D", "\uFDF2");
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
                return string.Empty;
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
                    cf.Character = CharacterMap.GetMappedCharacter(cf.SourceChar);
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
                if (!RightToLeft)
                {
                    Reverse(_SourceChars, 0, text.Length - 1);

                    int invalidIndex = -1000;
                    int firstLTRIndex = invalidIndex;

                    for (int i = 0; i < text.Length; i++)
                    {
                        CharInfo cf = _SourceChars[i];
                        if (cf.Character == null || cf.Character.LeftToRight || cf.SourceChar == ' ')
                        {
                            if (firstLTRIndex == invalidIndex && cf.SourceChar != ' ')
                                firstLTRIndex = i;
                        }
                        else
                        {
                            if (firstLTRIndex != invalidIndex)
                            {
                                int spaceCount = 0;
                                for (int k = i - 1; k >= 0; k--)
                                {
                                    if (_SourceChars[k].SourceChar == ' ')
                                        spaceCount++;
                                    else
                                        break;
                                }

                                Reverse(_SourceChars, firstLTRIndex, i - 1 - spaceCount);
                                firstLTRIndex = invalidIndex;
                            }
                        }
                    }

                    if (firstLTRIndex != invalidIndex)
                    {
                        Reverse(_SourceChars, firstLTRIndex, text.Length - 1);
                        firstLTRIndex = invalidIndex;
                    }
                }

                StringBuilder result = new StringBuilder();
                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo cf = _SourceChars[i];
                    if (cf != null && cf.Character != null)
                        result.Append(cf.Character[cf.Form]);
                    else
                        result.Append(cf.SourceChar);
                }

                if (ConvertLigature)
                {
                    foreach (var li in _Ligatures)
                        result.Replace(li.Source, li.Replace);
                }
                return result.ToString();
            }
        }

        private void Reverse(CharInfo[] chars, int startIndex, int endIndex)
        {
            while (startIndex < endIndex)
            {
                var temp = chars[startIndex];
                chars[startIndex] = chars[endIndex];
                chars[endIndex] = temp;

                startIndex++;
                endIndex--;
            }
        }


        public void AddLigature(string source, string replace)
        {
            _Ligatures.Add(new Ligature { Source = source, Replace = replace });
        }

        class Ligature
        {
            public string Source { get; set; }
            public string Replace { get; set; }
        }
    }
}