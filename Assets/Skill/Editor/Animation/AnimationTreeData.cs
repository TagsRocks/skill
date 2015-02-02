using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimationTreeData : IXmlElementSerializable, ICollection<AnimNodeData>
    {
        #region Properties
        /// <summary> Name of tree. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }

        /// <summary> array of connections. create internally when loading file and set before save  </summary>
        public ConnectionData[] Connections { get; set; }

        public float Zoom { get; set; }
        public float PanX { get; set; }
        public float PanY { get; set; }
        public AnimationTreeParameter[] Parameters { get; set; }

        public AnimationTreeProfileData[] Profiles { get; set; }
        public string SkinMesh { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of Behavior Tree
        /// </summary>
        public AnimationTreeData()
        {
            this._Nodes = new List<AnimNodeData>();
            this.Name = "NewAnimationTree";
            this.SkinMesh = "";
            this.Zoom = 1;
            this.Profiles = new AnimationTreeProfileData[0];
            this.Parameters = new AnimationTreeParameter[0];
            AddRoot();
        }

        void AddRoot()
        {
            bool rooFound = false;
            foreach (var item in _Nodes)
            {
                if (item.NodeType == AnimNodeType.Root)
                {
                    rooFound = true;
                    break;
                }
            }

            if (!rooFound)
            {
                AnimationTreeRootData root = new AnimationTreeRootData() { Name = "Root" };
                root.X = 100;
                root.Y = 200;
                this._Nodes.Add(root);
            }
        }
        #endregion

        #region ICollection<AnimNodeData>

        List<AnimNodeData> _Nodes;
        public void Add(AnimNodeData item)
        {
            if (!Contains(item))
                _Nodes.Add(item);
        }

        public void Clear()
        {
            _Nodes.Clear();
        }

        public bool Contains(AnimNodeData item)
        {
            return _Nodes.Contains(item);
        }

        public void CopyTo(AnimNodeData[] array, int arrayIndex)
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

        public bool Remove(AnimNodeData item)
        {
            return _Nodes.Remove(item);
        }

        public IEnumerator<AnimNodeData> GetEnumerator()
        {
            return _Nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Nodes as System.Collections.IEnumerable).GetEnumerator();
        }

        public AnimNodeData Find(int id)
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

        public XmlElement ToXmlElement()
        {
            GenerateIds();

            XmlElement animationTree = new XmlElement("AnimationTree");
            animationTree.SetAttributeValue("Name", Name);
            animationTree.SetAttributeValue("Zoom", Zoom);
            animationTree.SetAttributeValue("PanX", PanX);
            animationTree.SetAttributeValue("PanY", PanY);


            XmlElement parametersElement = new XmlElement("Parameters");
            if (Parameters != null)
            {
                parametersElement.SetAttributeValue("Count", Parameters.Length);
                foreach (var item in Parameters)
                {
                    XmlElement p = item.ToXmlElement();
                    parametersElement.AppendChild(p);
                }
            }
            animationTree.AppendChild(parametersElement);


            XmlElement profilesElement = new XmlElement("Profiles");
            if (Profiles != null)
            {
                profilesElement.SetAttributeValue("Count", Profiles.Length);
                foreach (var item in Profiles)
                {
                    XmlElement p = item.ToXmlElement();
                    profilesElement.AppendChild(p);
                }
            }
            animationTree.AppendChild(profilesElement);


            XmlElement nodes = new XmlElement("Nodes");
            nodes.SetAttributeValue("Count", Count);
            // write each behavior without children hierarchy
            // children will be wrote in array of ids in selector and an id in decorator
            foreach (var item in this)
            {
                XmlElement n = item.ToXmlElement();
                nodes.AppendChild(n);
            }
            animationTree.AppendChild(nodes);

            XmlElement connections = new XmlElement("Connections");
            // Write Connections            
            if (this.Connections != null)
            {
                foreach (var connection in this.Connections)
                    connections.AppendChild(connection.ToXmlElement());
            }
            animationTree.AppendChild(connections);

            return animationTree;
        }


        #endregion

        #region Load

        public static AnimNodeData CreateNode(AnimNodeType nodeType)
        {
            AnimNodeData result = null;
            switch (nodeType)
            {
                case AnimNodeType.Sequence:
                    result = new AnimNodeSequenceData();
                    break;
                case AnimNodeType.Override:
                    result = new AnimNodeOverrideData();
                    break;
                case AnimNodeType.Blend1D:
                    result = new AnimNodeBlend1DData();
                    break;
                case AnimNodeType.Blend2D:
                    result = new AnimNodeBlend2DData();
                    break;
                case AnimNodeType.Additive:
                    result = new AnimNodeAdditiveBlendingData();
                    break;
                case AnimNodeType.BlendByIndex:
                    result = new AnimNodeBlendByIndexData();
                    break;
                case AnimNodeType.Root:
                    result = new AnimationTreeRootData();
                    break;
                //case AnimNodeType.SubTree:
                //    result = new AnimNodeSubTreeData();
                //    break;                
            }
            return result;
        }

        public static AnimNodeData CreateNode(XmlElement node)
        {
            AnimNodeType nodeType = node.GetAttributeValueAsEnum<AnimNodeType>("NodeType", AnimNodeType.None);
            if (nodeType != AnimNodeType.None)
                return CreateNode(nodeType);
            return null;
        }

        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "");
            this.Zoom = e.GetAttributeValueAsFloat("Zoom", Zoom);
            this.PanX = e.GetAttributeValueAsFloat("PanX", PanX);
            this.PanY = e.GetAttributeValueAsFloat("PanY", PanY);

            XmlElement parametersElement = e["Parameters"];
            if (parametersElement != null)
            {
                int stateCount = parametersElement.GetAttributeValueAsInt("Count", 0);
                this.Parameters = new AnimationTreeParameter[stateCount];
                int i = 0;
                foreach (var element in parametersElement)
                {
                    if (element.Name == "Parameter")
                    {
                        AnimationTreeParameter s = new AnimationTreeParameter();
                        s.Load(element);
                        this.Parameters[i++] = s;
                    }
                }
            }


            XmlElement profilesElement = e["Profiles"];
            if (profilesElement != null)
            {
                int profileCount = profilesElement.GetAttributeValueAsInt("Count", 0);
                this.Profiles = new AnimationTreeProfileData[profileCount];
                int i = 0;
                foreach (var element in profilesElement)
                {
                    if (element.Name == "Profile")
                    {
                        AnimationTreeProfileData p = new AnimationTreeProfileData();
                        p.Load(element);
                        this.Profiles[i++] = p;
                    }
                }
            }

            XmlElement nodes = e["Nodes"];
            //int count = nodes.GetAttributeValueAsInt("Count", 0);
            Clear();
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    AnimNodeData node = CreateNode(item);
                    if (node != null)
                    {
                        node.Load(item);
                        this.Add(node);
                    }
                }
            }
            AddRoot();

            // create connections
            XmlElement connections = e["Connections"];
            List<ConnectionData> animationConnections = new List<ConnectionData>();
            if (connections != null)
            {
                foreach (var element in connections)
                {
                    ConnectionData connection = new ConnectionData(this);
                    connection.Load(element);
                    animationConnections.Add(connection);
                }
            }
            this.Connections = animationConnections.ToArray();
        }
        #endregion

        #region Helper

        public bool IsParameterExist(string paramName)
        {
            if (Parameters != null)
            {
                foreach (var item in Parameters)
                {
                    if (item.Name == paramName)
                        return true;
                }
            }
            return false;
        }

        #endregion
    }
}
