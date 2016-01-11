using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Text
{
    /// <summary>
    /// Default persian text converter
    /// </summary>
    public static class Persian
    {
        /// <summary>
        /// Retrieves singleton instance of PersianTextConverter
        /// </summary>
        /// <returns>PersianTextConverter</returns>
        public static Skill.Framework.Text.PersianTextConverter GeConverterInstance()
        {
            CheckData();
            return _Converter;
        }

        /// <summary>
        /// Retrieves singleton instance of PersianCharacterMap
        /// </summary>
        /// <returns>PersianCharacterMap</returns>
        public static Skill.Framework.Text.PersianCharacterMap GeCharacterMapInstance()
        {
            CheckData();
            return _CharacterMap;
        }

        private static Skill.Framework.Text.PersianTextConverter _Converter;
        private static Skill.Framework.Text.PersianCharacterMap _CharacterMap;

        private static void CheckData()
        {
            if (_Converter == null)
            {
                _CharacterMap = new Skill.Framework.Text.PersianCharacterMap();
                _Converter = new Skill.Framework.Text.PersianTextConverter(_CharacterMap);
            }
        }

        /// <summary>
        /// Convert text to persian
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>converted text to persian</returns>
        public static string Convert(string text)
        {
            if (text.Contains("\n"))
            {
                string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.None);
                StringBuilder builder = new StringBuilder();
                for (int i = lines.Length - 1; i >= 0; i--)
                    builder.AppendLine(lines[i]);

                text = builder.ToString();
            }

            CheckData();
            return _Converter.Convert(text);
        }

        public static string Convert(int i)
        {
            CheckData();
            return _Converter.Convert(i.ToString());
        }
    }
}
