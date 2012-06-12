using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Skill.DataModels.AI;
using Skill.DataModels.Animation;

namespace Skill.CodeGeneration
{
    public interface ICodeGenerator
    {
        string Extension { get; }
        void Generate(BehaviorTree bTree);
        void Generate(AnimationTree aTree);
        void Write(StreamWriter writer);
        void WritePartial(StreamWriter writer, string oldCode);
    }
}
