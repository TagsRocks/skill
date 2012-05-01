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

        private static char[] OldeCodeTimeChars = new char[] {' ','\'','\"','\n','\r','\t' };

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

        public void Build(AI.BehaviorTree tree, string localDir, string name, string postfix)
        {
            Document doc = new Document();
            doc.AddDefaultUsings();
            doc.AddUsingSkillAI();

            BehaviorTreeClass btClass = new BehaviorTreeClass(tree);
            doc.Add(btClass);

            CreateFile(doc, localDir, name, postfix);

        }


        public void Build(Animation.AnimationTree tree, string localDir, string name, string postfix)
        {
            Document doc = new Document();
            doc.AddDefaultUsings();
            doc.AddUsingSkillAnimation();

            AnimationTreeClass atClass = new AnimationTreeClass(tree);
            doc.Add(atClass);

            CreateFile(doc, localDir, name, postfix);

        }

        private void CreateFile(Document doc, string localDir, string name, string postfix)
        {
            string destinationDir = System.IO.Path.Combine(_UnityDirectory, localDir);
            // write editor generated file            
            CreateDirectory(destinationDir);

            string filename = System.IO.Path.Combine(destinationDir, name + Document.Extension);
            string partialFilename = System.IO.Path.Combine(destinationDir, name + postfix + Document.Extension);

            if (File.Exists(filename)) File.Delete(filename);
            FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);
            doc.Write(writer);
            writer.Close();
            file.Close();

            string oldCode = null;

            // writer user file
            if (File.Exists(partialFilename))
            {
                oldCode = File.ReadAllText(partialFilename).Trim(OldeCodeTimeChars);
                File.Delete(partialFilename);                
            }

            file = new FileStream(partialFilename, FileMode.Create, FileAccess.Write);
            writer = new StreamWriter(file);
            doc.WritePartial(writer, oldCode);
            writer.Close();
            file.Close();
        }
    }
}
