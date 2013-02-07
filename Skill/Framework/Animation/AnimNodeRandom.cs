using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    //public class RandomInfo
    //{
    //    /// <summary> Chance that this child will be picked. If all indexes are zero, then every index has an equal chance. </summary>
    //    public float Chance { get; set; }
    //    /// <summary> Minimum number of loops to play this animation. If zero, only plays once.  </summary>
    //    public int LoopCountMin { get; set; }
    //    /// <summary> Maximum number of loops to play this animation. </summary>
    //    public int LoopCoundMax { get; set; }
    //    /// <summary> Time to blend into this index. </summary>
    //    public float BlendInTime { get; set; }

    //    public RandomInfo()
    //    {
    //        Chance = 1.0f;
    //        LoopCountMin = LoopCoundMax = 1;
    //        BlendInTime = 0.3f;
    //    }
    //}

    /// <summary>
    /// This blend node allows the Anim Tree to randomly blend between inputs set by the user.
    /// select random node when BecameRelevant and continue to update that until cease relevant.
    /// </summary>
    public class AnimNodeRandom : AnimNodeBlendByIndex
    {
        /// <summary>
        /// Get or set chance of each node. (make sure chances be >0)
        /// </summary>
        public float[] Chances { get; private set; }

        /// <summary>
        /// Sum of all chances
        /// </summary>
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

        /// <summary>
        /// Create new instance of AnimNodeRandom
        /// </summary>
        /// <param name="childCoun">number of children</param>
        public AnimNodeRandom(int childCoun)
            : base(childCoun)
        {
            Chances = new float[childCoun];
            for (int i = 0; i < childCoun; i++) Chances[i] = 1.0f;
        }

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnBecameRelevant(AnimationTreeState state)
        {
            SelectedChildIndex = PickRandomChildIndex();
            base.OnBecameRelevant(state);
        }

        /// <summary>
        /// Select a child index based i\on there chances
        /// </summary>
        /// <returns></returns>
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
