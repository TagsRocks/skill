using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.CodeGeneration
{
    /// <summary>
    /// Defines information about extra file to copy in code generation result directory
    /// </summary>
    public class RequiredFile
    {
        /// <summary> Source file name (relative to SkillStudio.exe )</summary>
        public string SourceFile { get; private set; }
        /// <summary> Destination directory to copy file to</summary>
        public string DestinationDirectory { get; private set; }
        /// <summary> Is this file required for editor.</summary>
        public bool IsEditor { get; private set; }

        /// <summary>
        /// Create a RequiredFile
        /// </summary>
        /// <param name="sourceFile">Source file name (relative to SkillStudio.exe )</param>
        /// <param name="destinationDirectory">Destination directory to copy file to</param>       
        /// <param name="isEditor"> Is this file required for editor. </param>
        public RequiredFile(string sourceFile, string destinationDirectory, bool isEditor = false)
        {
            this.SourceFile = sourceFile;
            this.DestinationDirectory = destinationDirectory;
            this.IsEditor = isEditor;
        }
    }
}
