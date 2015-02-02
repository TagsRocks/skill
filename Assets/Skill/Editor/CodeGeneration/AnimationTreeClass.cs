using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.Animation;

namespace Skill.Editor.CodeGeneration
{
    /// <summary>
    /// Generate a c# class for AnimationTree
    /// </summary>
    class AnimationTreeClass : Class
    {
        #region Variables
        public static string[] AnimNodeEventParams = new string[] { "object sender", "System.EventArgs args" };
        //List<AnimNode> _Nodes;// list of AnimNode in hierarchy
        private AnimationTreeData _Tree;// Animation tree model 
        private StringBuilder _CreateTreeMethodBody;
        #endregion


        #region Constructor

        /// <summary>
        /// Create an instance of AnimationTreeClass
        /// </summary>
        /// <param name="tree">AnimationTree model</param>
        public AnimationTreeClass(AnimationTreeData tree)
            : base(tree.Name)
        {
            this._Tree = tree;
            this._CreateTreeMethodBody = new StringBuilder();

            // look at each AnimNode in tree and create apropriate variables and properties for them
            ProcessNodes();

            AddInherit("Skill.Framework.Animation.AnimationTree");

            StringBuilder awakeBody = new StringBuilder();
            awakeBody.AppendLine("base.Awake();");
            // create profiles in constructor
            if (tree.Profiles != null && tree.Profiles.Length > 0)
            {
                foreach (var profile in tree.Profiles)
                    awakeBody.AppendLine(string.Format("this.AddProfile(\"{0}\",\"{1}\");", profile.Name, profile.Format));
                awakeBody.AppendLine(string.Format("this.Profile = \"{0}\";", tree.Profiles[0].Name));
            }
            else
                awakeBody.AppendLine("this.Profile = string.Empty;");

            if (!string.IsNullOrEmpty(tree.SkinMesh))
            {
                awakeBody.AppendLine("if (base.Animation != null)");
                awakeBody.AppendLine(string.Format("{0}.ApplyMixingTransforms(this.Animation);", tree.SkinMesh));
            }

            // create constructor
            Method awake = new Method("void", "Awake", awakeBody.ToString()) { Modifier = Modifiers.Protected, SubMethod = SubMethod.Override };
            Add(awake);

            // create protected CreateTree method
            Method createTree = new Method("Skill.Framework.Animation.AnimNode", "CreateTree", this._CreateTreeMethodBody.ToString());
            createTree.IsPartial = false;
            createTree.SubMethod = SubMethod.Override;
            createTree.Modifier = Modifiers.Protected;
            Add(createTree);

            CreateParameters();
        }

        #region CreateParameters
        private void CreateParameters()
        {
            if (_Tree.Parameters != null)
            {
                foreach (var p in _Tree.Parameters)
                    CreateParameter(p);
            }
        }

        private void CreateParameter(AnimationTreeParameter parameter)
        {
            string type = "float";
            if (parameter.Type == AnimationTreeParameterType.Integer)
                type = "int";

            string initialValue = parameter.DefaultValue.ToString();
            if (parameter.Type == AnimationTreeParameterType.Float)
                initialValue += "f";

            Variable v = new Variable(type, parameter.Name, initialValue);
            Add(v);

            ParameterProperty p = new ParameterProperty(type, parameter.Name, Variable.GetName(parameter.Name), _Tree)
            {
                Comment = parameter.Comment,
                Modifier = Modifiers.Public,
                SubMethod = SubMethod.None
            };

            Add(p);
        }

        class ParameterProperty : Property
        {
            public ParameterProperty(string type, string name, string value, AnimationTreeData tree)
                : base(type, name, value, true)
            {
                base.Multiline = true;

                StringBuilder setBuffer = new StringBuilder(_Set);
                string vatiableName = Variable.GetName(name);

                foreach (var node in tree)
                {
                    switch (node.NodeType)
                    {
                        case AnimNodeType.Override:
                            if (((AnimNodeOverrideData)node).Parameter == name)
                            {
                                setBuffer.AppendLine();
                                setBuffer.AppendFormat("{0}.Value = {1};", Variable.GetName(node.Name), vatiableName);
                            }
                            break;
                        case AnimNodeType.BlendByIndex:
                            if (((AnimNodeBlendByIndexData)node).Parameter == name)
                            {
                                setBuffer.AppendLine();
                                setBuffer.AppendFormat("{0}.SelectedChildIndex = {1};", Variable.GetName(node.Name), vatiableName);
                            }
                            break;
                        case AnimNodeType.Blend1D:
                            if (((AnimNodeBlend1DData)node).Parameter == name)
                            {
                                setBuffer.AppendLine();
                                setBuffer.AppendFormat("{0}.Value = {1};", Variable.GetName(node.Name), vatiableName);
                            }
                            break;
                        case AnimNodeType.Blend2D:
                            if (((AnimNodeBlend2DData)node).Parameter1 == name)
                            {
                                setBuffer.AppendLine();
                                setBuffer.AppendFormat("{0}.ValueX = {1};", Variable.GetName(node.Name), vatiableName);
                            }
                            if (((AnimNodeBlend2DData)node).Parameter2 == name)
                            {
                                setBuffer.AppendLine();
                                setBuffer.AppendFormat("{0}.ValueY = {1};", Variable.GetName(node.Name), vatiableName);
                            }
                            break;
                        case AnimNodeType.Additive:
                            if (((AnimNodeAdditiveBlendingData)node).Parameter == name)
                            {
                                setBuffer.AppendLine();
                                setBuffer.AppendFormat("{0}.Value = {1};", Variable.GetName(node.Name), vatiableName);
                            }
                            break;
                    }
                }

                _Set = setBuffer.ToString();
            }
        }
        #endregion

