using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Skill.Editor.AI;
using Skill.Editor.Animation;

namespace Skill.CodeGeneration
{
    /// <summary>
    /// The interface that used by Skill Studio to generate codes.
    /// It could be possible to implement code generator for other languages
    /// C# code generator is implemented.
    /// </summary>    
    public interface ICodeGenerator
    {        
        /// <summary> Specify whether generated code result containes extra code for debuging </summary>
        bool IsDebuging { get; set; }

        /// <summary> Target File Extension ( .cs for c# -- .js for javascript -- ...) </summary>
        string Extension { get; }

        /// <summary> Whether generate code has partial part </summary>
        bool HasPartial { get; }



        /// <summary>
        /// Reset generator and prepare to generate another output
        /// </summary>
        void Reset();

        /// <summary>
        /// Generate code for BehaviorTree
        /// </summary>
        /// <param name="bTree">BehaviorTree containing raw data</param>        
        void Generate(BehaviorTreeData bTree);

        /// <summary>
        /// Generate code for AnimationTree
        /// </summary>
        /// <param name="aTree">AnimationTree containing raw data</param>
        void Generate(AnimationTreeData aTree);

        /// <summary>
        /// Generate code for SkinMesh
        /// </summary>
        /// <param name="skinMesh">SkinMesh</param>
        void Generate(Skill.Editor.Animation.SkinMeshData skinMesh);

        /// <summary>
        /// Generate code for SaveData
        /// </summary>
        /// <param name="saveData">SaveData</param>
        void Generate(Skill.Editor.IO.SaveData saveData);



        /// <summary>
        /// Generate Code for SharedAccessKeys that used by BehaviorTree
        /// </summary>
        /// <param name="sharedAccessKeys">SharedAccessKeys</param>
        void Generate(Skill.Editor.AI.SharedAccessKeysData sharedAccessKeys);

        /// <summary>
        /// Write designer part of code
        /// </summary>
        /// <param name="writer">Stream to write</param>
        void Write(StreamWriter writer);

        /// <summary>
        /// Write partial part of code which user can edit them
        /// </summary>
        /// <param name="writer">Stream to write</param>
        /// <param name="oldCode">Previous user code</param>
        /// <remarks>
        /// You should keep user code safe and generate additional code if needed
        /// </remarks>
        void WritePartial(StreamWriter writer, string oldCode);        
    }
}
