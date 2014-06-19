using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Interaction logic for BehaviorTreeRemoteDebugger.xaml
    /// </summary>
    public partial class BehaviorTreeRemoteDebugger : TabDocument
    {
        #region MessageBoxLogger
        class BTLogger : Skill.Net.ILogger
        {
            private delegate void LogHandler(string message);

            private BehaviorTreeRemoteDebugger _Debugger;
            private LogHandler _LogHandler;
            public BTLogger(BehaviorTreeRemoteDebugger debugger)
            {
                _Debugger = debugger;
                _LogHandler = Log;
            }

            public void LogError(Exception ex)
            {
                if (!ex.Message.EndsWith("\n"))
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, string.Format("{0}\n", ex.Message));
                else
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, ex.Message);
            }

            public void LogError(string errorMsg)
            {
                if (!errorMsg.EndsWith("\n"))
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, string.Format("{0}\n", errorMsg));
                else
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, errorMsg);
            }

            public void LogWarning(string warningMsg)
            {
                if (!warningMsg.EndsWith("\n"))
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, string.Format("{0}\n", warningMsg));
                else
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, warningMsg);
            }

            public void LogMessage(string msg)
            {
                if (!msg.EndsWith("\n"))
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, string.Format("{0}\n", msg));
                else
                    _Debugger.Dispatcher.BeginInvoke(_LogHandler, msg);
            }
            void Log(string message)
            {
                if (!message.EndsWith("\n"))
                    _Debugger._RtxtLogs.AppendText(string.Format("{0}\n", message));
                else
                    _Debugger._RtxtLogs.AppendText(message);
            }
        }
        #endregion

        delegate void MessageHandler(Skill.Net.Message msg);
        delegate void ChangeTreeHandler(Skill.Diagnostics.ControllerListItem newTree);

        private BehaviorViewModel[] _Behaviors;
        private Skill.Net.Client _Client;
        private Skill.Diagnostics.ControllerListItem _SelectedItem;

        public ObservableCollection<Skill.Diagnostics.ControllerListItem> Identifiers { get; private set; }

        public BehaviorTreeRemoteDebugger()
            : base(null)
        {
            Identifiers = new ObservableCollection<Skill.Diagnostics.ControllerListItem>();
            InitializeComponent();
            Skill.Net.Logger.ReplaceInstance(new BTLogger(this));
            ValidateUnityDebugServerIP();

            _TxtServerIP.Text = Properties.Settings.Default.UnityServerIP;
            _ServerPort.Value = Properties.Settings.Default.UnityServerPort;
            _BufferSize.Value = Properties.Settings.Default.UnityServerBufferSize;

            this.Loaded += BehaviorTreeRemoteDebugger_Loaded;
        }

        void Client_Closed(object sender, EventArgs e)
        {
        }

        void Client_Message(object sender, Net.Message msg)
        {
            System.Windows.Threading.DispatcherPriority priority = System.Windows.Threading.DispatcherPriority.Normal;

            switch ((Skill.Diagnostics.MessageType)msg.Type)
            {
                case Skill.Diagnostics.MessageType.Disconnect:
                    this.Dispatcher.BeginInvoke(new MessageHandler(Disconnect), priority, msg);
                    break;
                case Skill.Diagnostics.MessageType.Text:
                    this.Dispatcher.BeginInvoke(new MessageHandler(LogMessage), priority, msg);
                    break;
                case Skill.Diagnostics.MessageType.ControllersList:
                    this.Dispatcher.BeginInvoke(new MessageHandler(RefreshList), priority, msg);
                    break;
                case Skill.Diagnostics.MessageType.BehaviorTree:
                    this.Dispatcher.BeginInvoke(new MessageHandler(CreateTree), priority, msg);
                    break;
                case Skill.Diagnostics.MessageType.BehaviorTreeUpdate:
                    this.Dispatcher.BeginInvoke(new MessageHandler(UpdateTree), priority, msg);
                    break;
                case Skill.Diagnostics.MessageType.ControllerNotFound:
                    if (_SelectedItem != null && _SelectedItem.InstanceId == ((Skill.Diagnostics.ControllerNotFoundMessage)msg).Identifier)
                        this.Dispatcher.BeginInvoke(new ChangeTreeHandler(ChangeTree), priority, null);
                    break;
                default:
                    break;
            }
        }

        void BehaviorTreeRemoteDebugger_Loaded(object sender, RoutedEventArgs e)
        {
            ParentDocument.Closing += ParentDocument_Closing;
        }

        void ParentDocument_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_Client != null && _Client.IsConnected)
            {
                if (System.Windows.MessageBox.Show("Disconnect and close", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    Disconnect();
            }
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_Client != null && _Client.IsConnected)
                {
                    Disconnect();
                }
                else
                {
                    if (IsSaveUnityDebugServerValid())
                    {
                        SaveUnityDebugServerIP();
                        string serverIP = _TxtServerIP.Text;
                        int serverPort = _ServerPort.Value.Value;

                        _Client = new Net.Client(new Skill.Diagnostics.MessageTranslator(), _BufferSize.Value.Value * 1024);
                        _Client.Message += Client_Message;
                        _Client.Closed += Client_Closed;

                        _Client.Connect(serverIP, serverPort);
                        if (!_Client.IsConnected)
                        {
                            return;
                        }
                        else
                            RefreshList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                UpdateBtnConnect();
            }
        }

        private void UpdateBtnConnect()
        {
            _BtnConnect.Content = (_Client != null && _Client.IsConnected) ? "Disconnect" : "Connect";
        }

        private void Disconnect()
        {
            if (_Client != null && _Client.IsConnected)
            {
                _Client.Close();
                _Client.Message -= Client_Message;
                _Client.Closed -= Client_Closed;
                _Client = null;
            }
            UpdateBtnConnect();
            Identifiers.Clear();
            _GraphView.BehaviorTree = null;
        }

        private void Disconnect(Skill.Net.Message msg)
        {
            if (_Client != null && _Client.IsConnected)
            {
                _Client.Close();
                _Client.Message -= Client_Message;
                _Client.Closed -= Client_Closed;
                _Client = null;
            }
            UpdateBtnConnect();
            Identifiers.Clear();
            _GraphView.BehaviorTree = null;
        }

        private void RefreshList()
        {
            if (_Client == null || !_Client.IsConnected) return;
            Skill.Diagnostics.RequestControllersListMessage msg = new Diagnostics.RequestControllersListMessage();
            _Client.SendMessage(msg);
        }

        private void BtnRefreshList_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        private void CmbIdentifiers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IgnoreChangeIdentifier)
            {
                if (_CmbIdentifiers.SelectedItem != null)
                {
                    Skill.Diagnostics.ControllerListItem item = _CmbIdentifiers.SelectedItem as Skill.Diagnostics.ControllerListItem;
                    if (item != null)
                        ChangeTree(item);
                    else
                        ChangeTree(null);
                }
                else
                {
                    ChangeTree(null);
                }
            }
        }

        private void LogMessage(Skill.Net.Message msg)
        {
            Skill.Net.TextMessage textMsg = msg as Skill.Net.TextMessage;
            Skill.Net.Logger.LogWarning(textMsg.Text);
        }

        private void UpdateTree(Skill.Net.Message msg)
        {
            Diagnostics.BehaviorTreeUpdateMessage updateMsg = msg as Diagnostics.BehaviorTreeUpdateMessage;

            if (_GraphView.BehaviorTree != null && _Behaviors != null && _SelectedItem != null && _SelectedItem.InstanceId == updateMsg.Identifier)
            {
                if (updateMsg.SequenceCount > 0)
                {

                    if (updateMsg.BehaviorResults.Length == _Behaviors.Length)
                    {
                        if (_GraphView.BehaviorTree.Root != _Behaviors[updateMsg.ExecutionSequence[0]])
                            _GraphView.BehaviorTree.ChangeState(_Behaviors[updateMsg.ExecutionSequence[0]].Name);
                        if (_GraphView.BehaviorTree.Root != null)
                        {
                            _GraphView.BehaviorTree.Root.Debug.UpdateChildren();

                            for (int i = 0; i < updateMsg.BehaviorResults.Length; i++)
                            {
                                if (_Behaviors[i] != null)
                                    _Behaviors[i].Debug.Behavior.Result = (Framework.AI.BehaviorResult)updateMsg.BehaviorResults[i];
                            }

                            // notify debug behaviors that all ExecutionSequence is visited
                            for (int i = 0; i < updateMsg.SequenceCount; i++)
                            {
                                var item = _Behaviors[updateMsg.ExecutionSequence[i]];
                                if (item != null)
                                    item.Debug.IsVisited = true;
                            }
                            _GraphView.BehaviorTree.Root.Debug.ValidateBrush(true);
                        }
                    }
                    else
                    {
                        Skill.Net.Logger.LogError("Invalid BehaviorTree Update data");
                        ChangeTree(null);
                    }
                }
            }
        }

        private void CreateTree(Skill.Net.Message msg)
        {
            Diagnostics.BehaviorTreeMessage btMsg = msg as Diagnostics.BehaviorTreeMessage;

            if (_SelectedItem != null && msg != null && btMsg.Identifier == _SelectedItem.InstanceId)
            {
                try
                {
                    _GraphView.BehaviorTree = new BehaviorTreeViewModel(btMsg.Tree);
                    _GraphView.BehaviorTree.IsDebuging = true;
                    if (btMsg.Behaviors != null)
                    {
                        _Behaviors = new BehaviorViewModel[btMsg.Behaviors.Length];
                        for (int i = 0; i < _Behaviors.Length; i++)
                            _Behaviors[i] = _GraphView.BehaviorTree.Behaviors.FirstOrDefault<BehaviorViewModel>(b => b.Name == btMsg.Behaviors[i]);

                    }
                }
                catch (Exception ex)
                {
                    _GraphView.BehaviorTree = null;
                    Skill.Net.Logger.LogError(ex.Message);
                }

            }
            else
            {
                Skill.Net.Logger.LogError("Invalid tree data");
            }
        }

        private bool _IgnoreChangeIdentifier;
        private void RefreshList(Skill.Net.Message msg)
        {
            Diagnostics.ControllersListMessage btListMsg = msg as Diagnostics.ControllersListMessage;

            if (btListMsg.Controllers == null || btListMsg.Controllers.Length == 0)
            {
                Identifiers.Clear();
                _CmbIdentifiers.SelectedItem = null;
            }
            else
            {
                _IgnoreChangeIdentifier = true;
                Skill.Diagnostics.ControllerListItem selectedItem = _CmbIdentifiers.SelectedItem as Skill.Diagnostics.ControllerListItem;

                if (selectedItem != null)
                {
                    for (int i = 0; i < btListMsg.Controllers.Length; i++)
                    {
                        if (btListMsg.Controllers[i].InstanceId == selectedItem.InstanceId)
                        {
                            btListMsg.Controllers[i] = selectedItem;
                            break;
                        }
                    }
                }

                Identifiers.Clear();
                foreach (var item in btListMsg.Controllers)
                    Identifiers.Add(item);

                if (selectedItem != null)
                {
                    if (_CmbIdentifiers.Items.Contains(selectedItem))
                        _CmbIdentifiers.SelectedItem = selectedItem;
                    else
                    {
                        _IgnoreChangeIdentifier = false;
                        _CmbIdentifiers.SelectedItem = null;
                    }
                }


                _IgnoreChangeIdentifier = false;
            }
        }

        private bool Exist(IEnumerable<Diagnostics.ControllerListItem> list, Diagnostics.ControllerListItem item)
        {
            foreach (var i in list)
            {
                if (item.Equals(i)) return true;
            }
            return false;
        }
        private void ChangeTree(Skill.Diagnostics.ControllerListItem newTree)
        {
            if (_Client != null && _Client.IsConnected)
            {
                if (newTree == null)
                {
                    if (_SelectedItem != null)
                    {
                        Skill.Diagnostics.RegisterControllerBTMessage msg = new Diagnostics.RegisterControllerBTMessage();
                        msg.Identifier = _SelectedItem.InstanceId;
                        msg.Register = false;
                        _Client.SendMessage(msg);
                    }
                    _GraphView.BehaviorTree = null;
                    _SelectedItem = null;
                }
                else if (_SelectedItem != null && newTree.InstanceId == _SelectedItem.InstanceId) return;
                else
                {
                    _SelectedItem = newTree;
                    Skill.Diagnostics.RegisterControllerBTMessage msg = new Diagnostics.RegisterControllerBTMessage();
                    msg.Identifier = _SelectedItem.InstanceId;
                    msg.Register = true;
                    _Client.SendMessage(msg);
                }

                _GraphView.RefreshGraph();
            }
        }

        private void ValidateUnityDebugServerIP()
        {
            bool change = false;
            if (string.IsNullOrEmpty(Properties.Settings.Default.UnityServerIP))
            {
                Properties.Settings.Default.UnityServerIP = Skill.Net.Server.GetMyIP();
                change = true;
            }

            if (Properties.Settings.Default.UnityServerPort < 1 || Properties.Settings.Default.UnityServerPort > ushort.MaxValue)
            {
                Properties.Settings.Default.UnityServerPort = 4200;// Skill.Diagnostics.BehaviorTreeDebugServer.DefaultPort;
                change = true;
            }

            if (Properties.Settings.Default.UnityServerBufferSize < _BufferSize.Minimum || Properties.Settings.Default.UnityServerBufferSize > _BufferSize.Maximum)
            {
                Properties.Settings.Default.UnityServerBufferSize = 20;
                change = true;
            }
            if (change)
            {
                Properties.Settings.Default.Save();
            }
        }

        private void SaveUnityDebugServerIP()
        {
            string serverIP = _TxtServerIP.Text;
            int serverPort = _ServerPort.Value.Value;
            int serverBufferSize = _BufferSize.Value.Value;

            Properties.Settings.Default.UnityServerIP = serverIP;
            Properties.Settings.Default.UnityServerPort = serverPort;
            Properties.Settings.Default.UnityServerBufferSize = serverBufferSize;

            Properties.Settings.Default.Save();
        }

        private bool IsSaveUnityDebugServerValid()
        {
            string serverIP = _TxtServerIP.Text;
            int serverPort = _ServerPort.Value.Value;
            int serverBufferSize = _BufferSize.Value.Value;

            bool valid = true;

            if (string.IsNullOrEmpty(serverIP))
            {
                valid = false;
                Skill.Net.Logger.LogError("Invalid server ip");
            }
            if (serverPort < 1 || serverPort > ushort.MaxValue)
            {
                valid = false;
                Skill.Net.Logger.LogError("Invalid server port");
            }

            if (serverBufferSize < _BufferSize.Minimum || serverBufferSize > _BufferSize.Maximum)
            {
                valid = false;
                Skill.Net.Logger.LogError("Invalid buffer size ");
            }

            return valid;
        }

        public int instanceId { get; set; }
    }
}
