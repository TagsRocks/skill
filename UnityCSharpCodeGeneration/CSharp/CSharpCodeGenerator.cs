﻿using System;
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
        private RequiredFile[] _RequiredFiles = new RequiredFile[]
        {
            new RequiredFile("Skill.Framework.dll","",false),
            new RequiredFile("Skill.Framework.xml","",false),
            new RequiredFile("Skill.Framework.pdb","",false),
            new RequiredFile("Skill.Net.dll","",false),
            new RequiredFile("Skill.Net.pdb","",false),
            new RequiredFile("Skill.Net.xml","",false),
            new RequiredFile("Skill.Diagnostics.dll","",false),            
            new RequiredFile("Skill.Diagnostics.pdb","",false),
            new RequiredFile("Skill.Diagnostics.xml","",false),
            new RequiredFile("Skill.DataModels.dll","",false),
            new RequiredFile("Skill.Editor.dll","",true),
            new RequiredFile("Skill.Editor.xml","",true),
            new RequiredFile("Skill.Editor.pdb","",true),
            new RequiredFile("Resources","",true),            
            
        };
        public RequiredFile[] RequiredFiles { get { return _RequiredFiles; } }

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
        public void Generate(BehaviorTree bTree)
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
        public void Generate(Skill.DataModels.AI.SharedAccessKeys sharedAccessKeys)
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
        public void Generate(AnimationTree aTree)
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
        public void Generate(Skill.DataModels.Animation.SkinMesh skinMesh)
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
        public void Generate(Skill.DataModels.IO.SaveData saveGame)
        {

            SaveDataClass sgClass = new SaveDataClass(saveGame);
            _Document.AddUsingSkillIO();
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

        /// <summary> Local address of assets directory relative to game directory</summary>        
        public string AssetsPath { get { return "Assets"; } }
        /// <summary> Local address of editor directory relative to assets directory</summary>       
        public string EditorPath { get { return "Skill\\Editor"; } }
        /// <summary> Local address of scripts directory relative to assets directory</summary>        
        public string ScriptsPath { get { return "Skill\\Scripts"; } }
        /// <summary> Local address of plugins(skill dlls) directory relative to assets directory</summary>        
        public string PluginsPath { get { return "Skill\\Plugins"; } }
        /// <summary> Local address of designer(generated code files) directory relative to assets directory</summary>        
        public string DesignerPath { get { return "Skill\\Designer"; } }
    }
}
