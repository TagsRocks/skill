using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Diagnostics
{

    #region RequestControllersListMessage
    public class RequestControllersListMessage : Skill.Net.Message
    {
        public override int Type { get { return (int)MessageType.RequestControllersList; } }

        public override void WriteData(Skill.Net.MessageStream stream)
        {
            stream.Write(true);
        }

        public override void ReadData(Skill.Net.MessageStream stream)
        {
            stream.ReadBoolean();
        }
    }
    #endregion

    #region RegisterControllerBTMessage
    public class RegisterControllerBTMessage : Skill.Net.Message
    {
        public override int Type { get { return (int)MessageType.RegisterControllerBT; } }
        /// <summary>
        /// Id of Controller
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// True for register, false for reject behavior tree
        /// </summary>
        public bool Register { get; set; }

        public override void WriteData(Skill.Net.MessageStream stream)
        {
            stream.Write(Identifier);
            stream.Write(Register);
        }

        public override void ReadData(Skill.Net.MessageStream stream)
        {
            Identifier = stream.ReadInt32();
            Register = stream.ReadBoolean();
        }
    }
    #endregion

    #region ControllersListMessage

    public class ControllerListItem
    {
        public int InstanceId { get; set; }
        public string Identifier { get; set; }

        public bool Equals(ControllerListItem obj)
        {
            ControllerListItem other = obj as ControllerListItem;
            if (other != null)
                return this.Identifier == other.Identifier && this.InstanceId == other.InstanceId;
            else
                return false;
        }
    }

    public class ControllersListMessage : Skill.Net.Message
    {
        public override int Type { get { return (int)MessageType.ControllersList; } }
        public ControllerListItem[] Controllers { get; set; }

        public override void WriteData(Skill.Net.MessageStream stream)
        {
            if (Controllers != null)
            {
                stream.Write(Controllers.Length);
                for (int i = 0; i < Controllers.Length; i++)
                {
                    stream.Write(Controllers[i].InstanceId);
                    stream.Write(Controllers[i].Identifier);
                }
            }
            else
            {
                stream.Write(0);
            }
        }

        public override void ReadData(Skill.Net.MessageStream stream)
        {
            int treeCount = stream.ReadInt32();
            if (treeCount > 0)
            {
                Controllers = new ControllerListItem[treeCount];
                for (int i = 0; i < treeCount; i++)
                {
                    Controllers[i] = new ControllerListItem()
                    {
                        InstanceId = stream.ReadInt32(),
                        Identifier = stream.ReadString()
                    };
                }
            }
            else
            {
                Controllers = null;
            }
        }
    }
    #endregion

    #region BehaviorTreeMessage
    public class BehaviorTreeMessage : Skill.Net.Message
    {
        public override int Type { get { return (int)MessageType.BehaviorTree; } }
        public int Identifier { get; set; }
        public Skill.DataModels.AI.BehaviorTree Tree { get; set; }
        /// <summary> Name of behaviors </summary>
        public string[] Behaviors { get; set; }

        public override void WriteData(Skill.Net.MessageStream stream)
        {
            stream.Write(Identifier);
            System.Xml.Linq.XDocument document = new System.Xml.Linq.XDocument(new System.Xml.Linq.XDeclaration("1.0", "utf-8", "yes"));
            if (Tree != null)
                document.Add(Tree.ToXElement());
            stream.Write(document);
            stream.Write(Behaviors.Length);
            for (int i = 0; i < Behaviors.Length; i++)
                stream.Write(Behaviors[i]);
        }

        public override void ReadData(Skill.Net.MessageStream stream)
        {
            this.Identifier = stream.ReadInt32();
            this.Tree = new DataModels.AI.BehaviorTree();
            System.Xml.Linq.XDocument document = stream.ReadXml();
            if (document != null)
                Tree.Load(document.Elements().First());
            int idCount = stream.ReadInt32();
            this.Behaviors = new string[idCount];
            for (int i = 0; i < idCount; i++)
                this.Behaviors[i] = stream.ReadString();
        }
    }
    #endregion

    #region BehaviorTreeUpdateMessage
    public class BehaviorTreeUpdateMessage : Skill.Net.Message
    {
        public override int Type { get { return (int)MessageType.BehaviorTreeUpdate; } }

        /// <summary> Id of Controller</summary>
        public int Identifier { get; set; }
        /// <summary> Number of Behaviors in update sequences </summary>
        public int SequenceCount { get; set; }
        /// <summary> index of behaviors in  ExecutionSequence</summary>
        public int[] ExecutionSequence { get; set; }
        /// <summary> Result of behaviors </summary>
        public int[] BehaviorResults { get; set; }

        public override void WriteData(Skill.Net.MessageStream stream)
        {
            stream.Write(Identifier);
            if (SequenceCount < 0) SequenceCount = 0;
            stream.Write(SequenceCount);
            for (int i = 0; i < SequenceCount; i++)
                stream.Write(ExecutionSequence[i]);



            stream.Write(BehaviorResults.Length);
            for (int i = 0; i < BehaviorResults.Length; i++)
                stream.Write(BehaviorResults[i]);
        }

        public override void ReadData(Skill.Net.MessageStream stream)
        {
            this.Identifier = stream.ReadInt32();
            this.SequenceCount = stream.ReadInt32();
            if (this.ExecutionSequence == null || this.ExecutionSequence.Length < this.SequenceCount)
                this.ExecutionSequence = new int[this.SequenceCount];
            for (int i = 0; i < this.SequenceCount; i++)
                this.ExecutionSequence[i] = stream.ReadInt32();

            int idResult = stream.ReadInt32();
            if (this.BehaviorResults == null || this.BehaviorResults.Length < idResult)
                this.BehaviorResults = new int[idResult];
            for (int i = 0; i < idResult; i++)
                this.BehaviorResults[i] = stream.ReadInt32();
        }
    }
    #endregion

    #region ControllerNotFoundMessage
    public class ControllerNotFoundMessage : Skill.Net.Message
    {
        public override int Type { get { return (int)MessageType.ControllerNotFound; } }
        public int Identifier { get; set; }

        public override void WriteData(Skill.Net.MessageStream stream)
        {
            stream.Write(Identifier);
        }

        public override void ReadData(Skill.Net.MessageStream stream)
        {
            Identifier = stream.ReadInt32();
        }
    }
    #endregion
}
