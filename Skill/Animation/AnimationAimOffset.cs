using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{



    public class AnimationAimOffset : AnimationNode, ICollection<AnimationAimOffsetProfile>
    {
        private UnityEngine.Vector2 _Aim;
        private UnityEngine.Vector2 _AngleOffset;

        private List<AnimationAimOffsetProfile> _Profiles;
        private AnimationAimOffsetProfile _SelectedProfile = null;

        public event EventHandler ProfileChanged;

        public override float Length
        {
            get
            {
                float maxW = 0;
                AnimationNode maxChild = null;
                for (int i = 0; i < ChildCount; i++)
                {
                    AnimationNode child = this[i];
                    if (child != null)
                    {
                        if (child.Weight >= maxW)
                        {
                            maxW = child.Weight;
                            maxChild = child;
                        }
                    }
                }
                if (maxChild != null)
                    return maxChild.Length;
                return 0;
            }
        }

        public AnimationNode Normal { get { return this[0]; } set { this[0] = value; } }
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
        protected virtual void OnProfileChanged()
        {
            if (_SelectedProfile != null)
            {
                CenterCenter.AnimationName = _SelectedProfile.CenterCenter != null ? _SelectedProfile.CenterCenter : "";
                CenterUp.AnimationName = _SelectedProfile.CenterUp != null ? _SelectedProfile.CenterUp : "";
                CenterDown.AnimationName = _SelectedProfile.CenterDown != null ? _SelectedProfile.CenterDown : "";

                LeftCenter.AnimationName = _SelectedProfile.LeftCenter != null ? _SelectedProfile.LeftCenter : "";
                LeftUp.AnimationName = _SelectedProfile.LeftUp != null ? _SelectedProfile.LeftUp : "";
                LeftDown.AnimationName = _SelectedProfile.LeftDown != null ? _SelectedProfile.LeftDown : "";

                RightCenter.AnimationName = _SelectedProfile.RightCenter != null ? _SelectedProfile.RightCenter : "";
                RightUp.AnimationName = _SelectedProfile.RightUp != null ? _SelectedProfile.RightUp : "";
                RightDown.AnimationName = _SelectedProfile.RightDown != null ? _SelectedProfile.RightDown : "";

            }

            if (ProfileChanged != null) ProfileChanged(this, EventArgs.Empty);
        }

        public AnimationSequence CenterCenter { get { return (AnimationSequence)this[1]; } private set { this[1] = value; } }
        public AnimationSequence CenterUp { get { return (AnimationSequence)this[2]; } private set { this[2] = value; } }
        public AnimationSequence CenterDown { get { return (AnimationSequence)this[3]; } private set { this[3] = value; } }
        public AnimationSequence LeftCenter { get { return (AnimationSequence)this[4]; } private set { this[4] = value; } }
        public AnimationSequence LeftUp { get { return (AnimationSequence)this[5]; } private set { this[5] = value; } }
        public AnimationSequence LeftDown { get { return (AnimationSequence)this[6]; } private set { this[6] = value; } }
        public AnimationSequence RightCenter { get { return (AnimationSequence)this[7]; } private set { this[7] = value; } }
        public AnimationSequence RightUp { get { return (AnimationSequence)this[8]; } private set { this[8] = value; } }
        public AnimationSequence RightDown { get { return (AnimationSequence)this[9]; } private set { this[9] = value; } }

        public bool IsAimEnable { get; set; }

        public UnityEngine.Vector2 Aim { get { return _Aim; } set { _Aim = value; } }
        public UnityEngine.Vector2 AngleOffset { get { return _AngleOffset; } set { _AngleOffset = value; } }
        public float AimX { get { return _Aim.x; } set { _Aim.x = value; } }
        public float AimY { get { return _Aim.y; } set { _Aim.y = value; } }
        public float AngleOffsetX { get { return _AngleOffset.x; } set { _AngleOffset.x = value; } }
        public float AngleOffsetY { get { return _AngleOffset.y; } set { _AngleOffset.y = value; } }

        public AnimationAimOffset()
            : base(10)
        {
            this.CenterCenter = new AnimationSequence();
            this.CenterUp = new AnimationSequence();
            this.CenterDown = new AnimationSequence();
            this.LeftCenter = new AnimationSequence();
            this.LeftUp = new AnimationSequence();
            this.LeftDown = new AnimationSequence();
            this.RightCenter = new AnimationSequence();
            this.RightUp = new AnimationSequence();
            this.RightDown = new AnimationSequence();
            _Profiles = new List<AnimationAimOffsetProfile>();
        }

        protected override void Updating()
        {
            Normal.Weight = Weight;
            if (_SelectedProfile != null && IsAimEnable)
            {
                float x = UnityEngine.Mathf.Clamp(AimX + AngleOffsetX, -1, -1);
                float y = UnityEngine.Mathf.Clamp(AimY + AngleOffsetY, -1, -1);

                if (x < 0)// left side is enable
                {
                    DisableRight();
                    if (y < 0) // left down side is enable
                    {
                        LeftUp.Weight = 0;
                        CenterUp.Weight = 0;

                        LeftCenter.Weight = ((-x + (1.0f + y)) / 2) * Weight;
                        LeftDown.Weight = ((-x + (-y)) / 2) * Weight;
                        CenterCenter.Weight = (((1.0f + x) + (1.0f + y)) / 2) * Weight;
                        CenterDown.Weight = (((1.0f + x) + (-y)) / 2) * Weight;
                    }
                    else // left up side is enable
                    {
                        LeftDown.Weight = 0;
                        CenterDown.Weight = 0;

                        LeftCenter.Weight = ((-x + (1.0f - y)) / 2) * Weight;
                        LeftUp.Weight = ((-x + y) / 2) * Weight;
                        CenterCenter.Weight = (((1.0f + x) + (1.0f - y)) / 2) * Weight;
                        CenterUp.Weight = (((1.0f + x) + y) / 2) * Weight;
                    }
                }
                else// right side is enable
                {
                    DisableLeft();
                    if (y < 0) // right down side is enable
                    {
                        RightUp.Weight = 0;
                        CenterUp.Weight = 0;

                        RightCenter.Weight = ((x + (1.0f + y)) / 2) * Weight;
                        RightDown.Weight = ((x + (-y)) / 2) * Weight;
                        CenterCenter.Weight = (((1.0f - x) + (1.0f + y)) / 2) * Weight;
                        CenterDown.Weight = (((1.0f - x) + (-y)) / 2) * Weight;
                    }
                    else // right up side is enable
                    {
                        RightDown.Weight = 0;
                        CenterDown.Weight = 0;

                        RightCenter.Weight = ((x + (1.0f - y)) / 2) * Weight;
                        RightUp.Weight = ((x + y) / 2) * Weight;
                        CenterCenter.Weight = (((1.0f - x) + (1.0f - y)) / 2) * Weight;
                        CenterUp.Weight = (((1.0f - x) + y) / 2) * Weight;
                    }
                }
            }
            else
            {
                for (int i = 1; i < this.ChildCount; i++)
                {
                    this[i].Weight = 0;
                }
            }
        }

        private void DisableLeft()
        {
            LeftUp.Weight = 0;
            LeftDown.Weight = 0;
            LeftCenter.Weight = 0;
        }

        private void DisableRight()
        {
            RightUp.Weight = 0;
            RightDown.Weight = 0;
            RightCenter.Weight = 0;
        }

        public override void SetLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            Normal.SetLayer(manager, parentSuggestLayer);
            AnimationLayer layer = manager.NewLayer(LayerMode.BlendAll);
            this.CenterCenter.SetLayer(manager, layer);
            this.CenterUp.SetLayer(manager, layer);
            this.CenterDown.SetLayer(manager, layer);
            this.LeftCenter.SetLayer(manager, layer);
            this.LeftUp.SetLayer(manager, layer);
            this.LeftDown.SetLayer(manager, layer);
            this.RightCenter.SetLayer(manager, layer);
            this.RightUp.SetLayer(manager, layer);
            this.RightDown.SetLayer(manager, layer);
        }

        public override void Destroy()
        {
            _Profiles.Clear();
            base.Destroy();
        }

        public AnimationAimOffsetProfile GetProfile(string profile)
        {
            foreach (var item in _Profiles)
            {
                if (item.Name == profile)
                    return item;
            }
            return null;
        }

        public void Add(AnimationAimOffsetProfile item)
        {
            if (item == null)
                throw new ArgumentNullException("Invalid profile for AnimationAimOffset (profile is null)");
            if (GetProfile(item.Name) != null)
                throw new ArgumentException("profile with same name exist");
            _Profiles.Add(item);
            if (_SelectedProfile == null)
                _SelectedProfile = _Profiles[0];
        }

        public void Clear()
        {
            _Profiles.Clear();
        }

        public bool Contains(AnimationAimOffsetProfile item)
        {
            return _Profiles.Contains(item);
        }

        public void CopyTo(AnimationAimOffsetProfile[] array, int arrayIndex)
        {
            _Profiles.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Profiles.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(AnimationAimOffsetProfile item)
        {
            bool result = _Profiles.Remove(item);
            if (result && item == _SelectedProfile)
                if (Count > 0)
                    _SelectedProfile = _Profiles[0];
                else
                    _SelectedProfile = null;
            return result;
        }

        public new IEnumerator<AnimationAimOffsetProfile> GetEnumerator()
        {
            return _Profiles.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Profiles as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}
