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
            CheckData();
            return _Converter.Convert(text);
        }
    }
}
