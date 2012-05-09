using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// In a game, you often want a weapon held by a character to point where that player is aiming.
    /// Because an actor is defined by a collision cylinder that is only able to rotate on the yaw axis and not the pitch or roll axis,
    /// it's difficult to simply rotate the actor to make the player's aim.
    /// Even then, you may want the character's feet to remain locked in place.
    /// 
    /// for more information : 
    /// http://udn.epicgames.com/Three/AnimationNodes.html#AnimNodeAimOffset
    /// </summary>
    public class AnimNodeAimOffset : AnimNodeBlendBase
    {
        /// <summary> Index of CenterCenter child</summary>
        public const int CenterCenterIndex = 1;
        /// <summary> Index of Centerup child</summary>
        public const int CenterUpIndex = 2;
        /// <summary> Index of CenterDown child</summary>
        public const int CenterDownIndex = 3;
        /// <summary> Index of LeftCenter child</summary>
        public const int LeftCenterIndex = 4;
        /// <summary> Index of LeftUp child</summary>
        public const int LeftUpIndex = 5;
        /// <summary> Index of LeftDown child</summary>
        public const int LeftDownIndex = 6;
        /// <summary> Index of RightCenter child</summary>
        public const int RightCenterIndex = 7;
        /// <summary> Index of RightUp child</summary>
        public const int RightUpIndex = 8;
        /// <summary> Index of RightDown child</summary>
        public const int RightDownIndex = 9;



        private UnityEngine.Vector2 _Aim; // current aim vector
        private UnityEngine.Vector2 _AngleOffset; // current aim offset vector

        private List<AnimNodeAimOffsetProfile> _Profiles; // list of profiles
        private AnimNodeAimOffsetProfile _SelectedProfile = null; // selected profile

        /// <summary>
        /// Whether use AnimationTree profiling method?
        /// </summary>
        public bool UseTreeProfile { get; set; }

        /// <summary>
        /// Occurs when profile changed
        /// </summary>
        public event EventHandler ProfileChanged;

        /// <summary>
        /// The AnimNode that use input blendmode specified by parent
        /// </summary>
        public AnimNode NormalNode { get { return this[0]; } set { this[0] = value; } }

        /// <summary>
        /// Get of set profile
        /// </summary>
        public string Profile
        {
            get { return (_SelectedProfile != null) ? _SelectedProfile.Name : ""; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _SelectedProfile = null;
                else
                {
                    var p = GetProfile(value);
                    if (p != _SelectedProfile)
                    {
                        _SelectedProfile = p;
                        OnProfileChanged();
                    }
                }
            }
        }

        /// <summary>
        /// call ProfileChanged event and set new profile to children
        /// </summary>
        protected virtual void OnProfileChanged()
        {
            if (_SelectedProfile != null)
            {
                for (int i = 1; i < ChildCount; i++)
                {
                    AnimNodeSequence child = (AnimNodeSequence)this[i];
                    child.UseTreeProfile = UseTreeProfile;
                    child.AnimationName = _SelectedProfile.Animations[i - 1];
                }
            }

            if (ProfileChanged != null) ProfileChanged(this, EventArgs.Empty);
        }

        /// <summary> Retrieves CenterCenter child node </summary>
        public AnimNodeSequence CenterCenterNode { get { return (AnimNodeSequence)this[CenterCenterIndex]; } private set { this[CenterCenterIndex] = value; } }
        /// <summary> Retrieves CenterUp child node </summary>
        public AnimNodeSequence CenterUpNode { get { return (AnimNodeSequence)this[CenterUpIndex]; } private set { this[CenterUpIndex] = value; } }
        /// <summary> Retrieves CenterDown child node </summary>
        public AnimNodeSequence CenterDownNode { get { return (AnimNodeSequence)this[CenterDownIndex]; } private set { this[CenterDownIndex] = value; } }
        /// <summary> Retrieves LeftCenter child node </summary>
        public AnimNodeSequence LeftCenterNode { get { return (AnimNodeSequence)this[LeftCenterIndex]; } private set { this[LeftCenterIndex] = value; } }
        /// <summary> Retrieves LeftUp child node </summary>
        public AnimNodeSequence LeftUpNode { get { return (AnimNodeSequence)this[LeftUpIndex]; } private set { this[LeftUpIndex] = value; } }
        /// <summary> Retrieves LeftDown child node </summary>
        public AnimNodeSequence LeftDownNode { get { return (AnimNodeSequence)this[LeftDownIndex]; } private set { this[LeftDownIndex] = value; } }
        /// <summary> Retrieves RightCenter child node </summary>
        public AnimNodeSequence RightCenterNode { get { return (AnimNodeSequence)this[RightCenterIndex]; } private set { this[RightCenterIndex] = value; } }
        /// <summary> Retrieves RightUp child node </summary>
        public AnimNodeSequence RightUpNode { get { return (AnimNodeSequence)this[RightUpIndex]; } private set { this[RightUpIndex] = value; } }
        /// <summary> Retrieves RightDown child node </summary>
        public AnimNodeSequence RightDownNode { get { return (AnimNodeSequence)this[RightDownIndex]; } private set { this[RightDownIndex] = value; } }

        /// <summary>
        /// Whether anim layer is enable?
        /// </summary>
        public bool IsAimEnable { get; set; }

        /// <summary> Normalized vector aim offset./ </summary>
        public UnityEngine.Vector2 Aim { get { return _Aim; } set { _Aim = value; } }
        /// <summary> Aim offset to append to Aim before processing.</summary>
        public UnityEngine.Vector2 AngleOffset { get { return _AngleOffset; } set { _AngleOffset = value; } }

        /// <summary> Normalized horizontal aim offset. </summary>
        public float AimX { get { return _Aim.x; } set { _Aim.x = value; } }
        /// <summary> Normalized veritcal aim offset. </summary>
        public float AimY { get { return _Aim.y; } set { _Aim.y = value; } }
        /// <summary> Horizontal aim offset to append to Aim before processing. </summary>
        public float AngleOffsetX { get { return _AngleOffset.x; } set { _AngleOffset.x = value; } }
        /// <summary> Vertical aim offset to append to Aim before processing. </summary>
        public float AngleOffsetY { get { return _AngleOffset.y; } set { _AngleOffset.y = value; } }

        /// <summary>
        /// Create an instance of AnimNodeAimOffset node
        /// </summary>
        public AnimNodeAimOffset()
            : base(10)
        {
            this.UseTreeProfile = true;
            _Profiles = new List<AnimNodeAimOffsetProfile>();
            for (int i = 1; i < ChildCount; i++)
            {
                this[i] = new AnimNodeSequence() { WrapMode = UnityEngine.WrapMode.ClampForever };
            }
        }

        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>
        protected override void CalcBlendWeights(ref float[] blendWeights)
        {
            blendWeights[0] = 1;// normal node is 1
            if (_SelectedProfile != null && IsAimEnable)
            {
                float x = UnityEngine.Mathf.Clamp(AimX + AngleOffsetX, -1, 1);
                float y = UnityEngine.Mathf.Clamp(AimY + AngleOffsetY, -1, 1);

                if (x < 0)// left side is enable
                {
                    DisableRight(ref blendWeights);
                    if (y < 0) // left down side is enable
                    {
                        blendWeights[LeftUpIndex] = 0;
                        blendWeights[CenterUpIndex] = 0;

                        blendWeights[LeftCenterIndex] = ((-x + (1.0f + y)) / 2);
                        blendWeights[LeftDownIndex] = ((-x + (-y)) / 2);
                        blendWeights[CenterCenterIndex] = (((1.0f + x) + (1.0f + y)) / 2);
                        blendWeights[CenterDownIndex] = (((1.0f + x) + (-y)) / 2);
                    }
                    else // left up side is enable
                    {
                        blendWeights[LeftDownIndex] = 0;
                        blendWeights[CenterDownIndex] = 0;

                        blendWeights[LeftCenterIndex] = ((-x + (1.0f - y)) / 2);
                        blendWeights[LeftUpIndex] = ((-x + y) / 2);
                        blendWeights[CenterCenterIndex] = (((1.0f + x) + (1.0f - y)) / 2);
                        blendWeights[CenterUpIndex] = (((1.0f + x) + y) / 2);
                    }
                }
                else// right side is enable
                {
                    DisableLeft(ref blendWeights);
                    if (y < 0) // right down side is enable
                    {
                        blendWeights[RightUpIndex] = 0;
                        blendWeights[CenterUpIndex] = 0;

                        blendWeights[RightCenterIndex] = ((x + (1.0f + y)) / 2);
                        blendWeights[RightDownIndex] = ((x + (-y)) / 2);
                        blendWeights[CenterCenterIndex] = (((1.0f - x) + (1.0f + y)) / 2);
                        blendWeights[CenterDownIndex] = (((1.0f - x) + (-y)) / 2);
                    }
                    else // right up side is enable
                    {
                        blendWeights[RightDownIndex] = 0;
                        blendWeights[CenterDownIndex] = 0;

                        blendWeights[RightCenterIndex] = ((x + (1.0f - y)) / 2);
                        blendWeights[RightUpIndex] = ((x + y) / 2);
                        blendWeights[CenterCenterIndex] = (((1.0f - x) + (1.0f - y)) / 2);
                        blendWeights[CenterUpIndex] = (((1.0f - x) + y) / 2);
                    }
                }
            }
            else
            {
                for (int i = 1; i < this.ChildCount; i++)
                {
                    blendWeights[i] = 0;
                }
            }
        }

        private void DisableLeft(ref float[] blendWeights)
        {
            blendWeights[LeftUpIndex] = 0;
            blendWeights[LeftDownIndex] = 0;
            blendWeights[LeftCenterIndex] = 0;
        }

        private void DisableRight(ref float[] blendWeights)
        {
            blendWeights[RightUpIndex] = 0;
            blendWeights[RightDownIndex] = 0;
            blendWeights[RightCenterIndex] = 0;
        }

        /// <summary>
        /// Allow each node to get apropriate AnimationLayer
        /// </summary>
        /// <param name="manager">LayerManager to create layer</param>
        /// <param name="parentSuggestLayer">AnimationLayer suggested by parent. (layer of child at index 0)</param>
        public override void SelectLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            NormalNode.SelectLayer(manager, parentSuggestLayer);
            AnimationLayer layer = manager.Create(UnityEngine.AnimationBlendMode.Blend);
            for (int i = 1; i < ChildCount; i++)
            {
                this[i].SelectLayer(manager, layer);
            }
        }

        /// <summary>
        /// Destroy hierarchy of Children
        /// </summary>
        public override void Destroy()
        {
            _Profiles.Clear();
            base.Destroy();
        }

        /// <summary>
        /// Get profile by index
        /// </summary>
        /// <param name="profile">name of profile</param>
        /// <returns>if success AnimNodeAimOffsetProfile, otherwise null</returns>
        public AnimNodeAimOffsetProfile GetProfile(string profile)
        {
            foreach (var item in _Profiles)
            {
                if (item.Name == profile)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Add new AnimNodeAimOffsetProfile
        /// </summary>
        /// <param name="profile">AnimNodeAimOffsetProfile to add</param>
        public void AddProfile(AnimNodeAimOffsetProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("Invalid profile for AnimationAimOffset (profile is null)");
            if (GetProfile(profile.Name) != null)
                throw new ArgumentException("profile with same name exist");
            _Profiles.Add(profile);
            if (_SelectedProfile == null)
                Profile = _Profiles[0].Name;
        }

        /// <summary>
        /// Remove all profiles
        /// </summary>
        public void RemoveAllProfiles()
        {
            _Profiles.Clear();
        }

        /// <summary>
        /// Determines whether specified profile is in profiles
        /// </summary>
        /// <param name="profile">profile to check</param>
        /// <returns>true if contains, otherwise false</returns>
        public bool Contains(AnimNodeAimOffsetProfile profile)
        {
            return _Profiles.Contains(profile);
        }

        /// <summary>
        /// Retrieves number of profiles
        /// </summary>
        public int ProfileCount
        {
            get { return _Profiles.Count; }
        }

        /// <summary>
        /// Remove specified profile
        /// </summary>
        /// <param name="profile">profile to remove</param>
        /// <returns>true for success, otherwise false</returns>
        public bool RemoveProfile(AnimNodeAimOffsetProfile profile)
        {
            bool result = _Profiles.Remove(profile);
            if (result && profile == _SelectedProfile)
                if (ProfileCount > 0)
                    _SelectedProfile = _Profiles[0];
                else
                    _SelectedProfile = null;
            return result;
        }
    }
}
