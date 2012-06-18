using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.CodeGeneration.CSharp
{
    static class CommentWriter
    {
        public static void Write(System.IO.TextWriter writer, string comment)
        {
            if (!string.IsNullOrEmpty(comment))// write comment
            {
                string[] lines = comment.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                writer.WriteLine("/// <summary>");
                foreach (var line in lines)
                {
                    writer.Write(string.Format("/// {0}", line));
                }
                writer.WriteLine("/// </summary>");
            }
        }
    }
}
