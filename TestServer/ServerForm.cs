using Skill.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace TestServer
{
    public partial class ServerForm : Form
    {
        public delegate void UpdateRichEditCallback(string text);
        public delegate void UpdateClientListCallback();

        private Server _Server;

        public ServerForm()
        {
            InitializeComponent();
            Logger.LoggerInstance = new MessageBoxLogger();
            _TxtIP.Text = Server.GetMyIP();
            _Server = new Server(new DefaultMessageTranslator());
            _Server.ClientConnected += _Server_ClientConnected;
            _Server.ClientDisconnected += _Server_ClientDisconnected;
        }

        void _Server_ClientDisconnected(object sender, Worker worker)
        {
            worker.Message -= worker_Message;
        }

        void _Server_ClientConnected(object sender, Worker worker)
        {
            worker.Message += worker_Message;
            TextMessage msg = new TextMessage();
            msg.Text = "Welcome client " + worker.Name;
            worker.SendMessage(msg);

            // Update the list box showing the list of clients (thread safe call)
            UpdateClientListControl();
        }

        void worker_Message(object sender, Skill.Net.Message msg)
        {
            if (msg.Type == (int)MessageType.Text)
            {
                AppendToRichEditControl(string.Format("{0} : {1}", ((ServerWorker)sender), ((TextMessage)msg).Text));
                TextMessage reply = new TextMessage();
                // Send back the reply to the client
                reply.Text = "Server Reply :" + ((TextMessage)msg).Text.ToUpper();
                ((ServerWorker)sender).SendMessage(reply);
            }
        }

        // This method could be called by either the main thread or any of the
        // worker threads
        private void AppendToRichEditControl(string msg)
        {
            // Check to see if this method is called from a thread 
            // other than the one created the control
            if (InvokeRequired)
            {
                // We cannot update the GUI on this thread.
                // All GUI controls are to be updated by the main (GUI) thread.
                // Hence we will use the invoke method on the control which will
                // be called when the Main thread is free
                // Do UI update on UI thread
                object[] pList = { msg };
                _RTxtReceivedMsg.BeginInvoke(new UpdateRichEditCallback(OnUpdateRichEdit), pList);
            }
            else
            {
                // This is the main thread which created this control, hence update it
                // directly 
                OnUpdateRichEdit(msg);
            }
        }
        // This UpdateRichEdit will be run back on the UI thread
        // (using System.EventHandler signature
        // so we don't need to define a new
        // delegate type here)
        private void OnUpdateRichEdit(string msg)
        {
            _RTxtReceivedMsg.AppendText(msg);
            _RTxtReceivedMsg.AppendText("\n");
        }

        private void UpdateClientListControl()
        {
            if (InvokeRequired) // Is this called from a thread other than the one created
            // the control
            {
                // We cannot update the GUI on this thread.
                // All GUI controls are to be updated by the main (GUI) thread.
                // Hence we will use the invoke method on the control which will
                // be called when the Main thread is free
                // Do UI update on UI thread
                _LbClientList.BeginInvoke(new UpdateClientListCallback(UpdateClientList), null);
            }
            else
            {
                // This is the main thread which created this control, hence update it
                // directly 
                UpdateClientList();
            }
        }

        // Update the list of clients that is displayed
        void UpdateClientList()
        {
            _LbClientList.Items.Clear();
            for (int i = 0; i < _Server.ClientCount; i++)
            {
                ServerWorker workerSocket = _Server[i];
                if (workerSocket != null)
                {
                    if (workerSocket.IsConnected)
                    {
                        _LbClientList.Items.Add(workerSocket.Name);
                    }
                }
            }
        }

        private void UpdateControls(bool listening)
        {
            _BtnStartListen.Enabled = !listening;
            _BtnStopListen.Enabled = listening;
        }

        private void _TxtStartListen_Click(object sender, EventArgs e)
        {
            // Check the port value
            if (string.IsNullOrEmpty(_TxtPort.Text))
            {
                MessageBox.Show("Please enter a Port Number");
                return;
            }

            try
            {
                string portStr = _TxtPort.Text;
                int port = System.Convert.ToInt32(portStr);
                _Server.Start(port);
                UpdateControls(true);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void _BtnStopListen_Click(object sender, EventArgs e)
        {
            _Server.Close();
            UpdateControls(false);
        }

        private void _BtnSendMsg_Click(object sender, EventArgs e)
        {
            try
            {
                TextMessage msg = new TextMessage();
                msg.Text = string.Format("Server Msg: {0}", _RTxtSendMsg.Text);
                for (int i = 0; i < _Server.ClientCount; i++)
                {
                    ServerWorker worker = _Server[i];
                    if (worker != null)
                    {
                        if (worker.IsConnected)
                        {
                            worker.SendMessage(msg);
                        }
                    }
                }

                _RTxtSendMsg.Clear();
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void _BtnClear_Click(object sender, EventArgs e)
        {
            _RTxtReceivedMsg.Clear();
        }

        private void _BtnClose_Click(object sender, EventArgs e)
        {
            _Server.Close();
            Close();
        }
    }


    class MessageBoxLogger : ILogger
    {
        public void LogError(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        public void LogError(string errorMsg)
        {
            MessageBox.Show(errorMsg);
        }

        public void LogWarning(string warningMsg)
        {
            MessageBox.Show(warningMsg);
        }

        public void LogMessage(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
