using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.Editor.Animation
{
    public class AnimationTree : IXElement, ICollection<AnimationNode>
    {
        #region static Variables
        public static string Extension = ".sat";// extension of editor file
        public static string FilterExtension = "Animation Tree|*" + Extension;// filter that used by OpenFileDialog 
        #endregion

        #region Properties
        /// <summary> Name of tree. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }

        /// <summary> array of connections. create internally when loading file and set before save  </summary>
        public AnimationConnection[] Connections { get; set; }

        public double Scale { get; set; }
        public double HorizontalOffset { get; set; }
        public double VerticalOffset { get; set; }

        public AnimationTreeRoot Root
        {
            get
            {
                AddRoot();
                AnimationTreeRoot root = null;
                foreach (AnimationNode node in this)
                {
                    if (node.NodeType == AnimationNodeType.Root)
                    {
                        root = node as AnimationTreeRoot;
                        break;
                    }
                }
                return root;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of Behavior Tree
        /// </summary>
        public AnimationTree()
        {
            this._Nodes = new List<AnimationNode>();
            this.Name = "NewAnimationTree";
            this.Scale = 1;
            AddRoot();
        }

        void AddRoot()
        {
            if (_Nodes.Where(n => n.NodeType == AnimationNodeType.Root).Count() == 0)
            {
                AnimationTreeRoot root = new AnimationTreeRoot();
                root.X = 100;
                root.Y = 200;
                this._Nodes.Add(root);
            }
        }
        #endregion

        #region ICollection<Behavior>
        List<AnimationNode> _Nodes;
        public void Add(AnimationNode item)
        {
            if (!Contains(item))
                _Nodes.Add(item);
        }

        public void Clear()
        {
            _Nodes.Clear();
        }

        public bool Contains(AnimationNode item)
        {
            return _Nodes.Contains(item);
        }

        public void CopyTo(AnimationNode[] array, int arrayIndex)
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

        public bool Remove(AnimationNode item)
        {
            return _Nodes.Remove(item);
        }

        public IEnumerator<AnimationNode> GetEnumerator()
        {
            return _Nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Nodes as System.Collections.IEnumerable).GetEnumerator();
        }

        public AnimationNode Find(int id)
        {
            foreach (var item in this)
            {
                if (item.Id == id) return item;
            }
            return null;
        }
        #endregion

        #region Save

        public XElement ToXElement()
        {
            XElement animationTree = new XElement("AnimationTree");
            animationTree.SetAttributeValue("Name", Name);

            animationTree.SetAttributeValue("Scale", Scale);
            animationTree.SetAttributeValue("HorizontalOffset", HorizontalOffset);
            animationTree.SetAttributeValue("VerticalOffset", VerticalOffset);

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
                    connections.Add(connection.ToXElement());
                }
            }
            animationTree.Add(connections);
            return animationTree;
        }

        public void Save(string fileName)
        {
            string dir = System.IO.Path.GetDirectoryName(fileName);
            string name = System.IO.Path.GetFileNameWithoutExtension(fileName);

            int tempPostfix = 0;
            string tempFile = System.IO.Path.Combine(dir, name + "_Temp" + tempPostfix + Extension);

            while (System.IO.File.Exists(tempFile))
            {
                tempPostfix++;
                tempFile = System.IO.Path.Combine(dir, name + "_Temp" + tempPostfix + Extension);
            }

            System.IO.FileStream file = new System.IO.FileStream(tempFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            XDocument document = new XDocument();
            XElement btTtree = ToXElement();
            document.Add(btTtree);
            document.Save(file);
            file.Close();

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
            System.IO.File.Move(tempFile, fileName);
        }
        #endregion

        #region Load

        private static AnimationNode GetNode(XElement node)
        {
            AnimationNode result = null;
            AnimationNodeType nodeType;
            if (Enum.TryParse<AnimationNodeType>(node.Name.ToString(), false, out nodeType))
            {
                switch (nodeType)
                {
                    case AnimationNodeType.Sequence:
                        result = new AnimationSequence();
                        break;
                    case AnimationNodeType.Override:
                        result = new AnimationOverride();
                        break;
                    case AnimationNodeType.BlendBySpeed:
                        result = new AnimationBlendBySpeed();
                        break;
                    case AnimationNodeType.BlendByPosture:
                        result = new AnimationBlendByPosture();
                        break;
                    case AnimationNodeType.BlendByIdle:
                        result = new AnimationBlendByIdle();
                        break;
                    case AnimationNodeType.Blend4Directional:
                        result = new AnimationBlend4Directional();
                        break;
                    case AnimationNodeType.AimOffset:
                        result = new AnimationAimOffset();
                        break;
                    case AnimationNodeType.AdditiveBlending:
                        result = new AnimationAdditiveBlending();
                        break;
                    case AnimationNodeType.Random:
                        result = new AnimationNodeRandom();
                        break;
                    case AnimationNodeType.SwitchByIndex:
                        result = new AnimationSwitchByIndex();
                        break;
                    case AnimationNodeType.Root:
                        result = new AnimationTreeRoot();
                        break;
                }
            }
            return result;
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "");
            this.Scale = e.GetAttributeValueAsDouble("Scale", Scale);
            this.HorizontalOffset = e.GetAttributeValueAsDouble("HorizontalOffset", HorizontalOffset);
            this.VerticalOffset = e.GetAttributeValueAsDouble("VerticalOffset", VerticalOffset);

            XElement nodes = e.Elements().First(p => p.Name == "Nodes");
            int count = int.Parse(nodes.Attribute("Count").Value);
            Clear();
            foreach (var item in nodes.Elements())
            {
                AnimationNode node = GetNode(item);
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
                    AnimationConnection animationConnection = new AnimationConnection();
                    animationConnection.Load(element);
                    animationConnections.Add(animationConnection);
                }
            }
            this.Connections = animationConnections.ToArray();
            AddRoot();
        }

        public static AnimationTree Load(string fileName)
        {
            System.IO.FileStream file = System.IO.File.OpenRead(fileName);
            XDocument document = XDocument.Load(file);
            AnimationTree tree = new AnimationTree();
            tree.Load(document.Elements().First());
            tree.Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            file.Close();
            return tree;
        }
        #endregion
    }
}
