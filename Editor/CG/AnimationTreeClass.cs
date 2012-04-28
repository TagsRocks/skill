using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.Animation;

namespace Skill.Editor.CG
{
    class AnimationTreeClass : Class
    {
        #region Variables
        List<Animation.AnimationNode> _Nodes;// list of AnimationNode in hierarchy
        private Animation.AnimationTree _Tree;// Animation tree model 
        private StringBuilder _CreateTreeMethodBody;
        #endregion


        #region Constructor
        public AnimationTreeClass(Animation.AnimationTree tree)
            : base(tree.Name)
        {
            this._Tree = tree;
            this._Nodes = new List<Animation.AnimationNode>();
            this._CreateTreeMethodBody = new StringBuilder();
            CreateNodeList();
            ProcessNodes();
            AddInherit("Skill.Animation.AnimationTree");


            StringBuilder constructorBody = new StringBuilder();
            // create profiles
            if (tree.Root.Profiles != null && tree.Root.Profiles.Length > 0)
            {
                foreach (var profile in tree.Root.Profiles)
                {
                    constructorBody.AppendLine(string.Format("this.AddProfile(\"{0}\",\"{1}\");", profile.Name, profile.Format));
                }
                constructorBody.AppendLine(string.Format("this.Profile = \"{0}\";", tree.Root.Profiles[0].Name));
            }
            else
                constructorBody.AppendLine("this.Profile = string.Empty;");


            Method constructor = new Method("", Name, constructorBody.ToString(), "Skill.Controllers.IController controller");
            constructor.Modifiers = Modifiers.Public;
            constructor.BaseMethod = ":base(controller)";

            Add(constructor);

            Method createTree = new Method("Skill.Animation.AnimationNode", "CreateTree", this._CreateTreeMethodBody.ToString());
            createTree.IsPartial = false;
            createTree.SubMethod = SubMethod.Override;
            createTree.Modifiers = Modifiers.Protected;
            Add(createTree);
        }
        #endregion

        #region CreateNodeList
        private void CreateNodeList()
        {
            foreach (var node in _Tree)
            {
                if (IsInHierarchy(node.Id))
                    _Nodes.Add(node);
            }
        }
        private bool IsInHierarchy(int nodeId)
        {
            foreach (var connection in _Tree.Connections)
            {
                if (connection.SourceId == nodeId || connection.SinkId == nodeId)
                    return true;
            }
            return false;
        }
        #endregion

        #region Process

        private AnimationNode Find(int id)
        {
            foreach (var item in _Nodes)
            {
                if (item.Id == id) return item;
            }
            return null;
        }

