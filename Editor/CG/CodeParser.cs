using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.CG
{
    class CodeParser
    {
        public string Code { get; private set; }

        public CodeParser(string code)
        {
            Code = code;
        }

        void AcrossSpaces(ref int index)
        {
            while (index >= 0 && index < Code.Length && ( Code[index] == ' ' || Code[index] == '\n' || Code[index] == '\r' || Code[index] == '\t')) index++;
        }
        void NextSpace(ref int index)
        {
            while (index >= 0 && index < Code.Length && Code[index] != ' ') index++;
        }

        bool AcrossWord(string word, ref int index)
        {
            int p = index;
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] != Code[index++])
                    break;
            }
            return index > p + word.Length;
        }

        int IndexOf(params string[] sequenceWords)
        {
            bool success = false;
            int index = 0;
            int start = 0;
            while (!success && index >= 0 && start < Code.Length)
            {
                NextSpace(ref start);
                int start2 = 0;
                index = start2 = Code.IndexOf(sequenceWords[0], start);
                if (start2 >= 0 && start2 >= start)
                {
                    for (int j = 0; j < sequenceWords.Length; j++)
                    {
                        success = AcrossWord(sequenceWords[j], ref start2);
                        NextSpace(ref start2);
                        AcrossSpaces(ref start2);
                    }
                }
                start = start2;
            }
            return index;
        }
    }
}
