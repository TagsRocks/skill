using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Diagnostics
{

    /// <summary>
    /// Debug server
    /// </summary>
    /// <remarks>
    /// Name this gameobject to something like AAAAADebugServer to make sure this is the first object in scene
    /// </remarks>
    [AddComponentMenu("Skill/Diagnostics/Debug Server")]
    public class DebugServer : MonoBehaviour, Skill.Framework.IControllerManager
    {
        public const int DefaultPort = 4200;

        /// <summary> Just to make sure only one instance of this object is running </summary>
        public static DebugServer Instance { get; private set; }

        /// <summary> Only run server if the game run in editor </summary>
        public bool EditorOnly = true;
        /// <summary> Log additional messages </summary>
        public bool Log = true;
        /// <summary> Make the game run even when in background </summary>
        public bool RunInBackground = true;
        /// <summary> Port of server </summary>
        public int Port = DefaultPort;
        /// <summary> Size of buffer in kilobytes to use in messaging</summary>
        public int BufferSize = 100;// 100 kb

        private Skill.Net.Server _Server;
        private List<Client> _Clients;

        private List<Skill.Framework.Controller> _Controllers;

        /// <summary>
        /// Is valid to run server
        /// </summary>
        private bool IsValid
        {
            get
            {
                if (EditorOnly && !Application.isEditor && (Port > 0 && Port <= ushort.MaxValue))
                    return false;
                return Instance == this;
            }
        }

        void Awake()
        {
            if (Instance == null)// make sure this is the first instance of server
            {
                Instance = this;
                Skill.Net.Logger.ReplaceInstance(new UnityLogger());
                Skill.Framework.Global.Register(this); // register to get access to all controllers
                _Clients = new List<Client>();
                _Controllers = new List<Skill.Framework.Controller>();
            }
            else
                Debug.LogWarning("More than one instance of Debug Server found. you need only one.");
        }

        void Start()
        {
            if (IsValid)
            {
                StartServer();
            }
        }

        void Update()
        {
            foreach (Client client in _Clients)
            {
                client.Update();
            }
        }

        void OnDestroy()
        {
            if (IsValid)
            {
                foreach (var c in _Clients)
                    c.Close();
                CloseServer();
                Skill.Framework.Global.UnRegister(this);
                _Controllers.Clear();
            }
        }

        private void CloseServer()
        {
            if (IsValid)
            {
                if (_Server != null && _Server.IsListening)
                {
                    _Server.Close();
                    _Server.ClientConnected -= _Server_ClientConnected;
                    _Server.ClientDisconnected -= _Server_ClientDisconnected;
                    if (Log)
                        Skill.Net.Logger.LogMessage("Debug Server Closed");
                }
            }
        }

        private void StartServer()
        {
            if (IsValid)
            {
                CloseServer();

                if (RunInBackground) Application.runInBackground = true;
                _Server = new Skill.Net.Server(new MessageTranslator(), BufferSize * 1024);
                _Server.ClientConnected += _Server_ClientConnected;
                _Server.ClientDisconnected += _Server_ClientDisconnected;
                _Server.Start(Port);
                if (Log)
                    Skill.Net.Logger.LogMessage("Debug Server Started");
            }
        }

        void _Server_ClientDisconnected(object sender, Skill.Net.Worker worker)
        {
            Client client = null;
            foreach (var item in _Clients)
            {
                if (item.Worker == worker)
                {
                    client = item;
                    break;
                }
            }
            if (client != null)
            {
                _Clients.Remove(client);
                if (Log)
                    Skill.Net.Logger.LogMessage("Client disconnected from DebugServer");
            }
            else
            {
                if (Log)
                    Skill.Net.Logger.LogMessage("An invalid client try to disconnected from DebugServer");
            }
        }

        void _Server_ClientConnected(object sender, Skill.Net.Worker worker)
        {
            _Clients.Add(new Client(worker, this));
            if (Log)
                Skill.Net.Logger.LogMessage("Client connected to DebugServer");
        }

        // register behavioral controllers that has valid identifier
        public void Register(Skill.Framework.Controller controller)
        {
            if (controller == null) return;
            if (!string.IsNullOrEmpty(controller.Identifier))
            {
                _Controllers.Add(controller);
            }
        }

        // unregister behavioral controllers that has valid identifier
        public bool UnRegister(Skill.Framework.Controller controller)
        {
            if (controller == null) return false;
            if (!string.IsNullOrEmpty(controller.Identifier))
            {
                // try to find controller in list
                int index = -1;
                for (int i = 0; i < _Controllers.Count; i++)
                {
                    if (_Controllers[i] == controller)
                    {
                        index = i;
                        break;
                    }
                }

                // notify all clients that this controller is unregistered
                if (index >= 0)
                {
                    foreach (var c in _Clients)
                        c.Unregister(controller);

                    // remove controller from list
                    _Controllers.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }



        // get message that contains list of all available behviortree
        private ControllersListMessage GetList()
        {
            ControllersListMessage msg = new ControllersListMessage();

            // each controller has a behaviortree
            msg.Controllers = new ControllerListItem[_Controllers.Count];
            for (int i = 0; i < _Controllers.Count; i++)
            {
                msg.Controllers[i] = new ControllerListItem() { Identifier = _Controllers[i].Identifier, InstanceId = _Controllers[i].GetInstanceID() };
            }

            return msg;
        }

        // retrieve controller by id
        private Skill.Framework.Controller GetController(int id)
        {
            foreach (var c in _Controllers)
            {
                if (c.GetInstanceID() == id)
                    return c;
            }
            return null;
        }


        class Client
        {
            private DebugServer _Server;
            private ArrayList _MessagesToSend;
            private ArrayList _RegisteredBehaviors;
            private ArrayList _MessagesToProcess;

            public Skill.Net.Worker Worker { get; private set; }

            public Client(Skill.Net.Worker worker, DebugServer server)
            {
                this._MessagesToProcess = ArrayList.Synchronized(new ArrayList());
                this._MessagesToSend = ArrayList.Synchronized(new ArrayList());
                this._RegisteredBehaviors = ArrayList.Synchronized(new ArrayList());
                this.Worker = worker;
                this._Server = server;

                this.Worker.Message += _Worker_Message;
                this.Worker.Closed += Worker_Closed;
            }

            void Worker_Closed(object sender, EventArgs e)
            {
                this.Worker.Message -= _Worker_Message;
                Close();
            }

            void _Worker_Message(object sender, Skill.Net.Message msg)
            {
                lock (_MessagesToProcess)
                {
                    _MessagesToProcess.Add(msg);
                }
            }

            void Behavior_Updated(object sender, EventArgs e)
            {
                Skill.Framework.AI.BehaviorTree bt = (Skill.Framework.AI.BehaviorTree)sender;
                lock (_RegisteredBehaviors)
                {
                    foreach (BehaviorTreeData item in _RegisteredBehaviors)
                    {
                        if (item.Tree == bt)
                        {
                            item.Update();
                            lock (_MessagesToSend)
                            {
                                _MessagesToSend.Add(item.BTUpdateMessage);
                            }
                            break;
                        }
                    }
                }
            }


            void ProcessMessage(Skill.Net.Message msg)
            {
                Skill.Net.Message msgToSend = null;
                switch ((MessageType)msg.Type)
                {
                    case MessageType.Disconnect:
                        break;
                    case MessageType.Text:
                        //Skill.Net.Logger.LogMessage(((Skill.Net.TextMessage)msg).Text);
                        break;
                    case MessageType.RequestControllersList:
                        msgToSend = _Server.GetList();
                        if (_Server.Log)
                            Skill.Net.Logger.LogMessage("RequestBehaviorTreeList Message recieved");
                        break;
                    case MessageType.RegisterControllerBT:
                        RegisterControllerBTMessage registerMsg = msg as RegisterControllerBTMessage;
                        if (registerMsg != null)
                        {
                            Skill.Framework.Controller controller = _Server.GetController(registerMsg.Identifier);
                            if (controller != null && controller.Behavior != null)
                            {
                                if (registerMsg.Register)
                                {
                                    var bttag = Register(controller);

                                    if (bttag != null)
                                    {
                                        msgToSend = bttag.BTMessage;
                                    }
                                }
                                else
                                    Unregister(controller);
                            }
                            else
                            {
                                ControllerNotFoundMessage notFoundMsg = new ControllerNotFoundMessage();
                                notFoundMsg.Identifier = registerMsg.Identifier;
                                msgToSend = notFoundMsg;
                            }
                            if (_Server.Log)
                                Skill.Net.Logger.LogMessage("RegisterBehaviorTree Message revieved");
                        }
                        break;
                    default:
                        break;
                }

                if (msgToSend != null)
                {
                    lock (_MessagesToSend)
                    {
                        _MessagesToSend.Add(msgToSend);
                    }
                }
            }

            public void Update()
            {
                lock (_MessagesToProcess)
                {
                    foreach (Skill.Net.Message msg in _MessagesToProcess)
                    {
                        ProcessMessage(msg);
                    }
                    _MessagesToProcess.Clear();
                }

                lock (_MessagesToSend)
                {
                    if (Worker.IsConnected)
                    {
                        foreach (Skill.Net.Message msg in _MessagesToSend)
                        {
                            Worker.SendMessage(msg);
                        }
                    }
                    _MessagesToSend.Clear();
                }
            }

            public BehaviorTreeData Register(Skill.Framework.Controller controller)
            {
                if (controller.Behavior != null)
                {
                    lock (_RegisteredBehaviors)
                    {
                        foreach (BehaviorTreeData item in _RegisteredBehaviors)
                        {
                            if (item.Tree == controller.Behavior)
                                return item;
                        }
                        controller.Behavior.Updated += Behavior_Updated;
                        BehaviorTreeData btt = new BehaviorTreeData(controller.Behavior, controller.GetInstanceID(), controller.Identifier);
                        _RegisteredBehaviors.Add(btt);
                        return btt;
                    }
                }

                return null;
            }

            public void Unregister(Skill.Framework.Controller controller)
            {
                lock (_RegisteredBehaviors)
                {
                    BehaviorTreeData btt = null;
                    foreach (BehaviorTreeData item in _RegisteredBehaviors)
                    {
                        if (item.Id == controller.GetInstanceID())
                        {
                            btt = item;
                            break;
                        }
                    }
                    if (btt != null)
                    {
                        btt.Tree.Updated -= Behavior_Updated;
                        _RegisteredBehaviors.Remove(btt);
                    }
                }
            }

            public void Close()
            {
                lock (_RegisteredBehaviors)
                {
                    foreach (BehaviorTreeData item in _RegisteredBehaviors)
                    {
                        item.Tree.Updated -= Behavior_Updated;
                    }
                    _RegisteredBehaviors.Clear();
                }
                lock (_MessagesToSend)
                {
                    _MessagesToSend.Clear();
                }
            }
        }

        class BehaviorTreeData
        {
            public Skill.Framework.AI.BehaviorTree Tree { get; private set; }
            public BehaviorTreeMessage BTMessage { get; private set; }
            public BehaviorTreeUpdateMessage BTUpdateMessage { get; private set; }
            public int Id { get; private set; }

            private List<BehaviorData> _BehaviorList;
            private List<BehaviorData> _States;

            public BehaviorTreeData(Skill.Framework.AI.BehaviorTree tree, int id, string name)
            {
                this.Tree = tree;

                this.Id = id;
                _BehaviorList = new List<BehaviorData>();
                _States = new List<BehaviorData>();
                foreach (var s in Tree.States)
                {
                    var bData = CreateList(_BehaviorList, s);
                    if (bData != null)
                    {
                        _States.Add(bData);
                    }
                }

                Skill.DataModels.AI.BehaviorTree data = new Skill.DataModels.AI.BehaviorTree();
                data.DefaultState = Tree.DefaultState;
                data.Name = name;
                data.States = new Skill.DataModels.AI.BehaviorTreeState[_States.Count];
                for (int i = 0; i < _States.Count; i++)
                    data.States[i] = _States[i].Data;

                BTMessage = new BehaviorTreeMessage();
                BTMessage.Identifier = id;
                BTMessage.Tree = data;
                BTMessage.Behaviors = new string[_BehaviorList.Count];
                for (int i = 0; i < _BehaviorList.Count; i++)                
                    BTMessage.Behaviors[i] = _BehaviorList[i].Behavior.Name;                    
                
                BTUpdateMessage = new BehaviorTreeUpdateMessage();
                BTUpdateMessage.Identifier = id;
                BTUpdateMessage.BehaviorResults = new int[_BehaviorList.Count];
            }

            private BehaviorData CreateList(List<BehaviorData> behaviorList, Skill.Framework.AI.Behavior behavior)
            {
                if (behavior == null) return null;
                BehaviorData bData = null;
                foreach (BehaviorData bt in behaviorList)
                {
                    if (bt.Behavior == behavior)
                    {
                        bData = bt;
                        break;
                    }
                }


                if (bData == null)
                {
                    bData = new BehaviorData(behavior);
                    behaviorList.Add(bData);
                }
                if (behavior.Type == Skill.Framework.AI.BehaviorType.Composite)
                {
                    foreach (Skill.Framework.AI.BehaviorContainer b in (Skill.Framework.AI.Composite)behavior)
                    {
                        var child = CreateList(behaviorList, b.Behavior);
                        if (child != null)
                        {
                            if (!bData.Data.Contains(child.Data))
                                bData.Data.Add(child.Data);
                        }
                    }
                }
                else if (behavior.Type == Skill.Framework.AI.BehaviorType.Decorator)
                {
                    if (((Skill.Framework.AI.Decorator)behavior).Child != null)
                    {
                        var child = CreateList(behaviorList, ((Skill.Framework.AI.Decorator)behavior).Child.Behavior);
                        if (child != null)
                        {
                            if (!bData.Data.Contains(child.Data))
                                bData.Data.Add(child.Data);
                        }
                    }
                }

                return bData;

            }

            public void Update()
            {
                for (int i = 0; i < _BehaviorList.Count; i++)
                    BTUpdateMessage.BehaviorResults[i] = (int)_BehaviorList[i].Behavior.Result;

                BTUpdateMessage.SequenceCount = Tree.Status.SequenceCount;
                BTUpdateMessage.ExecutionSequence = new int[Tree.Status.SequenceCount];
                for (int i = 0; i < Tree.Status.SequenceCount; i++)
                {
                    BTUpdateMessage.ExecutionSequence[i] = IndexOf(Tree.Status.ExecutionSequence[i]);
                }
            }

            private int IndexOf(Skill.Framework.AI.Behavior behavior)
            {
                for (int i = 0; i < _BehaviorList.Count; i++)
                {
                    if (_BehaviorList[i].Behavior == behavior)
                        return i;
                }
                return -1;
            }

        }


        /// <summary>
        /// Convert back behavior to data
        /// </summary>
        class BehaviorData
        {
            public Skill.DataModels.AI.Behavior Data { get; private set; }
            public Skill.Framework.AI.Behavior Behavior { get; private set; }

            public BehaviorData(Skill.Framework.AI.Behavior behavior)
            {
                this.Behavior = behavior;
                switch (Behavior.Type)
                {
                    case Skill.Framework.AI.BehaviorType.Action:
                        this.Data = CreateAction(behavior);
                        break;
                    case Skill.Framework.AI.BehaviorType.Condition:
                        this.Data = CreateCondition(behavior);
                        break;
                    case Skill.Framework.AI.BehaviorType.Decorator:
                        this.Data = CreateDecorator(behavior);
                        break;
                    case Skill.Framework.AI.BehaviorType.Composite:
                        this.Data = CreateComposite(behavior);
                        break;
                    case Skill.Framework.AI.BehaviorType.ChangeState:
                        this.Data = CreateChangeState(behavior);
                        break;
                    default:
                        break;
                }


                if (this.Data != null)
                {
                    this.Data.Concurrency = (Skill.DataModels.AI.ConcurrencyMode)(int)this.Behavior.Concurrency;
                    this.Data.Name = this.Behavior.Name;
                    this.Data.Weight = this.Behavior.Weight;
                }
            }

            private Skill.DataModels.AI.Behavior CreateChangeState(Skill.Framework.AI.Behavior behavior)
            {
                Skill.DataModels.AI.ChangeState cs = new Skill.DataModels.AI.ChangeState();
                cs.DestinationState = ((Skill.Framework.AI.ChangeState)behavior).DestinationState;
                return cs;
            }

            private Skill.DataModels.AI.Behavior CreateComposite(Skill.Framework.AI.Behavior behavior)
            {
                Skill.DataModels.AI.Composite composite = null;

                switch (((Skill.Framework.AI.Composite)behavior).CompositeType)
                {
                    case Skill.Framework.AI.CompositeType.Sequence:
                        composite = new Skill.DataModels.AI.SequenceSelector();
                        break;
                    case Skill.Framework.AI.CompositeType.Concurrent:
                        composite = new Skill.DataModels.AI.ConcurrentSelector();
                        ((Skill.DataModels.AI.ConcurrentSelector)composite).BreakOnConditionFailure = ((Skill.Framework.AI.ConcurrentSelector)behavior).BreakOnConditionFailure;
                        ((Skill.DataModels.AI.ConcurrentSelector)composite).FailurePolicy = (Skill.DataModels.AI.FailurePolicy)((Skill.Framework.AI.ConcurrentSelector)behavior).FailurePolicy;
                        ((Skill.DataModels.AI.ConcurrentSelector)composite).SuccessPolicy = (Skill.DataModels.AI.SuccessPolicy)((Skill.Framework.AI.ConcurrentSelector)behavior).SuccessPolicy;
                        break;
                    case Skill.Framework.AI.CompositeType.Random:
                        composite = new Skill.DataModels.AI.RandomSelector();
                        break;
                    case Skill.Framework.AI.CompositeType.Priority:
                        composite = new Skill.DataModels.AI.PrioritySelector();
                        ((Skill.DataModels.AI.PrioritySelector)composite).Priority = (Skill.DataModels.AI.PriorityType)((Skill.Framework.AI.PrioritySelector)behavior).Priority;
                        break;
                    case Skill.Framework.AI.CompositeType.State:
                        composite = new Skill.DataModels.AI.BehaviorTreeState();
                        ((Skill.DataModels.AI.PrioritySelector)composite).Priority = (Skill.DataModels.AI.PriorityType)((Skill.Framework.AI.PrioritySelector)behavior).Priority;
                        break;
                    case Skill.Framework.AI.CompositeType.Loop:
                        composite = new Skill.DataModels.AI.LoopSelector();
                        ((Skill.DataModels.AI.LoopSelector)composite).LoopCount = ((Skill.Framework.AI.LoopSelector)behavior).LoopCount;
                        break;
                    default:
                        break;
                }

                return composite;
            }

            private Skill.DataModels.AI.Behavior CreateDecorator(Skill.Framework.AI.Behavior behavior)
            {
                Skill.DataModels.AI.Decorator decorator = null;
                switch (((Skill.Framework.AI.Decorator)behavior).DecoratorType)
                {
                    case Skill.Framework.AI.DecoratorType.Default:
                        decorator = new Skill.DataModels.AI.Decorator();
                        break;
                    case Skill.Framework.AI.DecoratorType.AccessLimit:
                        decorator = new Skill.DataModels.AI.AccessLimitDecorator();
                        ((Skill.DataModels.AI.AccessLimitDecorator)decorator).AccessKey = ((Skill.Framework.AI.AccessLimitDecorator)behavior).AccessKey.Key;
                        break;
                    default:
                        break;
                }

                decorator.NeverFail = ((Skill.Framework.AI.Decorator)behavior).NeverFail;
                return decorator;

            }

            private Skill.DataModels.AI.Behavior CreateCondition(Skill.Framework.AI.Behavior behavior)
            {
                Skill.DataModels.AI.Condition condition = new Skill.DataModels.AI.Condition();
                return condition;
            }

            private Skill.DataModels.AI.Behavior CreateAction(Skill.Framework.AI.Behavior behavior)
            {
                Skill.DataModels.AI.Action action = new Skill.DataModels.AI.Action();
                return action;
            }
        }
    }

}
