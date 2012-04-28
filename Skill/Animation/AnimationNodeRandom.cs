using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class RandomInfo
    {
        /// <summary> Chance that this child will be picked. If all indexes are zero, then every index has an equal chance. </summary>
        public float Chance { get; set; }
        /// <summary> Minimum number of loops to play this animation. If zero, only plays once.  </summary>
        public int LoopCountMin { get; set; }
        /// <summary> Maximum number of loops to play this animation. </summary>
        public int LoopCoundMax { get; set; }
        /// <summary> Time to blend into this index. </summary>
        public float BlendInTime { get; set; }

        public RandomInfo()
        {
            Chance = 1.0f;
            LoopCountMin = LoopCoundMax = 1;
            BlendInTime = 0.3f;
        }
    }

    public class AnimationNodeRandom : AnimationSwitchBase
    {
        public float[] Chances { get; private set; }

        public float TotalChance
        {
            get
            {
                float sum = 0;
                for (int i = 0; i < ChildCount; i++)
                {
                    sum += Chances[i];
                }
                return sum;
            }
        }

        private int _ActiveChildIndex;

        public AnimationNodeRandom(int childCoun)
            : base(childCoun)
        {
            Chances = new float[childCoun];
            for (int i = 0; i < childCoun; i++) Chances[i] = 1.0f;
        }

        protected override void OnBecameRelevant()
        {
            _ActiveChildIndex = PickRandomChildIndex();            
            base.OnBecameRelevant();
        }

        protected override int SelectActiveChildIndex()
        {
            return _ActiveChildIndex;
        }

        private int PickRandomChildIndex()
        {
            float total = TotalChance;

            var rnd = UnityEngine.Random.Range(0, total);
            for (int i = 0; i < ChildCount; i++)
            {
                float weight = Chances[i];
                if (rnd < weight)
                    return i;
                rnd -= weight;
            }
            return 0;
        }


    }
}
