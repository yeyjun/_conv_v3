namespace RemoteController
{
    partial class MainForm
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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageLicense = new System.Windows.Forms.TabPage();
            this.lblExpiryPreview = new System.Windows.Forms.Label();
            this.chkPermanent = new System.Windows.Forms.CheckBox();
            this.cmbDurationUnit = new System.Windows.Forms.ComboBox();
            this.nudDuration = new System.Windows.Forms.NumericUpDown();
            this.lblValidity = new System.Windows.Forms.Label();
            this.btnSaveLicenseFile = new System.Windows.Forms.Button();
            this.btnCopyKey = new System.Windows.Forms.Button();
            this.txtGeneratedKey = new System.Windows.Forms.TextBox();
            this.lblKeyResult = new System.Windows.Forms.Label();
            this.btnGenerateKey = new System.Windows.Forms.Button();
            this.txtStoreCode = new System.Windows.Forms.TextBox();
            this.lblStoreCode = new System.Windows.Forms.Label();
            this.tabPageRemote = new System.Windows.Forms.TabPage();
            this.lblSetupSteps = new System.Windows.Forms.Label();
            this.radNetwork = new System.Windows.Forms.RadioButton();
            this.radBluetooth = new System.Windows.Forms.RadioButton();
            this.lblHost = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.nud_port = new System.Windows.Forms.NumericUpDown();
            this.lblCertThumbprint = new System.Windows.Forms.Label();
            this.txtCertThumbprint = new System.Windows.Forms.TextBox();
            this.lblBtDevice = new System.Windows.Forms.Label();
            this.cmbBtDevice = new System.Windows.Forms.ComboBox();
            this.btnRefreshDevices = new System.Windows.Forms.Button();
            this.lblToken = new System.Windows.Forms.Label();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.chkShowToken = new System.Windows.Forms.CheckBox();
            this.btnPing = new System.Windows.Forms.Button();
            this.btnGetStatus = new System.Windows.Forms.Button();
            this.btnActivateRemote = new System.Windows.Forms.Button();
            this.btnManageMenus = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tabPageFleet = new System.Windows.Forms.TabPage();
            this.lblFleetHint = new System.Windows.Forms.Label();
            this.dgvFleet = new System.Windows.Forms.DataGridView();
            this.colStoreName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLatitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLongitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddFleet = new System.Windows.Forms.Button();
            this.btnEditFleet = new System.Windows.Forms.Button();
            this.btnRemoveFleet = new System.Windows.Forms.Button();
            this.btnRefreshAllFleet = new System.Windows.Forms.Button();
            this.btnUseSelected = new System.Windows.Forms.Button();
            this.lblFleetStatus = new System.Windows.Forms.Label();
            this.saveFileDialogLicense = new System.Windows.Forms.SaveFileDialog();
            this.tabControlMain.SuspendLayout();
            this.tabPageLicense.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuration)).BeginInit();
            this.tabPageRemote.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).BeginInit();
            this.tabPageFleet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFleet)).BeginInit();
            this.SuspendLayout();
            //
            // tabControlMain
            //
            this.tabControlMain.Controls.Add(this.tabPageLicense);
            this.tabControlMain.Controls.Add(this.tabPageRemote);
            this.tabControlMain.Controls.Add(this.tabPageFleet);
            this.tabControlMain.Location = new System.Drawing.Point(4, 4);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(462, 512);
            this.tabControlMain.TabIndex = 0;
            //
            // tabPageLicense
            //
            this.tabPageLicense.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageLicense.Controls.Add(this.lblExpiryPreview);
            this.tabPageLicense.Controls.Add(this.chkPermanent);
            this.tabPageLicense.Controls.Add(this.cmbDurationUnit);
            this.tabPageLicense.Controls.Add(this.nudDuration);
            this.tabPageLicense.Controls.Add(this.lblValidity);
            this.tabPageLicense.Controls.Add(this.btnSaveLicenseFile);
            this.tabPageLicense.Controls.Add(this.btnCopyKey);
            this.tabPageLicense.Controls.Add(this.txtGeneratedKey);
            this.tabPageLicense.Controls.Add(this.lblKeyResult);
            this.tabPageLicense.Controls.Add(this.btnGenerateKey);
            this.tabPageLicense.Controls.Add(this.txtStoreCode);
            this.tabPageLicense.Controls.Add(this.lblStoreCode);
            this.tabPageLicense.Location = new System.Drawing.Point(4, 22);
            this.tabPageLicense.Name = "tabPageLicense";
            this.tabPageLicense.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLicense.Size = new System.Drawing.Size(454, 486);
            this.tabPageLicense.TabIndex = 0;
            this.tabPageLicense.Text = "License";
            //
            // lblStoreCode
            //
            this.lblStoreCode.AutoSize = true;
            this.lblStoreCode.Location = new System.Drawing.Point(10, 15);
            this.lblStoreCode.Name = "lblStoreCode";
            this.lblStoreCode.Size = new System.Drawing.Size(64, 13);
            this.lblStoreCode.TabIndex = 0;
            this.lblStoreCode.Text = "Store Code:";
            //
            // txtStoreCode
            //
            this.txtStoreCode.Location = new System.Drawing.Point(100, 12);
            this.txtStoreCode.Name = "txtStoreCode";
            this.txtStoreCode.Size = new System.Drawing.Size(200, 22);
            this.txtStoreCode.TabIndex = 1;
            //
            // btnGenerateKey
            //
            this.btnGenerateKey.Location = new System.Drawing.Point(310, 10);
            this.btnGenerateKey.Name = "btnGenerateKey";
            this.btnGenerateKey.Size = new System.Drawing.Size(130, 26);
            this.btnGenerateKey.TabIndex = 2;
            this.btnGenerateKey.Text = "Generate Key";
            this.btnGenerateKey.UseVisualStyleBackColor = true;
            this.btnGenerateKey.Click += new System.EventHandler(this.btnGenerateKey_Click);
            //
            // lblKeyResult
            //
            this.lblKeyResult.AutoSize = true;
            this.lblKeyResult.Location = new System.Drawing.Point(10, 48);
            this.lblKeyResult.Name = "lblKeyResult";
            this.lblKeyResult.Size = new System.Drawing.Size(70, 13);
            this.lblKeyResult.TabIndex = 3;
            this.lblKeyResult.Text = "License Key:";
            //
            // txtGeneratedKey
            //
            this.txtGeneratedKey.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtGeneratedKey.Location = new System.Drawing.Point(100, 45);
            this.txtGeneratedKey.Name = "txtGeneratedKey";
            this.txtGeneratedKey.ReadOnly = true;
            this.txtGeneratedKey.Size = new System.Drawing.Size(340, 23);
            this.txtGeneratedKey.TabIndex = 4;
            //
            // btnCopyKey
            //
            this.btnCopyKey.Location = new System.Drawing.Point(10, 82);
            this.btnCopyKey.Name = "btnCopyKey";
            this.btnCopyKey.Size = new System.Drawing.Size(100, 28);
            this.btnCopyKey.TabIndex = 5;
            this.btnCopyKey.Text = "Copy";
            this.btnCopyKey.UseVisualStyleBackColor = true;
            this.btnCopyKey.Click += new System.EventHandler(this.btnCopyKey_Click);
            //
            // btnSaveLicenseFile
            //
            this.btnSaveLicenseFile.Location = new System.Drawing.Point(120, 82);
            this.btnSaveLicenseFile.Name = "btnSaveLicenseFile";
            this.btnSaveLicenseFile.Size = new System.Drawing.Size(220, 28);
            this.btnSaveLicenseFile.TabIndex = 6;
            this.btnSaveLicenseFile.Text = "Save License File (.tmrgo)...";
            this.btnSaveLicenseFile.UseVisualStyleBackColor = true;
            this.btnSaveLicenseFile.Click += new System.EventHandler(this.btnSaveLicenseFile_Click);
            //
            // lblValidity
            //
            this.lblValidity.AutoSize = true;
            this.lblValidity.Location = new System.Drawing.Point(10, 128);
            this.lblValidity.Name = "lblValidity";
            this.lblValidity.Size = new System.Drawing.Size(46, 13);
            this.lblValidity.TabIndex = 7;
            this.lblValidity.Text = "Validity:";
            //
            // nudDuration
            //
            this.nudDuration.Location = new System.Drawing.Point(100, 125);
            this.nudDuration.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.nudDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudDuration.Name = "nudDuration";
            this.nudDuration.Size = new System.Drawing.Size(60, 22);
            this.nudDuration.TabIndex = 8;
            this.nudDuration.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // cmbDurationUnit
            //
            this.cmbDurationUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDurationUnit.Items.AddRange(new object[] { "Days", "Months", "Years" });
            this.cmbDurationUnit.Location = new System.Drawing.Point(168, 125);
            this.cmbDurationUnit.Name = "cmbDurationUnit";
            this.cmbDurationUnit.Size = new System.Drawing.Size(90, 23);
            this.cmbDurationUnit.TabIndex = 9;
            //
            // chkPermanent
            //
            this.chkPermanent.AutoSize = true;
            this.chkPermanent.Location = new System.Drawing.Point(268, 128);
            this.chkPermanent.Name = "chkPermanent";
            this.chkPermanent.Size = new System.Drawing.Size(140, 17);
            this.chkPermanent.TabIndex = 10;
            this.chkPermanent.Text = "No expiry (permanent)";
            this.chkPermanent.UseVisualStyleBackColor = true;
            this.chkPermanent.CheckedChanged += new System.EventHandler(this.chkPermanent_CheckedChanged);
            //
            // lblExpiryPreview
            //
            this.lblExpiryPreview.AutoSize = true;
            this.lblExpiryPreview.Location = new System.Drawing.Point(10, 158);
            this.lblExpiryPreview.Name = "lblExpiryPreview";
            this.lblExpiryPreview.Size = new System.Drawing.Size(96, 13);
            this.lblExpiryPreview.TabIndex = 11;
            this.lblExpiryPreview.Text = "Expires: (not generated yet)";
            //
            // tabPageRemote
            //
            this.tabPageRemote.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageRemote.Controls.Add(this.txtLog);
            this.tabPageRemote.Controls.Add(this.btnManageMenus);
            this.tabPageRemote.Controls.Add(this.btnActivateRemote);
            this.tabPageRemote.Controls.Add(this.btnGetStatus);
            this.tabPageRemote.Controls.Add(this.btnPing);
            this.tabPageRemote.Controls.Add(this.chkShowToken);
            this.tabPageRemote.Controls.Add(this.txtToken);
            this.tabPageRemote.Controls.Add(this.lblToken);
            this.tabPageRemote.Controls.Add(this.btnRefreshDevices);
            this.tabPageRemote.Controls.Add(this.cmbBtDevice);
            this.tabPageRemote.Controls.Add(this.lblBtDevice);
            this.tabPageRemote.Controls.Add(this.txtCertThumbprint);
            this.tabPageRemote.Controls.Add(this.lblCertThumbprint);
            this.tabPageRemote.Controls.Add(this.nud_port);
            this.tabPageRemote.Controls.Add(this.lblPort);
            this.tabPageRemote.Controls.Add(this.txtHost);
            this.tabPageRemote.Controls.Add(this.lblHost);
            this.tabPageRemote.Controls.Add(this.radBluetooth);
            this.tabPageRemote.Controls.Add(this.radNetwork);
            this.tabPageRemote.Controls.Add(this.lblSetupSteps);
            this.tabPageRemote.Location = new System.Drawing.Point(4, 22);
            this.tabPageRemote.Name = "tabPageRemote";
            this.tabPageRemote.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRemote.Size = new System.Drawing.Size(454, 486);
            this.tabPageRemote.TabIndex = 1;
            this.tabPageRemote.Text = "Remote Control";
            //
            // lblSetupSteps
            //
            this.lblSetupSteps.AutoSize = true;
            this.lblSetupSteps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSetupSteps.Location = new System.Drawing.Point(10, 8);
            this.lblSetupSteps.MaximumSize = new System.Drawing.Size(434, 0);
            this.lblSetupSteps.Name = "lblSetupSteps";
            this.lblSetupSteps.Size = new System.Drawing.Size(430, 65);
            this.lblSetupSteps.TabIndex = 0;
            this.lblSetupSteps.Text = "Before connecting, on the POS terminal: open System Config (Ctrl+Shift+S) and e" +
    "nter the Admin PIN, go to the Remote Control tab, check \"Enable remote contro" +
    "l\", click Generate Token, then Save. That screen also shows the IP address, c" +
    "ertificate thumbprint, and (if enabled) Bluetooth address to use below.";
            //
            // radNetwork
            //
            this.radNetwork.AutoSize = true;
            this.radNetwork.Checked = true;
            this.radNetwork.Location = new System.Drawing.Point(10, 86);
            this.radNetwork.Name = "radNetwork";
            this.radNetwork.Size = new System.Drawing.Size(105, 17);
            this.radNetwork.TabIndex = 1;
            this.radNetwork.TabStop = true;
            this.radNetwork.Text = "Network (TCP)";
            this.radNetwork.UseVisualStyleBackColor = true;
            this.radNetwork.CheckedChanged += new System.EventHandler(this.radTransport_CheckedChanged);
            //
            // radBluetooth
            //
            this.radBluetooth.AutoSize = true;
            this.radBluetooth.Location = new System.Drawing.Point(150, 86);
            this.radBluetooth.Name = "radBluetooth";
            this.radBluetooth.Size = new System.Drawing.Size(76, 17);
            this.radBluetooth.TabIndex = 2;
            this.radBluetooth.Text = "Bluetooth";
            this.radBluetooth.UseVisualStyleBackColor = true;
            this.radBluetooth.CheckedChanged += new System.EventHandler(this.radTransport_CheckedChanged);
            //
            // lblHost
            //
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(10, 114);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(50, 13);
            this.lblHost.TabIndex = 3;
            this.lblHost.Text = "Host/IP:";
            //
            // txtHost
            //
            this.txtHost.Location = new System.Drawing.Point(100, 111);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(150, 22);
            this.txtHost.TabIndex = 4;
            //
            // lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(260, 114);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 13);
            this.lblPort.TabIndex = 5;
            this.lblPort.Text = "Port:";
            //
            // nud_port
            //
            this.nud_port.Location = new System.Drawing.Point(300, 111);
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
            this.nud_port.Size = new System.Drawing.Size(80, 22);
            this.nud_port.TabIndex = 6;
            this.nud_port.Value = new decimal(new int[] {
            51820,
            0,
            0,
            0});
            //
            // lblCertThumbprint
            //
            this.lblCertThumbprint.AutoSize = true;
            this.lblCertThumbprint.Location = new System.Drawing.Point(10, 144);
            this.lblCertThumbprint.Name = "lblCertThumbprint";
            this.lblCertThumbprint.Size = new System.Drawing.Size(129, 13);
            this.lblCertThumbprint.TabIndex = 7;
            this.lblCertThumbprint.Text = "Cert Thumbprint (optional):";
            //
            // txtCertThumbprint
            //
            this.txtCertThumbprint.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtCertThumbprint.Location = new System.Drawing.Point(150, 141);
            this.txtCertThumbprint.Name = "txtCertThumbprint";
            this.txtCertThumbprint.Size = new System.Drawing.Size(290, 21);
            this.txtCertThumbprint.TabIndex = 8;
            //
            // lblBtDevice
            //
            this.lblBtDevice.AutoSize = true;
            this.lblBtDevice.Location = new System.Drawing.Point(10, 114);
            this.lblBtDevice.Name = "lblBtDevice";
            this.lblBtDevice.Size = new System.Drawing.Size(80, 13);
            this.lblBtDevice.TabIndex = 9;
            this.lblBtDevice.Text = "Paired Device:";
            this.lblBtDevice.Visible = false;
            //
            // cmbBtDevice
            //
            this.cmbBtDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtDevice.FormattingEnabled = true;
            this.cmbBtDevice.Location = new System.Drawing.Point(100, 111);
            this.cmbBtDevice.Name = "cmbBtDevice";
            this.cmbBtDevice.Size = new System.Drawing.Size(250, 21);
            this.cmbBtDevice.TabIndex = 10;
            this.cmbBtDevice.Visible = false;
            //
            // btnRefreshDevices
            //
            this.btnRefreshDevices.Location = new System.Drawing.Point(360, 110);
            this.btnRefreshDevices.Name = "btnRefreshDevices";
            this.btnRefreshDevices.Size = new System.Drawing.Size(80, 24);
            this.btnRefreshDevices.TabIndex = 11;
            this.btnRefreshDevices.Text = "Refresh";
            this.btnRefreshDevices.UseVisualStyleBackColor = true;
            this.btnRefreshDevices.Visible = false;
            this.btnRefreshDevices.Click += new System.EventHandler(this.btnRefreshDevices_Click);
            //
            // lblToken
            //
            this.lblToken.AutoSize = true;
            this.lblToken.Location = new System.Drawing.Point(10, 178);
            this.lblToken.Name = "lblToken";
            this.lblToken.Size = new System.Drawing.Size(75, 13);
            this.lblToken.TabIndex = 12;
            this.lblToken.Text = "Shared Token:";
            //
            // txtToken
            //
            this.txtToken.Location = new System.Drawing.Point(100, 175);
            this.txtToken.Name = "txtToken";
            this.txtToken.Size = new System.Drawing.Size(250, 22);
            this.txtToken.TabIndex = 13;
            this.txtToken.UseSystemPasswordChar = true;
            //
            // chkShowToken
            //
            this.chkShowToken.AutoSize = true;
            this.chkShowToken.Location = new System.Drawing.Point(360, 178);
            this.chkShowToken.Name = "chkShowToken";
            this.chkShowToken.Size = new System.Drawing.Size(54, 17);
            this.chkShowToken.TabIndex = 14;
            this.chkShowToken.Text = "Show";
            this.chkShowToken.UseVisualStyleBackColor = true;
            this.chkShowToken.CheckedChanged += new System.EventHandler(this.chkShowToken_CheckedChanged);
            //
            // btnPing
            //
            this.btnPing.Location = new System.Drawing.Point(10, 210);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(100, 28);
            this.btnPing.TabIndex = 15;
            this.btnPing.Text = "Ping";
            this.btnPing.UseVisualStyleBackColor = true;
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            //
            // btnGetStatus
            //
            this.btnGetStatus.Location = new System.Drawing.Point(120, 210);
            this.btnGetStatus.Name = "btnGetStatus";
            this.btnGetStatus.Size = new System.Drawing.Size(130, 28);
            this.btnGetStatus.TabIndex = 16;
            this.btnGetStatus.Text = "Get Status";
            this.btnGetStatus.UseVisualStyleBackColor = true;
            this.btnGetStatus.Click += new System.EventHandler(this.btnGetStatus_Click);
            //
            // btnActivateRemote
            //
            this.btnActivateRemote.Location = new System.Drawing.Point(260, 210);
            this.btnActivateRemote.Name = "btnActivateRemote";
            this.btnActivateRemote.Size = new System.Drawing.Size(180, 28);
            this.btnActivateRemote.TabIndex = 17;
            this.btnActivateRemote.Text = "Push License Key";
            this.btnActivateRemote.UseVisualStyleBackColor = true;
            this.btnActivateRemote.Click += new System.EventHandler(this.btnActivateRemote_Click);
            //
            // btnManageMenus
            //
            this.btnManageMenus.Location = new System.Drawing.Point(10, 240);
            this.btnManageMenus.Name = "btnManageMenus";
            this.btnManageMenus.Size = new System.Drawing.Size(430, 28);
            this.btnManageMenus.TabIndex = 18;
            this.btnManageMenus.Text = "Manage Menus...";
            this.btnManageMenus.UseVisualStyleBackColor = true;
            this.btnManageMenus.Click += new System.EventHandler(this.btnManageMenus_Click);
            //
            // txtLog
            //
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.Location = new System.Drawing.Point(10, 274);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(430, 200);
            this.txtLog.TabIndex = 19;
            //
            // tabPageFleet
            //
            this.tabPageFleet.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageFleet.Controls.Add(this.lblFleetStatus);
            this.tabPageFleet.Controls.Add(this.btnUseSelected);
            this.tabPageFleet.Controls.Add(this.btnRefreshAllFleet);
            this.tabPageFleet.Controls.Add(this.btnRemoveFleet);
            this.tabPageFleet.Controls.Add(this.btnEditFleet);
            this.tabPageFleet.Controls.Add(this.btnAddFleet);
            this.tabPageFleet.Controls.Add(this.dgvFleet);
            this.tabPageFleet.Controls.Add(this.lblFleetHint);
            this.tabPageFleet.Location = new System.Drawing.Point(4, 22);
            this.tabPageFleet.Name = "tabPageFleet";
            this.tabPageFleet.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFleet.Size = new System.Drawing.Size(454, 486);
            this.tabPageFleet.TabIndex = 2;
            this.tabPageFleet.Text = "Fleet";
            //
            // lblFleetHint
            //
            this.lblFleetHint.AutoSize = true;
            this.lblFleetHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblFleetHint.Location = new System.Drawing.Point(10, 8);
            this.lblFleetHint.MaximumSize = new System.Drawing.Size(434, 0);
            this.lblFleetHint.Name = "lblFleetHint";
            this.lblFleetHint.Size = new System.Drawing.Size(430, 39);
            this.lblFleetHint.TabIndex = 0;
            this.lblFleetHint.Text = "Terminals known to this copy of RemoteController - kept locally in this file on" +
    "ly, there is no central server. \"Refresh All\" pings every entry with its save" +
    "d token to check whether it\'s actually online.";
            //
            // dgvFleet
            //
            this.dgvFleet.AllowUserToAddRows = false;
            this.dgvFleet.AllowUserToDeleteRows = false;
            this.dgvFleet.AllowUserToResizeRows = false;
            this.dgvFleet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStoreName,
            this.colHost,
            this.colPort,
            this.colLatitude,
            this.colLongitude,
            this.colStatus});
            this.dgvFleet.Location = new System.Drawing.Point(10, 50);
            this.dgvFleet.MultiSelect = false;
            this.dgvFleet.Name = "dgvFleet";
            this.dgvFleet.ReadOnly = true;
            this.dgvFleet.RowHeadersWidth = 24;
            this.dgvFleet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFleet.Size = new System.Drawing.Size(434, 300);
            this.dgvFleet.TabIndex = 1;
            this.dgvFleet.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFleet_CellDoubleClick);
            //
            // colStoreName
            //
            this.colStoreName.HeaderText = "Store Name";
            this.colStoreName.Name = "colStoreName";
            this.colStoreName.ReadOnly = true;
            this.colStoreName.Width = 110;
            //
            // colHost
            //
            this.colHost.HeaderText = "Host";
            this.colHost.Name = "colHost";
            this.colHost.ReadOnly = true;
            this.colHost.Width = 95;
            //
            // colPort
            //
            this.colPort.HeaderText = "Port";
            this.colPort.Name = "colPort";
            this.colPort.ReadOnly = true;
            this.colPort.Width = 50;
            //
            // colLatitude
            //
            this.colLatitude.HeaderText = "Latitude";
            this.colLatitude.Name = "colLatitude";
            this.colLatitude.ReadOnly = true;
            this.colLatitude.Width = 60;
            //
            // colLongitude
            //
            this.colLongitude.HeaderText = "Longitude";
            this.colLongitude.Name = "colLongitude";
            this.colLongitude.ReadOnly = true;
            this.colLongitude.Width = 65;
            //
            // colStatus
            //
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 100;
            //
            // btnAddFleet
            //
            this.btnAddFleet.Location = new System.Drawing.Point(10, 358);
            this.btnAddFleet.Name = "btnAddFleet";
            this.btnAddFleet.Size = new System.Drawing.Size(90, 28);
            this.btnAddFleet.TabIndex = 2;
            this.btnAddFleet.Text = "Add...";
            this.btnAddFleet.UseVisualStyleBackColor = true;
            this.btnAddFleet.Click += new System.EventHandler(this.btnAddFleet_Click);
            //
            // btnEditFleet
            //
            this.btnEditFleet.Location = new System.Drawing.Point(106, 358);
            this.btnEditFleet.Name = "btnEditFleet";
            this.btnEditFleet.Size = new System.Drawing.Size(90, 28);
            this.btnEditFleet.TabIndex = 3;
            this.btnEditFleet.Text = "Edit...";
            this.btnEditFleet.UseVisualStyleBackColor = true;
            this.btnEditFleet.Click += new System.EventHandler(this.btnEditFleet_Click);
            //
            // btnRemoveFleet
            //
            this.btnRemoveFleet.Location = new System.Drawing.Point(202, 358);
            this.btnRemoveFleet.Name = "btnRemoveFleet";
            this.btnRemoveFleet.Size = new System.Drawing.Size(90, 28);
            this.btnRemoveFleet.TabIndex = 4;
            this.btnRemoveFleet.Text = "Remove";
            this.btnRemoveFleet.UseVisualStyleBackColor = true;
            this.btnRemoveFleet.Click += new System.EventHandler(this.btnRemoveFleet_Click);
            //
            // btnRefreshAllFleet
            //
            this.btnRefreshAllFleet.Location = new System.Drawing.Point(10, 392);
            this.btnRefreshAllFleet.Name = "btnRefreshAllFleet";
            this.btnRefreshAllFleet.Size = new System.Drawing.Size(150, 28);
            this.btnRefreshAllFleet.TabIndex = 5;
            this.btnRefreshAllFleet.Text = "Refresh All";
            this.btnRefreshAllFleet.UseVisualStyleBackColor = true;
            this.btnRefreshAllFleet.Click += new System.EventHandler(this.btnRefreshAllFleet_Click);
            //
            // btnUseSelected
            //
            this.btnUseSelected.Location = new System.Drawing.Point(166, 392);
            this.btnUseSelected.Name = "btnUseSelected";
            this.btnUseSelected.Size = new System.Drawing.Size(180, 28);
            this.btnUseSelected.TabIndex = 6;
            this.btnUseSelected.Text = "Use Selected in Remote Control";
            this.btnUseSelected.UseVisualStyleBackColor = true;
            this.btnUseSelected.Click += new System.EventHandler(this.btnUseSelected_Click);
            //
            // lblFleetStatus
            //
            this.lblFleetStatus.AutoSize = true;
            this.lblFleetStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblFleetStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblFleetStatus.Location = new System.Drawing.Point(10, 428);
            this.lblFleetStatus.MaximumSize = new System.Drawing.Size(434, 0);
            this.lblFleetStatus.Name = "lblFleetStatus";
            this.lblFleetStatus.Size = new System.Drawing.Size(101, 12);
            this.lblFleetStatus.TabIndex = 7;
            //
            // saveFileDialogLicense
            //
            this.saveFileDialogLicense.DefaultExt = "tmrgo";
            this.saveFileDialogLicense.Filter = "License Key File (*.tmrgo)|*.tmrgo|All files (*.*)|*.*";
            this.saveFileDialogLicense.Title = "Save License Key File";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 524);
            this.Controls.Add(this.tabControlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "A10D-up - Remote Controller";
            this.tabControlMain.ResumeLayout(false);
            this.tabPageLicense.ResumeLayout(false);
            this.tabPageLicense.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuration)).EndInit();
            this.tabPageRemote.ResumeLayout(false);
            this.tabPageRemote.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).EndInit();
            this.tabPageFleet.ResumeLayout(false);
            this.tabPageFleet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFleet)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageLicense;
        private System.Windows.Forms.Label lblStoreCode;
        private System.Windows.Forms.TextBox txtStoreCode;
        private System.Windows.Forms.Button btnGenerateKey;
        private System.Windows.Forms.Label lblKeyResult;
        private System.Windows.Forms.TextBox txtGeneratedKey;
        private System.Windows.Forms.Button btnCopyKey;
        private System.Windows.Forms.Button btnSaveLicenseFile;
        private System.Windows.Forms.Label lblValidity;
        private System.Windows.Forms.NumericUpDown nudDuration;
        private System.Windows.Forms.ComboBox cmbDurationUnit;
        private System.Windows.Forms.CheckBox chkPermanent;
        private System.Windows.Forms.Label lblExpiryPreview;
        private System.Windows.Forms.TabPage tabPageRemote;
        private System.Windows.Forms.Label lblSetupSteps;
        private System.Windows.Forms.RadioButton radNetwork;
        private System.Windows.Forms.RadioButton radBluetooth;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nud_port;
        private System.Windows.Forms.Label lblCertThumbprint;
        private System.Windows.Forms.TextBox txtCertThumbprint;
        private System.Windows.Forms.Label lblBtDevice;
        private System.Windows.Forms.ComboBox cmbBtDevice;
        private System.Windows.Forms.Button btnRefreshDevices;
        private System.Windows.Forms.Label lblToken;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.CheckBox chkShowToken;
        private System.Windows.Forms.Button btnPing;
        private System.Windows.Forms.Button btnGetStatus;
        private System.Windows.Forms.Button btnActivateRemote;
        private System.Windows.Forms.Button btnManageMenus;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TabPage tabPageFleet;
        private System.Windows.Forms.Label lblFleetHint;
        private System.Windows.Forms.DataGridView dgvFleet;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStoreName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLatitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLongitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Button btnAddFleet;
        private System.Windows.Forms.Button btnEditFleet;
        private System.Windows.Forms.Button btnRemoveFleet;
        private System.Windows.Forms.Button btnRefreshAllFleet;
        private System.Windows.Forms.Button btnUseSelected;
        private System.Windows.Forms.Label lblFleetStatus;
        private System.Windows.Forms.SaveFileDialog saveFileDialogLicense;
    }
}
