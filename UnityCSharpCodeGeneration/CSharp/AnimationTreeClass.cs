using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.Animation;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// Generate a c# class for AnimationTree
    /// </summary>
    class AnimationTreeClass : Class
    {
        #region Variables
        public static string[] AnimNodeEventParams = new string[] { "Skill.Animation.AnimNode sender", "Skill.Animation.AnimationTreeState state" };
        //List<AnimNode> _Nodes;// list of AnimNode in hierarchy
        private AnimationTree _Tree;// Animation tree model 
        private StringBuilder _CreateTreeMethodBody;
        #endregion


        #region Constructor

        /// <summary>
        /// Create an instance of AnimationTreeClass
        /// </summary>
        /// <param name="tree">AnimationTree model</param>
        public AnimationTreeClass(AnimationTree tree)
            : base(tree.Name)
        {
            this._Tree = tree;
            this._CreateTreeMethodBody = new StringBuilder();

            // look at each AnimNode in tree and create apropriate variables and properties for them
            ProcessNodes();

            AddInherit("Skill.Animation.AnimationTree");


            StringBuilder constructorBody = new StringBuilder();
            // create profiles in constructor
            if (tree.Profiles != null && tree.Profiles.Length > 0)
            {
                foreach (var profile in tree.Profiles)
                {
                    constructorBody.AppendLine(string.Format("this.AddProfile(\"{0}\",\"{1}\");", profile.Name, profile.Format));
                }
                constructorBody.AppendLine(string.Format("this.Profile = \"{0}\";", tree.Profiles[0].Name));
            }
            else
                constructorBody.AppendLine("this.Profile = string.Empty;");

            // create constructor
            Method constructor = new Method("", Name, constructorBody.ToString());
            constructor.Modifiers = Modifiers.Public;
            Add(constructor);

            // create protected CreateTree method
            Method createTree = new Method("Skill.Animation.AnimNode", "CreateTree", this._CreateTreeMethodBody.ToString());
            createTree.IsPartial = false;
            createTree.SubMethod = SubMethod.Override;
            createTree.Modifiers = Modifiers.Protected;
            Add(createTree);
        }
        #endregion

        #region Process

        private void ProcessNodes()
        {
            foreach (var node in _Tree)
            {
                switch (node.NodeType)
                {
                    case AnimNodeType.Sequence:
                        CreateSequence((AnimNodeSequence)node);
                        break;
                    case AnimNodeType.Override:
                        CreateOverride((AnimNodeOverride)node);
                        break;
                    case AnimNodeType.BlendBySpeed:
                        CreateBlendBySpeed((AnimNodeBlendBySpeed)node);
                        break;
                    case AnimNodeType.BlendByPosture:
                        CreateBlendByPosture((AnimNodeBlendByPosture)node);
                        break;
                    case AnimNodeType.BlendByIdle:
                        CreateBlendByIdle((AnimNodeBlendByIdle)node);
                        break;
                    case AnimNodeType.Blend4Directional:
                        CreateBlend4Directional((AnimNodeBlend4Directional)node);
                        break;
                    case AnimNodeType.AimOffset:
                        CreateAimOffset((AnimNodeAimOffset)node);
                        break;
                    case AnimNodeType.AdditiveBlending:
                        CreateAdditiveBlending((AnimNodeAdditiveBlending)node);
                        break;
                    case AnimNodeType.Random:
                        CreateRandom((AnimNodeRandom)node);
                        break;
                    case AnimNodeType.BlendByIndex:
                        CreateSwitchByIndex((AnimNodeBlendByIndex)node);
                        break;
                    case AnimNodeType.SubTree:
                        CreateSubTree((AnimNodeSubTree)node);
                        break;
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            foreach (var connection in _Tree.Connections)
            {
                if (connection.Source != null && connection.Sink != null && connection.Sink.NodeType != AnimNodeType.Root)
                {
                    if (connection.Source.NodeType == AnimNodeType.SubTree)
                    {
                        _CreateTreeMethodBody.AppendLine(string.Format("this.{0}[{1}] = this.{2}.Root;", Variable.GetName(connection.Sink.Name), connection.SinkConnectorIndex, Variable.GetName(connection.Source.Name)));
                    }
                    else
                    {
                        _CreateTreeMethodBody.AppendLine(string.Format("this.{0}[{1}] = this.{2};", Variable.GetName(connection.Sink.Name), connection.SinkConnectorIndex, Variable.GetName(connection.Source.Name)));
                    }
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            // return root
            AnimNode root = null;
            foreach (var node in _Tree)
            {
                if (node.NodeType == AnimNodeType.Root)
                {
                    foreach (var connection in _Tree.Connections)
                    {
                        if (connection.Sink == node)
                        {
                            root = connection.Source;
                            break;
                        }
                    }
                    break;
                }
            }

            if (root != null)
            {
                if (root.NodeType == AnimNodeType.SubTree)
                {
                    _CreateTreeMethodBody.AppendLine(string.Format("return this.{0}.Root;", Variable.GetName(root.Name)));
                }
                else
                {
                    _CreateTreeMethodBody.AppendLine(string.Format("return this.{0};", Variable.GetName(root.Name)));
                }
            }
            else
            {
                _CreateTreeMethodBody.AppendLine("return null;");
            }
        }




        private void SetProperty(AnimNode node, string pName, object pValue)
        {
            _CreateTreeMethodBody.AppendLine(SetProperty(node.Name, pName, pValue));
        }

        private void SetSharedParameters(AnimNode node)
        {
            SetProperty(node, "Name", string.Format("\"{0}\"", node.Name));
            if (node.NodeType != AnimNodeType.Sequence && node.BlendTime != 0.3f) // default is 0.3f
                SetProperty(node, "BlendTime", node.BlendTime.ToString() + "f");
            CreateEvents(node);
        }

        private void CreateEvents(AnimNode node)
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
            Add(new Property(type, name, Variable.GetName(name), false) { Modifiers = Modifiers.Public, Comment = comment });
        }

        private void CreateProperty(string type, AnimNode node)
        {
            if (node.IsPublic)
                CreateProperty(type, node.Name, node.Comment);
            else
                CreateVariable(type, node.Name);
        }



        private void CreateSequence(AnimNodeSequence node)
        {
            // create action variable            
            CreateProperty("Skill.Animation.AnimNodeSequence", node);


            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeSequence(\"{1}\");", Variable.GetName(node.Name), node.AnimationName));

            // set parameters
            if (node.Speed != 1.0f) // default is o.1f
                SetProperty(node, "Speed", node.Speed.ToString() + "f");
            SetProperty(node, "WrapMode", "UnityEngine.WrapMode." + node.WrapMode.ToString());

            if (!node.UseTreeProfile) // default is true
                SetProperty(node, "UseTreeProfile", node.UseTreeProfile.ToString().ToLower());

            if (node.Synchronize) // default is false
                SetProperty(node, "Synchronize", node.Synchronize.ToString().ToLower());

            if (node.MixingTransforms != null && node.MixingTransforms.Length > 0) // default is null            
                SetProperty(node, "MixingTransforms", CreateStringArray(node.MixingTransforms));

            if (node.RootMotion.PositionX) // default is false            
                SetProperty(node, "RootMotion.State.PositionX", "true");
            if (node.RootMotion.PositionY) // default is false            
                SetProperty(node, "RootMotion.State.PositionY", "true");
            if (node.RootMotion.PositionZ) // default is false            
                SetProperty(node, "RootMotion.State.PositionZ", "true");

            if (node.RootMotion.PositionX || node.RootMotion.PositionY || node.RootMotion.PositionZ)
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.RootMotion.SetKeyframes({1}.PositionKeyframes.{2} );", Variable.GetName(node.Name), System.IO.Path.GetFileNameWithoutExtension(_Tree.SkinMesh), Property.GetName(node.AnimationName)));

            SetSharedParameters(node);
        }

        private void CreateOverride(AnimNodeOverride node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeOverride", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeOverride({1});", Variable.GetName(node.Name), node.Inputs.Length));

            // set parameters
            if (node.OverridePeriod > 0.0f) // default is 0.0f
                SetProperty(node, "OverridePeriod", node.OverridePeriod.ToString() + "f");

            SetSharedParameters(node);
        }

        private void CreateBlendBySpeed(AnimNodeBlendBySpeed node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeBlendBySpeed", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeBlendBySpeed({1});", Variable.GetName(node.Name), node.Constraints.Length - 1));

            // set parameters
            if (node.BlendDownTime != 0.3f) // default is 0.3f
                SetProperty(node, "BlendDownTime", node.BlendDownTime.ToString() + "f");

            if (node.BlendDownDelay > 0.0f) // default is 0.0f
                SetProperty(node, "BlendDownDelay", node.BlendDownDelay.ToString() + "f");

            if (node.BlendUpTime != 0.3f) // default is 0.3f
                SetProperty(node, "BlendUpTime", node.BlendUpTime.ToString() + "f");

            if (node.BlendUpDelay > 0.0f) // default is 0.0f
                SetProperty(node, "BlendUpDelay", node.BlendUpDelay.ToString() + "f");

            if (node.BlendDownPercent > 0.0f) // default is 0.0f
                SetProperty(node, "BlendDownPercent", node.BlendDownPercent.ToString() + "f");

            for (int i = 0; i < node.Constraints.Length; i++)
            {
                SetProperty(node, string.Format("Constraints[{0}]", i), node.Constraints[i].ToString() + "f");
            }

            SetSharedParameters(node);
        }

        private void CreateRandom(AnimNodeRandom node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeRandom", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeRandom({1});", Variable.GetName(node.Name), node.Chances.Length));

            // set parameters            
            for (int i = 0; i < node.Chances.Length; i++)
            {
                SetProperty(node, string.Format("Chances[{0}]", i), node.Chances[i].ToString() + "f");
            }

            SetSharedParameters(node);
        }

        private void CreateSwitchByIndex(AnimNodeBlendByIndex node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeBlendByIndex", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeBlendByIndex({1});", Variable.GetName(node.Name), node.Inputs.Length));

            // set parameters
            SetSharedParameters(node);
        }


        private void CreateBlendByPosture(AnimNodeBlendByPosture node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeBlendByPosture", node);

            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeBlendByPosture();", Variable.GetName(node.Name)));


            SetSharedParameters(node);
        }

        private void CreateBlendByIdle(AnimNodeBlendByIdle node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeBlendByIdle", node);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeBlendByIdle();", Variable.GetName(node.Name)));
            SetSharedParameters(node);
        }

        private void CreateBlend4Directional(AnimNodeBlend4Directional node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeBlend4Directional", node);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeBlend4Directional();", Variable.GetName(node.Name)));
            SetSharedParameters(node);
        }

        private void CreateAimOffset(AnimNodeAimOffset node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeAimOffset", node);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeAimOffset();", Variable.GetName(node.Name)));

            // set profiles
            foreach (var profile in node.Profiles)
            {
                StringBuilder profileStr = new StringBuilder();
                profileStr.Append(string.Format("this.{0}.AddProfile(new Skill.Animation.AnimNodeAimOffsetProfile(){{ Name = \"{1}\" ", Variable.GetName(node.Name), profile.Name));

                if (!string.IsNullOrEmpty(profile.CenterCenter.AnimationName))
                    profileStr.Append(string.Format(", CenterCenter = \"{0}\"", profile.CenterCenter.AnimationName));
                if (!string.IsNullOrEmpty(profile.CenterUp.AnimationName))
                    profileStr.Append(string.Format(", CenterUp = \"{0}\"", profile.CenterUp.AnimationName));
                if (!string.IsNullOrEmpty(profile.CenterDown.AnimationName))
                    profileStr.Append(string.Format(", CenterDown = \"{0}\"", profile.CenterDown.AnimationName));

                if (!string.IsNullOrEmpty(profile.LeftCenter.AnimationName))
                    profileStr.Append(string.Format(", LeftCenter = \"{0}\"", profile.LeftCenter.AnimationName));
                if (!string.IsNullOrEmpty(profile.LeftUp.AnimationName))
                    profileStr.Append(string.Format(", LeftUp = \"{0}\"", profile.LeftUp.AnimationName));
                if (!string.IsNullOrEmpty(profile.LeftDown.AnimationName))
                    profileStr.Append(string.Format(", LeftDown = \"{0}\"", profile.LeftDown.AnimationName));

                if (!string.IsNullOrEmpty(profile.RightCenter.AnimationName))
                    profileStr.Append(string.Format(", RightCenter = \"{0}\"", profile.RightCenter.AnimationName));
                if (!string.IsNullOrEmpty(profile.RightUp.AnimationName))
                    profileStr.Append(string.Format(", RightUp = \"{0}\"", profile.RightUp.AnimationName));
                if (!string.IsNullOrEmpty(profile.RightDown.AnimationName))
                    profileStr.Append(string.Format(", RightDown = \"{0}\"", profile.RightDown.AnimationName));

                if (profile.CenterCenter.MixingTransforms != null && profile.CenterCenter.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", CenterCenterMTs = {0}", base.CreateStringArray(profile.CenterCenter.MixingTransforms)));
                if (profile.CenterUp.MixingTransforms != null && profile.CenterUp.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", CenterUpMTs = {0}", base.CreateStringArray(profile.CenterUp.MixingTransforms)));
                if (profile.CenterDown.MixingTransforms != null && profile.CenterDown.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", CenterDownMTs = {0}", base.CreateStringArray(profile.CenterDown.MixingTransforms)));

                if (profile.LeftCenter.MixingTransforms != null && profile.LeftCenter.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", LeftCenterMTs = {0}", base.CreateStringArray(profile.LeftCenter.MixingTransforms)));
                if (profile.LeftUp.MixingTransforms != null && profile.LeftUp.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", LeftUpMTs = {0}", base.CreateStringArray(profile.LeftUp.MixingTransforms)));
                if (profile.LeftDown.MixingTransforms != null && profile.LeftDown.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", LeftDownMTs = {0}", base.CreateStringArray(profile.LeftDown.MixingTransforms)));

                if (profile.RightCenter.MixingTransforms != null && profile.RightCenter.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", RightCenterMTs = {0}", base.CreateStringArray(profile.RightCenter.MixingTransforms)));
                if (profile.RightUp.MixingTransforms != null && profile.RightUp.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", RightUpMTs = {0}", base.CreateStringArray(profile.RightUp.MixingTransforms)));
                if (profile.RightDown.MixingTransforms != null && profile.RightDown.MixingTransforms.Length > 0)
                    profileStr.Append(string.Format(", RightDownMTs = {0}", base.CreateStringArray(profile.RightDown.MixingTransforms)));

                profileStr.Append("});");
                _CreateTreeMethodBody.AppendLine(profileStr.ToString());
            }

            if (node.ProfileChanged)
            {
                Add(new Method("void", node.Name + "_ProfileChanged", "", "object sender", "EventArgs e") { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.ProfileChanged += new EventHandler({1});", Variable.GetName(node.Name), node.Name + "_ProfileChanged"));
            }

            // set properties
            if (!node.UseTreeProfile) // default is true
                SetProperty(node, "UseTreeProfile", node.UseTreeProfile.ToString().ToLower());
            if (node.IsLoop) // default is false
                SetProperty(node, "IsLoop", node.IsLoop.ToString().ToLower());

            SetSharedParameters(node);
        }

        private void CreateAdditiveBlending(AnimNodeAdditiveBlending node)
        {
            // create action variable
            CreateProperty("Skill.Animation.AnimNodeAdditiveBlending", node);
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Animation.AnimNodeAdditiveBlending();", Variable.GetName(node.Name)));
            SetSharedParameters(node);
        }

        private void CreateSubTree(AnimNodeSubTree node)
        {
            string className = System.IO.Path.GetFileNameWithoutExtension(node.TreeAddress);

            // create action variable
            CreateProperty(className, node);

            // new inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new {1}();", Variable.GetName(node.Name), className));
        }

        #endregion
    }
}
