using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor
{
    /// <summary>
    /// Defines information to create new project
    /// </summary>
    public class NewProjectInfo
    {
        /// <summary> Name of project </summary>
        public string Name { get; set; }
        /// <summary> localtion to save project </summary>
        public string Location { get; set; }
        /// <summary> localtion generate code </summary>
        public string UnityProjectLocaltion { get; set; }
    }
}
