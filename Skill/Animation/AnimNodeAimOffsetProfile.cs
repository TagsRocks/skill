using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Represent data required for AnimNodeAimOffset profile 
    /// </summary>
    public class AnimNodeAimOffsetProfile
    {
        private AnimNodeSequence[] _Sequences;

        /// <summary>
        /// Retrieves AnimNodeSequence by index
        /// </summary>
        /// <param name="index">Zero based index between 0 - 9</param>
        /// <returns>AnimNodeSequence</returns>
        public AnimNodeSequence this[int index] { get { return _Sequences[index]; } }

        /// <summary> Name of Profile </summary>
        public string Name;

        /// <summary> AnimationName of CenterCenter child</summary>
        public string CenterCenter { get { return _Sequences[AnimNodeAimOffset.CenterCenterIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.CenterCenterIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of CenterUp child</summary>
        public string CenterUp { get { return _Sequences[AnimNodeAimOffset.CenterUpIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.CenterUpIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of CenterDown child</summary>
        public string CenterDown { get { return _Sequences[AnimNodeAimOffset.CenterDownIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.CenterDownIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of LeftCenter child</summary>
        public string LeftCenter { get { return _Sequences[AnimNodeAimOffset.LeftCenterIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.LeftCenterIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of LeftUp child</summary>
        public string LeftUp { get { return _Sequences[AnimNodeAimOffset.LeftUpIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.LeftUpIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of LeftDown child</summary>
        public string LeftDown { get { return _Sequences[AnimNodeAimOffset.LeftDownIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.LeftDownIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of RightCenter child</summary>
        public string RightCenter { get { return _Sequences[AnimNodeAimOffset.RightCenterIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.RightCenterIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of RightUp child</summary>
        public string RightUp { get { return _Sequences[AnimNodeAimOffset.RightUpIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.RightUpIndex - 1].AnimationName = value; } }
        /// <summary> AnimationName of RightDown child</summary>
        public string RightDown { get { return _Sequences[AnimNodeAimOffset.RightDownIndex - 1].AnimationName; } set { _Sequences[AnimNodeAimOffset.RightDownIndex - 1].AnimationName = value; } }


        /// <summary> MixingTransforms of CenterCenter child</summary>
        public string[] CenterCenterMTs { get { return _Sequences[AnimNodeAimOffset.CenterCenterIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.CenterCenterIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of CenterUp child</summary>
        public string[] CenterUpMTs { get { return _Sequences[AnimNodeAimOffset.CenterUpIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.CenterUpIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of CenterDown child</summary>
        public string[] CenterDownMTs { get { return _Sequences[AnimNodeAimOffset.CenterDownIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.CenterDownIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of LeftCenter child</summary>
        public string[] LeftCenterMTs { get { return _Sequences[AnimNodeAimOffset.LeftCenterIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.LeftCenterIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of LeftUp child</summary>
        public string[] LeftUpMTs { get { return _Sequences[AnimNodeAimOffset.LeftUpIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.LeftUpIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of LeftDown child</summary>
        public string[] LeftDownMTs { get { return _Sequences[AnimNodeAimOffset.LeftDownIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.LeftDownIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of RightCenter child</summary>
        public string[] RightCenterMTs { get { return _Sequences[AnimNodeAimOffset.RightCenterIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.RightCenterIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of RightUp child</summary>
        public string[] RightUpMTs { get { return _Sequences[AnimNodeAimOffset.RightUpIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.RightUpIndex - 1].MixingTransforms = value; } }
        /// <summary> MixingTransforms of RightDown child</summary>
        public string[] RightDownMTs { get { return _Sequences[AnimNodeAimOffset.RightDownIndex - 1].MixingTransforms; } set { _Sequences[AnimNodeAimOffset.RightDownIndex - 1].MixingTransforms = value; } }



        /// <summary>
        /// Create an instance of AnimNodeAimOffsetProfile
        /// </summary>
        public AnimNodeAimOffsetProfile()
        {
            this._Sequences = new AnimNodeSequence[9];
            for (int i = 0; i < _Sequences.Length; i++)
                this._Sequences[i] = new AnimNodeSequence() { WrapMode = UnityEngine.WrapMode.ClampForever };
        }        
    }
}
