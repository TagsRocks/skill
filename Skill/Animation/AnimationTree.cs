using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
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
    public abstract class AnimationTree
    {
        private UnityEngine.Vector3 _RootMotion;
        private AnimationTreeState _State;
        private Dictionary<string, string> _Profiles;// list of profiles

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
                    if (_CurrentProfile != null)
                    {
                        string format;
                        if (!_Profiles.TryGetValue(_CurrentProfile, out format))
                        {
                            format = "{0}";
                        }
                        Root.SetFormat(format);
                    }
                }
            }
        }

        /// <summary> Retrieves RootMotion at current frame </summary>
        public UnityEngine.Vector3 RootMotion { get { return _RootMotion; } }

        /// <summary>
        /// Override by subclass to create hierarchy of AnimNodes and return root node
        /// </summary>
        /// <returns>Root node of tree</returns>
        protected abstract AnimNode CreateTree();

        /// <summary>
        /// Create an instance of AnimationTree
        /// </summary>        
        public AnimationTree()
        {
            this._State = new AnimationTreeState(this);
            this._Profiles = new Dictionary<string, string>();

            Root = CreateTree();
            LayerManager = new AnimationLayerManager();
            Root.SelectLayer(LayerManager, LayerManager.Create(UnityEngine.AnimationBlendMode.Blend));
        }

        /// <summary>
        /// Add new profile
        /// </summary>
        /// <param name="name">Name of profile</param>
        /// <param name="format">Format of profile in C#</param>        
        public void AddProfile(string name, string format)
        {
            if (_Profiles.ContainsKey(name))
                throw new ArgumentException("This AnimationTree contains profile with same name");
            _Profiles.Add(name, format);
        }

        /// <summary>
        /// Remove profile by name
        /// </summary>
        /// <param name="name">Name of profile</param>
        /// <returns></returns>
        public bool RemoveProfile(string name)
        {
            return _Profiles.Remove(name);
        }

        /// <summary>
        /// Initialize UnityEngine.Animation. call this at 'Awake', or 'Start' method of MonoBehavior based class
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation to initialize</param>        
        public virtual void Initialize(UnityEngine.Animation animationComponent)
        {
            Root.Initialize(animationComponent);

            string currentProfile = Profile;
            foreach (var p in _Profiles)
            {
                if (p.Key == currentProfile) continue;
                Profile = p.Key;
                Root.Initialize(animationComponent);
            }
            Profile = currentProfile;
        }

        /// <summary>
        /// Sync all layers used in this AnimationTree. (maybe not useful because we do'nt use 'CrossFade' method)
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation to sync layers</param>
        public void SyncLayers(UnityEngine.Animation animationComponent)
        {
            foreach (var layer in LayerManager.Layers)
            {
                animationComponent.SyncLayer(layer.LayerIndex);
            }
        }

        /// <summary>
        /// Update AnimationTree in new state each frame
        /// </summary>
        /// <param name="controller">optional controller to send throw AnimNodes</param>
        public void Update(Controllers.Controller controller = null)
        {            
            _State.Controller = controller;
            foreach (var layer in LayerManager.Layers)
                layer.BeginUpdate();

            Root.Weight = 1;
            Root.Update(_State);

            _RootMotion = UnityEngine.Vector3.zero;
            foreach (var layer in LayerManager.Layers)
            {
                layer.Update();
                _RootMotion += layer.RootMotion;
            }
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
            {
                layer.Apply(animationComponent);
            }
        }

        //public void EndUpdate()
        //{
        //    foreach (var layer in _LayerManager.Layers)
        //    {
        //        layer.CleanUpActiveList();
        //    }
        //}

        /// <summary>
        /// Destroy hierarchy
        /// </summary>
        public void Destroy()
        {
            if (Root != null)
            {
                Root.Destroy();
            }
            Root = null;
        }
    }
}
