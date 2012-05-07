using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Skill.Editor.CG
{
    public class Builder
    {
        string _ProjectName;
        string _UnityDirectory;

        private static char[] OldeCodeTimeChars = new char[] { ' ', '\'', '\"', '\n', '\r', '\t' };

        public Builder(string projctName, string unityDir)
        {
            this._ProjectName = projctName;
            this._UnityDirectory = unityDir;
        }

        public string BackupDirectory { get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skill\\Backup Files", _ProjectName); } }

        private void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        public void Build(AI.BehaviorTree tree, string localDir, string name)
        {
            Document doc = new Document();
            doc.AddDefaultUsings();
            doc.AddUsingSkillAI();

            BehaviorTreeClass btClass = new BehaviorTreeClass(tree);
            doc.Add(btClass);

            CreateFile(doc, localDir, name);

        }


        public void Build(Animation.AnimationTree tree, string localDir, string name)
        {
            Document doc = new Document();
            doc.AddDefaultUsings();
            doc.AddUsingSkillAnimation();

            AnimationTreeClass atClass = new AnimationTreeClass(tree);
            doc.Add(atClass);

            CreateFile(doc, localDir, name);

        }

        private void CreateFile(Document doc, string localDir, string name)
        {
            string destinationDir = System.IO.Path.Combine(_UnityDirectory, localDir);
            string destinationDesignerDir = System.IO.Path.Combine(_UnityDirectory, "Designer", localDir);
            // write editor generated file            
            CreateDirectory(destinationDir);
            CreateDirectory(destinationDesignerDir);

            string designerfilename = System.IO.Path.Combine(destinationDesignerDir, name + ".designer" + Document.Extension);
            string userFilename = System.IO.Path.Combine(destinationDir, name + Document.Extension);

            if (File.Exists(designerfilename)) File.Delete(designerfilename);
            FileStream file = new FileStream(designerfilename, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);
            doc.Write(writer);
            writer.Close();
            file.Close();

            string oldCode = null;

            // writer user file
            if (File.Exists(userFilename))
            {
                oldCode = File.ReadAllText(userFilename).Trim(OldeCodeTimeChars);
                File.Delete(userFilename);
            }

            file = new FileStream(userFilename, FileMode.Create, FileAccess.Write);
            writer = new StreamWriter(file);
            doc.WritePartial(writer, oldCode);
            writer.Close();
            file.Close();
        }
    }
}