        #endregion

        #region Process

        private void ProcessNodes()
        {
            foreach (var node in _Tree)
            {
                switch (node.NodeType)
                {
                    case AnimNodeType.Sequence:
                        CreateSequence((AnimNodeSequenceData)node);
                        break;
                    case AnimNodeType.Override:
                        CreateOverride((AnimNodeOverrideData)node);
                        break;
                    case AnimNodeType.Blend1D:
                        CreateBlend1D((AnimNodeBlend1DData)node);
                        break;
                    case AnimNodeType.Blend2D:
                        CreateBlend2D((AnimNodeBlend2DData)node);
                        break;
                    case AnimNodeType.Additive:
                        CreateAdditiveBlending((AnimNodeAdditiveBlendingData)node);
                        break;
                    case AnimNodeType.BlendByIndex:
                        CreateBlendByIndex((AnimNodeBlendByIndexData)node);
                        break;
                    //case AnimNodeType.SubTree:
                    //    CreateSubTree((AnimNodeSubTreeData)node);
                    //    break;
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            foreach (var connection in _Tree.Connections)
            {
                if (connection.Start != null && connection.End != null && connection.End.NodeType != AnimNodeType.Root)
                {
                    //if (connection.Start.NodeType == AnimNodeType.SubTree)
                    //{
                    //    _CreateTreeMethodBody.AppendLine(string.Format("this.{0}[{1}] = this.{2}.Root;", Variable.GetName(connection.End.Name), connection.EndConnectorIndex, Variable.GetName(connection.Start.Name)));
                    //}
                    //else
                    //{
                    _CreateTreeMethodBody.AppendLine(string.Format("this.{0}[{1}] = this.{2};", Variable.GetName(connection.End.Name), connection.EndConnectorIndex, Variable.GetName(connection.Start.Name)));
                    //}
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            // return root
            AnimNodeData root = null;
            foreach (var node in _Tree)
            {
                if (node.NodeType == AnimNodeType.Root)
                {
                    foreach (var connection in _Tree.Connections)
                    {
                        if (connection.End == node)
                        {
                            root = connection.Start;
                            break;
                        }
                    }
                    break;
                }
            }

            if (root != null)
            {
                //if (root.NodeType == AnimNodeType.SubTree)
                //{
                //    _CreateTreeMethodBody.AppendLine(string.Format("return this.{0}.Root;", Variable.GetName(root.Name)));
                //}
                //else
                //{
                _CreateTreeMethodBody.AppendLine(string.Format("return this.{0};", Variable.GetName(root.Name)));
                //}
            }
            else
            {
                _CreateTreeMethodBody.AppendLine("return null;");
            }
        }






        private void SetProperty(AnimNodeData node, string pName, object pValue)
        {
            _CreateTreeMethodBody.AppendLine(SetProperty(node.Name, pName, pValue));
        }

        private void SetSharedParameters(AnimNodeData node)
        {
            SetProperty(node, "Name", string.Format("\"{0}\"", node.Name));
            if (node is AnimNodeBlendBaseData)
            {
                if (((AnimNodeBlendBaseData)node).NewLayer)// default is false
                    SetProperty(node, "NewLayer", "true");
            }
            CreateEvents(node);
        }

        private void CreateEvents(AnimNodeData node)
        {
            // create BecameRelevant event handler and assign it to BecameRelevant event
            if (node.BecameRelevant)
            {
                string eventName = node.Name + "_BecameRelevant";
                Add(new Method("void", eventName, "", AnimNodeEventParams) { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.BecameRelevant += new EventHandler({1});", Variable.GetName(node.Name), eventName));
            }
            // create CeaseRelevant event handler and assign it to CeaseRelevant event
            if (node.CeaseRelevant)
            {
                string eventName = node.Name + "_CeaseRelevant";
                Add(new Method("void", eventName, "", AnimNodeEventParams) { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.CeaseRelevant += new EventHandler({1});", Variable.GetName(node.Name), eventName));
            }
        }

        private void CreateVariable(string type, string name)
        {
            Add(new Variable(type, name, "null"));
        }

        // create variable and property
        private void CreateProperty(string type, string name, string comment = null)
        {
            CreateVariable(type, name);
            Add(new Property(type, name, Variable.GetName(name), false) { Modifier = Modifiers.Public, Comment = comment });
        }

        private void CreateProperty(string type, AnimNodeData node)
        {
            if (node.IsPublic)
                CreateProperty(type, node.Name, node.Comment);
            else
                CreateVariable(type, node.Name);
        }

        private void CreateSequence(AnimNodeSequenceData node)
        {
            // create action variable            
            CreateProperty("Skill.Framework.Animation.AnimNodeSequence", node);


            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.Animation.AnimNodeSequence(\"{1}\");", Variable.GetName(node.Name), node.AnimationName));

            // set parameters
            if (node.Speed != 1.0f) // default is o.1f
                SetProperty(node, "Speed", node.Speed.ToString() + "f");
            SetProperty(node, "WrapMode", "UnityEngine.WrapMode." + node.WrapMode.ToString());

            if (!node.UseTreeProfile) // default is true
                SetProperty(node, "UseTreeProfile", node.UseTreeProfile.ToString().ToLower());

            if (node.Sync)// default is false
                SetProperty(node, "Sync", "true");


            _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.RootMotion.SetKeyframes({1}.RootMotionKeyframes.{2} );", Variable.GetName(node.Name), _Tree.SkinMesh, Property.GetName(node.AnimationName)));

            SetSharedParameters(node);
        }

        private void CreateOverride(AnimNodeOverrideData node)
        {
            // create action variable
            CreateProperty("Skill.Framework.Animation.AnimNodeOverride", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.Animation.AnimNodeOverride();", Variable.GetName(node.Name)));

            SetSharedParameters(node);
        }

        private void CreateBlend1D(AnimNodeBlend1DData node)
        {
            // create action variable
            CreateProperty("Skill.Framework.Animation.AnimNodeBlend1D", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.Animation.AnimNodeBlend1D({1});", Variable.GetName(node.Name), node.Inputs.Length));

            for (int i = 0; i < node.Thresholds.Length; i++)
                SetProperty(node, string.Format("Thresholds[{0}]", i), node.Thresholds[i].ToString() + "f");

            SetSharedParameters(node);
        }

        private void CreateBlend2D(AnimNodeBlend2DData node)
        {
            // create action variable
            CreateProperty("Skill.Framework.Animation.AnimNodeBlend2D", node);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.Animation.AnimNodeBlend2D({1});", Variable.GetName(node.Name), node.Inputs.Length));

            for (int i = 0; i < node.Thresholds.Length; i++)
                SetProperty(node, string.Format("Thresholds[{0}]", i), string.Format("new UnityEngine.Vector2({0}f,{1}f)", node.Thresholds[i].X, node.Thresholds[i].Y));

            SetSharedParameters(node);
        }

        private void CreateBlendByIndex(AnimNodeBlendByIndexData node)
        {
            // create action variable
            CreateProperty("Skill.Framework.Animation.AnimNodeBlendByIndex", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.Animation.AnimNodeBlendByIndex({1});", Variable.GetName(node.Name), node.Inputs.Length));

            if (node.NodeType != AnimNodeType.Sequence && node.BlendTime != 0.3f) // default is 0.3f
                SetProperty(node, "BlendTime", node.BlendTime.ToString() + "f");

            if (!string.IsNullOrEmpty(node.Enum))
            {
                EnumClass enumClass = new EnumClass(node.Enum) { Modifier = Modifiers.Public };
                for (int i = 0; i < node.Inputs.Length; i++)
                    enumClass.Add(node.Inputs[i].Name, i);
                this.Add(enumClass);
            }
            // set parameters
            SetSharedParameters(node);
        }




        private void CreateAdditiveBlending(AnimNodeAdditiveBlendingData node)
        {
            // create action variable
            CreateProperty("Skill.Framework.Animation.AnimNodeAdditiveBlending", node);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.Animation.AnimNodeAdditiveBlending();", Variable.GetName(node.Name)));
            SetSharedParameters(node);
        }

        #endregion
    }
}
