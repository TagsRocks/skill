using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.AI;
using Skill.Editor.Animation;
using System.IO;
using Skill.CodeGeneration;
using System.ComponentModel;

namespace Skill.Editor.CodeGeneration
{
    /// <summary>
    /// Implement a C# Plugin Code Generator
    /// </summary>
    [DisplayName("Skill-Unity-CSharp")]
    public class CSharpCodeGenerator : ICodeGenerator
    {
        public bool IsDebuging { get; set; }


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
        public void Generate(BehaviorTreeData bTree)
        {
            Reset();
            BehaviorTreeClass btClass = new BehaviorTreeClass(bTree);
            _Document.AddUsingSkillAI();
            _Document.Add(btClass);
            HasPartial = bTree.ExpandMethods;
        }

        public void Reset()
        {
            _Document = new Document();
            _Document.AddDefaultUsings();
        }

        /// <summary>
        /// Generate code for SharedAccessKeys
        /// </summary>
        /// <param name="sharedAccessKeys">SharedAccessKeys</param>
        public void Generate(Skill.Editor.AI.SharedAccessKeysData sharedAccessKeys)
        {
            SharedAccessKeysClass saClass = new SharedAccessKeysClass(sharedAccessKeys);
            _Document.AddUsingSkillAI();
            _Document.Add(saClass);
            HasPartial = false;
        }

        /// <summary>
        /// generate code for AnimationTree
        /// </summary>
        /// <param name="aTree">AnimationTree</param>
        public void Generate(AnimationTreeData aTree)
        {
            AnimationTreeClass atClass = new AnimationTreeClass(aTree);
            _Document.AddUsingSkillAnimation();
            _Document.Add(atClass);
            HasPartial = true;
        }

        /// <summary>
        /// Generate code for SkinMesh
        /// </summary>
        /// <param name="skinMesh">SkinMesh</param>
        public void Generate(Skill.Editor.Animation.SkinMeshData skinMesh)
        {
            SkinMeshClass skClass = new SkinMeshClass(skinMesh);
            _Document.AddUsingSkillAnimation();
            _Document.Add(skClass);
            HasPartial = false;
        }


        /// <summary>
        /// Generate code for SaveData
        /// </summary>
        /// <param name="saveGame">SaveData</param>
        public void Generate(Skill.Editor.IO.SaveData saveGame)
        {
            if (!string.IsNullOrEmpty(saveGame.Namespace))
            {
                Skill.Editor.IO.SaveData.GeneratingInstance = saveGame;
                Namespace ns = new Namespace(saveGame.Namespace);
                SaveDataClass sgClass = new SaveDataClass(saveGame);
                ns.Add(sgClass);
                _Document.AddUsingSkillIO();
                _Document.Add(ns);
                HasPartial = false;
                Skill.Editor.IO.SaveData.GeneratingInstance = null;
            }
            else
            {
                Skill.Editor.IO.SaveData.GeneratingInstance = saveGame;
                SaveDataClass sgClass = new SaveDataClass(saveGame);
                _Document.AddUsingSkillIO();
                _Document.Add(sgClass);
                HasPartial = false;
                Skill.Editor.IO.SaveData.GeneratingInstance = null;
            }
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
