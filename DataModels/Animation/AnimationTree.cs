using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimationTree : IXElement, ICollection<AnimNode>
    {
        #region Properties
        /// <summary> Name of tree. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }

        /// <summary> array of connections. create internally when loading file and set before save  </summary>
        public AnimationConnection[] Connections { get; set; }

        public double Scale { get; set; }
        public double HorizontalOffset { get; set; }
        public double VerticalOffset { get; set; }

        public AnimationTreeProfile[] Profiles { get; set; }

        public string SkinMesh { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of Behavior Tree
        /// </summary>
        public AnimationTree()
        {
            this._Nodes = new List<AnimNode>();
            this.Name = "NewAnimationTree";
            this.SkinMesh = "";
            this.Scale = 1;
            this.Profiles = new AnimationTreeProfile[0];
            AddRoot();
        }

        void AddRoot()
        {
            if (_Nodes.Where(n => n.NodeType == AnimNodeType.Root).Count() == 0)
            {
                AnimationTreeRoot root = new AnimationTreeRoot();
                root.X = 100;
                root.Y = 200;
                this._Nodes.Add(root);
            }
        }
        #endregion

        #region ICollection<Behavior>
        List<AnimNode> _Nodes;
        public void Add(AnimNode item)
        {
            if (!Contains(item))
                _Nodes.Add(item);
        }

        public void Clear()
        {
            _Nodes.Clear();
        }

        public bool Contains(AnimNode item)
        {
            return _Nodes.Contains(item);
        }

        public void CopyTo(AnimNode[] array, int arrayIndex)
        {
            _Nodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Nodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(AnimNode item)
        {
            return _Nodes.Remove(item);
        }

        public IEnumerator<AnimNode> GetEnumerator()
        {
            return _Nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Nodes as System.Collections.IEnumerable).GetEnumerator();
        }

        public AnimNode Find(int id)
        {
            foreach (var item in this)
            {
                if (item.Id == id) return item;
            }
            return null;
        }
        #endregion

        #region Save

        private void GenerateIds()
        {
            int id = 0;
            // set all nodes id to -1
            foreach (var node in this)
            {
                node.Id = id++;
            }
        }

        public XElement ToXElement()
        {
            GenerateIds();

            XElement animationTree = new XElement("AnimationTree");
            animationTree.SetAttributeValue("Name", Name);
            animationTree.SetAttributeValue("SkinMesh", SkinMesh != null ? SkinMesh : "");


            animationTree.SetAttributeValue("Scale", Scale);
            animationTree.SetAttributeValue("HorizontalOffset", HorizontalOffset);
            animationTree.SetAttributeValue("VerticalOffset", VerticalOffset);

            XElement profiles = new XElement("Profiles");
            if (Profiles != null)
            {
                profiles.SetAttributeValue("Count", Profiles.Length);
                foreach (var item in Profiles)
                {
                    XElement p = item.ToXElement();
                    profiles.Add(p);
                }
            }
            animationTree.Add(profiles);


            XElement nodes = new XElement("Nodes");
            nodes.SetAttributeValue("Count", Count);
            // write each behavior without children hierarchy
            // children will be wrote in array of ids in selector and an id in decorator
            foreach (var item in this)
            {
                XElement n = item.ToXElement();
                nodes.Add(n);
            }
            animationTree.Add(nodes);

            XElement connections = new XElement("Connections");
            // Write Connections            
            if (this.Connections != null)
            {
                foreach (var connection in this.Connections)
                {
                    connections.Add(connection.Info.ToXElement());
                }
            }
            animationTree.Add(connections);
            AddRoot();

            return animationTree;
        }


        #endregion

        #region Load

        public static AnimNode CreateNode(AnimNodeType nodeType)
        {
            AnimNode result = null;
            switch (nodeType)
            {
                case AnimNodeType.Sequence:
                    result = new AnimNodeSequence();
                    break;
                case AnimNodeType.Override:
                    result = new AnimNodeOverride();
                    break;
                case AnimNodeType.BlendBySpeed:
                    result = new AnimNodeBlendBySpeed();
                    break;
                case AnimNodeType.BlendByPosture:
                    result = new AnimNodeBlendByPosture();
                    break;
                case AnimNodeType.BlendByIdle:
                    result = new AnimNodeBlendByIdle();
                    break;
                case AnimNodeType.Blend4Directional:
                    result = new AnimNodeBlend4Directional();
                    break;
                case AnimNodeType.AimOffset:
                    result = new AnimNodeAimOffset();
                    break;
                case AnimNodeType.AdditiveBlending:
                    result = new AnimNodeAdditiveBlending();
                    break;
                case AnimNodeType.Random:
                    result = new AnimNodeRandom();
                    break;
                case AnimNodeType.BlendByIndex:
                    result = new AnimNodeBlendByIndex();
                    break;
                case AnimNodeType.Root:
                    result = new AnimationTreeRoot();
                    break;
            }
            return result;
        }

        public static AnimNode CreateNode(XElement node)
        {
            AnimNode result = null;
            AnimNodeType nodeType = AnimNodeType.None;

            bool isCorrect = false;
            try
            {
                nodeType = (AnimNodeType)Enum.Parse(typeof(AnimNodeType), node.Name.ToString(), false);
                isCorrect = true;
            }
            catch (Exception)
            {
                isCorrect = false;
            }
            if (isCorrect)
            {
                result = CreateNode(nodeType);
            }
            return result;
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "");
            this.SkinMesh = e.GetAttributeValueAsString("SkinMesh", "");
            this.Scale = e.GetAttributeValueAsDouble("Scale", Scale);
            this.HorizontalOffset = e.GetAttributeValueAsDouble("HorizontalOffset", HorizontalOffset);
            this.VerticalOffset = e.GetAttributeValueAsDouble("VerticalOffset", VerticalOffset);

            XElement profiles = e.FindChildByName("Profiles");
            if (profiles != null)
            {
                int profileCount = profiles.GetAttributeValueAsInt("Count", 0);
                this.Profiles = new AnimationTreeProfile[profileCount];
                int i = 0;
                foreach (var element in profiles.Elements().Where(p => p.Name == "Profile"))
                {
                    AnimationTreeProfile p = new AnimationTreeProfile();
                    p.Load(element);
                    this.Profiles[i++] = p;
                }
            }

            XElement nodes = e.Elements().First(p => p.Name == "Nodes");
            int count = int.Parse(nodes.Attribute("Count").Value);
            Clear();
            foreach (var item in nodes.Elements())
            {
                AnimNode node = CreateNode(item);
                if (node != null)
                {
                    node.Load(item);
                    this.Add(node);
                }
            }

            // create connections
            XElement connections = e.FindChildByName("Connections");
            List<AnimationConnection> animationConnections = new List<AnimationConnection>();
            if (connections != null)
            {
                foreach (var element in connections.Elements())
                {
                    AnimationConnectionInfo info = new AnimationConnectionInfo();
                    info.Load(element);

                    AnimationConnection animationConnection = new AnimationConnection(Find(info.SourceId), Find(info.SinkId), info.SinkConnectorIndex);
                    animationConnections.Add(animationConnection);
                }
            }
            this.Connections = animationConnections.ToArray();
        }
        #endregion
    }
}
