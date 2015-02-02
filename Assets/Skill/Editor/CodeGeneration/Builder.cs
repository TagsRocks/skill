using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.AI;
using Skill.Editor.Animation;
using System.Collections.Generic;

namespace Skill.Editor
{
    public static class Builder
    {
        private static char[] OldeCodeTimeChars = new char[] { ' ', '\'', '\"', '\n', '\r', '\t' };
        private static CodeGeneration.CSharpCodeGenerator _Generator;
        private static void ValidateGenerator()
        {
            if (_Generator == null)
            {
                _Generator = new CodeGeneration.CSharpCodeGenerator();
#if UNITY_EDITOR
                _Generator.IsDebuging = true;
#endif
            }
        }

        private static void CreateDirectory(string dir)
        {
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
        }

        private static bool IsPathValid(string path)
        {
            try
            {
                System.IO.Path.GetFullPath(path);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private static string GetLocalPath(string path)
        {
            int index = path.IndexOf("assets", System.StringComparison.OrdinalIgnoreCase);
            if (index > 0)
                return path.Substring(index);
            else
                return path;
        }

        public static void Build(Skill.Editor.AI.BehaviorTreeData tree, string localDir, string name)
        {
            ValidateGenerator();
            _Generator.Reset();
            _Generator.Generate(tree);
            CreateFile(_Generator, localDir, name);
        }

        public static void Build(Skill.Editor.AI.SharedAccessKeysData keys, string localDir, string name)
        {
            ValidateGenerator();
            _Generator.Reset();
            _Generator.Generate(keys);
            CreateFile(_Generator, localDir, name);
        }

        public static void Build(Skill.Editor.Animation.AnimationTreeData tree, string localDir, string name)
        {
            ValidateGenerator();
            _Generator.Reset();
            _Generator.Generate(tree);
            CreateFile(_Generator, localDir, name);
        }

        public static void Build(Skill.Editor.Animation.SkinMeshData mesh, string localDir, string name)
        {
            ValidateGenerator();
            _Generator.Reset();
            _Generator.Generate(mesh);
            CreateFile(_Generator, localDir, name);
        }

        public static void Build(Skill.Editor.IO.SaveData saveData, string localDir, string name)
        {
            ValidateGenerator();
            _Generator.Reset();
            _Generator.Generate(saveData);
            CreateFile(_Generator, localDir, name);
        }

        private static void CreateFile(Skill.CodeGeneration.ICodeGenerator generator, string localDir, string name)
        {

            string destinationDir = System.IO.Path.Combine(Application.dataPath, localDir);
            if (!IsPathValid(destinationDir))
            {
                Debug.LogError("Invalid build path");
                return;
            }
            // write editor generated file            
            CreateDirectory(destinationDir);

            string designerfilename = System.IO.Path.Combine(destinationDir, name + "." + "Designer" + generator.Extension);
            string userFilename = System.IO.Path.Combine(destinationDir, name + generator.Extension);

            if (System.IO.File.Exists(designerfilename)) System.IO.File.Delete(designerfilename);
            System.IO.FileStream file = new System.IO.FileStream(designerfilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
            generator.Write(writer);
            writer.Close();
            file.Close();

            if (generator.HasPartial)
            {
                string oldCode = null;
                // writer user file
                if (System.IO.File.Exists(userFilename))
                {
                    oldCode = System.IO.File.ReadAllText(userFilename).Trim(OldeCodeTimeChars);
                    System.IO.File.Delete(userFilename);
                }

                file = new System.IO.FileStream(userFilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                writer = new System.IO.StreamWriter(file);
                generator.WritePartial(writer, oldCode);
            }
            else if (System.IO.File.Exists(userFilename))
            {
                System.IO.File.Delete(userFilename);
            }
            writer.Close();
            file.Close();

            if (System.IO.File.Exists(designerfilename))
                AssetDatabase.ImportAsset(GetLocalPath(designerfilename));

            if (System.IO.File.Exists(userFilename))
                AssetDatabase.ImportAsset(GetLocalPath(userFilename));


        }
    }
}
