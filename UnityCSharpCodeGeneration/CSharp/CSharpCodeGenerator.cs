using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using Skill.DataModels.Animation;
using System.IO;
using Skill.CodeGeneration;
using System.ComponentModel;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// Implement a C# Plugin Code Generator
    /// </summary>
    [DisplayName("Skill-Unity-CSharp")]
    public class CSharpCodeGenerator : ICodeGenerator
    {
        /// <summary>
        /// Extension of C# file ( .cs )
        /// </summary>
        public string Extension { get { return ".cs"; } }

        /// <summary> Whether generate code has partial part </summary>
        public bool HasPartial { get; private set; }

        private Document _Document;

        /// <summary>
        /// Generate Code for BehaviorTree 
        /// </summary>
        /// <param name="bTree">BehaviorTree </param>
        public void Generate(BehaviorTree bTree)
        {
            Reset();
            BehaviorTreeClass btClass = new BehaviorTreeClass(bTree);
            _Document.Add(btClass);
            HasPartial = true;
        }

        public void Reset()
        {
            _Document = new Document();
            _Document.AddDefaultUsings();
            _Document.AddUsingSkillAI();
        }

        /// <summary>
        /// Generate code for SharedAccessKeys
        /// </summary>
        /// <param name="sharedAccessKeys">SharedAccessKeys</param>
        public void Generate(Skill.DataModels.AI.SharedAccessKeys sharedAccessKeys)
        {
            SharedAccessKeysClass saClass = new SharedAccessKeysClass(sharedAccessKeys);
            _Document.Add(saClass);
            HasPartial = false;
        }

        /// <summary>
        /// generate code for AnimationTree
        /// </summary>
        /// <param name="aTree">AnimationTree</param>
        public void Generate(AnimationTree aTree)
        {
           AnimationTreeClass atClass = new AnimationTreeClass(aTree);
            _Document.Add(atClass);
            HasPartial = true;
        }        

        /// <summary>
        /// Generate code for SaveData
        /// </summary>
        /// <param name="saveGame">SaveData</param>
        public void Generate(Skill.DataModels.IO.SaveData saveGame)
        {
           
            SaveDataClass sgClass = new SaveDataClass(saveGame);
            _Document.Add(sgClass);
            HasPartial = false;
        }

        /// <summary>
        /// Write designer part of code
        /// </summary>
        /// <param name="writer">Stream</param>
        public void Write(StreamWriter writer)
        {
            _Document.Write(writer);
        }

        /// <summary>
        /// Write user part of code
        /// </summary>
        /// <param name="writer">Stream</param>
        /// <param name="oldCode">Old user code</param>
        public void WritePartial(StreamWriter writer, string oldCode)
        {
            _Document.WritePartial(writer, oldCode);
        }
    }
}
