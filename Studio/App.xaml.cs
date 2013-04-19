using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Skill.Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {            
            string file = null;
            if (e.Args != null && e.Args.Length > 0)
            {
                foreach (var arg in e.Args)
                {
                    if (!string.IsNullOrEmpty(arg))
                    {
                        if (arg != null && System.IO.File.Exists(arg))
                        {
                            if (System.IO.Path.GetExtension(arg) == Project.Extension)
                            {
                                Skill.Studio.MainWindow.ProjectAddressToOpen = arg;
                                break;
                            }
                        }

                        // after installing Skill Studio i noticed that when you double click on a .skProj file the filename passed as arguments.
                        // this is fine until filename contains space like "f:/My Documents/Skill/Projects/SkillTutorial.skproj"
                        // because 'My Documents' contains space it will separated as two arguments as "f:/My" and "Documents/Skill/Projects/SkillTutorial.skproj"
                        // i couldn't find a solution for this yet.
                        // i don't know about other characters like space.
                        // if anyone read this source and find a solution please let me know

                        // just do stupid code to handle this issue
                        if (System.IO.Path.IsPathRooted(arg))
                            file = arg;
                        else if (!string.IsNullOrEmpty(arg))
                            file += " " + arg;                        

                        if (file != null && System.IO.File.Exists(file))
                        {
                            if (System.IO.Path.GetExtension(file) == Project.Extension)
                            {
                                Skill.Studio.MainWindow.ProjectAddressToOpen = file;
                                break;
                            }
                        }
                    }
                }
            }

            base.OnStartup(e);
        }
    }
}
