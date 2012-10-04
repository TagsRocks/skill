using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Text
{
    /// <summary>
    /// Default PersianCharacterMap 
    /// </summary>
    public class PersianCharacterMap : IPersianCharacterMap
    {
        /// <summary>
        /// A dictionary that maps each word to equivalent persian character
        /// </summary>
        public Dictionary<char, PersianCharacter> Map { get; private set; }

        // persian characters

        /// <summary> الف </summary>
        public PersianCharacter Alef { get; private set; }
        /// <summary> ب </summary>
        public PersianCharacter Beh { get; private set; }
        /// <summary> پ </summary>
        public PersianCharacter Peh { get; private set; }
        /// <summary> ت </summary>
        public PersianCharacter Teh { get; private set; }
        /// <summary> ث </summary>
        public PersianCharacter Theh { get; private set; }
        /// <summary> ج </summary>
        public PersianCharacter Jeem { get; private set; }
        /// <summary> چ </summary>
        public PersianCharacter Cheh { get; private set; }
        /// <summary> ح </summary>
        public PersianCharacter Hah { get; private set; }
        /// <summary> خ </summary>
        public PersianCharacter Khah { get; private set; }
        /// <summary> د </summary>
        public PersianCharacter Dal { get; private set; }
        /// <summary> ذ </summary>
        public PersianCharacter Thal { get; private set; }
        /// <summary> ر </summary>
        public PersianCharacter Reh { get; private set; }
        /// <summary> ز </summary>
        public PersianCharacter Zain { get; private set; }
        /// <summary> ژ </summary>
        public PersianCharacter Jeh { get; private set; }
        /// <summary> س </summary>
        public PersianCharacter Seen { get; private set; }
        /// <summary> ش </summary>
        public PersianCharacter Sheen { get; private set; }
        /// <summary> ص </summary>
        public PersianCharacter Sad { get; private set; }
        /// <summary> ض </summary>
        public PersianCharacter Dad { get; private set; }
        /// <summary> ط </summary>
        public PersianCharacter Tah { get; private set; }
        /// <summary> ظ </summary>
        public PersianCharacter Zah { get; private set; }
        /// <summary> ع </summary>
        public PersianCharacter Ain { get; private set; }
        /// <summary> غ </summary>
        public PersianCharacter Ghain { get; private set; }
        /// <summary> ف </summary>
        public PersianCharacter Feh { get; private set; }
        /// <summary> ق </summary>
        public PersianCharacter Qaf { get; private set; }
        /// <summary> ک </summary>
        public PersianCharacter Kaf { get; private set; }
        /// <summary> گ </summary>
        public PersianCharacter Gaf { get; private set; }
        /// <summary> ل </summary>
        public PersianCharacter Lam { get; private set; }
        /// <summary> م </summary>
        public PersianCharacter Meem { get; private set; }
        /// <summary> ن </summary>
        public PersianCharacter Noon { get; private set; }
        /// <summary> و </summary>
        public PersianCharacter Waw { get; private set; }
        /// <summary> ه </summary>
        public PersianCharacter Heh { get; private set; }
        /// <summary> ی </summary>
        public PersianCharacter Yeh { get; private set; }

        // numeric

        /// <summary> 0 </summary>
        public PersianCharacter Zero { get; private set; }
        /// <summary> 1 </summary>
        public PersianCharacter One { get; private set; }
        /// <summary> 2 </summary>
        public PersianCharacter Two { get; private set; }
        /// <summary> 3 </summary>
        public PersianCharacter Three { get; private set; }
        /// <summary> 4 </summary>
        public PersianCharacter Four { get; private set; }
        /// <summary> 5 </summary>
        public PersianCharacter Five { get; private set; }
        /// <summary> 6 </summary>
        public PersianCharacter Six { get; private set; }
        /// <summary> 7 </summary>
        public PersianCharacter Seven { get; private set; }
        /// <summary> 8 </summary>
        public PersianCharacter Eight { get; private set; }
        /// <summary> 9 </summary>
        public PersianCharacter Nine { get; private set; }

        // other

        /// <summary> اً </summary>
        public PersianCharacter Fathatan { get; private set; }
        /// <summary> اٌ </summary>
        public PersianCharacter Dammatan { get; private set; }
        /// <summary> اٍ </summary>
        public PersianCharacter Kasratan { get; private set; }
        /// <summary> اَ </summary>
        public PersianCharacter Fatha { get; private set; }
        /// <summary> اُ </summary>
        public PersianCharacter Damma { get; private set; }
        /// <summary> اِ </summary>
        public PersianCharacter Kasra { get; private set; }
        /// <summary> اّ </summary>
        public PersianCharacter Shadda { get; private set; }

        /// <summary> ريال </summary>
        public PersianCharacter RialSign { get; private set; }
        /// <summary> ، </summary>
        public PersianCharacter Comma { get; private set; }
        /// <summary> ؛ </summary>
        public PersianCharacter Semicolon { get; private set; }
        /// <summary> آ </summary>
        public PersianCharacter AlefMadda { get; private set; }
        /// <summary> ؤ </summary>
        public PersianCharacter WawWithHamzaAbove { get; private set; }
        /// <summary> أ </summary>
        public PersianCharacter AlefWithHamzaAbove { get; private set; }
        /// <summary> إ </summary>
        public PersianCharacter AlefWithHamzaBelow { get; private set; }
        /// <summary> ء </summary>
        public PersianCharacter Hamza { get; private set; }
        /// <summary> ؟ </summary>
        public PersianCharacter QuestionMark { get; private set; }
        /// <summary> ـ </summary>
        public PersianCharacter Tatweel { get; private set; }
        /// <summary> ( </summary>
        public PersianCharacter LeftParenthesis { get; private set; }
        /// <summary> ) </summary>
        public PersianCharacter RightParenthesis { get; private set; }
        /// <summary> ئ </summary>
        public PersianCharacter YehWithHamzaAbove { get; private set; }

        // Ligature
        /// <summary> لا </summary>
        public PersianCharacter LigatureLam { get; private set; }
        /// <summary> الله </summary>
        public PersianCharacter LigatureAllah { get; private set; }

        private void CreatePersianCharacters()
        {
            // alefba
            this.Alef = new PersianCharacter('\uFE8E', '\uFE8D');
            this.Beh = new PersianCharacter('\uFE91', '\uFE92', '\uFE90', '\uFE8F');
            this.Peh = new PersianCharacter('\uFB58', '\uFB59', '\uFB57', '\uFB56');
            this.Teh = new PersianCharacter('\uFE97', '\uFE98', '\uFE96', '\uFE95');
            this.Theh = new PersianCharacter('\uFE9B', '\uFE9C', '\uFE9A', '\uFE99');
            this.Jeem = new PersianCharacter('\uFE9F', '\uFEA0', '\uFE9E', '\uFE9D');
            this.Cheh = new PersianCharacter('\uFB7C', '\uFB7D', '\uFB7B', '\uFB7A');
            this.Hah = new PersianCharacter('\uFEA3', '\uFEA4', '\uFEA2', '\uFEA1');
            this.Khah = new PersianCharacter('\uFEA7', '\uFEA8', '\uFEA6', '\uFEA5');
            this.Dal = new PersianCharacter('\uFEAA', '\uFEA9');
            this.Thal = new PersianCharacter('\uFEAC', '\uFEAB');
            this.Reh = new PersianCharacter('\uFEAE', '\uFEAD');
            this.Zain = new PersianCharacter('\uFEB0', '\uFEAF');
            this.Jeh = new PersianCharacter('\uFB8B', '\uFB8A');
            this.Seen = new PersianCharacter('\uFEB3', '\uFEB4', '\uFEB2', '\uFEB1');
            this.Sheen = new PersianCharacter('\uFEB7', '\uFEB8', '\uFEB6', '\uFEB5');
            this.Sad = new PersianCharacter('\uFEBB', '\uFEBC', '\uFEBA', '\uFEB9');
            this.Dad = new PersianCharacter('\uFEBF', '\uFEC0', '\uFEBE', '\uFEBD');
            this.Tah = new PersianCharacter('\uFEC3', '\uFEC4', '\uFEC2', '\uFEC1');
            this.Zah = new PersianCharacter('\uFEC7', '\uFEC8', '\uFEC6', '\uFEC5');
            this.Ain = new PersianCharacter('\uFECB', '\uFECC', '\uFECA', '\uFEC9');
            this.Ghain = new PersianCharacter('\uFECF', '\uFED0', '\uFECE', '\uFECD');
            this.Feh = new PersianCharacter('\uFED3', '\uFED4', '\uFED2', '\uFED1');
            this.Qaf = new PersianCharacter('\uFED7', '\uFED8', '\uFED6', '\uFED5');
            this.Kaf = new PersianCharacter('\uFEDB', '\uFEDC', '\uFEDA', '\uFED9');
            this.Gaf = new PersianCharacter('\uFB94', '\uFB95', '\uFB93', '\uFB92');
            this.Lam = new PersianCharacter('\uFEDF', '\uFEE0', '\uFEDE', '\uFEDD');
            this.Meem = new PersianCharacter('\uFEE3', '\uFEE4', '\uFEE2', '\uFEE1');
            this.Noon = new PersianCharacter('\uFEE7', '\uFEE8', '\uFEE6', '\uFEE5');
            this.Waw = new PersianCharacter('\uFEEE', '\uFEED');
            this.Heh = new PersianCharacter('\uFEEB', '\uFEEC', '\uFEEA', '\uFEE9');
            this.Yeh = new PersianCharacter('\uFBFE', '\uFBFF', '\uFBFD', '\uFBFC');

            // numeric
            this.Zero = new PersianCharacter('\u0660');
            this.One = new PersianCharacter('\u0661');
            this.Two = new PersianCharacter('\u0662');
            this.Three = new PersianCharacter('\u0663');
            this.Four = new PersianCharacter('\u0664');
            this.Five = new PersianCharacter('\u0665');
            this.Six = new PersianCharacter('\u0666');
            this.Seven = new PersianCharacter('\u0667');
            this.Eight = new PersianCharacter('\u0668');
            this.Nine = new PersianCharacter('\u0669');

            // other

            this.Fathatan = new PersianCharacter('\u064B');
            this.Dammatan = new PersianCharacter('\u064C');
            this.Kasratan = new PersianCharacter('\u064D');
            this.Fatha = new PersianCharacter('\u064E');
            this.Damma = new PersianCharacter('\u064F');
            this.Kasra = new PersianCharacter('\u0650');
            this.Shadda = new PersianCharacter('\u0651');
            this.RialSign = new PersianCharacter('\uFDFC');
            this.Comma = new PersianCharacter('\u060C');
            this.Semicolon = new PersianCharacter('\u061B');
            this.AlefMadda = new PersianCharacter('\uFE82', '\uFE81');
            this.WawWithHamzaAbove = new PersianCharacter('\uFE86', '\uFE85');
            this.AlefWithHamzaAbove = new PersianCharacter('\uFE84', '\uFE83');
            this.AlefWithHamzaBelow = new PersianCharacter('\uFE88', '\uFE87');
            this.Hamza = new PersianCharacter('\uFE80');
            this.QuestionMark = new PersianCharacter('\u061F');
            this.Tatweel = new PersianCharacter('\u0640', '\u0640', '\u0640', '\u0640');
            this.LeftParenthesis = new PersianCharacter('\uFD3E');
            this.RightParenthesis = new PersianCharacter('\uFD3F');
            this.YehWithHamzaAbove = new PersianCharacter('\uFE8B', '\uFE8C', '\uFE8A', '\uFE89');

            // Ligature
            this.LigatureLam = new PersianCharacter('\uFEFC', '\uFEFB');
            this.LigatureAllah = new PersianCharacter('\uFDF2');

        }

        /// <summary>
        /// Use first set of persian characters in unicode ( '\u0660', '\u0661', ... , '\u0669' )
        /// </summary>
        public void UseFirstNumerics()
        {
            Zero.SetData('\u0660');
            One.SetData('\u0661');
            Two.SetData('\u0662');
            Three.SetData('\u0663');
            Four.SetData('\u0664');
            Five.SetData('\u0665');
            Six.SetData('\u0666');
            Seven.SetData('\u0667');
            Eight.SetData('\u0668');
            Nine.SetData('\u0669');
        }
        /// <summary>
        /// Use second set of persian characters in unicode ( '\u06F0', '\u06F1', ... , '\u06F9' )
        /// </summary>
        public void UseSecondNumerics()
        {
            Zero.SetData('\u06F0');
            One.SetData('\u06F1');
            Two.SetData('\u06F2');
            Three.SetData('\u06F3');
            Four.SetData('\u06F4');
            Five.SetData('\u06F5');
            Six.SetData('\u06F6');
            Seven.SetData('\u06F7');
            Eight.SetData('\u06F8');
            Nine.SetData('\u06F9');
        }

        private void CreateMap(PersianCharacter pc, params char[] keys)
        {
            char c = ' ';
            try
            {

                foreach (var k in keys)
                {
                    c = k;
                    Map.Add(k, pc);
                }
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Key exist " + c);
            }
        }

        private void CreateMaps()
        {
            // create maps for alefba characters
            CreateMap(Alef, '\uFE8E', '\uFE8D', 'h', 'ا');
            CreateMap(Beh, '\uFE91', '\uFE92', '\uFE90', '\uFE8F', 'f', 'ب');
            CreateMap(Peh, '\uFB58', '\uFB59', '\uFB57', '\uFB56', '\\', 'پ');
            CreateMap(Teh, '\uFE97', '\uFE98', '\uFE96', '\uFE95', 'j', 'ت');
            CreateMap(Theh, '\uFE9B', '\uFE9C', '\uFE9A', '\uFE99', 'e', 'ث');
            CreateMap(Jeem, '\uFE9F', '\uFEA0', '\uFE9E', '\uFE9D', '[', 'ج');
            CreateMap(Cheh, '\uFB7C', '\uFB7D', '\uFB7B', '\uFB7A', ']', 'چ');
            CreateMap(Hah, '\uFEA3', '\uFEA4', '\uFEA2', '\uFEA1', 'p', 'ح');
            CreateMap(Khah, '\uFEA7', '\uFEA8', '\uFEA6', '\uFEA5', 'o', 'خ');
            CreateMap(Dal, '\uFEAA', '\uFEA9', 'n', 'د');
            CreateMap(Thal, '\uFEAC', '\uFEAB', 'b', 'ذ');
            CreateMap(Reh, '\uFEAE', '\uFEAD', 'v', 'ر');
            CreateMap(Zain, '\uFEB0', '\uFEAF', 'c', 'ز');
            CreateMap(Jeh, '\uFB8B', '\uFB8A', 'C', 'ژ');
            CreateMap(Seen, '\uFEB3', '\uFEB4', '\uFEB2', '\uFEB1', 's', 'س');
            CreateMap(Sheen, '\uFEB7', '\uFEB8', '\uFEB6', '\uFEB5', 'a', 'ش');
            CreateMap(Sad, '\uFEBB', '\uFEBC', '\uFEBA', '\uFEB9', 'w', 'ص');
            CreateMap(Dad, '\uFEBF', '\uFEC0', '\uFEBE', '\uFEBD', 'q', 'ض');
            CreateMap(Tah, '\uFEC3', '\uFEC4', '\uFEC2', '\uFEC1', 'x', 'ط');
            CreateMap(Zah, '\uFEC7', '\uFEC8', '\uFEC6', '\uFEC5', 'z', 'ظ');
            CreateMap(Ain, '\uFECB', '\uFECC', '\uFECA', '\uFEC9', 'u', 'ع');
            CreateMap(Ghain, '\uFECF', '\uFED0', '\uFECE', '\uFECD', 'y', 'غ');
            CreateMap(Feh, '\uFED3', '\uFED4', '\uFED2', '\uFED1', 't', 'ف');
            CreateMap(Qaf, '\uFED7', '\uFED8', '\uFED6', '\uFED5', 'r', 'ق');
            CreateMap(Kaf, '\uFEDB', '\uFEDC', '\uFEDA', '\uFED9', ';', 'ک', '\u0643');
            CreateMap(Gaf, '\uFB94', '\uFB95', '\uFB93', '\uFB92', '\'', 'گ');
            CreateMap(Lam, '\uFEDF', '\uFEE0', '\uFEDE', '\uFEDD', 'g', 'ل');
            CreateMap(Meem, '\uFEE3', '\uFEE4', '\uFEE2', '\uFEE1', 'l', 'م');
            CreateMap(Noon, '\uFEE7', '\uFEE8', '\uFEE6', '\uFEE5', 'k', 'ن');
            CreateMap(Waw, '\uFEEE', '\uFEED', ',', 'و', 'U');
            CreateMap(Heh, '\uFEEB', '\uFEEC', '\uFEEA', '\uFEE9', 'i', 'ه');
            CreateMap(Yeh, '\uFBFE', '\uFBFF', '\uFBFD', '\uFBFC', 'd', '\u0649', '\u064A');

            // create maps for numeric characters
            CreateMap(Zero, '0', '\u0660', '\u06F0');
            CreateMap(One, '1', '\u0661', '\u06F1');
            CreateMap(Two, '2', '\u0662', '\u06F2');
            CreateMap(Three, '3', '\u0663', '\u06F3');
            CreateMap(Four, '4', '\u0664', '\u06F4');
            CreateMap(Five, '5', '\u0665', '\u06F5');
            CreateMap(Six, '6', '\u0666', '\u06F6');
            CreateMap(Seven, '7', '\u0667', '\u06F7');
            CreateMap(Eight, '8', '\u0668', '\u06F8');
            CreateMap(Nine, '9', '\u0669', '\u06F9');

            // create maps for other characters
            CreateMap(Fathatan, '\u064B', 'Q');
            CreateMap(Dammatan, '\u064C', 'W');
            CreateMap(Kasratan, '\u064D', 'E');
            CreateMap(Fatha, '\u064E', 'A');
            CreateMap(Damma, '\u064F', 'S');
            CreateMap(Kasra, '\u0650', 'D');
            CreateMap(Shadda, '\u0651', 'F');
            CreateMap(RialSign, '\uFDFC', 'R');
            CreateMap(Comma, '\u060C', 'T');
            CreateMap(Semicolon, '\u061B', 'Y');
            CreateMap(AlefMadda, '\uFE82', '\uFE81', 'H', '\u0622');
            CreateMap(WawWithHamzaAbove, '\uFE86', '\uFE85', 'V', '\u0624');
            CreateMap(AlefWithHamzaAbove, '\uFE84', '\uFE83', 'N', '\u0623');
            CreateMap(AlefWithHamzaBelow, '\uFE88', '\uFE87', 'B', '\u0625');
            CreateMap(Hamza, '\uFE80', 'M');
            CreateMap(QuestionMark, '\u061F', '?');
            CreateMap(Tatweel, '\u0640');
            CreateMap(LeftParenthesis, '\uFD3E', ')');
            CreateMap(RightParenthesis, '\uFD3F', '(');
            CreateMap(YehWithHamzaAbove, '\uFE8B', '\uFE8C', '\uFE8A', '\uFE89', 'm', 'ئ');

            // Ligature
            CreateMap(LigatureLam, '\uFEFC', '\uFEFB');
            CreateMap(LigatureAllah, '\uFDF2');
        }

        /// <summary>
        /// Create a PersianCharacterMap
        /// </summary>
        public PersianCharacterMap()
        {
            Map = new Dictionary<char, PersianCharacter>();
            CreatePersianCharacters();
            CreateMaps();
        }
    }
}
