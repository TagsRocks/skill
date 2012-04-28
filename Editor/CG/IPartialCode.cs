using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.CG
{
    /// <summary>
    /// all class contains two file
    /// one that created automatically by editor and regenerated on build time
    /// other is partial code that user must write own code
    /// </summary>
    interface IPartialCode
    {        
        /// <summary>
        /// Write all of auto code
        /// </summary>
        /// <param name="writer">Stream</param>
        void Write(System.IO.StreamWriter writer);
        /// <summary>
        /// Write all codes that does not exist in oldcode
        /// </summary>
        /// <param name="writer">stream</param>
        /// <param name="oldCode">old code that contains user code, does not allowed to modify this code, just add new functionallity</param>
        void WritePartial(System.IO.StreamWriter writer,string oldCode);
    }

}
