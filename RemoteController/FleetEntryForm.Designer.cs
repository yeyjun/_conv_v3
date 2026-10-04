namespace RemoteController
{
    partial class FleetEntryForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblStoreName = new System.Windows.Forms.Label();
            this.txtStoreName = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.nud_port = new System.Windows.Forms.NumericUpDown();
            this.lblToken = new System.Windows.Forms.Label();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.chkShowToken = new System.Windows.Forms.CheckBox();
            this.lblLatitude = new System.Windows.Forms.Label();
            this.txtLatitude = new System.Windows.Forms.TextBox();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.txtLongitude = new System.Windows.Forms.TextBox();
            this.btnDetectLocation = new System.Windows.Forms.Button();
            this.lblDetectStatus = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).BeginInit();
            this.SuspendLayout();
            //
            // lblStoreName
            //
            this.lblStoreName.AutoSize = true;
            this.lblStoreName.Location = new System.Drawing.Point(12, 15);
            this.lblStoreName.Name = "lblStoreName";
            this.lblStoreName.Size = new System.Drawing.Size(66, 13);
            this.lblStoreName.TabIndex = 0;
            this.lblStoreName.Text = "Store Name:";
            //
            // txtStoreName
            //
            this.txtStoreName.Location = new System.Drawing.Point(140, 12);
            this.txtStoreName.Name = "txtStoreName";
            this.txtStoreName.Size = new System.Drawing.Size(220, 22);
            this.txtStoreName.TabIndex = 1;
            //
            // lblHost
            //
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(12, 45);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(50, 13);
            this.lblHost.TabIndex = 2;
            this.lblHost.Text = "Host/IP:";
            //
            // txtHost
            //
            this.txtHost.Location = new System.Drawing.Point(140, 42);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(220, 22);
            this.txtHost.TabIndex = 3;
            //
            // lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(12, 75);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 13);
            this.lblPort.TabIndex = 4;
            this.lblPort.Text = "Port:";
            //
            // nud_port
            //
            this.nud_port.Location = new System.Drawing.Point(140, 72);
            this.nud_port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nud_port.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nud_port.Name = "nud_port";
            this.nud_port.Size = new System.Drawing.Size(100, 22);
            this.nud_port.TabIndex = 5;
            this.nud_port.Value = new decimal(new int[] {
            51820,
            0,
            0,
            0});
            //
            // lblToken
            //
            this.lblToken.AutoSize = true;
            this.lblToken.Location = new System.Drawing.Point(12, 105);
            this.lblToken.Name = "lblToken";
            this.lblToken.Size = new System.Drawing.Size(41, 13);
            this.lblToken.TabIndex = 6;
            this.lblToken.Text = "Token:";
            //
            // txtToken
            //
            this.txtToken.Location = new System.Drawing.Point(140, 102);
            this.txtToken.Name = "txtToken";
            this.txtToken.Size = new System.Drawing.Size(180, 22);
            this.txtToken.TabIndex = 7;
            this.txtToken.UseSystemPasswordChar = true;
            //
            // chkShowToken
            //
            this.chkShowToken.AutoSize = true;
            this.chkShowToken.Location = new System.Drawing.Point(326, 105);
            this.chkShowToken.Name = "chkShowToken";
            this.chkShowToken.Size = new System.Drawing.Size(54, 17);
            this.chkShowToken.TabIndex = 8;
            this.chkShowToken.Text = "Show";
            this.chkShowToken.UseVisualStyleBackColor = true;
            this.chkShowToken.CheckedChanged += new System.EventHandler(this.chkShowToken_CheckedChanged);
            //
            // lblLatitude
            //
            this.lblLatitude.AutoSize = true;
            this.lblLatitude.Location = new System.Drawing.Point(12, 135);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(50, 13);
            this.lblLatitude.TabIndex = 9;
            this.lblLatitude.Text = "Latitude:";
            //
            // txtLatitude
            //
            this.txtLatitude.Location = new System.Drawing.Point(140, 132);
            this.txtLatitude.Name = "txtLatitude";
            this.txtLatitude.Size = new System.Drawing.Size(100, 22);
            this.txtLatitude.TabIndex = 10;
            //
            // lblLongitude
            //
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Location = new System.Drawing.Point(12, 165);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(60, 13);
            this.lblLongitude.TabIndex = 11;
            this.lblLongitude.Text = "Longitude:";
            //
            // txtLongitude
            //
            this.txtLongitude.Location = new System.Drawing.Point(140, 162);
            this.txtLongitude.Name = "txtLongitude";
            this.txtLongitude.Size = new System.Drawing.Size(100, 22);
            this.txtLongitude.TabIndex = 12;
            //
            // btnDetectLocation
            //
            this.btnDetectLocation.Location = new System.Drawing.Point(140, 192);
            this.btnDetectLocation.Name = "btnDetectLocation";
            this.btnDetectLocation.Size = new System.Drawing.Size(220, 26);
            this.btnDetectLocation.TabIndex = 13;
            this.btnDetectLocation.Text = "Detect (approximate)...";
            this.btnDetectLocation.UseVisualStyleBackColor = true;
            this.btnDetectLocation.Click += new System.EventHandler(this.btnDetectLocation_Click);
            //
            // lblDetectStatus
            //
            this.lblDetectStatus.AutoSize = true;
            this.lblDetectStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblDetectStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblDetectStatus.Location = new System.Drawing.Point(12, 224);
            this.lblDetectStatus.MaximumSize = new System.Drawing.Size(348, 0);
            this.lblDetectStatus.Name = "lblDetectStatus";
            this.lblDetectStatus.Size = new System.Drawing.Size(348, 26);
            this.lblDetectStatus.TabIndex = 14;
            this.lblDetectStatus.Text = "Looks up the host\'s public IP with a free geolocation service - city-level acc" +
    "uracy at best, and meaningless for a private/LAN address. Always fine to type " +
    "the real coordinates in yourself instead.";
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(140, 270);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 30);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(250, 270);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // FleetEntryForm
            //
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(380, 315);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblDetectStatus);
            this.Controls.Add(this.btnDetectLocation);
            this.Controls.Add(this.txtLongitude);
            this.Controls.Add(this.lblLongitude);
            this.Controls.Add(this.txtLatitude);
            this.Controls.Add(this.lblLatitude);
            this.Controls.Add(this.chkShowToken);
            this.Controls.Add(this.txtToken);
            this.Controls.Add(this.lblToken);
            this.Controls.Add(this.nud_port);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.txtStoreName);
            this.Controls.Add(this.lblStoreName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FleetEntryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Fleet Entry";
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblStoreName;
        private System.Windows.Forms.TextBox txtStoreName;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nud_port;
        private System.Windows.Forms.Label lblToken;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.CheckBox chkShowToken;
        private System.Windows.Forms.Label lblLatitude;
        private System.Windows.Forms.TextBox txtLatitude;
        private System.Windows.Forms.Label lblLongitude;
        private System.Windows.Forms.TextBox txtLongitude;
        private System.Windows.Forms.Button btnDetectLocation;
        private System.Windows.Forms.Label lblDetectStatus;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
