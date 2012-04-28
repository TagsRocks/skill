using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public abstract class AnimationTree
    {
        private AnimationLayerManager _LayerManager;
        private Dictionary<string, string> _Profiles;

        public int LayerCount { get { return _LayerManager.Layers.Count; } }
        public AnimationNode Root { get; private set; }
        public object UserData { get; set; }
        public Skill.Controllers.IController Controller { get; private set; }

        private string _CurrentProfile;
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
                        SetFormat(format, Root);
                    }
                }
            }
        }

        protected abstract AnimationNode CreateTree();

        public AnimationTree(Skill.Controllers.IController controller)
        {
            this.Controller = controller;
            this._Profiles = new Dictionary<string, string>();

            Root = CreateTree();
            _LayerManager = new AnimationLayerManager();
            Root.SetLayer(_LayerManager, _LayerManager.DefaultCrossFade);
            //Profile = "";
        }

        private void SetFormat(string format, AnimationNode node)
        {
            if (node is AnimationSequence)
            {
                ((AnimationSequence)node).Format = format;
            }
            else if (node.ChildCount > 0)
            {
                foreach (var child in node)
                {
                    if (child != null)
                    {
                        SetFormat(format, child);
                    }
                }
            }
        }

        public void AddProfile(string name, string format)
        {
            if (_Profiles.ContainsKey(name))
                throw new ArgumentException("This AnimationTree contains profile with same name");
            _Profiles.Add(name, format);
        }
        public bool RemoveProfile(string name)
        {
            return _Profiles.Remove(name);
        }

        public virtual void CollectInfo(UnityEngine.Animation animationComponent)
        {
            Root.CollectInfo(animationComponent);
        }
        public virtual void SyncLayers(UnityEngine.Animation animationComponent)
        {
            for (int i = 0; i < this.LayerCount; i++)
            {
                animationComponent.SyncLayer(i);
            }
        }

        public void BeginUpdate()
        {
            Root.Weight = 1;
            Root.Update();

            foreach (var layer in _LayerManager.Layers)
            {
                layer.Update();
            }
        }

        public void Apply(UnityEngine.Animation animationComponent)
        {
            foreach (var layer in _LayerManager.Layers)
            {
                layer.Apply(animationComponent);
            }
        }

        public void EndUpdate()
        {
            foreach (var layer in _LayerManager.Layers)
            {
                layer.CleanUpActiveList();
            }
        }

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
