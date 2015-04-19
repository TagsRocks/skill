using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// Base class for AnimationTrees
    /// </summary>
    /// <remarks>
    /// The AnimationTree use unity Animation Layers system ( see .... )
    /// All AnimNodes in AnimationTre seprated to two category SingleLayer and MultiLayer
    /// SingleLayers AnimNodes use single layer to blend between their children, whitch means use same layer index for all children
    /// MultiLayer AnimNodes use a layer per child and blend them togather (like BlendByIdle)
    /// to know about which AnimNode inherites from SingleLayer or MultiLayer see hierarchy inheritance of classes
    /// the idea of designing this AnimationTree system comes from Unreal Engine AnimaionTree ( see : http://udn.epicgames.com/Three/AnimationNodes.html )
    /// </remarks>

    [RequireComponent(typeof(UnityEngine.Animation))]
    public abstract class AnimationTree : DynamicBehaviour
    {
        private class AnimationTreeProfile
        {
            public string Name { get; private set; }
            public string Format { get; private set; }

            public AnimationTreeProfile(string name, string format)
            {
                this.Name = name;
                this.Format = format;
            }
        }

        public bool ApplyRootMotion = false;
        
        private bool _RootMotionChanged;
        private UnityEngine.Vector3 _DeltaPosition;
        private UnityEngine.Vector3 _DeltaRotation;

        private List<AnimationTreeProfile> _Profiles;// list of profiles
        public UnityEngine.Animation Animation { get; private set; }

        /// <summary>
        /// Maximum layer used by this AnimationTree
        /// </summary>
        public int LayerCount { get { return LayerManager.Layers.Count; } }
        /// <summary>
        /// The root AnimNode
        /// </summary>
        public AnimNode Root { get; private set; }
        /// <summary>
        /// User data
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// LayerManager
        /// </summary>
        public AnimationLayerManager LayerManager { get; private set; }


        private string _CurrentProfile;// current selected profile

        /// <summary>
        /// Get or set profile by name
        /// </summary>
        /// <remarks>
        /// Profiles can be used in situations like : you want to swich weapon type (Rifle, Pistol, ...)
        /// all of this weapons have same AnimationClips (Reload, Fire, RunForward, WalkForward, ...).
        /// in such situations it's possible to name AnimationClips in standard format like this :
        /// 
        /// Rifle_Reload,  Rifle_Fire,  Rifle_RunForward,  Rifle_WalkForward,   ...
        /// Pistol_Reload, Pistol_Fire, Pistol_RunForward, Pistol_WalkForward, ...
        /// 
        /// by this naming method the formats of profiles can be
        /// 
        /// Rifle_{0}
        /// Pistol_{0}
        ///         
        /// then set AnimationName property of AnimNodeSequences to
        /// 
        /// Reload,  Fire,  RunForward,  WalkForward,   ...
        /// 
        /// by this method at any time you can switch between weapons.
        /// </remarks>
        public string Profile
        {
            get { return _CurrentProfile; }
            set
            {
                if (_CurrentProfile != value)
                {
                    _CurrentProfile = value;
                    string format = "{0}";
                    if (!string.IsNullOrEmpty(_CurrentProfile))
                    {
                        foreach (var item in _Profiles)
                        {
                            if (item.Name.Equals(_CurrentProfile, StringComparison.OrdinalIgnoreCase))
                                format = item.Format;
                        }
                    }
                    Root.SetFormat(format);
                }
            }
        }

        /// <summary> Delta root position since previous update </summary>
        public UnityEngine.Vector3 DeltaPosition { get { return _DeltaPosition; } set { _DeltaPosition = value; } }

        /// <summary> Delta root rotation since previous update </summary>
        public UnityEngine.Vector3 DeltaRotation { get { return _DeltaRotation * Mathf.Rad2Deg; } set { _DeltaRotation = value * Mathf.Deg2Rad; } }

        /// <summary>
        /// Override by subclass to create hierarchy of AnimNodes and return root node
        /// </summary>
        /// <returns>Root node of tree</returns>
        protected abstract AnimNode CreateTree();


        protected override void GetReferences()
        {            
            this.Animation = GetComponent<UnityEngine.Animation>();
            if (this.Animation == null)
                UnityEngine.Debug.Log("AnimationTree needs a UnityEngine.Animation component");
            base.GetReferences();
        }

        protected override void Awake()
        {
            base.Awake();
            this._Profiles = new List<AnimationTreeProfile>();
            Root = CreateTree();
            LayerManager = new AnimationLayerManager();
            Root.SelectLayer(LayerManager, LayerManager.Create(UnityEngine.AnimationBlendMode.Blend));
            if (this.Animation != null)
                Root.Initialize(this.Animation);
        }

        /// <summary>
        /// Add new profile
        /// </summary>
        /// <param name="name">Name of profile</param>
        /// <param name="format">Format of profile in C#</param>        
        public void AddProfile(string name, string format)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(format))
                throw new ArgumentNullException("Invalid AnimationTree profile");

            if (!format.Contains("{0}"))
                throw new ArgumentNullException("Invalid AnimationTree profile format");

            foreach (var item in _Profiles)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException(string.Format("The AnimationTree already contains profile with name '{0}'", name));
            }
            _Profiles.Add(new AnimationTreeProfile(name, format));
        }

        /// <summary>
        /// Remove profile by name
        /// </summary>
        /// <param name="name">Name of profile</param>
        /// <returns></returns>
        public bool RemoveProfile(string name)
        {
            AnimationTreeProfile profile = null;
            foreach (var item in _Profiles)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    profile = item;
                    break;
                }
            }

            if (profile != null)
                return _Profiles.Remove(profile);

            return false;
        }



        /// <summary>
        /// Update AnimationTree in new state each frame
        /// </summary>
        protected override void Update()
        {
            foreach (var layer in LayerManager.Layers)
                layer.BeginUpdate();

            Root.BlendWeight.SetBoth(1.0f);
            Root.Update();

            if (ApplyRootMotion)
            {
                foreach (var layer in LayerManager.Layers)
                {
                    layer.Update();
                    _DeltaPosition += layer.DeltaPosition;
                    _DeltaRotation += layer.DeltaRotation;
                }
                _RootMotionChanged = true;
            }

            Apply(this.Animation);

            base.Update();
        }

        protected virtual void FixedUpdate()
        {
            if (ApplyRootMotion && _RootMotionChanged)
            {
                if (_Rigidbody != null)
                {
                    _Rigidbody.MovePosition(transform.position + transform.TransformDirection(_DeltaPosition));
                }
                else
                {
                    transform.position += _DeltaPosition;
                }
            }
            _DeltaRotation = _DeltaPosition = UnityEngine.Vector3.zero;
            _RootMotionChanged = false;
        }

        /// <summary>
        /// Apply AnimationTree
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation to update</param>                       
        /// <remarks>
        /// you can apply this AnimationTree to more than one UnityEngine.Animation each frame (instancing)
        /// Make sure default AnimationClip of Animation setted to 'none' in editor
        /// </remarks>
        public void Apply(UnityEngine.Animation animationComponent)
        {
            if (animationComponent.clip != null)
                animationComponent[animationComponent.clip.name].enabled = false;
            animationComponent.clip = null;
            foreach (var layer in LayerManager.Layers)
                layer.Apply(animationComponent);
        }

        /// <summary>
        /// Destroy hierarchy
        /// </summary>
        protected override void OnDestroy()
        {
            if (Root != null)
                Root.Destroy();
            Root = null;
            base.OnDestroy();
        }
    }
}