        private void ProcessNodes()
        {
            foreach (var node in _Nodes)
            {
                switch (node.NodeType)
                {
                    case AnimationNodeType.Sequence:
                        CreateSequence((AnimationSequence)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.Override:
                        CreateOverride((AnimationOverride)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.BlendBySpeed:
                        CreateBlendBySpeed((AnimationBlendBySpeed)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.BlendByPosture:
                        CreateBlendByPosture((AnimationBlendByPosture)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.BlendByIdle:
                        CreateBlendByIdle((AnimationBlendByIdle)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.Blend4Directional:
                        CreateBlend4Directional((AnimationBlend4Directional)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.AimOffset:
                        CreateAimOffset((AnimationAimOffset)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.AdditiveBlending:
                        CreateAdditiveBlending((AnimationAdditiveBlending)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.Random:
                        CreateRandom((AnimationNodeRandom)node);
                        break;
                    case Skill.Editor.Animation.AnimationNodeType.SwitchByIndex:
                        CreateSwitchByIndex((AnimationSwitchByIndex)node);
                        break;
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            foreach (var connection in _Tree.Connections)
            {
                AnimationNode child = Find(connection.SourceId);
                AnimationNode parent = Find(connection.SinkId);
                if (child != null && parent != null && parent.NodeType != AnimationNodeType.Root)
                {
                    _CreateTreeMethodBody.AppendLine(string.Format("this.{0}[{1}] = this.{2};", Variable.GetName(parent.Name), connection.SinkConnectorIndex, Variable.GetName(child.Name)));
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            // return root
            AnimationNode root = null;
            foreach (var node in _Nodes)
            {
                if (node.NodeType == AnimationNodeType.Root)
                {
                    foreach (var connection in _Tree.Connections)
                    {
                        if (connection.SinkId == node.Id)
                        {
                            root = Find(connection.SourceId);
                            break;
                        }
                    }
                    break;
                }
            }

            if (root != null)
                _CreateTreeMethodBody.AppendLine(string.Format("return this.{0};", Variable.GetName(root.Name)));
            else
                _CreateTreeMethodBody.AppendLine("return null;");
        }

        private void SetProperty(AnimationNode node, string pName, object pValue)
        {
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.{1} = {2};", Variable.GetName(node.Name), pName, pValue));
        }

        private void SetSharedParameters(AnimationNode node)
        {
            SetProperty(node, "Name", string.Format("\"{0}\"", node.Name));
            if (node.BlendTime != 0.3f) // default is 0.3f
                SetProperty(node, "BlendTime", node.BlendTime.ToString() + "f");
            CreateEvents(node);
        }

        private void CreateEvents(AnimationNode node)
        {
            // create failure event handler and assign it to failure event
            if (node.BecameRelevant)
            {
                Add(new Method("void", node.Name + "_BecameRelevant", "", "object sender", "EventArgs e") { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.BecameRelevant += new EventHandler({1});", Variable.GetName(node.Name), node.Name + "_BecameRelevant"));
            }
            if (node.CeaseRelevant)
            {
                Add(new Method("void", node.Name + "_CeaseRelevant", "", "object sender", "EventArgs e") { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.CeaseRelevant += new EventHandler({1});", Variable.GetName(node.Name), node.Name + "_CeaseRelevant"));
            }
        }

        private void CreateVariable(string type, string name)
        {
            Add(new Variable(type, name, "null"));
        }

        private void CreateProperty(string type, string name)
        {
            CreateVariable(type, name);
            Add(new Property(type, name, Variable.GetName(name), false) { Modifiers = Modifiers.Public });
        }

        private void CreateSequence(AnimationSequence node)
        {
            // create action variable
            if (node.IsPublic)
                CreateProperty("Skill.Animation.AnimationSequence", node.Name);
            else
                CreateVariable("Skill.Animation.AnimationSequence", node.Name);


            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationSequence(\"{1}\");", Variable.GetName(node.Name), node.AnimationName));

            // set parameters
            if (node.Speed != 1.0f) // default is o.1f
                SetProperty(node, "Speed", node.Speed.ToString() + "f");
            SetProperty(node, "WrapMode", "UnityEngine.WrapMode." + node.WrapMode.ToString());

            SetSharedParameters(node);
        }

        private void CreateOverride(AnimationOverride node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationOverride", node.Name);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationOverride({1});", Variable.GetName(node.Name),node.ChildrenNames.Length));

            // set parameters
            if (node.OverridePeriod > 0.0f) // default is 0.0f
                SetProperty(node, "OverridePeriod", node.OverridePeriod.ToString() + "f");

            SetSharedParameters(node);
        }

        private void CreateBlendBySpeed(AnimationBlendBySpeed node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationBlendBySpeed", node.Name);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationBlendBySpeed({1});", Variable.GetName(node.Name), node.Constraints.Count - 1));

            // set parameters
            if (node.BlendDownTime != 0.3f) // default is 0.3f
                SetProperty(node, "BlendDownTime", node.BlendDownTime.ToString() + "f");
            SetProperty(node, "BlendDownDelay", node.BlendDownDelay.ToString() + "f");

            if (node.BlendUpTime != 0.3f) // default is 0.3f
                SetProperty(node, "BlendUpTime", node.BlendUpTime.ToString() + "f");
            SetProperty(node, "BlendUpDelay", node.BlendUpDelay.ToString() + "f");

            for (int i = 0; i < node.Constraints.Count; i++)
            {
                SetProperty(node, string.Format("Constraints[{0}]", i), node.Constraints[i].Property.ToString() + "f");
            }

            SetSharedParameters(node);
        }

        private void CreateRandom(AnimationNodeRandom node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationNodeRandom", node.Name);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationNodeRandom({1});", Variable.GetName(node.Name), node.Chances.Count));

            // set parameters            
            for (int i = 0; i < node.Chances.Count; i++)
            {
                SetProperty(node, string.Format("Chances[{0}]", i), node.Chances[i].Property.ToString() + "f");
            }

            SetSharedParameters(node);
        }

        private void CreateSwitchByIndex(AnimationSwitchByIndex node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationSwitchByIndex", node.Name);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationSwitchByIndex({1});", Variable.GetName(node.Name), node.ChildrenNames.Length));

            // set parameters
            SetSharedParameters(node);
        }


        private void CreateBlendByPosture(AnimationBlendByPosture node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationBlendByPosture", node.Name);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationBlendByPosture();", Variable.GetName(node.Name)));


            SetSharedParameters(node);
        }

        private void CreateBlendByIdle(AnimationBlendByIdle node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationBlendByIdle", node.Name);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationBlendByIdle();", Variable.GetName(node.Name)));
            SetSharedParameters(node);
        }

        private void CreateBlend4Directional(AnimationBlend4Directional node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationBlend4Directional", node.Name);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationBlend4Directional();", Variable.GetName(node.Name)));
            SetSharedParameters(node);
        }

        private void CreateAimOffset(AnimationAimOffset node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationAimOffset", node.Name);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationAimOffset();", Variable.GetName(node.Name)));

            // set profiles
            foreach (var profile in node.Profiles)
            {
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Add(new AnimationAimOffsetProfile({1}));", Variable.GetName(node.Name),
                   string.Format("Name = \"{0}\", CenterCenter = \"{1}\", CenterUp = \"{2}\", CenterDown = \"{3}\", LeftCenter = \"{4}\", LeftUp = \"{5}\", LeftDown = \"{6}\", RightCenter = \"{7}\", RightUp = \"{8}\", RightDown = \"{9}\"",
                          profile.Name, profile.CenterCenter, profile.CenterUp, profile.CenterDown, profile.LeftCenter, profile.LeftUp, profile.LeftDown, profile.RightCenter, profile.RightUp, profile.RightDown)));
            }

            if (node.ProfileChanged)
            {
                Add(new Method("void", node.Name + "_ProfileChanged", "", "object sender", "EventArgs e") { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.ProfileChanged += new EventHandler({1});", Variable.GetName(node.Name), node.Name + "_ProfileChanged"));
            }
            SetSharedParameters(node);
        }

        private void CreateAdditiveBlending(AnimationAdditiveBlending node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimationAdditiveBlending", node.Name);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimationAdditiveBlending();", Variable.GetName(node.Name)));
            SetSharedParameters(node);
        }

        #endregion
    }
}
