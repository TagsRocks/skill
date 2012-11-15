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

        // A dictionary that maps each word to equivalent persian character        
        private Dictionary<char, PersianCharacter> _Map;

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


        // special characters

        /// <summary> ! </summary>
        public PersianCharacter ExclamationMark { get; private set; }
        /// <summary> . </summary>
        public PersianCharacter Dot { get; private set; }
        /// <summary> $ </summary>
        public PersianCharacter DollarSign { get; private set; }
        /// <summary> % </summary>
        public PersianCharacter PercentSign { get; private set; }
        /// <summary> and </summary>
        public PersianCharacter Ampersand { get; private set; }
        /// <summary> * </summary>
        public PersianCharacter Asterisk { get; private set; }
        /// <summary> + </summary>
        public PersianCharacter PlusSign { get; private set; }
        /// <summary> - </summary>
        public PersianCharacter HyphenMinus { get; private set; }
        /// <summary> . </summary>
        public PersianCharacter FullStop { get; private set; }
        /// <summary> = </summary>
        public PersianCharacter EqualSign { get; private set; }
        /// <summary> lth; </summary>
        public PersianCharacter LessThanSign { get; private set; }
        /// <summary> gth; </summary>
        public PersianCharacter GreaterThanSign { get; private set; }
        /// <summary> @ </summary>
        public PersianCharacter CommercialAt { get; private set; }
        /// <summary> ^ </summary>
        public PersianCharacter CircumflexAccent { get; private set; }
        /// <summary> _ </summary>
        public PersianCharacter LowLine { get; private set; }

        /// <summary> \n </summary>
        public PersianCharacter NewLine { get; private set; }
        /// <summary> \t </summary>
        public PersianCharacter Tab { get; private set; }

        // Ligature
        /// <summary> لا </summary>
        public PersianCharacter LigatureLam { get; private set; }
        /// <summary> الله </summary>
        public PersianCharacter LigatureAllah { get; private set; }


        /// <summary>
        /// Whether convert english characters to equivalent persian characters (as on keyboard)?
        /// </summary>
        public bool ConvertEnglishCharacters { get; private set; }

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
            this.Zero = new PersianCharacter('\u0660') { LeftToRight = true };
            this.One = new PersianCharacter('\u0661') { LeftToRight = true };
            this.Two = new PersianCharacter('\u0662') { LeftToRight = true };
            this.Three = new PersianCharacter('\u0663') { LeftToRight = true };
            this.Four = new PersianCharacter('\u0664') { LeftToRight = true };
            this.Five = new PersianCharacter('\u0665') { LeftToRight = true };
            this.Six = new PersianCharacter('\u0666') { LeftToRight = true };
            this.Seven = new PersianCharacter('\u0667') { LeftToRight = true };
            this.Eight = new PersianCharacter('\u0668') { LeftToRight = true };
            this.Nine = new PersianCharacter('\u0669') { LeftToRight = true };

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

            // special characters                        
            this.ExclamationMark = new PersianCharacter('!');
            this.Dot = new PersianCharacter('·');
            this.DollarSign = new PersianCharacter('$');
            this.PercentSign = new PersianCharacter('%');
            this.Ampersand = new PersianCharacter('&');
            this.Asterisk = new PersianCharacter('*');
            this.PlusSign = new PersianCharacter('+');
            this.HyphenMinus = new PersianCharacter('-');
            this.FullStop = new PersianCharacter('.');
            this.EqualSign = new PersianCharacter('=');
            this.LessThanSign = new PersianCharacter('<');
            this.GreaterThanSign = new PersianCharacter('>');
            this.CommercialAt = new PersianCharacter('@');
            this.CircumflexAccent = new PersianCharacter('^');
            this.LowLine = new PersianCharacter('_');

            this.NewLine = new PersianCharacter('\n');
            this.Tab = new PersianCharacter('\t');

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
                    _Map.Add(k, pc);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Key exist " + c, "keys", ex);
            }
        }

        private void CreateMaps()
        {
            // create maps for alefba characters
            CreateMap(Alef, '\uFE8E', '\uFE8D', 'ا');
            CreateMap(Beh, '\uFE91', '\uFE92', '\uFE90', '\uFE8F', 'ب');
            CreateMap(Peh, '\uFB58', '\uFB59', '\uFB57', '\uFB56', 'پ');
            CreateMap(Teh, '\uFE97', '\uFE98', '\uFE96', '\uFE95', 'ت');
            CreateMap(Theh, '\uFE9B', '\uFE9C', '\uFE9A', '\uFE99', 'ث');
            CreateMap(Jeem, '\uFE9F', '\uFEA0', '\uFE9E', '\uFE9D', 'ج');
            CreateMap(Cheh, '\uFB7C', '\uFB7D', '\uFB7B', '\uFB7A', 'چ');
            CreateMap(Hah, '\uFEA3', '\uFEA4', '\uFEA2', '\uFEA1', 'ح');
            CreateMap(Khah, '\uFEA7', '\uFEA8', '\uFEA6', '\uFEA5', 'خ');
            CreateMap(Dal, '\uFEAA', '\uFEA9', 'د');
            CreateMap(Thal, '\uFEAC', '\uFEAB', 'ذ');
            CreateMap(Reh, '\uFEAE', '\uFEAD', 'ر');
            CreateMap(Zain, '\uFEB0', '\uFEAF', 'ز');
            CreateMap(Jeh, '\uFB8B', '\uFB8A', 'ژ');
            CreateMap(Seen, '\uFEB3', '\uFEB4', '\uFEB2', '\uFEB1', 'س');
            CreateMap(Sheen, '\uFEB7', '\uFEB8', '\uFEB6', '\uFEB5', 'ش');
            CreateMap(Sad, '\uFEBB', '\uFEBC', '\uFEBA', '\uFEB9', 'ص');
            CreateMap(Dad, '\uFEBF', '\uFEC0', '\uFEBE', '\uFEBD', 'ض');
            CreateMap(Tah, '\uFEC3', '\uFEC4', '\uFEC2', '\uFEC1', 'ط');
            CreateMap(Zah, '\uFEC7', '\uFEC8', '\uFEC6', '\uFEC5', 'ظ');
            CreateMap(Ain, '\uFECB', '\uFECC', '\uFECA', '\uFEC9', 'ع');
            CreateMap(Ghain, '\uFECF', '\uFED0', '\uFECE', '\uFECD', 'غ');
            CreateMap(Feh, '\uFED3', '\uFED4', '\uFED2', '\uFED1', 'ف');
            CreateMap(Qaf, '\uFED7', '\uFED8', '\uFED6', '\uFED5', 'ق');
            CreateMap(Kaf, '\uFEDB', '\uFEDC', '\uFEDA', '\uFED9', 'ک', '\u0643');
            CreateMap(Gaf, '\uFB94', '\uFB95', '\uFB93', '\uFB92', 'گ');
            CreateMap(Lam, '\uFEDF', '\uFEE0', '\uFEDE', '\uFEDD', 'ل');
            CreateMap(Meem, '\uFEE3', '\uFEE4', '\uFEE2', '\uFEE1', 'م');
            CreateMap(Noon, '\uFEE7', '\uFEE8', '\uFEE6', '\uFEE5', 'ن');
            CreateMap(Waw, '\uFEEE', '\uFEED', 'و');
            CreateMap(Heh, '\uFEEB', '\uFEEC', '\uFEEA', '\uFEE9', 'ه');
            CreateMap(Yeh, '\uFBFE', '\uFBFF', '\uFBFD', '\uFBFC', '\u0649', '\u064A', '\u06CC');

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
            CreateMap(Fathatan, '\u064B');
            CreateMap(Dammatan, '\u064C');
            CreateMap(Kasratan, '\u064D');
            CreateMap(Fatha, '\u064E');
            CreateMap(Damma, '\u064F');
            CreateMap(Kasra, '\u0650');
            CreateMap(Shadda, '\u0651');
            CreateMap(RialSign, '\uFDFC');
            CreateMap(Comma, '\u060C');
            CreateMap(Semicolon, '\u061B');
            CreateMap(AlefMadda, '\uFE82', '\uFE81', '\u0622');
            CreateMap(WawWithHamzaAbove, '\uFE86', '\uFE85', '\u0624');
            CreateMap(AlefWithHamzaAbove, '\uFE84', '\uFE83', '\u0623');
            CreateMap(AlefWithHamzaBelow, '\uFE88', '\uFE87', '\u0625');
            CreateMap(Hamza, '\uFE80');
            CreateMap(QuestionMark, '\u061F', '?');
            CreateMap(Tatweel, '\u0640');
            CreateMap(LeftParenthesis, '\uFD3E', ')');
            CreateMap(RightParenthesis, '\uFD3F', '(');
            CreateMap(YehWithHamzaAbove, '\uFE8B', '\uFE8C', '\uFE8A', '\uFE89', 'ئ');

            // Ligature
            CreateMap(LigatureLam, '\uFEFC', '\uFEFB');
            CreateMap(LigatureAllah, '\uFDF2');

            // special character                                
            CreateMap(ExclamationMark, '!');
            CreateMap(Dot, '·');
            CreateMap(DollarSign, '$');
            CreateMap(PercentSign, '%');
            CreateMap(Ampersand, '&');
            CreateMap(Asterisk, '*');
            CreateMap(PlusSign, '+');
            CreateMap(HyphenMinus, '-');
            CreateMap(FullStop, '.');
            CreateMap(EqualSign, '=');
            CreateMap(LessThanSign, '<');
            CreateMap(GreaterThanSign, '>');
            CreateMap(CommercialAt, '@');
            CreateMap(CircumflexAccent, '^');
            CreateMap(LowLine, '_');
            CreateMap(NewLine, '\n');
            CreateMap(Tab, '\t');

            if (ConvertEnglishCharacters)
            {
                CreateMap(Alef, 'h');
                CreateMap(Beh, 'f');
                CreateMap(Peh, '\\');
                CreateMap(Teh, 'j');
                CreateMap(Theh, 'e');
                CreateMap(Jeem, '[');
                CreateMap(Cheh, ']');
                CreateMap(Hah, 'p');
                CreateMap(Khah, 'o');
                CreateMap(Dal, 'n');
                CreateMap(Thal, 'b');
                CreateMap(Reh, 'v');
                CreateMap(Zain, 'c');
                CreateMap(Jeh, 'C');
                CreateMap(Seen, 's');
                CreateMap(Sheen, 'a');
                CreateMap(Sad, 'w');
                CreateMap(Dad, 'q');
                CreateMap(Tah, 'x');
                CreateMap(Zah, 'z');
                CreateMap(Ain, 'u');
                CreateMap(Ghain, 'y');
                CreateMap(Feh, 't');
                CreateMap(Qaf, 'r');
                CreateMap(Kaf, ';');
                CreateMap(Gaf, '\'');
                CreateMap(Lam, 'g');
                CreateMap(Meem, 'l');
                CreateMap(Noon, 'k');
                CreateMap(Waw, 'U', ',');
                CreateMap(Heh, 'i');
                CreateMap(Yeh, 'd');


                // create maps for other characters
                CreateMap(Fathatan, 'Q');
                CreateMap(Dammatan, 'W');
                CreateMap(Kasratan, 'E');
                CreateMap(Fatha, 'A');
                CreateMap(Damma, 'S');
                CreateMap(Kasra, 'D');
                CreateMap(Shadda, 'F');
                CreateMap(RialSign, 'R');
                CreateMap(Comma, 'T');
                CreateMap(Semicolon, 'Y');
                CreateMap(AlefMadda, 'H');
                CreateMap(WawWithHamzaAbove, 'V');
                CreateMap(AlefWithHamzaAbove, 'N');
                CreateMap(AlefWithHamzaBelow, 'B');
                CreateMap(Hamza, 'M');
                CreateMap(YehWithHamzaAbove, 'm');
            }
        }

        /// <summary>
        /// Create a PersianCharacterMap
        /// </summary>
        /// <param name="convertEnglishCharacters">Whether convert english characters to equivalent persian characters (as on keyboard)?</param>
        public PersianCharacterMap(bool convertEnglishCharacters = true)
        {
            this.ConvertEnglishCharacters = convertEnglishCharacters;
            this._Map = new Dictionary<char, PersianCharacter>();
            CreatePersianCharacters();
            CreateMaps();
        }

        /// <summary>
        /// maps each none persian chracter to equivalent persian character
        /// </summary>
        /// <param name="c">The character to remap to persian character</param>
        /// <returns>if there is a map for given character returns a PersianCharacter, otherwise null.</returns>
        public PersianCharacter GetMappedCharacter(char c)
        {
            PersianCharacter result = null;
            if (!_Map.TryGetValue(c, out result))
                result = null;
            return result;
        }
    }
}
