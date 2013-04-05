using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio
{
    /// <summary>
    /// Defines information to create new project
    /// </summary>
    public class NewProjectInfo
    {

        /// <summary> Name of project </summary>
        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(UnityProjectDirectory))
                    return System.IO.Path.GetFileNameWithoutExtension(UnityProjectDirectory);
                return "";
            }
        }
        /// <summary> directory of unity project </summary>
        public string UnityProjectDirectory { get; set; }        
    }
}
