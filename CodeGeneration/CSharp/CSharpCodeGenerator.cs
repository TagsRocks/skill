using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using Skill.DataModels.Animation;
using System.IO;

namespace Skill.CodeGeneration.CSharp
{
    public class CSharpCodeGenerator : ICodeGenerator
    {
        public string Extension
        {
            get { return ".cs"; }
        }

        private Document _Document;
        public void Generate(BehaviorTree bTree)
        {
            _Document = new Document();
            _Document.AddDefaultUsings();
            _Document.AddUsingSkillAI();

            BehaviorTreeClass btClass = new BehaviorTreeClass(bTree);
            _Document.Add(btClass);
        }

        public void Generate(AnimationTree aTree)
        {
            _Document = new Document();
            _Document.AddDefaultUsings();
            _Document.AddUsingSkillAnimation();

            AnimationTreeClass atClass = new AnimationTreeClass(aTree);
            _Document.Add(atClass);
        }
        public void Generate(Skill.DataModels.IO.SaveGame saveGame)
        {
            _Document = new Document();
        }


        public void Write(StreamWriter writer)
        {
            _Document.Write(writer);
        }

        public void WritePartial(StreamWriter writer, string oldCode)
        {
            _Document.WritePartial(writer, oldCode);
        }
    }
}
