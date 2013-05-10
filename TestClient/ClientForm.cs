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

namespace TestClient
{
    public partial class ClientSocketForm : Form
    {

        public delegate void UpdateRichEditCallback(string text);

        Client _Client;

        public ClientSocketForm()
        {
            InitializeComponent();
            Logger.LoggerInstance = new MessageBoxLogger();
            _TxtServerIP.Text = Server.GetMyIP();
        }

        private void UpdateControls(bool connected)
        {
            _BtnConnect.Enabled = !connected;
            _BtnDisconnect.Enabled = connected;
            string connectStatus = connected ? "Connected" : "Not Connected";
            _TxtConnectStatus.Text = connectStatus;
        }

        private void _BtnConnect_Click(object sender, EventArgs e)
        {
            // See if we have text on the IP and Port text fields
            if (string.IsNullOrEmpty(_TxtServerIP.Text) || string.IsNullOrEmpty(_TxtServerPort.Text))
            {
                MessageBox.Show("IP Address and Port Number are required to connect to the Server");
                return;
            }
            try
            {

                UpdateControls(false);
                _Client = new Client(new DefaultMessageTranslator());
                _Client.Connect(_TxtServerIP.Text, System.Convert.ToInt16(_TxtServerPort.Text));
                if (_Client.IsConnected)
                    UpdateControls(true);
                _Client.Message += _Client_Message;
            }
            catch (SocketException se)
            {
                string str;
                str = "Connection failed, is the server running?\n" + se.Message;
                MessageBox.Show(str);
                UpdateControls(false);
            }
        }

        void _Client_Message(object sender, Skill.Net.Message msg)
        {
            switch ((MessageType)msg.Type)
            {
                case MessageType.Disconnect:
                    _Client.Close();
                    break;
                case MessageType.Text:
                    if (_RTRxMessage.InvokeRequired)
                    {
                        object[] pList = { ((TextMessage)msg).Text };
                        _RTRxMessage.BeginInvoke(new UpdateRichEditCallback(OnUpdateRichEdit), pList);
                    }
                    else
                    {
                        _RTRxMessage.Text = _RTRxMessage.Text + ((TextMessage)msg).Text;
                    }
                    break;
                default:
                    break;
            }
        }

        // This UpdateRichEdit will be run back on the UI thread
        // (using System.EventHandler signature
        // so we don't need to define a new
        // delegate type here)
        private void OnUpdateRichEdit(string msg)
        {
            _RTRxMessage.AppendText(msg);
            _RTRxMessage.AppendText("\n");            
        }

        private void Disconnect()
        {
            if (_Client != null)
            {
                _Client.Message -= _Client_Message;
                _Client.Close();
                _Client = null;
            }
            UpdateControls(false);
        }

        private void _BtnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void _BtnSendMessage_Click(object sender, EventArgs e)
        {
            if (_Client.IsConnected)
            {
                TextMessage msg = new TextMessage();
                msg.Text = _RTTxMessage.Text;
                _Client.SendMessage(msg);
                _RTTxMessage.Clear();
            }
        }

        private void _BtnClear_Click(object sender, EventArgs e)
        {
            _RTRxMessage.Clear();
        }

        private void _BtnClose_Click(object sender, EventArgs e)
        {
            Disconnect();
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
