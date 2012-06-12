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
        public string Name { get; set; }
        /// <summary> localtion to save project </summary>
        public string Location { get; set; }
        /// <summary> localtion generate code </summary>
        public string OutputLocaltion { get; set; }


        public string Filename { get { return System.IO.Path.Combine(Location, Name, Name + Project.Extension); } }
    }
}
