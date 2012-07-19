using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Skill.CodeGeneration;

namespace Skill.Studio
{
    public static class Builder
    {
        private static char[] OldeCodeTimeChars = new char[] { ' ', '\'', '\"', '\n', '\r', '\t' };

        private static void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        public static void Build(Skill.DataModels.AI.BehaviorTree tree, string localDir, string name)
        {
            PluginManager.Plugin.Reset();
            PluginManager.Plugin.Generate(tree);
            CreateFile(PluginManager.Plugin, localDir, name);

        }

        public static void Build(Skill.DataModels.AI.SharedAccessKeys keys, string localDir, string name)
        {
            PluginManager.Plugin.Reset();
            PluginManager.Plugin.Generate(keys);
            CreateFile(PluginManager.Plugin, localDir, name);

        }

        public static void Build(Skill.DataModels.Animation.AnimationTree tree, string localDir, string name)
        {
            PluginManager.Plugin.Reset();
            PluginManager.Plugin.Generate(tree);
            CreateFile(PluginManager.Plugin, localDir, name);
        }

        public static void Build(Skill.DataModels.IO.SaveData saveGame, string localDir, string name)
        {
            PluginManager.Plugin.Reset();
            PluginManager.Plugin.Generate(saveGame);
            CreateFile(PluginManager.Plugin, localDir, name);
        }

        private static void CreateFile(ICodeGenerator generator, string localDir, string name)
        {
            string destinationDir = MainWindow.Instance.Project.GetOutputPath(localDir);
            string destinationDesignerDir = MainWindow.Instance.Project.GetDesignerOutputPath(localDir);
            // write editor generated file            
            CreateDirectory(destinationDir);
            CreateDirectory(destinationDesignerDir);

            string designerfilename = System.IO.Path.Combine(destinationDesignerDir, name + "." + MainWindow.Instance.Project.DesignerName + generator.Extension);
            string userFilename = System.IO.Path.Combine(destinationDir, name + generator.Extension);

            if (File.Exists(designerfilename)) File.Delete(designerfilename);
            FileStream file = new FileStream(designerfilename, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);
            generator.Write(writer);
            writer.Close();
            file.Close();

            if (generator.HasPartial)
            {
                string oldCode = null;
                // writer user file
                if (File.Exists(userFilename))
                {
                    oldCode = File.ReadAllText(userFilename).Trim(OldeCodeTimeChars);
                    File.Delete(userFilename);
                }

                file = new FileStream(userFilename, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(file);
                generator.WritePartial(writer, oldCode);
            }
            writer.Close();
            file.Close();
        }
    }
}
