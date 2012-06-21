using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// Helper class to handle write Comment
    /// </summary>
    static class CommentWriter
    {
        /// <summary>
        /// Write given comment in apropriate format
        /// </summary>
        /// <param name="writer">Stream to write</param>
        /// <param name="comment">Comment</param>
        public static void Write(System.IO.TextWriter writer, string comment)
        {
            if (!string.IsNullOrEmpty(comment))// write comment
            {
                string[] lines = comment.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length == 1)
                {
                    writer.WriteLine(string.Format("/// <summary> {0} </summary>", lines[0]));
                }
                else
                {
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
}
