namespace TestClient
{
    partial class ClientSocketForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private System.Windows.Forms.Label _LblMsgFromServe;
        private System.Windows.Forms.Label _LblServerPort;
        private System.Windows.Forms.Label _LblServerIP;
        private System.Windows.Forms.Button _BtnDisconnect;
        private System.Windows.Forms.TextBox _TxtServerIP;
        private System.Windows.Forms.Label _LblConnectStatus;
        private System.Windows.Forms.Button _BtnConnect;
        private System.Windows.Forms.TextBox _TxtServerPort;
        private System.Windows.Forms.RichTextBox _RTRxMessage;
        private System.Windows.Forms.Label _LblMsgToServer;
        private System.Windows.Forms.TextBox _TxtConnectStatus;
        private System.Windows.Forms.RichTextBox _RTTxMessage;
        private System.Windows.Forms.Button _BtnSendMessage;
        private System.Windows.Forms.Button _BtnClose;
        private System.Windows.Forms.Button _BtnClear;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._BtnClose = new System.Windows.Forms.Button();
            this._BtnSendMessage = new System.Windows.Forms.Button();
            this._RTTxMessage = new System.Windows.Forms.RichTextBox();
            this._TxtConnectStatus = new System.Windows.Forms.TextBox();
            this._LblMsgToServer = new System.Windows.Forms.Label();
            this._RTRxMessage = new System.Windows.Forms.RichTextBox();
            this._TxtServerPort = new System.Windows.Forms.TextBox();
            this._BtnConnect = new System.Windows.Forms.Button();
            this._LblConnectStatus = new System.Windows.Forms.Label();
            this._TxtServerIP = new System.Windows.Forms.TextBox();
            this._BtnDisconnect = new System.Windows.Forms.Button();
            this._LblServerIP = new System.Windows.Forms.Label();
            this._LblServerPort = new System.Windows.Forms.Label();
            this._LblMsgFromServe = new System.Windows.Forms.Label();
            this._BtnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _BtnClose
            // 
            this._BtnClose.Location = new System.Drawing.Point(440, 216);
            this._BtnClose.Name = "_BtnClose";
            this._BtnClose.Size = new System.Drawing.Size(64, 24);
            this._BtnClose.TabIndex = 11;
            this._BtnClose.Text = "Close";
            this._BtnClose.Click += new System.EventHandler(this._BtnClose_Click);
            // 
            // _BtnSendMessage
            // 
            this._BtnSendMessage.Location = new System.Drawing.Point(8, 184);
            this._BtnSendMessage.Name = "_BtnSendMessage";
            this._BtnSendMessage.Size = new System.Drawing.Size(240, 24);
            this._BtnSendMessage.TabIndex = 14;
            this._BtnSendMessage.Text = "Send Message";
            this._BtnSendMessage.Click += new System.EventHandler(this._BtnSendMessage_Click);
            // 
            // _RTTxMessage
            // 
            this._RTTxMessage.Location = new System.Drawing.Point(8, 80);
            this._RTTxMessage.Name = "_RTTxMessage";
            this._RTTxMessage.Size = new System.Drawing.Size(240, 96);
            this._RTTxMessage.TabIndex = 2;
            this._RTTxMessage.Text = "";
            // 
            // _TxtConnectStatus
            // 
            this._TxtConnectStatus.BackColor = System.Drawing.SystemColors.Control;
            this._TxtConnectStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._TxtConnectStatus.ForeColor = System.Drawing.SystemColors.HotTrack;
            this._TxtConnectStatus.Location = new System.Drawing.Point(128, 224);
            this._TxtConnectStatus.Name = "_TxtConnectStatus";
            this._TxtConnectStatus.ReadOnly = true;
            this._TxtConnectStatus.Size = new System.Drawing.Size(240, 13);
            this._TxtConnectStatus.TabIndex = 10;
            this._TxtConnectStatus.Text = "Not Connected";
            // 
            // _LblMsgToServer
            // 
            this._LblMsgToServer.Location = new System.Drawing.Point(8, 64);
            this._LblMsgToServer.Name = "_LblMsgToServer";
            this._LblMsgToServer.Size = new System.Drawing.Size(120, 16);
            this._LblMsgToServer.TabIndex = 9;
            this._LblMsgToServer.Text = "Message To Server";
            // 
            // _RTRxMessage
            // 
            this._RTRxMessage.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this._RTRxMessage.Location = new System.Drawing.Point(256, 80);
            this._RTRxMessage.Name = "_RTRxMessage";
            this._RTRxMessage.ReadOnly = true;
            this._RTRxMessage.Size = new System.Drawing.Size(248, 128);
            this._RTRxMessage.TabIndex = 1;
            this._RTRxMessage.Text = "";
            // 
            // _TxtServerPort
            // 
            this._TxtServerPort.Location = new System.Drawing.Point(112, 31);
            this._TxtServerPort.Name = "_TxtServerPort";
            this._TxtServerPort.Size = new System.Drawing.Size(48, 20);
            this._TxtServerPort.TabIndex = 6;
            this._TxtServerPort.Text = "8000";
            // 
            // _BtnConnect
            // 
            this._BtnConnect.BackColor = System.Drawing.SystemColors.HotTrack;
            this._BtnConnect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._BtnConnect.ForeColor = System.Drawing.Color.Yellow;
            this._BtnConnect.Location = new System.Drawing.Point(344, 8);
            this._BtnConnect.Name = "_BtnConnect";
            this._BtnConnect.Size = new System.Drawing.Size(72, 48);
            this._BtnConnect.TabIndex = 7;
            this._BtnConnect.Text = "Connect To Server";
            this._BtnConnect.UseVisualStyleBackColor = false;
            this._BtnConnect.Click += new System.EventHandler(this._BtnConnect_Click);
            // 
            // _LblConnectStatus
            // 
            this._LblConnectStatus.Location = new System.Drawing.Point(0, 224);
            this._LblConnectStatus.Name = "_LblConnectStatus";
            this._LblConnectStatus.Size = new System.Drawing.Size(104, 16);
            this._LblConnectStatus.TabIndex = 13;
            this._LblConnectStatus.Text = "Connection Status";
            // 
            // _TxtServerIP
            // 
            this._TxtServerIP.Location = new System.Drawing.Point(112, 8);
            this._TxtServerIP.Name = "_TxtServerIP";
            this._TxtServerIP.Size = new System.Drawing.Size(152, 20);
            this._TxtServerIP.TabIndex = 3;
            // 
            // _BtnDisconnect
            // 
            this._BtnDisconnect.BackColor = System.Drawing.Color.Red;
            this._BtnDisconnect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._BtnDisconnect.ForeColor = System.Drawing.Color.Yellow;
            this._BtnDisconnect.Location = new System.Drawing.Point(432, 8);
            this._BtnDisconnect.Name = "_BtnDisconnect";
            this._BtnDisconnect.Size = new System.Drawing.Size(72, 48);
            this._BtnDisconnect.TabIndex = 15;
            this._BtnDisconnect.Text = "Disconnet From Server";
            this._BtnDisconnect.UseVisualStyleBackColor = false;
            this._BtnDisconnect.Click += new System.EventHandler(this._BtnDisconnect_Click);
            // 
            // _LblServerIP
            // 
            this._LblServerIP.Location = new System.Drawing.Point(8, 8);
            this._LblServerIP.Name = "_LblServerIP";
            this._LblServerIP.Size = new System.Drawing.Size(96, 16);
            this._LblServerIP.TabIndex = 4;
            this._LblServerIP.Text = "Server IP Address";
            // 
            // _LblServerPort
            // 
            this._LblServerPort.Location = new System.Drawing.Point(8, 33);
            this._LblServerPort.Name = "_LblServerPort";
            this._LblServerPort.Size = new System.Drawing.Size(64, 16);
            this._LblServerPort.TabIndex = 5;
            this._LblServerPort.Text = "Server Port";
            // 
            // _LblMsgFromServe
            // 
            this._LblMsgFromServe.Location = new System.Drawing.Point(256, 64);
            this._LblMsgFromServe.Name = "_LblMsgFromServe";
            this._LblMsgFromServe.Size = new System.Drawing.Size(192, 16);
            this._LblMsgFromServe.TabIndex = 8;
            this._LblMsgFromServe.Text = "Message From Server";
            // 
            // _BtnClear
            // 
            this._BtnClear.Location = new System.Drawing.Point(376, 216);
            this._BtnClear.Name = "_BtnClear";
            this._BtnClear.Size = new System.Drawing.Size(64, 24);
            this._BtnClear.TabIndex = 16;
            this._BtnClear.Text = "Clear";
            this._BtnClear.Click += new System.EventHandler(this._BtnClear_Click);
            // 
            // ClientSocketForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(512, 244);
            this.Controls.Add(this._BtnClear);
            this.Controls.Add(this._BtnDisconnect);
            this.Controls.Add(this._BtnSendMessage);
            this.Controls.Add(this._LblConnectStatus);
            this.Controls.Add(this._BtnClose);
            this.Controls.Add(this._TxtConnectStatus);
            this.Controls.Add(this._LblMsgToServer);
            this.Controls.Add(this._LblMsgFromServe);
            this.Controls.Add(this._BtnConnect);
            this.Controls.Add(this._TxtServerPort);
            this.Controls.Add(this._LblServerPort);
            this.Controls.Add(this._LblServerIP);
            this.Controls.Add(this._TxtServerIP);
            this.Controls.Add(this._RTTxMessage);
            this.Controls.Add(this._RTRxMessage);
            this.Name = "ClientSocketForm";
            this.Text = "Socket Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}

