namespace TestServer
{
    partial class ServerForm
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

        private System.Windows.Forms.Label _Lbl1;
        private System.Windows.Forms.Label _Lbl2;
        private System.Windows.Forms.Label _Lbl3;
        private System.Windows.Forms.Label _Lbl4;
        private System.Windows.Forms.Label _Lbl5;
        private System.Windows.Forms.Label _Lbl6;
        private System.Windows.Forms.RichTextBox _RTxtReceivedMsg;
        private System.Windows.Forms.TextBox _TxtPort;
        private System.Windows.Forms.TextBox _TxtMsg;
        private System.Windows.Forms.Button _BtnStopListen;
        private System.Windows.Forms.RichTextBox _RTxtSendMsg;
        private System.Windows.Forms.TextBox _TxtIP;
        private System.Windows.Forms.Button _BtnStartListen;
        private System.Windows.Forms.Button _BtnSendMsg;
        private System.Windows.Forms.Button _BtnClose;
        private System.Windows.Forms.ListBox _LbClientList;
        private System.Windows.Forms.Button _BtnClear;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._BtnClose = new System.Windows.Forms.Button();
            this._BtnSendMsg = new System.Windows.Forms.Button();
            this._BtnStartListen = new System.Windows.Forms.Button();
            this._TxtIP = new System.Windows.Forms.TextBox();
            this._RTxtSendMsg = new System.Windows.Forms.RichTextBox();
            this._Lbl1 = new System.Windows.Forms.Label();
            this._BtnStopListen = new System.Windows.Forms.Button();
            this._TxtMsg = new System.Windows.Forms.TextBox();
            this._Lbl4 = new System.Windows.Forms.Label();
            this._Lbl5 = new System.Windows.Forms.Label();
            this._TxtPort = new System.Windows.Forms.TextBox();
            this._RTxtReceivedMsg = new System.Windows.Forms.RichTextBox();
            this._Lbl2 = new System.Windows.Forms.Label();
            this._Lbl3 = new System.Windows.Forms.Label();
            this._LbClientList = new System.Windows.Forms.ListBox();
            this._Lbl6 = new System.Windows.Forms.Label();
            this._BtnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _BtnClose
            // 
            this._BtnClose.Location = new System.Drawing.Point(321, 328);
            this._BtnClose.Name = "_BtnClose";
            this._BtnClose.Size = new System.Drawing.Size(88, 24);
            this._BtnClose.TabIndex = 11;
            this._BtnClose.Text = "Close";
            this._BtnClose.Click += new System.EventHandler(this._BtnClose_Click);
            // 
            // _BtnSendMsg
            // 
            this._BtnSendMsg.Location = new System.Drawing.Point(16, 144);
            this._BtnSendMsg.Name = "_BtnSendMsg";
            this._BtnSendMsg.Size = new System.Drawing.Size(192, 24);
            this._BtnSendMsg.TabIndex = 7;
            this._BtnSendMsg.Text = "Send Message";
            this._BtnSendMsg.Click += new System.EventHandler(this._BtnSendMsg_Click);
            // 
            // _BtnStartListen
            // 
            this._BtnStartListen.BackColor = System.Drawing.Color.Blue;
            this._BtnStartListen.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._BtnStartListen.ForeColor = System.Drawing.Color.Yellow;
            this._BtnStartListen.Location = new System.Drawing.Point(227, 16);
            this._BtnStartListen.Name = "_BtnStartListen";
            this._BtnStartListen.Size = new System.Drawing.Size(88, 40);
            this._BtnStartListen.TabIndex = 4;
            this._BtnStartListen.Text = "Start Listening";
            this._BtnStartListen.UseVisualStyleBackColor = false;
            this._BtnStartListen.Click += new System.EventHandler(this._TxtStartListen_Click);
            // 
            // _TxtIP
            // 
            this._TxtIP.Location = new System.Drawing.Point(88, 16);
            this._TxtIP.Name = "_TxtIP";
            this._TxtIP.ReadOnly = true;
            this._TxtIP.Size = new System.Drawing.Size(120, 20);
            this._TxtIP.TabIndex = 12;
            // 
            // _RTxtSendMsg
            // 
            this._RTxtSendMsg.Location = new System.Drawing.Point(16, 87);
            this._RTxtSendMsg.Name = "_RTxtSendMsg";
            this._RTxtSendMsg.Size = new System.Drawing.Size(192, 57);
            this._RTxtSendMsg.TabIndex = 6;
            this._RTxtSendMsg.Text = "";
            // 
            // _Lbl1
            // 
            this._Lbl1.Location = new System.Drawing.Point(16, 40);
            this._Lbl1.Name = "_Lbl1";
            this._Lbl1.Size = new System.Drawing.Size(48, 16);
            this._Lbl1.TabIndex = 1;
            this._Lbl1.Text = "Port";
            // 
            // _BtnStopListen
            // 
            this._BtnStopListen.BackColor = System.Drawing.Color.Red;
            this._BtnStopListen.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._BtnStopListen.ForeColor = System.Drawing.Color.Yellow;
            this._BtnStopListen.Location = new System.Drawing.Point(321, 16);
            this._BtnStopListen.Name = "_BtnStopListen";
            this._BtnStopListen.Size = new System.Drawing.Size(88, 40);
            this._BtnStopListen.TabIndex = 5;
            this._BtnStopListen.Text = "Stop Listening";
            this._BtnStopListen.UseVisualStyleBackColor = false;
            this._BtnStopListen.Click += new System.EventHandler(this._BtnStopListen_Click);
            // 
            // _TxtMsg
            // 
            this._TxtMsg.BackColor = System.Drawing.SystemColors.Control;
            this._TxtMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._TxtMsg.ForeColor = System.Drawing.SystemColors.HotTrack;
            this._TxtMsg.Location = new System.Drawing.Point(112, 340);
            this._TxtMsg.Name = "_TxtMsg";
            this._TxtMsg.ReadOnly = true;
            this._TxtMsg.Size = new System.Drawing.Size(192, 13);
            this._TxtMsg.TabIndex = 14;
            this._TxtMsg.Text = "None";
            // 
            // _Lbl4
            // 
            this._Lbl4.Location = new System.Drawing.Point(16, 71);
            this._Lbl4.Name = "_Lbl4";
            this._Lbl4.Size = new System.Drawing.Size(192, 16);
            this._Lbl4.TabIndex = 8;
            this._Lbl4.Text = "Broadcast Message To Clients";
            // 
            // _Lbl5
            // 
            this._Lbl5.Location = new System.Drawing.Point(217, 71);
            this._Lbl5.Name = "_Lbl5";
            this._Lbl5.Size = new System.Drawing.Size(192, 16);
            this._Lbl5.TabIndex = 10;
            this._Lbl5.Text = "Message Received From Clients";
            // 
            // _TxtPort
            // 
            this._TxtPort.Location = new System.Drawing.Point(88, 40);
            this._TxtPort.Name = "_TxtPort";
            this._TxtPort.Size = new System.Drawing.Size(40, 20);
            this._TxtPort.TabIndex = 0;
            this._TxtPort.Text = "8000";
            // 
            // _TTxtReceivedMsg
            // 
            this._RTxtReceivedMsg.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this._RTxtReceivedMsg.Location = new System.Drawing.Point(217, 88);
            this._RTxtReceivedMsg.Name = "_TTxtReceivedMsg";
            this._RTxtReceivedMsg.ReadOnly = true;
            this._RTxtReceivedMsg.Size = new System.Drawing.Size(192, 232);
            this._RTxtReceivedMsg.TabIndex = 9;
            this._RTxtReceivedMsg.Text = "";
            // 
            // _Lbl2
            // 
            this._Lbl2.Location = new System.Drawing.Point(16, 16);
            this._Lbl2.Name = "_Lbl2";
            this._Lbl2.Size = new System.Drawing.Size(56, 16);
            this._Lbl2.TabIndex = 2;
            this._Lbl2.Text = "Server IP";
            // 
            // _Lbl3
            // 
            this._Lbl3.Location = new System.Drawing.Point(0, 338);
            this._Lbl3.Name = "_Lbl3";
            this._Lbl3.Size = new System.Drawing.Size(112, 16);
            this._Lbl3.TabIndex = 13;
            this._Lbl3.Text = "Status Message:";
            // 
            // _LbClientList
            // 
            this._LbClientList.BackColor = System.Drawing.SystemColors.Control;
            this._LbClientList.Location = new System.Drawing.Point(16, 199);
            this._LbClientList.Name = "_LbClientList";
            this._LbClientList.Size = new System.Drawing.Size(192, 121);
            this._LbClientList.TabIndex = 15;
            // 
            // _Lbl6
            // 
            this._Lbl6.Location = new System.Drawing.Point(16, 176);
            this._Lbl6.Name = "_Lbl6";
            this._Lbl6.Size = new System.Drawing.Size(184, 16);
            this._Lbl6.TabIndex = 16;
            this._Lbl6.Text = "Connected Clients";
            // 
            // _BtnClear
            // 
            this._BtnClear.Location = new System.Drawing.Point(232, 328);
            this._BtnClear.Name = "_BtnClear";
            this._BtnClear.Size = new System.Drawing.Size(88, 24);
            this._BtnClear.TabIndex = 17;
            this._BtnClear.Text = "Clear";
            this._BtnClear.Click += new System.EventHandler(this._BtnClear_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(424, 357);
            this.Controls.Add(this._BtnClear);
            this.Controls.Add(this._Lbl6);
            this.Controls.Add(this._LbClientList);
            this.Controls.Add(this._TxtMsg);
            this.Controls.Add(this._TxtIP);
            this.Controls.Add(this._TxtPort);
            this.Controls.Add(this._Lbl3);
            this.Controls.Add(this._BtnClose);
            this.Controls.Add(this._Lbl5);
            this.Controls.Add(this._RTxtReceivedMsg);
            this.Controls.Add(this._Lbl4);
            this.Controls.Add(this._BtnSendMsg);
            this.Controls.Add(this._RTxtSendMsg);
            this.Controls.Add(this._BtnStopListen);
            this.Controls.Add(this._BtnStartListen);
            this.Controls.Add(this._Lbl2);
            this.Controls.Add(this._Lbl1);
            this.Name = "Form1";
            this.Text = "SocketServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

