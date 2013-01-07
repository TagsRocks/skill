using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Framework.AI;

namespace Skill.Studio.AI.Editor
{
    class DebugRandomService : IRandomService
    {
        Random _Random;

        public DebugRandomService()
        {
            _Random = new Random();
        }

        public float Range(float min, float max)
        {
            return Convert.ToSingle(min + _Random.NextDouble() * (max - min));
        }
    }
}
