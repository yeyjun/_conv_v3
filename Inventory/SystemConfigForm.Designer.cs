namespace Inventory
{
    partial class SystemConfigForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea networkChartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend networkLegend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series downloadSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series uploadSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lstConfigSections = new System.Windows.Forms.ListBox();
            this.pnlConfigHost = new System.Windows.Forms.Panel();
            this.tabControlConfig = new System.Windows.Forms.TabControl();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.gbxStoreInfo = new System.Windows.Forms.GroupBox();
            this.txtStoreEmail = new System.Windows.Forms.TextBox();
            this.lblStoreEmail = new System.Windows.Forms.Label();
            this.txtStoreCode = new System.Windows.Forms.TextBox();
            this.lblStoreCode = new System.Windows.Forms.Label();
            this.txtStoreName = new System.Windows.Forms.TextBox();
            this.lblStoreName = new System.Windows.Forms.Label();
            this.gbxDatabase = new System.Windows.Forms.GroupBox();
            this.lblDbNote = new System.Windows.Forms.Label();
            this.nud_connectionTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.chkShowDbPassword = new System.Windows.Forms.CheckBox();
            this.txtDbPassword = new System.Windows.Forms.TextBox();
            this.lblDbPassword = new System.Windows.Forms.Label();
            this.txtDbUser = new System.Windows.Forms.TextBox();
            this.lblDbUser = new System.Windows.Forms.Label();
            this.txtDbName = new System.Windows.Forms.TextBox();
            this.lblDbName = new System.Windows.Forms.Label();
            this.txtDbServer = new System.Windows.Forms.TextBox();
            this.lblDbServer = new System.Windows.Forms.Label();
            this.gbxSecurity = new System.Windows.Forms.GroupBox();
            this.chkShowAdminPin = new System.Windows.Forms.CheckBox();
            this.txtAdminPin = new System.Windows.Forms.TextBox();
            this.lblAdminPin = new System.Windows.Forms.Label();
            this.chkShowPin = new System.Windows.Forms.CheckBox();
            this.txtManagerPin = new System.Windows.Forms.TextBox();
            this.lblManagerPin = new System.Windows.Forms.Label();
            this.gbxSalesDefaults = new System.Windows.Forms.GroupBox();
            this.lblAutoHoldMinutes = new System.Windows.Forms.Label();
            this.nud_autoHoldMinutes = new System.Windows.Forms.NumericUpDown();
            this.chkAutoHoldEnabled = new System.Windows.Forms.CheckBox();
            this.cbxDefaultSelltype = new System.Windows.Forms.ComboBox();
            this.chkDefaultSelltype = new System.Windows.Forms.CheckBox();
            this.chkBlockZeroPriceItems = new System.Windows.Forms.CheckBox();
            this.gbxAlerts = new System.Windows.Forms.GroupBox();
            this.lblAlertAutoDismissSeconds = new System.Windows.Forms.Label();
            this.nud_alertAutoDismissSeconds = new System.Windows.Forms.NumericUpDown();
            this.chkAlertAutoDismissEnabled = new System.Windows.Forms.CheckBox();
            this.gbxStartup = new System.Windows.Forms.GroupBox();
            this.chkAutoStartWithWindows = new System.Windows.Forms.CheckBox();
            this.tabPageNetwork = new System.Windows.Forms.TabPage();
            this.lblNetworkHint = new System.Windows.Forms.Label();
            this.chartNetwork = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblNetworkStatus = new System.Windows.Forms.Label();
            this.btnToggleMonitor = new System.Windows.Forms.Button();
            this.tabPageMenus = new System.Windows.Forms.TabPage();
            this.clbMenus = new System.Windows.Forms.CheckedListBox();
            this.lblMenusHint = new System.Windows.Forms.Label();
            this.tabPagePrinter = new System.Windows.Forms.TabPage();
            this.lblCounterReceiptHint = new System.Windows.Forms.Label();
            this.chkSaveCounterReceiptAsJpeg = new System.Windows.Forms.CheckBox();
            this.chkSetAsSystemDefaultPrinter = new System.Windows.Forms.CheckBox();
            this.lblPaperSizeInfo = new System.Windows.Forms.Label();
            this.cbxPaperWidth = new System.Windows.Forms.ComboBox();
            this.chkPaperWidthIsDefault = new System.Windows.Forms.CheckBox();
            this.lblPaperWidth = new System.Windows.Forms.Label();
            this.lblCounterReceiptPath = new System.Windows.Forms.Label();
            this.txtCounterReceiptPath = new System.Windows.Forms.TextBox();
            this.btnBrowseCounterReceiptPath = new System.Windows.Forms.Button();
            this.cbxReceiptPrinter = new System.Windows.Forms.ComboBox();
            this.lblReceiptPrinter = new System.Windows.Forms.Label();
            this.lblPrinterHint = new System.Windows.Forms.Label();
            this.tabPageAppearance = new System.Windows.Forms.TabPage();
            this.lblAppearanceHint = new System.Windows.Forms.Label();
            this.chkEnableAccent = new System.Windows.Forms.CheckBox();
            this.lblBgColor = new System.Windows.Forms.Label();
            this.pnlBgSwatch = new System.Windows.Forms.Panel();
            this.btnPickBgColor = new System.Windows.Forms.Button();
            this.lblFgColor = new System.Windows.Forms.Label();
            this.pnlFgSwatch = new System.Windows.Forms.Panel();
            this.btnPickFgColor = new System.Windows.Forms.Button();
            this.lblAppearanceNote = new System.Windows.Forms.Label();
            this.lblFontScale = new System.Windows.Forms.Label();
            this.nud_fontScalePt = new System.Windows.Forms.NumericUpDown();
            this.lblFontScaleUnit = new System.Windows.Forms.Label();
            this.lblFontScaleHint = new System.Windows.Forms.Label();
            this.lblLogoUrl = new System.Windows.Forms.Label();
            this.txtLogoUrl = new System.Windows.Forms.TextBox();
            this.lblLogoUrlHint = new System.Windows.Forms.Label();
            this.chkLayoutEditEnabled = new System.Windows.Forms.CheckBox();
            this.lblLayoutEditHint = new System.Windows.Forms.Label();
            this.colorDialogAccent = new System.Windows.Forms.ColorDialog();
            this.tabPageLayout = new System.Windows.Forms.TabPage();
            this.lblLayoutRevisionHint = new System.Windows.Forms.Label();
            this.txtLayoutRevision = new System.Windows.Forms.TextBox();
            this.btnRefreshLayoutText = new System.Windows.Forms.Button();
            this.btnApplyLayoutText = new System.Windows.Forms.Button();
            this.btnLoadLayoutFile = new System.Windows.Forms.Button();
            this.btnSaveLayoutFile = new System.Windows.Forms.Button();
            this.openFileDialogLayout = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogLayout = new System.Windows.Forms.SaveFileDialog();
            this.tabPageSchedule = new System.Windows.Forms.TabPage();
            this.lblScheduleHint = new System.Windows.Forms.Label();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.cbxReportFrequency = new System.Windows.Forms.ComboBox();
            this.lblScheduleStatus = new System.Windows.Forms.Label();
            this.btnSendReportNow = new System.Windows.Forms.Button();
            this.lblSendVia = new System.Windows.Forms.Label();
            this.rdoProviderSmtp = new System.Windows.Forms.RadioButton();
            this.rdoProviderGmail = new System.Windows.Forms.RadioButton();
            this.gbxGmail = new System.Windows.Forms.GroupBox();
            this.lblGmailNote = new System.Windows.Forms.Label();
            this.txtGmailSender = new System.Windows.Forms.TextBox();
            this.lblGmailSender = new System.Windows.Forms.Label();
            this.chkShowGmailRefreshToken = new System.Windows.Forms.CheckBox();
            this.txtGmailRefreshToken = new System.Windows.Forms.TextBox();
            this.lblGmailRefreshToken = new System.Windows.Forms.Label();
            this.chkShowGmailClientSecret = new System.Windows.Forms.CheckBox();
            this.txtGmailClientSecret = new System.Windows.Forms.TextBox();
            this.lblGmailClientSecret = new System.Windows.Forms.Label();
            this.txtGmailClientId = new System.Windows.Forms.TextBox();
            this.lblGmailClientId = new System.Windows.Forms.Label();
            this.gbxSmtp = new System.Windows.Forms.GroupBox();
            this.lblSmtpNote = new System.Windows.Forms.Label();
            this.chkShowSmtpPassword = new System.Windows.Forms.CheckBox();
            this.txtSmtpPassword = new System.Windows.Forms.TextBox();
            this.lblSmtpPassword = new System.Windows.Forms.Label();
            this.txtSmtpUser = new System.Windows.Forms.TextBox();
            this.lblSmtpUser = new System.Windows.Forms.Label();
            this.txtSmtpFrom = new System.Windows.Forms.TextBox();
            this.lblSmtpFrom = new System.Windows.Forms.Label();
            this.chkSmtpUseSsl = new System.Windows.Forms.CheckBox();
            this.nud_smtpPort = new System.Windows.Forms.NumericUpDown();
            this.lblSmtpPort = new System.Windows.Forms.Label();
            this.txtSmtpHost = new System.Windows.Forms.TextBox();
            this.lblSmtpHost = new System.Windows.Forms.Label();
            this.tabPageDataSync = new System.Windows.Forms.TabPage();
            this.lblSyncHint = new System.Windows.Forms.Label();
            this.lblAccessPath = new System.Windows.Forms.Label();
            this.txtAccessDbPath = new System.Windows.Forms.TextBox();
            this.btnBrowseAccessDbPath = new System.Windows.Forms.Button();
            this.btnSyncNow = new System.Windows.Forms.Button();
            this.lblSyncStatus = new System.Windows.Forms.Label();
            this.lblLastSync = new System.Windows.Forms.Label();
            this.grpUpload = new System.Windows.Forms.GroupBox();
            this.btnPushToOnline = new System.Windows.Forms.Button();
            this.lblPushStatus = new System.Windows.Forms.Label();
            this.lblLastPush = new System.Windows.Forms.Label();
            this.tabPageTrial = new System.Windows.Forms.TabPage();
            this.lblTrialHint = new System.Windows.Forms.Label();
            this.chkTrialEnabled = new System.Windows.Forms.CheckBox();
            this.lblTrialDays = new System.Windows.Forms.Label();
            this.nud_trialDays = new System.Windows.Forms.NumericUpDown();
            this.lblTrialStatus = new System.Windows.Forms.Label();
            this.lblLicenseKey = new System.Windows.Forms.Label();
            this.txtLicenseKey = new System.Windows.Forms.TextBox();
            this.lblSubscriptionTier = new System.Windows.Forms.Label();
            this.cmbSubscriptionTier = new System.Windows.Forms.ComboBox();
            this.btnActivateLicense = new System.Windows.Forms.Button();
            this.btnImportLicenseFile = new System.Windows.Forms.Button();
            this.openFileDialogLicense = new System.Windows.Forms.OpenFileDialog();
            this.chkTrialAutoClose = new System.Windows.Forms.CheckBox();
            this.lblTrialAutoCloseHint = new System.Windows.Forms.Label();
            this.tabPageDbSetup = new System.Windows.Forms.TabPage();
            this.lblDbSetupHint = new System.Windows.Forms.Label();
            this.lblNewDbName = new System.Windows.Forms.Label();
            this.txtNewDbName = new System.Windows.Forms.TextBox();
            this.chkSwitchToNewDb = new System.Windows.Forms.CheckBox();
            this.btnCreateDatabase = new System.Windows.Forms.Button();
            this.lblDbSetupStatus = new System.Windows.Forms.Label();
            this.tabPageRemoteControl = new System.Windows.Forms.TabPage();
            this.lblRemoteHint = new System.Windows.Forms.Label();
            this.lblRemoteIpAddress = new System.Windows.Forms.Label();
            this.btnRefreshIp = new System.Windows.Forms.Button();
            this.chkRemoteControlEnabled = new System.Windows.Forms.CheckBox();
            this.lblRemotePort = new System.Windows.Forms.Label();
            this.nud_remotePort = new System.Windows.Forms.NumericUpDown();
            this.lblRemoteToken = new System.Windows.Forms.Label();
            this.txtRemoteToken = new System.Windows.Forms.TextBox();
            this.chkShowRemoteToken = new System.Windows.Forms.CheckBox();
            this.btnGenerateRemoteToken = new System.Windows.Forms.Button();
            this.lblRemoteStatus = new System.Windows.Forms.Label();
            this.lblCertThumbprintHeader = new System.Windows.Forms.Label();
            this.txtCertThumbprint = new System.Windows.Forms.TextBox();
            this.btnCopyThumbprint = new System.Windows.Forms.Button();
            this.lblPublicIp = new System.Windows.Forms.Label();
            this.txtPublicIp = new System.Windows.Forms.TextBox();
            this.btnCheckPublicIp = new System.Windows.Forms.Button();
            this.lblBluetoothHeader = new System.Windows.Forms.Label();
            this.chkBluetoothEnabled = new System.Windows.Forms.CheckBox();
            this.lblBluetoothRadioInfo = new System.Windows.Forms.Label();
            this.tabPageMaintenance = new System.Windows.Forms.TabPage();
            this.lblMaintenanceHint = new System.Windows.Forms.Label();
            this.chkMaintenanceEnabled = new System.Windows.Forms.CheckBox();
            this.lblMaintenanceMessage = new System.Windows.Forms.Label();
            this.txtMaintenanceMessage = new System.Windows.Forms.TextBox();
            this.btnSaveMaintenance = new System.Windows.Forms.Button();
            this.btnRefreshMaintenance = new System.Windows.Forms.Button();
            this.lblMaintenanceStatus = new System.Windows.Forms.Label();
            this.tabPageLowStock = new System.Windows.Forms.TabPage();
            this.lblLowStockHint = new System.Windows.Forms.Label();
            this.chkLowStockEnabled = new System.Windows.Forms.CheckBox();
            this.lblLowStockThreshold = new System.Windows.Forms.Label();
            this.nudLowStockThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblLowStockInterval = new System.Windows.Forms.Label();
            this.nudLowStockInterval = new System.Windows.Forms.NumericUpDown();
            this.btnSaveLowStock = new System.Windows.Forms.Button();
            this.btnRefreshLowStock = new System.Windows.Forms.Button();
            this.lblLowStockStatus = new System.Windows.Forms.Label();
            this.tabPageReceiptTesting = new System.Windows.Forms.TabPage();
            this.lblReceiptTestingHint = new System.Windows.Forms.Label();
            this.chkPriceOverrideEnabled = new System.Windows.Forms.CheckBox();
            this.tbPriceOverride = new System.Windows.Forms.TrackBar();
            this.lblPriceOverrideValue = new System.Windows.Forms.Label();
            this.btnPreviewReceipt = new System.Windows.Forms.Button();
            this.tabPageItemSearch = new System.Windows.Forms.TabPage();
            this.clbItemSearchColumns = new System.Windows.Forms.CheckedListBox();
            this.lblItemSearchHint = new System.Windows.Forms.Label();
            this.btnItemSearchColumnUp = new System.Windows.Forms.Button();
            this.btnItemSearchColumnDown = new System.Windows.Forms.Button();
            this.chkItemSearchDateSortDescending = new System.Windows.Forms.CheckBox();
            this.chkItemSearchNameSortDescending = new System.Windows.Forms.CheckBox();
            this.tabPageCartColumns = new System.Windows.Forms.TabPage();
            this.clbCartColumns = new System.Windows.Forms.CheckedListBox();
            this.lblCartColumnsHint = new System.Windows.Forms.Label();
            this.btnCartColumnUp = new System.Windows.Forms.Button();
            this.btnCartColumnDown = new System.Windows.Forms.Button();
            this.openFileDialogAccess = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialogCounterReceipt = new System.Windows.Forms.FolderBrowserDialog();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblWarningBar = new System.Windows.Forms.Label();
            this.tooltipWarnings = new System.Windows.Forms.ToolTip();
            this.pnlConfigHost.SuspendLayout();
            this.tabControlConfig.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.gbxStoreInfo.SuspendLayout();
            this.gbxDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_connectionTimeout)).BeginInit();
            this.gbxSecurity.SuspendLayout();
            this.gbxSalesDefaults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_autoHoldMinutes)).BeginInit();
            this.gbxAlerts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_alertAutoDismissSeconds)).BeginInit();
            this.gbxStartup.SuspendLayout();
            this.tabPageNetwork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartNetwork)).BeginInit();
            this.tabPageMenus.SuspendLayout();
            this.tabPagePrinter.SuspendLayout();
            this.tabPageAppearance.SuspendLayout();
            this.tabPageLayout.SuspendLayout();
            this.tabPageSchedule.SuspendLayout();
            this.gbxGmail.SuspendLayout();
            this.gbxSmtp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_smtpPort)).BeginInit();
            this.tabPageDataSync.SuspendLayout();
            this.grpUpload.SuspendLayout();
            this.tabPageTrial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_trialDays)).BeginInit();
            this.tabPageDbSetup.SuspendLayout();
            this.tabPageRemoteControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_fontScalePt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_remotePort)).BeginInit();
            this.tabPageMaintenance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowStockThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowStockInterval)).BeginInit();
            this.tabPageLowStock.SuspendLayout();
            this.tabPageReceiptTesting.SuspendLayout();
            this.tabPageItemSearch.SuspendLayout();
            this.tabPageCartColumns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPriceOverride)).BeginInit();
            this.SuspendLayout();
            //
            // lblWarningBar
            //
            this.lblWarningBar.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.lblWarningBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWarningBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblWarningBar.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.lblWarningBar.Location = new System.Drawing.Point(12, 12);
            this.lblWarningBar.Name = "lblWarningBar";
            this.lblWarningBar.Size = new System.Drawing.Size(542, 26);
            this.lblWarningBar.TabIndex = 4;
            this.lblWarningBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWarningBar.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblWarningBar.Visible = false;
            //
            // tooltipWarnings
            //
            this.tooltipWarnings.AutoPopDelay = 15000;
            this.tooltipWarnings.InitialDelay = 300;
            this.tooltipWarnings.ReshowDelay = 100;
            //
            // lstConfigSections
            //
            // Replaces tabControlConfig's own tab strip with a flat, Visual Studio Options-style
            // category list (plain text, no tab chrome) - see pnlConfigHost below for how the
            // native strip is hidden while every existing TabPage/its controls stay untouched.
            this.lstConfigSections.BackColor = System.Drawing.Color.White;
            this.lstConfigSections.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstConfigSections.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstConfigSections.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.lstConfigSections.IntegralHeight = false;
            this.lstConfigSections.ItemHeight = 28;
            this.lstConfigSections.Items.AddRange(new object[] {
            "Settings",
            "Report Schedule",
            "Network Monitor",
            "Menus",
            "Printer",
            "Appearance",
            "Layout",
            "Data Sync",
            "Trial / Licensing",
            "Database Setup",
            "Remote Control",
            "Maintenance",
            "Low Stock Reminder",
            "Receipt Testing",
            "Item Search",
            "Cart Board"});
            this.lstConfigSections.Location = new System.Drawing.Point(12, 44);
            this.lstConfigSections.Name = "lstConfigSections";
            this.lstConfigSections.Size = new System.Drawing.Size(134, 468);
            this.lstConfigSections.TabIndex = 0;
            this.lstConfigSections.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstConfigSections_DrawItem);
            this.lstConfigSections.SelectedIndexChanged += new System.EventHandler(this.lstConfigSections_SelectedIndexChanged);
            //
            // pnlConfigHost
            //
            // Clips off tabControlConfig's own ~22px tab-header strip and ~4px left border - the
            // TabControl is shifted up/left by exactly that much (see tabControlConfig.Location
            // below) and this Panel's bounds (WinForms Panels clip children by default) crop that
            // part away, leaving only each TabPage's content visible, flush with this panel's edges.
            this.pnlConfigHost.BackColor = System.Drawing.Color.FloralWhite;
            this.pnlConfigHost.Controls.Add(this.tabControlConfig);
            this.pnlConfigHost.Location = new System.Drawing.Point(154, 44);
            this.pnlConfigHost.Name = "pnlConfigHost";
            this.pnlConfigHost.Size = new System.Drawing.Size(400, 468);
            this.pnlConfigHost.TabIndex = 1;
            //
            // tabControlConfig
            //
            this.tabControlConfig.Controls.Add(this.tabPageSettings);
            this.tabControlConfig.Controls.Add(this.tabPageSchedule);
            this.tabControlConfig.Controls.Add(this.tabPageNetwork);
            this.tabControlConfig.Controls.Add(this.tabPageMenus);
            this.tabControlConfig.Controls.Add(this.tabPagePrinter);
            this.tabControlConfig.Controls.Add(this.tabPageAppearance);
            this.tabControlConfig.Controls.Add(this.tabPageLayout);
            this.tabControlConfig.Controls.Add(this.tabPageDataSync);
            this.tabControlConfig.Controls.Add(this.tabPageTrial);
            this.tabControlConfig.Controls.Add(this.tabPageDbSetup);
            this.tabControlConfig.Controls.Add(this.tabPageRemoteControl);
            this.tabControlConfig.Controls.Add(this.tabPageMaintenance);
            this.tabControlConfig.Controls.Add(this.tabPageLowStock);
            this.tabControlConfig.Controls.Add(this.tabPageReceiptTesting);
            this.tabControlConfig.Controls.Add(this.tabPageItemSearch);
            this.tabControlConfig.Controls.Add(this.tabPageCartColumns);
            this.tabControlConfig.Location = new System.Drawing.Point(-4, -22);
            this.tabControlConfig.Name = "tabControlConfig";
            this.tabControlConfig.SelectedIndex = 0;
            this.tabControlConfig.Size = new System.Drawing.Size(400, 468);
            this.tabControlConfig.TabIndex = 0;
            this.tabControlConfig.TabStop = false;
            this.tabControlConfig.SelectedIndexChanged += new System.EventHandler(this.tabControlConfig_SelectedIndexChanged);
            //
            // tabPageSettings
            //
            this.tabPageSettings.AutoScroll = true;
            this.tabPageSettings.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageSettings.Controls.Add(this.gbxSalesDefaults);
            this.tabPageSettings.Controls.Add(this.gbxStoreInfo);
            this.tabPageSettings.Controls.Add(this.gbxDatabase);
            this.tabPageSettings.Controls.Add(this.gbxSecurity);
            this.tabPageSettings.Controls.Add(this.gbxAlerts);
            this.tabPageSettings.Controls.Add(this.gbxStartup);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(392, 372);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Settings";
            //
            // gbxStoreInfo
            //
            this.gbxStoreInfo.BackColor = System.Drawing.Color.MintCream;
            this.gbxStoreInfo.Controls.Add(this.txtStoreEmail);
            this.gbxStoreInfo.Controls.Add(this.lblStoreEmail);
            this.gbxStoreInfo.Controls.Add(this.txtStoreCode);
            this.gbxStoreInfo.Controls.Add(this.lblStoreCode);
            this.gbxStoreInfo.Controls.Add(this.txtStoreName);
            this.gbxStoreInfo.Controls.Add(this.lblStoreName);
            this.gbxStoreInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxStoreInfo.Location = new System.Drawing.Point(6, 187);
            this.gbxStoreInfo.Name = "gbxStoreInfo";
            this.gbxStoreInfo.Size = new System.Drawing.Size(380, 116);
            this.gbxStoreInfo.TabIndex = 1;
            this.gbxStoreInfo.TabStop = false;
            this.gbxStoreInfo.Text = "Store Information";
            //
            // lblStoreName
            //
            this.lblStoreName.AutoSize = true;
            this.lblStoreName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblStoreName.Location = new System.Drawing.Point(10, 27);
            this.lblStoreName.Name = "lblStoreName";
            this.lblStoreName.Size = new System.Drawing.Size(66, 13);
            this.lblStoreName.TabIndex = 0;
            this.lblStoreName.Text = "Store Name:";
            //
            // txtStoreName
            //
            this.txtStoreName.Location = new System.Drawing.Point(134, 24);
            this.txtStoreName.Name = "txtStoreName";
            this.txtStoreName.Size = new System.Drawing.Size(234, 22);
            this.txtStoreName.TabIndex = 1;
            //
            // lblStoreCode
            //
            this.lblStoreCode.AutoSize = true;
            this.lblStoreCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblStoreCode.Location = new System.Drawing.Point(10, 57);
            this.lblStoreCode.Name = "lblStoreCode";
            this.lblStoreCode.Size = new System.Drawing.Size(65, 13);
            this.lblStoreCode.TabIndex = 2;
            this.lblStoreCode.Text = "Store Code:";
            //
            // txtStoreCode
            //
            this.txtStoreCode.Location = new System.Drawing.Point(134, 54);
            this.txtStoreCode.Name = "txtStoreCode";
            this.txtStoreCode.Size = new System.Drawing.Size(100, 22);
            this.txtStoreCode.TabIndex = 3;
            //
            // lblStoreEmail
            //
            this.lblStoreEmail.AutoSize = true;
            this.lblStoreEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblStoreEmail.Location = new System.Drawing.Point(10, 87);
            this.lblStoreEmail.Name = "lblStoreEmail";
            this.lblStoreEmail.Size = new System.Drawing.Size(66, 13);
            this.lblStoreEmail.TabIndex = 4;
            this.lblStoreEmail.Text = "Email:";
            //
            // txtStoreEmail
            //
            this.txtStoreEmail.Location = new System.Drawing.Point(134, 84);
            this.txtStoreEmail.Name = "txtStoreEmail";
            this.txtStoreEmail.Size = new System.Drawing.Size(234, 22);
            this.txtStoreEmail.TabIndex = 5;
            //
            // gbxDatabase
            //
            this.gbxDatabase.BackColor = System.Drawing.Color.MintCream;
            this.gbxDatabase.Controls.Add(this.lblDbNote);
            this.gbxDatabase.Controls.Add(this.nud_connectionTimeout);
            this.gbxDatabase.Controls.Add(this.lblTimeout);
            this.gbxDatabase.Controls.Add(this.chkShowDbPassword);
            this.gbxDatabase.Controls.Add(this.txtDbPassword);
            this.gbxDatabase.Controls.Add(this.lblDbPassword);
            this.gbxDatabase.Controls.Add(this.txtDbUser);
            this.gbxDatabase.Controls.Add(this.lblDbUser);
            this.gbxDatabase.Controls.Add(this.txtDbName);
            this.gbxDatabase.Controls.Add(this.lblDbName);
            this.gbxDatabase.Controls.Add(this.txtDbServer);
            this.gbxDatabase.Controls.Add(this.lblDbServer);
            this.gbxDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxDatabase.Location = new System.Drawing.Point(6, 309);
            this.gbxDatabase.Name = "gbxDatabase";
            this.gbxDatabase.Size = new System.Drawing.Size(380, 192);
            this.gbxDatabase.TabIndex = 2;
            this.gbxDatabase.TabStop = false;
            this.gbxDatabase.Text = "Database Connection";
            //
            // lblDbServer
            //
            this.lblDbServer.AutoSize = true;
            this.lblDbServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblDbServer.Location = new System.Drawing.Point(10, 29);
            this.lblDbServer.Name = "lblDbServer";
            this.lblDbServer.Size = new System.Drawing.Size(45, 13);
            this.lblDbServer.TabIndex = 0;
            this.lblDbServer.Text = "Server:";
            //
            // txtDbServer
            //
            this.txtDbServer.Location = new System.Drawing.Point(134, 26);
            this.txtDbServer.Name = "txtDbServer";
            this.txtDbServer.Size = new System.Drawing.Size(234, 22);
            this.txtDbServer.TabIndex = 1;
            //
            // lblDbName
            //
            this.lblDbName.AutoSize = true;
            this.lblDbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblDbName.Location = new System.Drawing.Point(10, 59);
            this.lblDbName.Name = "lblDbName";
            this.lblDbName.Size = new System.Drawing.Size(59, 13);
            this.lblDbName.TabIndex = 2;
            this.lblDbName.Text = "Database:";
            //
            // txtDbName
            //
            this.txtDbName.Location = new System.Drawing.Point(134, 56);
            this.txtDbName.Name = "txtDbName";
            this.txtDbName.Size = new System.Drawing.Size(234, 22);
            this.txtDbName.TabIndex = 3;
            //
            // lblDbUser
            //
            this.lblDbUser.AutoSize = true;
            this.lblDbUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblDbUser.Location = new System.Drawing.Point(10, 89);
            this.lblDbUser.Name = "lblDbUser";
            this.lblDbUser.Size = new System.Drawing.Size(32, 13);
            this.lblDbUser.TabIndex = 4;
            this.lblDbUser.Text = "User:";
            //
            // txtDbUser
            //
            this.txtDbUser.Location = new System.Drawing.Point(134, 86);
            this.txtDbUser.Name = "txtDbUser";
            this.txtDbUser.Size = new System.Drawing.Size(234, 22);
            this.txtDbUser.TabIndex = 5;
            //
            // lblDbPassword
            //
            this.lblDbPassword.AutoSize = true;
            this.lblDbPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblDbPassword.Location = new System.Drawing.Point(10, 119);
            this.lblDbPassword.Name = "lblDbPassword";
            this.lblDbPassword.Size = new System.Drawing.Size(57, 13);
            this.lblDbPassword.TabIndex = 6;
            this.lblDbPassword.Text = "Password:";
            //
            // txtDbPassword
            //
            this.txtDbPassword.Location = new System.Drawing.Point(134, 116);
            this.txtDbPassword.Name = "txtDbPassword";
            this.txtDbPassword.Size = new System.Drawing.Size(170, 22);
            this.txtDbPassword.TabIndex = 7;
            this.txtDbPassword.UseSystemPasswordChar = true;
            //
            // chkShowDbPassword
            //
            this.chkShowDbPassword.AutoSize = true;
            this.chkShowDbPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkShowDbPassword.Location = new System.Drawing.Point(314, 118);
            this.chkShowDbPassword.Name = "chkShowDbPassword";
            this.chkShowDbPassword.Size = new System.Drawing.Size(54, 17);
            this.chkShowDbPassword.TabIndex = 8;
            this.chkShowDbPassword.Text = "Show";
            this.chkShowDbPassword.UseVisualStyleBackColor = true;
            this.chkShowDbPassword.CheckedChanged += new System.EventHandler(this.chkShowDbPassword_CheckedChanged);
            //
            // lblTimeout
            //
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblTimeout.Location = new System.Drawing.Point(10, 149);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(93, 13);
            this.lblTimeout.TabIndex = 9;
            this.lblTimeout.Text = "Timeout (sec):";
            //
            // nud_connectionTimeout
            //
            this.nud_connectionTimeout.Location = new System.Drawing.Point(134, 146);
            this.nud_connectionTimeout.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.nud_connectionTimeout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nud_connectionTimeout.Name = "nud_connectionTimeout";
            this.nud_connectionTimeout.Size = new System.Drawing.Size(80, 22);
            this.nud_connectionTimeout.TabIndex = 10;
            this.nud_connectionTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_connectionTimeout.Value = new decimal(new int[] { 30, 0, 0, 0 });
            //
            // lblDbNote
            //
            this.lblDbNote.AutoSize = true;
            this.lblDbNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblDbNote.ForeColor = System.Drawing.Color.Gray;
            this.lblDbNote.Location = new System.Drawing.Point(10, 172);
            this.lblDbNote.Name = "lblDbNote";
            this.lblDbNote.Size = new System.Drawing.Size(275, 12);
            this.lblDbNote.TabIndex = 11;
            this.lblDbNote.Text = "Changes here take effect after restarting the app.";
            //
            // gbxSecurity
            //
            this.gbxSecurity.BackColor = System.Drawing.Color.MintCream;
            this.gbxSecurity.Controls.Add(this.chkShowAdminPin);
            this.gbxSecurity.Controls.Add(this.txtAdminPin);
            this.gbxSecurity.Controls.Add(this.lblAdminPin);
            this.gbxSecurity.Controls.Add(this.chkShowPin);
            this.gbxSecurity.Controls.Add(this.txtManagerPin);
            this.gbxSecurity.Controls.Add(this.lblManagerPin);
            this.gbxSecurity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxSecurity.Location = new System.Drawing.Point(6, 507);
            this.gbxSecurity.Name = "gbxSecurity";
            this.gbxSecurity.Size = new System.Drawing.Size(380, 104);
            this.gbxSecurity.TabIndex = 3;
            this.gbxSecurity.TabStop = false;
            this.gbxSecurity.Text = "Security";
            //
            // gbxSalesDefaults
            //
            this.gbxSalesDefaults.BackColor = System.Drawing.Color.MintCream;
            this.gbxSalesDefaults.Controls.Add(this.lblAutoHoldMinutes);
            this.gbxSalesDefaults.Controls.Add(this.nud_autoHoldMinutes);
            this.gbxSalesDefaults.Controls.Add(this.chkAutoHoldEnabled);
            this.gbxSalesDefaults.Controls.Add(this.cbxDefaultSelltype);
            this.gbxSalesDefaults.Controls.Add(this.chkDefaultSelltype);
            this.gbxSalesDefaults.Controls.Add(this.chkBlockZeroPriceItems);
            this.gbxSalesDefaults.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxSalesDefaults.Location = new System.Drawing.Point(6, 6);
            this.gbxSalesDefaults.Name = "gbxSalesDefaults";
            this.gbxSalesDefaults.Size = new System.Drawing.Size(380, 175);
            this.gbxSalesDefaults.TabIndex = 0;
            this.gbxSalesDefaults.TabStop = false;
            this.gbxSalesDefaults.Text = "Sales Defaults";
            //
            // chkDefaultSelltype
            //
            this.chkDefaultSelltype.AutoSize = true;
            this.chkDefaultSelltype.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkDefaultSelltype.Location = new System.Drawing.Point(13, 25);
            this.chkDefaultSelltype.Name = "chkDefaultSelltype";
            this.chkDefaultSelltype.Size = new System.Drawing.Size(220, 17);
            this.chkDefaultSelltype.TabIndex = 0;
            this.chkDefaultSelltype.Text = "Default purchase type at startup:";
            this.chkDefaultSelltype.UseVisualStyleBackColor = true;
            this.chkDefaultSelltype.CheckedChanged += new System.EventHandler(this.chkDefaultSelltype_CheckedChanged);
            //
            // cbxDefaultSelltype
            //
            this.cbxDefaultSelltype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDefaultSelltype.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.cbxDefaultSelltype.FormattingEnabled = true;
            this.cbxDefaultSelltype.Items.AddRange(new object[] {
            "Wholesale",
            "Retail",
            "By Piece"});
            this.cbxDefaultSelltype.Location = new System.Drawing.Point(13, 52);
            this.cbxDefaultSelltype.Name = "cbxDefaultSelltype";
            this.cbxDefaultSelltype.Size = new System.Drawing.Size(150, 22);
            this.cbxDefaultSelltype.TabIndex = 1;
            //
            // chkAutoHoldEnabled
            //
            this.chkAutoHoldEnabled.AutoSize = true;
            this.chkAutoHoldEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkAutoHoldEnabled.Location = new System.Drawing.Point(13, 84);
            this.chkAutoHoldEnabled.Name = "chkAutoHoldEnabled";
            this.chkAutoHoldEnabled.Size = new System.Drawing.Size(220, 17);
            this.chkAutoHoldEnabled.TabIndex = 2;
            this.chkAutoHoldEnabled.Text = "Automatically hold an idle cart after:";
            this.chkAutoHoldEnabled.UseVisualStyleBackColor = true;
            this.chkAutoHoldEnabled.CheckedChanged += new System.EventHandler(this.chkAutoHoldEnabled_CheckedChanged);
            //
            // nud_autoHoldMinutes
            //
            this.nud_autoHoldMinutes.Location = new System.Drawing.Point(13, 111);
            this.nud_autoHoldMinutes.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
            this.nud_autoHoldMinutes.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nud_autoHoldMinutes.Name = "nud_autoHoldMinutes";
            this.nud_autoHoldMinutes.Size = new System.Drawing.Size(70, 22);
            this.nud_autoHoldMinutes.TabIndex = 3;
            this.nud_autoHoldMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_autoHoldMinutes.Value = new decimal(new int[] { 30, 0, 0, 0 });
            //
            // lblAutoHoldMinutes
            //
            this.lblAutoHoldMinutes.AutoSize = true;
            this.lblAutoHoldMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblAutoHoldMinutes.Location = new System.Drawing.Point(89, 114);
            this.lblAutoHoldMinutes.Name = "lblAutoHoldMinutes";
            this.lblAutoHoldMinutes.Size = new System.Drawing.Size(120, 13);
            this.lblAutoHoldMinutes.TabIndex = 4;
            this.lblAutoHoldMinutes.Text = "minute(s) of inactivity";
            //
            // chkBlockZeroPriceItems
            //
            this.chkBlockZeroPriceItems.AutoSize = true;
            this.chkBlockZeroPriceItems.Checked = true;
            this.chkBlockZeroPriceItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBlockZeroPriceItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkBlockZeroPriceItems.Location = new System.Drawing.Point(13, 137);
            this.chkBlockZeroPriceItems.Name = "chkBlockZeroPriceItems";
            this.chkBlockZeroPriceItems.Size = new System.Drawing.Size(340, 30);
            this.chkBlockZeroPriceItems.TabIndex = 4;
            this.chkBlockZeroPriceItems.Text = "Block items with no price (₱0.00) for the selected selling type\r\nfrom being add" +
    "ed to the cart";
            this.chkBlockZeroPriceItems.UseVisualStyleBackColor = true;
            //
            // gbxAlerts
            //
            this.gbxAlerts.BackColor = System.Drawing.Color.MintCream;
            this.gbxAlerts.Controls.Add(this.lblAlertAutoDismissSeconds);
            this.gbxAlerts.Controls.Add(this.nud_alertAutoDismissSeconds);
            this.gbxAlerts.Controls.Add(this.chkAlertAutoDismissEnabled);
            this.gbxAlerts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxAlerts.Location = new System.Drawing.Point(6, 617);
            this.gbxAlerts.Name = "gbxAlerts";
            this.gbxAlerts.Size = new System.Drawing.Size(380, 90);
            this.gbxAlerts.TabIndex = 4;
            this.gbxAlerts.TabStop = false;
            this.gbxAlerts.Text = "Alerts";
            //
            // chkAlertAutoDismissEnabled
            //
            this.chkAlertAutoDismissEnabled.AutoSize = true;
            this.chkAlertAutoDismissEnabled.Checked = true;
            this.chkAlertAutoDismissEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlertAutoDismissEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkAlertAutoDismissEnabled.Location = new System.Drawing.Point(13, 25);
            this.chkAlertAutoDismissEnabled.Name = "chkAlertAutoDismissEnabled";
            this.chkAlertAutoDismissEnabled.Size = new System.Drawing.Size(280, 17);
            this.chkAlertAutoDismissEnabled.TabIndex = 0;
            this.chkAlertAutoDismissEnabled.Text = "Automatically dismiss confirmation dialogs after:";
            this.chkAlertAutoDismissEnabled.UseVisualStyleBackColor = true;
            this.chkAlertAutoDismissEnabled.CheckedChanged += new System.EventHandler(this.chkAlertAutoDismissEnabled_CheckedChanged);
            //
            // nud_alertAutoDismissSeconds
            //
            this.nud_alertAutoDismissSeconds.Location = new System.Drawing.Point(13, 52);
            this.nud_alertAutoDismissSeconds.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            this.nud_alertAutoDismissSeconds.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nud_alertAutoDismissSeconds.Name = "nud_alertAutoDismissSeconds";
            this.nud_alertAutoDismissSeconds.Size = new System.Drawing.Size(70, 22);
            this.nud_alertAutoDismissSeconds.TabIndex = 1;
            this.nud_alertAutoDismissSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_alertAutoDismissSeconds.Value = new decimal(new int[] { 5, 0, 0, 0 });
            //
            // lblAlertAutoDismissSeconds
            //
            this.lblAlertAutoDismissSeconds.AutoSize = true;
            this.lblAlertAutoDismissSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblAlertAutoDismissSeconds.Location = new System.Drawing.Point(89, 55);
            this.lblAlertAutoDismissSeconds.Name = "lblAlertAutoDismissSeconds";
            this.lblAlertAutoDismissSeconds.Size = new System.Drawing.Size(120, 13);
            this.lblAlertAutoDismissSeconds.TabIndex = 2;
            this.lblAlertAutoDismissSeconds.Text = "second(s), defaults to No";
            //
            // gbxStartup
            //
            this.gbxStartup.BackColor = System.Drawing.Color.MintCream;
            this.gbxStartup.Controls.Add(this.chkAutoStartWithWindows);
            this.gbxStartup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxStartup.Location = new System.Drawing.Point(6, 713);
            this.gbxStartup.Name = "gbxStartup";
            this.gbxStartup.Size = new System.Drawing.Size(380, 60);
            this.gbxStartup.TabIndex = 5;
            this.gbxStartup.TabStop = false;
            this.gbxStartup.Text = "Startup";
            //
            // chkAutoStartWithWindows
            //
            this.chkAutoStartWithWindows.AutoSize = true;
            this.chkAutoStartWithWindows.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkAutoStartWithWindows.Location = new System.Drawing.Point(13, 25);
            this.chkAutoStartWithWindows.Name = "chkAutoStartWithWindows";
            this.chkAutoStartWithWindows.Size = new System.Drawing.Size(280, 17);
            this.chkAutoStartWithWindows.TabIndex = 0;
            this.chkAutoStartWithWindows.Text = "Start BrewCode automatically when Windows starts";
            this.chkAutoStartWithWindows.UseVisualStyleBackColor = true;
            //
            // lblManagerPin
            //
            this.lblManagerPin.AutoSize = true;
            this.lblManagerPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblManagerPin.Location = new System.Drawing.Point(10, 29);
            this.lblManagerPin.Name = "lblManagerPin";
            this.lblManagerPin.Size = new System.Drawing.Size(75, 13);
            this.lblManagerPin.TabIndex = 0;
            this.lblManagerPin.Text = "Manager PIN:";
            //
            // txtManagerPin
            //
            this.txtManagerPin.Location = new System.Drawing.Point(134, 26);
            this.txtManagerPin.Name = "txtManagerPin";
            this.txtManagerPin.Size = new System.Drawing.Size(100, 22);
            this.txtManagerPin.TabIndex = 1;
            this.txtManagerPin.UseSystemPasswordChar = true;
            //
            // chkShowPin
            //
            this.chkShowPin.AutoSize = true;
            this.chkShowPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkShowPin.Location = new System.Drawing.Point(244, 28);
            this.chkShowPin.Name = "chkShowPin";
            this.chkShowPin.Size = new System.Drawing.Size(54, 17);
            this.chkShowPin.TabIndex = 2;
            this.chkShowPin.Text = "Show";
            this.chkShowPin.UseVisualStyleBackColor = true;
            this.chkShowPin.CheckedChanged += new System.EventHandler(this.chkShowPin_CheckedChanged);
            //
            // lblAdminPin
            //
            this.lblAdminPin.AutoSize = true;
            this.lblAdminPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblAdminPin.Location = new System.Drawing.Point(10, 63);
            this.lblAdminPin.Name = "lblAdminPin";
            this.lblAdminPin.Size = new System.Drawing.Size(107, 13);
            this.lblAdminPin.TabIndex = 3;
            this.lblAdminPin.Text = "System Admin PIN:";
            //
            // txtAdminPin
            //
            this.txtAdminPin.Location = new System.Drawing.Point(134, 60);
            this.txtAdminPin.Name = "txtAdminPin";
            this.txtAdminPin.Size = new System.Drawing.Size(100, 22);
            this.txtAdminPin.TabIndex = 4;
            this.txtAdminPin.UseSystemPasswordChar = true;
            //
            // chkShowAdminPin
            //
            this.chkShowAdminPin.AutoSize = true;
            this.chkShowAdminPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkShowAdminPin.Location = new System.Drawing.Point(244, 62);
            this.chkShowAdminPin.Name = "chkShowAdminPin";
            this.chkShowAdminPin.Size = new System.Drawing.Size(54, 17);
            this.chkShowAdminPin.TabIndex = 5;
            this.chkShowAdminPin.Text = "Show";
            this.chkShowAdminPin.UseVisualStyleBackColor = true;
            this.chkShowAdminPin.CheckedChanged += new System.EventHandler(this.chkShowAdminPin_CheckedChanged);
            //
            // tabPageNetwork
            //
            this.tabPageNetwork.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageNetwork.Controls.Add(this.lblNetworkHint);
            this.tabPageNetwork.Controls.Add(this.chartNetwork);
            this.tabPageNetwork.Controls.Add(this.lblNetworkStatus);
            this.tabPageNetwork.Controls.Add(this.btnToggleMonitor);
            this.tabPageNetwork.Location = new System.Drawing.Point(4, 22);
            this.tabPageNetwork.Name = "tabPageNetwork";
            this.tabPageNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNetwork.Size = new System.Drawing.Size(392, 372);
            this.tabPageNetwork.TabIndex = 1;
            this.tabPageNetwork.Text = "Network Monitor";
            //
            // btnToggleMonitor
            //
            this.btnToggleMonitor.Location = new System.Drawing.Point(6, 6);
            this.btnToggleMonitor.Name = "btnToggleMonitor";
            this.btnToggleMonitor.Size = new System.Drawing.Size(150, 30);
            this.btnToggleMonitor.TabIndex = 0;
            this.btnToggleMonitor.Text = "Start Monitoring";
            this.btnToggleMonitor.UseVisualStyleBackColor = true;
            this.btnToggleMonitor.Click += new System.EventHandler(this.btnToggleMonitor_Click);
            //
            // lblNetworkStatus
            //
            this.lblNetworkStatus.AutoSize = true;
            this.lblNetworkStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblNetworkStatus.Location = new System.Drawing.Point(164, 14);
            this.lblNetworkStatus.Name = "lblNetworkStatus";
            this.lblNetworkStatus.Size = new System.Drawing.Size(24, 13);
            this.lblNetworkStatus.TabIndex = 1;
            this.lblNetworkStatus.Text = "Idle.";
            //
            // chartNetwork
            //
            networkChartArea.AxisX.Title = "Sample";
            networkChartArea.AxisY.Title = "Mbps";
            networkChartArea.Name = "ChartArea1";
            this.chartNetwork.ChartAreas.Add(networkChartArea);
            networkLegend.Name = "Legend1";
            this.chartNetwork.Legends.Add(networkLegend);
            this.chartNetwork.Location = new System.Drawing.Point(6, 42);
            this.chartNetwork.Name = "chartNetwork";
            downloadSeries.ChartArea = "ChartArea1";
            downloadSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            downloadSeries.Color = System.Drawing.Color.SteelBlue;
            downloadSeries.BorderWidth = 2;
            downloadSeries.Legend = "Legend1";
            downloadSeries.MarkerSize = 5;
            downloadSeries.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            downloadSeries.Name = "Download";
            uploadSeries.ChartArea = "ChartArea1";
            uploadSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            uploadSeries.Color = System.Drawing.Color.DarkOrange;
            uploadSeries.BorderWidth = 2;
            uploadSeries.Legend = "Legend1";
            uploadSeries.MarkerSize = 5;
            uploadSeries.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            uploadSeries.Name = "Upload";
            this.chartNetwork.Series.Add(downloadSeries);
            this.chartNetwork.Series.Add(uploadSeries);
            this.chartNetwork.Size = new System.Drawing.Size(380, 300);
            this.chartNetwork.TabIndex = 2;
            this.chartNetwork.Text = "chartNetwork";
            //
            // lblNetworkHint
            //
            this.lblNetworkHint.AutoSize = true;
            this.lblNetworkHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblNetworkHint.ForeColor = System.Drawing.Color.Gray;
            this.lblNetworkHint.Location = new System.Drawing.Point(6, 346);
            this.lblNetworkHint.Name = "lblNetworkHint";
            this.lblNetworkHint.Size = new System.Drawing.Size(275, 12);
            this.lblNetworkHint.TabIndex = 3;
            this.lblNetworkHint.Text = "Tests your internet connection every 15 seconds while running.";
            //
            // tabPageMenus
            //
            this.tabPageMenus.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageMenus.Controls.Add(this.clbMenus);
            this.tabPageMenus.Controls.Add(this.lblMenusHint);
            this.tabPageMenus.Location = new System.Drawing.Point(4, 22);
            this.tabPageMenus.Name = "tabPageMenus";
            this.tabPageMenus.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMenus.Size = new System.Drawing.Size(392, 372);
            this.tabPageMenus.TabIndex = 2;
            this.tabPageMenus.Text = "Menus";
            //
            // lblMenusHint
            //
            this.lblMenusHint.AutoSize = true;
            this.lblMenusHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblMenusHint.Location = new System.Drawing.Point(6, 6);
            this.lblMenusHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblMenusHint.Name = "lblMenusHint";
            this.lblMenusHint.Size = new System.Drawing.Size(300, 26);
            this.lblMenusHint.TabIndex = 0;
            this.lblMenusHint.Text = "Uncheck a menu to hide it from the menu bar. Takes effect immediately - no restart needed.";
            //
            // clbMenus
            //
            this.clbMenus.CheckOnClick = true;
            this.clbMenus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.clbMenus.FormattingEnabled = true;
            this.clbMenus.IntegralHeight = false;
            this.clbMenus.Location = new System.Drawing.Point(6, 40);
            this.clbMenus.Name = "clbMenus";
            this.clbMenus.Size = new System.Drawing.Size(380, 326);
            this.clbMenus.TabIndex = 1;
            //
            // tabPagePrinter
            //
            this.tabPagePrinter.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPagePrinter.Controls.Add(this.btnBrowseCounterReceiptPath);
            this.tabPagePrinter.Controls.Add(this.txtCounterReceiptPath);
            this.tabPagePrinter.Controls.Add(this.lblCounterReceiptPath);
            this.tabPagePrinter.Controls.Add(this.lblCounterReceiptHint);
            this.tabPagePrinter.Controls.Add(this.chkSaveCounterReceiptAsJpeg);
            this.tabPagePrinter.Controls.Add(this.chkSetAsSystemDefaultPrinter);
            this.tabPagePrinter.Controls.Add(this.chkPaperWidthIsDefault);
            this.tabPagePrinter.Controls.Add(this.cbxPaperWidth);
            this.tabPagePrinter.Controls.Add(this.lblPaperWidth);
            this.tabPagePrinter.Controls.Add(this.lblPaperSizeInfo);
            this.tabPagePrinter.Controls.Add(this.cbxReceiptPrinter);
            this.tabPagePrinter.Controls.Add(this.lblReceiptPrinter);
            this.tabPagePrinter.Controls.Add(this.lblPrinterHint);
            this.tabPagePrinter.Location = new System.Drawing.Point(4, 22);
            this.tabPagePrinter.Name = "tabPagePrinter";
            this.tabPagePrinter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePrinter.Size = new System.Drawing.Size(392, 372);
            this.tabPagePrinter.TabIndex = 3;
            this.tabPagePrinter.Text = "Printer";
            //
            // lblPrinterHint
            //
            this.lblPrinterHint.AutoSize = true;
            this.lblPrinterHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblPrinterHint.Location = new System.Drawing.Point(6, 6);
            this.lblPrinterHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblPrinterHint.Name = "lblPrinterHint";
            this.lblPrinterHint.Size = new System.Drawing.Size(370, 52);
            this.lblPrinterHint.TabIndex = 0;
            this.lblPrinterHint.Text = "Select the thermal receipt printer. Receipt page width is read from that printer\'s own paper settings in Windows by default - use Paper Width below to override it if the driver reports the wrong roll size.";
            //
            // lblReceiptPrinter
            //
            this.lblReceiptPrinter.AutoSize = true;
            this.lblReceiptPrinter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblReceiptPrinter.Location = new System.Drawing.Point(6, 68);
            this.lblReceiptPrinter.Name = "lblReceiptPrinter";
            this.lblReceiptPrinter.Size = new System.Drawing.Size(84, 13);
            this.lblReceiptPrinter.TabIndex = 1;
            this.lblReceiptPrinter.Text = "Receipt Printer:";
            //
            // cbxReceiptPrinter
            //
            this.cbxReceiptPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxReceiptPrinter.FormattingEnabled = true;
            this.cbxReceiptPrinter.Location = new System.Drawing.Point(6, 85);
            this.cbxReceiptPrinter.Name = "cbxReceiptPrinter";
            this.cbxReceiptPrinter.Size = new System.Drawing.Size(300, 21);
            this.cbxReceiptPrinter.TabIndex = 2;
            this.cbxReceiptPrinter.SelectedIndexChanged += new System.EventHandler(this.cbxReceiptPrinter_SelectedIndexChanged);
            //
            // lblPaperSizeInfo
            //
            this.lblPaperSizeInfo.AutoSize = true;
            this.lblPaperSizeInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblPaperSizeInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblPaperSizeInfo.Location = new System.Drawing.Point(6, 112);
            this.lblPaperSizeInfo.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblPaperSizeInfo.Name = "lblPaperSizeInfo";
            this.lblPaperSizeInfo.Size = new System.Drawing.Size(101, 12);
            this.lblPaperSizeInfo.TabIndex = 3;
            this.lblPaperSizeInfo.Text = "Detected paper size: -";
            //
            // lblPaperWidth
            //
            this.lblPaperWidth.AutoSize = true;
            this.lblPaperWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblPaperWidth.Location = new System.Drawing.Point(6, 136);
            this.lblPaperWidth.Name = "lblPaperWidth";
            this.lblPaperWidth.Size = new System.Drawing.Size(75, 13);
            this.lblPaperWidth.TabIndex = 4;
            this.lblPaperWidth.Text = "Paper Width:";
            //
            // cbxPaperWidth
            //
            this.cbxPaperWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPaperWidth.FormattingEnabled = true;
            this.cbxPaperWidth.Items.AddRange(new object[] {
            "Automatic (use detected size)",
            "80mm",
            "58mm"});
            this.cbxPaperWidth.Location = new System.Drawing.Point(6, 153);
            this.cbxPaperWidth.Name = "cbxPaperWidth";
            this.cbxPaperWidth.Size = new System.Drawing.Size(300, 21);
            this.cbxPaperWidth.TabIndex = 5;
            //
            // chkPaperWidthIsDefault
            //
            this.chkPaperWidthIsDefault.AutoSize = true;
            this.chkPaperWidthIsDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkPaperWidthIsDefault.Location = new System.Drawing.Point(6, 180);
            this.chkPaperWidthIsDefault.Name = "chkPaperWidthIsDefault";
            this.chkPaperWidthIsDefault.Size = new System.Drawing.Size(184, 17);
            this.chkPaperWidthIsDefault.TabIndex = 6;
            this.chkPaperWidthIsDefault.Text = "Set as default paper width";
            this.chkPaperWidthIsDefault.UseVisualStyleBackColor = true;
            this.chkPaperWidthIsDefault.CheckedChanged += new System.EventHandler(this.chkPaperWidthIsDefault_CheckedChanged);
            //
            // chkSetAsSystemDefaultPrinter
            //
            this.chkSetAsSystemDefaultPrinter.AutoSize = true;
            this.chkSetAsSystemDefaultPrinter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkSetAsSystemDefaultPrinter.Location = new System.Drawing.Point(6, 214);
            this.chkSetAsSystemDefaultPrinter.Name = "chkSetAsSystemDefaultPrinter";
            this.chkSetAsSystemDefaultPrinter.Size = new System.Drawing.Size(184, 17);
            this.chkSetAsSystemDefaultPrinter.TabIndex = 7;
            this.chkSetAsSystemDefaultPrinter.Text = "Also set as Windows default printer";
            this.chkSetAsSystemDefaultPrinter.UseVisualStyleBackColor = true;
            //
            // chkSaveCounterReceiptAsJpeg
            //
            this.chkSaveCounterReceiptAsJpeg.AutoSize = true;
            this.chkSaveCounterReceiptAsJpeg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkSaveCounterReceiptAsJpeg.Location = new System.Drawing.Point(6, 248);
            this.chkSaveCounterReceiptAsJpeg.Name = "chkSaveCounterReceiptAsJpeg";
            this.chkSaveCounterReceiptAsJpeg.Size = new System.Drawing.Size(240, 17);
            this.chkSaveCounterReceiptAsJpeg.TabIndex = 8;
            this.chkSaveCounterReceiptAsJpeg.Text = "Save counter\'s copy as JPEG instead of printing";
            this.chkSaveCounterReceiptAsJpeg.UseVisualStyleBackColor = true;
            this.chkSaveCounterReceiptAsJpeg.CheckedChanged += new System.EventHandler(this.chkSaveCounterReceiptAsJpeg_CheckedChanged);
            //
            // lblCounterReceiptHint
            //
            this.lblCounterReceiptHint.AutoSize = true;
            this.lblCounterReceiptHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblCounterReceiptHint.ForeColor = System.Drawing.Color.Gray;
            this.lblCounterReceiptHint.Location = new System.Drawing.Point(6, 272);
            this.lblCounterReceiptHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblCounterReceiptHint.Name = "lblCounterReceiptHint";
            this.lblCounterReceiptHint.Size = new System.Drawing.Size(101, 12);
            this.lblCounterReceiptHint.TabIndex = 9;
            this.lblCounterReceiptHint.Text = "The customer\'s copy always prints as normal. When checked, the counter\'s copy is saved as a JPEG to the folder below instead of using a second sheet.";
            //
            // lblCounterReceiptPath
            //
            this.lblCounterReceiptPath.AutoSize = true;
            this.lblCounterReceiptPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblCounterReceiptPath.Location = new System.Drawing.Point(6, 320);
            this.lblCounterReceiptPath.Name = "lblCounterReceiptPath";
            this.lblCounterReceiptPath.Size = new System.Drawing.Size(66, 13);
            this.lblCounterReceiptPath.TabIndex = 10;
            this.lblCounterReceiptPath.Text = "Save Folder:";
            //
            // txtCounterReceiptPath
            //
            this.txtCounterReceiptPath.Location = new System.Drawing.Point(6, 337);
            this.txtCounterReceiptPath.Name = "txtCounterReceiptPath";
            this.txtCounterReceiptPath.Size = new System.Drawing.Size(300, 22);
            this.txtCounterReceiptPath.TabIndex = 11;
            //
            // btnBrowseCounterReceiptPath
            //
            this.btnBrowseCounterReceiptPath.Location = new System.Drawing.Point(312, 336);
            this.btnBrowseCounterReceiptPath.Name = "btnBrowseCounterReceiptPath";
            this.btnBrowseCounterReceiptPath.Size = new System.Drawing.Size(74, 24);
            this.btnBrowseCounterReceiptPath.TabIndex = 12;
            this.btnBrowseCounterReceiptPath.Text = "Browse...";
            this.btnBrowseCounterReceiptPath.UseVisualStyleBackColor = true;
            this.btnBrowseCounterReceiptPath.Click += new System.EventHandler(this.btnBrowseCounterReceiptPath_Click);
            //
            // tabPageAppearance
            //
            this.tabPageAppearance.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageAppearance.Controls.Add(this.lblLayoutEditHint);
            this.tabPageAppearance.Controls.Add(this.chkLayoutEditEnabled);
            this.tabPageAppearance.Controls.Add(this.lblLogoUrlHint);
            this.tabPageAppearance.Controls.Add(this.txtLogoUrl);
            this.tabPageAppearance.Controls.Add(this.lblLogoUrl);
            this.tabPageAppearance.Controls.Add(this.lblFontScaleHint);
            this.tabPageAppearance.Controls.Add(this.lblFontScaleUnit);
            this.tabPageAppearance.Controls.Add(this.nud_fontScalePt);
            this.tabPageAppearance.Controls.Add(this.lblFontScale);
            this.tabPageAppearance.Controls.Add(this.lblAppearanceNote);
            this.tabPageAppearance.Controls.Add(this.btnPickFgColor);
            this.tabPageAppearance.Controls.Add(this.pnlFgSwatch);
            this.tabPageAppearance.Controls.Add(this.lblFgColor);
            this.tabPageAppearance.Controls.Add(this.btnPickBgColor);
            this.tabPageAppearance.Controls.Add(this.pnlBgSwatch);
            this.tabPageAppearance.Controls.Add(this.lblBgColor);
            this.tabPageAppearance.Controls.Add(this.chkEnableAccent);
            this.tabPageAppearance.Controls.Add(this.lblAppearanceHint);
            this.tabPageAppearance.Location = new System.Drawing.Point(4, 22);
            this.tabPageAppearance.Name = "tabPageAppearance";
            this.tabPageAppearance.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAppearance.Size = new System.Drawing.Size(392, 372);
            this.tabPageAppearance.TabIndex = 4;
            this.tabPageAppearance.Text = "Appearance";
            //
            // lblAppearanceHint
            //
            this.lblAppearanceHint.AutoSize = true;
            this.lblAppearanceHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblAppearanceHint.Location = new System.Drawing.Point(6, 6);
            this.lblAppearanceHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblAppearanceHint.Name = "lblAppearanceHint";
            this.lblAppearanceHint.Size = new System.Drawing.Size(300, 39);
            this.lblAppearanceHint.TabIndex = 0;
            this.lblAppearanceHint.Text = "Pick an accent background and foreground (text) color applied across the whole app. Leave disabled to keep each window\'s designed colors.";
            //
            // chkEnableAccent
            //
            this.chkEnableAccent.AutoSize = true;
            this.chkEnableAccent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkEnableAccent.Location = new System.Drawing.Point(6, 54);
            this.chkEnableAccent.Name = "chkEnableAccent";
            this.chkEnableAccent.Size = new System.Drawing.Size(158, 17);
            this.chkEnableAccent.TabIndex = 1;
            this.chkEnableAccent.Text = "Enable custom accent colors";
            this.chkEnableAccent.UseVisualStyleBackColor = true;
            this.chkEnableAccent.CheckedChanged += new System.EventHandler(this.chkEnableAccent_CheckedChanged);
            //
            // lblBgColor
            //
            this.lblBgColor.AutoSize = true;
            this.lblBgColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblBgColor.Location = new System.Drawing.Point(6, 85);
            this.lblBgColor.Name = "lblBgColor";
            this.lblBgColor.Size = new System.Drawing.Size(74, 13);
            this.lblBgColor.TabIndex = 2;
            this.lblBgColor.Text = "Background:";
            //
            // pnlBgSwatch
            //
            this.pnlBgSwatch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBgSwatch.Location = new System.Drawing.Point(92, 81);
            this.pnlBgSwatch.Name = "pnlBgSwatch";
            this.pnlBgSwatch.Size = new System.Drawing.Size(60, 23);
            this.pnlBgSwatch.TabIndex = 3;
            //
            // btnPickBgColor
            //
            this.btnPickBgColor.Location = new System.Drawing.Point(160, 80);
            this.btnPickBgColor.Name = "btnPickBgColor";
            this.btnPickBgColor.Size = new System.Drawing.Size(90, 25);
            this.btnPickBgColor.TabIndex = 4;
            this.btnPickBgColor.Text = "Choose...";
            this.btnPickBgColor.UseVisualStyleBackColor = true;
            this.btnPickBgColor.Click += new System.EventHandler(this.btnPickBgColor_Click);
            //
            // lblFgColor
            //
            this.lblFgColor.AutoSize = true;
            this.lblFgColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblFgColor.Location = new System.Drawing.Point(6, 120);
            this.lblFgColor.Name = "lblFgColor";
            this.lblFgColor.Size = new System.Drawing.Size(70, 13);
            this.lblFgColor.TabIndex = 5;
            this.lblFgColor.Text = "Foreground:";
            //
            // pnlFgSwatch
            //
            this.pnlFgSwatch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFgSwatch.Location = new System.Drawing.Point(92, 116);
            this.pnlFgSwatch.Name = "pnlFgSwatch";
            this.pnlFgSwatch.Size = new System.Drawing.Size(60, 23);
            this.pnlFgSwatch.TabIndex = 6;
            //
            // btnPickFgColor
            //
            this.btnPickFgColor.Location = new System.Drawing.Point(160, 115);
            this.btnPickFgColor.Name = "btnPickFgColor";
            this.btnPickFgColor.Size = new System.Drawing.Size(90, 25);
            this.btnPickFgColor.TabIndex = 7;
            this.btnPickFgColor.Text = "Choose...";
            this.btnPickFgColor.UseVisualStyleBackColor = true;
            this.btnPickFgColor.Click += new System.EventHandler(this.btnPickFgColor_Click);
            //
            // lblAppearanceNote
            //
            this.lblAppearanceNote.AutoSize = true;
            this.lblAppearanceNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblAppearanceNote.ForeColor = System.Drawing.Color.Gray;
            this.lblAppearanceNote.Location = new System.Drawing.Point(6, 150);
            this.lblAppearanceNote.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblAppearanceNote.Name = "lblAppearanceNote";
            this.lblAppearanceNote.Size = new System.Drawing.Size(101, 12);
            this.lblAppearanceNote.TabIndex = 8;
            this.lblAppearanceNote.Text = "Applies immediately to every open window, including this one, as soon as you save.";
            //
            // lblFontScale
            //
            this.lblFontScale.AutoSize = true;
            this.lblFontScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblFontScale.Location = new System.Drawing.Point(6, 185);
            this.lblFontScale.Name = "lblFontScale";
            this.lblFontScale.Size = new System.Drawing.Size(126, 13);
            this.lblFontScale.TabIndex = 9;
            this.lblFontScale.Text = "Font size adjustment:";
            //
            // nud_fontScalePt
            //
            this.nud_fontScalePt.DecimalPlaces = 1;
            this.nud_fontScalePt.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this.nud_fontScalePt.Location = new System.Drawing.Point(160, 181);
            this.nud_fontScalePt.Minimum = new decimal(new int[] { 6, 0, 0, System.Int32.MinValue });
            this.nud_fontScalePt.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            this.nud_fontScalePt.Name = "nud_fontScalePt";
            this.nud_fontScalePt.Size = new System.Drawing.Size(70, 22);
            this.nud_fontScalePt.TabIndex = 10;
            this.nud_fontScalePt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // lblFontScaleUnit
            //
            this.lblFontScaleUnit.AutoSize = true;
            this.lblFontScaleUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblFontScaleUnit.Location = new System.Drawing.Point(236, 185);
            this.lblFontScaleUnit.Name = "lblFontScaleUnit";
            this.lblFontScaleUnit.Size = new System.Drawing.Size(20, 13);
            this.lblFontScaleUnit.TabIndex = 11;
            this.lblFontScaleUnit.Text = "pt";
            //
            // lblFontScaleHint
            //
            this.lblFontScaleHint.AutoSize = true;
            this.lblFontScaleHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblFontScaleHint.ForeColor = System.Drawing.Color.Gray;
            this.lblFontScaleHint.Location = new System.Drawing.Point(6, 210);
            this.lblFontScaleHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblFontScaleHint.Name = "lblFontScaleHint";
            this.lblFontScaleHint.Size = new System.Drawing.Size(101, 12);
            this.lblFontScaleHint.TabIndex = 12;
            this.lblFontScaleHint.Text = "0 = each window\'s own designed text size. Added to (or subtracted from) every control\'s own font size, so relative sizes (headers vs. labels, etc.) are preserved.";
            //
            // lblLogoUrl
            //
            this.lblLogoUrl.AutoSize = true;
            this.lblLogoUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblLogoUrl.Location = new System.Drawing.Point(6, 256);
            this.lblLogoUrl.Name = "lblLogoUrl";
            this.lblLogoUrl.Size = new System.Drawing.Size(84, 13);
            this.lblLogoUrl.TabIndex = 13;
            this.lblLogoUrl.Text = "Logo image URL:";
            //
            // txtLogoUrl
            //
            this.txtLogoUrl.Location = new System.Drawing.Point(6, 273);
            this.txtLogoUrl.Name = "txtLogoUrl";
            this.txtLogoUrl.Size = new System.Drawing.Size(370, 22);
            this.txtLogoUrl.TabIndex = 14;
            //
            // lblLogoUrlHint
            //
            this.lblLogoUrlHint.AutoSize = true;
            this.lblLogoUrlHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblLogoUrlHint.ForeColor = System.Drawing.Color.Gray;
            this.lblLogoUrlHint.Location = new System.Drawing.Point(6, 298);
            this.lblLogoUrlHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblLogoUrlHint.Name = "lblLogoUrlHint";
            this.lblLogoUrlHint.Size = new System.Drawing.Size(101, 12);
            this.lblLogoUrlHint.TabIndex = 15;
            this.lblLogoUrlHint.Text = "Direct link to a PNG/JPG shown on receipts and as the title-bar icon. Leave blank to use the default Baclig Minimart logo. Takes effect next time the app starts.";
            //
            // chkLayoutEditEnabled
            //
            this.chkLayoutEditEnabled.AutoSize = true;
            this.chkLayoutEditEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkLayoutEditEnabled.Location = new System.Drawing.Point(6, 340);
            this.chkLayoutEditEnabled.Name = "chkLayoutEditEnabled";
            this.chkLayoutEditEnabled.Size = new System.Drawing.Size(230, 17);
            this.chkLayoutEditEnabled.TabIndex = 16;
            this.chkLayoutEditEnabled.Text = "Allow \"Edit Layout\" from the main menu";
            this.chkLayoutEditEnabled.UseVisualStyleBackColor = true;
            //
            // lblLayoutEditHint
            //
            this.lblLayoutEditHint.AutoSize = true;
            this.lblLayoutEditHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblLayoutEditHint.ForeColor = System.Drawing.Color.Gray;
            this.lblLayoutEditHint.Location = new System.Drawing.Point(6, 359);
            this.lblLayoutEditHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblLayoutEditHint.Name = "lblLayoutEditHint";
            this.lblLayoutEditHint.Size = new System.Drawing.Size(101, 12);
            this.lblLayoutEditHint.TabIndex = 17;
            this.lblLayoutEditHint.Text = "Must be checked here before the main menu\'s Edit Layout item will work - the admin PIN is still required there too.";
            //
            // colorDialogAccent
            //
            this.colorDialogAccent.FullOpen = true;
            //
            // tabPageLayout
            //
            this.tabPageLayout.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageLayout.Controls.Add(this.btnSaveLayoutFile);
            this.tabPageLayout.Controls.Add(this.btnLoadLayoutFile);
            this.tabPageLayout.Controls.Add(this.btnApplyLayoutText);
            this.tabPageLayout.Controls.Add(this.btnRefreshLayoutText);
            this.tabPageLayout.Controls.Add(this.txtLayoutRevision);
            this.tabPageLayout.Controls.Add(this.lblLayoutRevisionHint);
            this.tabPageLayout.Location = new System.Drawing.Point(4, 22);
            this.tabPageLayout.Name = "tabPageLayout";
            this.tabPageLayout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLayout.Size = new System.Drawing.Size(392, 400);
            this.tabPageLayout.TabIndex = 11;
            this.tabPageLayout.Text = "Layout";
            //
            // lblLayoutRevisionHint
            //
            this.lblLayoutRevisionHint.AutoSize = true;
            this.lblLayoutRevisionHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblLayoutRevisionHint.Location = new System.Drawing.Point(6, 6);
            this.lblLayoutRevisionHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblLayoutRevisionHint.Name = "lblLayoutRevisionHint";
            this.lblLayoutRevisionHint.Size = new System.Drawing.Size(300, 39);
            this.lblLayoutRevisionHint.TabIndex = 0;
            this.lblLayoutRevisionHint.Text = "The main screen\'s saved control positions and sizes as text. Copy this out as " +
    "a backup before a risky edit, hand-edit it, or load/save whole revisions as files.";
            //
            // txtLayoutRevision
            //
            this.txtLayoutRevision.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.txtLayoutRevision.Location = new System.Drawing.Point(6, 48);
            this.txtLayoutRevision.Multiline = true;
            this.txtLayoutRevision.Name = "txtLayoutRevision";
            this.txtLayoutRevision.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLayoutRevision.Size = new System.Drawing.Size(370, 260);
            this.txtLayoutRevision.TabIndex = 1;
            this.txtLayoutRevision.WordWrap = false;
            //
            // btnRefreshLayoutText
            //
            this.btnRefreshLayoutText.Location = new System.Drawing.Point(6, 314);
            this.btnRefreshLayoutText.Name = "btnRefreshLayoutText";
            this.btnRefreshLayoutText.Size = new System.Drawing.Size(120, 28);
            this.btnRefreshLayoutText.TabIndex = 2;
            this.btnRefreshLayoutText.Text = "Refresh";
            this.btnRefreshLayoutText.UseVisualStyleBackColor = true;
            this.btnRefreshLayoutText.Click += new System.EventHandler(this.btnRefreshLayoutText_Click);
            //
            // btnApplyLayoutText
            //
            this.btnApplyLayoutText.Location = new System.Drawing.Point(132, 314);
            this.btnApplyLayoutText.Name = "btnApplyLayoutText";
            this.btnApplyLayoutText.Size = new System.Drawing.Size(120, 28);
            this.btnApplyLayoutText.TabIndex = 3;
            this.btnApplyLayoutText.Text = "Apply";
            this.btnApplyLayoutText.UseVisualStyleBackColor = true;
            this.btnApplyLayoutText.Click += new System.EventHandler(this.btnApplyLayoutText_Click);
            //
            // btnLoadLayoutFile
            //
            this.btnLoadLayoutFile.Location = new System.Drawing.Point(6, 348);
            this.btnLoadLayoutFile.Name = "btnLoadLayoutFile";
            this.btnLoadLayoutFile.Size = new System.Drawing.Size(120, 28);
            this.btnLoadLayoutFile.TabIndex = 4;
            this.btnLoadLayoutFile.Text = "Load from File...";
            this.btnLoadLayoutFile.UseVisualStyleBackColor = true;
            this.btnLoadLayoutFile.Click += new System.EventHandler(this.btnLoadLayoutFile_Click);
            //
            // btnSaveLayoutFile
            //
            this.btnSaveLayoutFile.Location = new System.Drawing.Point(132, 348);
            this.btnSaveLayoutFile.Name = "btnSaveLayoutFile";
            this.btnSaveLayoutFile.Size = new System.Drawing.Size(120, 28);
            this.btnSaveLayoutFile.TabIndex = 5;
            this.btnSaveLayoutFile.Text = "Save to File...";
            this.btnSaveLayoutFile.UseVisualStyleBackColor = true;
            this.btnSaveLayoutFile.Click += new System.EventHandler(this.btnSaveLayoutFile_Click);
            //
            // openFileDialogLayout
            //
            this.openFileDialogLayout.DefaultExt = "xml";
            this.openFileDialogLayout.Filter = "Layout files (*.xml)|*.xml|All files (*.*)|*.*";
            //
            // saveFileDialogLayout
            //
            this.saveFileDialogLayout.DefaultExt = "xml";
            this.saveFileDialogLayout.FileName = "MainLayout.xml";
            this.saveFileDialogLayout.Filter = "Layout files (*.xml)|*.xml|All files (*.*)|*.*";
            //
            // tabPageSchedule
            //
            this.tabPageSchedule.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageSchedule.Controls.Add(this.gbxGmail);
            this.tabPageSchedule.Controls.Add(this.gbxSmtp);
            this.tabPageSchedule.Controls.Add(this.rdoProviderGmail);
            this.tabPageSchedule.Controls.Add(this.rdoProviderSmtp);
            this.tabPageSchedule.Controls.Add(this.lblSendVia);
            this.tabPageSchedule.Controls.Add(this.btnSendReportNow);
            this.tabPageSchedule.Controls.Add(this.lblScheduleStatus);
            this.tabPageSchedule.Controls.Add(this.cbxReportFrequency);
            this.tabPageSchedule.Controls.Add(this.lblFrequency);
            this.tabPageSchedule.Controls.Add(this.lblScheduleHint);
            this.tabPageSchedule.Location = new System.Drawing.Point(4, 22);
            this.tabPageSchedule.Name = "tabPageSchedule";
            this.tabPageSchedule.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSchedule.Size = new System.Drawing.Size(392, 372);
            this.tabPageSchedule.TabIndex = 5;
            this.tabPageSchedule.Text = "Report Schedule";
            //
            // lblScheduleHint
            //
            this.lblScheduleHint.AutoSize = true;
            this.lblScheduleHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblScheduleHint.Location = new System.Drawing.Point(6, 6);
            this.lblScheduleHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblScheduleHint.Name = "lblScheduleHint";
            this.lblScheduleHint.Size = new System.Drawing.Size(300, 39);
            this.lblScheduleHint.TabIndex = 0;
            this.lblScheduleHint.Text = "Choose how often the sales and inventory report (Store Information > Email) is sent automatically.";
            //
            // lblFrequency
            //
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblFrequency.Location = new System.Drawing.Point(6, 58);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(63, 13);
            this.lblFrequency.TabIndex = 1;
            this.lblFrequency.Text = "Frequency:";
            //
            // cbxReportFrequency
            //
            this.cbxReportFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxReportFrequency.FormattingEnabled = true;
            this.cbxReportFrequency.Location = new System.Drawing.Point(100, 55);
            this.cbxReportFrequency.Name = "cbxReportFrequency";
            this.cbxReportFrequency.Size = new System.Drawing.Size(150, 21);
            this.cbxReportFrequency.TabIndex = 2;
            //
            // lblScheduleStatus
            //
            this.lblScheduleStatus.AutoSize = true;
            this.lblScheduleStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblScheduleStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblScheduleStatus.Location = new System.Drawing.Point(6, 92);
            this.lblScheduleStatus.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblScheduleStatus.Name = "lblScheduleStatus";
            this.lblScheduleStatus.Size = new System.Drawing.Size(101, 24);
            this.lblScheduleStatus.TabIndex = 3;
            this.lblScheduleStatus.Text = "Last sent: never\r\nNext due: -";
            //
            // btnSendReportNow
            //
            this.btnSendReportNow.Location = new System.Drawing.Point(6, 132);
            this.btnSendReportNow.Name = "btnSendReportNow";
            this.btnSendReportNow.Size = new System.Drawing.Size(180, 30);
            this.btnSendReportNow.TabIndex = 4;
            this.btnSendReportNow.Text = "Send Report Now";
            this.btnSendReportNow.UseVisualStyleBackColor = true;
            this.btnSendReportNow.Click += new System.EventHandler(this.btnSendReportNow_Click);
            //
            // lblSendVia
            //
            this.lblSendVia.AutoSize = true;
            this.lblSendVia.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSendVia.Location = new System.Drawing.Point(6, 166);
            this.lblSendVia.Name = "lblSendVia";
            this.lblSendVia.Size = new System.Drawing.Size(56, 13);
            this.lblSendVia.TabIndex = 5;
            this.lblSendVia.Text = "Send via:";
            //
            // rdoProviderSmtp
            //
            this.rdoProviderSmtp.AutoSize = true;
            this.rdoProviderSmtp.Checked = true;
            this.rdoProviderSmtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.rdoProviderSmtp.Location = new System.Drawing.Point(70, 164);
            this.rdoProviderSmtp.Name = "rdoProviderSmtp";
            this.rdoProviderSmtp.Size = new System.Drawing.Size(52, 17);
            this.rdoProviderSmtp.TabIndex = 6;
            this.rdoProviderSmtp.TabStop = true;
            this.rdoProviderSmtp.Text = "SMTP";
            this.rdoProviderSmtp.UseVisualStyleBackColor = true;
            this.rdoProviderSmtp.CheckedChanged += new System.EventHandler(this.rdoProvider_CheckedChanged);
            //
            // rdoProviderGmail
            //
            this.rdoProviderGmail.AutoSize = true;
            this.rdoProviderGmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.rdoProviderGmail.Location = new System.Drawing.Point(140, 164);
            this.rdoProviderGmail.Name = "rdoProviderGmail";
            this.rdoProviderGmail.Size = new System.Drawing.Size(76, 17);
            this.rdoProviderGmail.TabIndex = 7;
            this.rdoProviderGmail.Text = "Gmail API";
            this.rdoProviderGmail.UseVisualStyleBackColor = true;
            this.rdoProviderGmail.CheckedChanged += new System.EventHandler(this.rdoProvider_CheckedChanged);
            //
            // gbxGmail
            //
            this.gbxGmail.BackColor = System.Drawing.Color.MintCream;
            this.gbxGmail.Controls.Add(this.lblGmailNote);
            this.gbxGmail.Controls.Add(this.txtGmailSender);
            this.gbxGmail.Controls.Add(this.lblGmailSender);
            this.gbxGmail.Controls.Add(this.chkShowGmailRefreshToken);
            this.gbxGmail.Controls.Add(this.txtGmailRefreshToken);
            this.gbxGmail.Controls.Add(this.lblGmailRefreshToken);
            this.gbxGmail.Controls.Add(this.chkShowGmailClientSecret);
            this.gbxGmail.Controls.Add(this.txtGmailClientSecret);
            this.gbxGmail.Controls.Add(this.lblGmailClientSecret);
            this.gbxGmail.Controls.Add(this.txtGmailClientId);
            this.gbxGmail.Controls.Add(this.lblGmailClientId);
            this.gbxGmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxGmail.Location = new System.Drawing.Point(6, 192);
            this.gbxGmail.Name = "gbxGmail";
            this.gbxGmail.Size = new System.Drawing.Size(380, 192);
            this.gbxGmail.TabIndex = 8;
            this.gbxGmail.TabStop = false;
            this.gbxGmail.Text = "Gmail API";
            this.gbxGmail.Visible = false;
            //
            // lblGmailClientId
            //
            this.lblGmailClientId.AutoSize = true;
            this.lblGmailClientId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblGmailClientId.Location = new System.Drawing.Point(10, 29);
            this.lblGmailClientId.Name = "lblGmailClientId";
            this.lblGmailClientId.Size = new System.Drawing.Size(56, 13);
            this.lblGmailClientId.TabIndex = 0;
            this.lblGmailClientId.Text = "Client ID:";
            //
            // txtGmailClientId
            //
            this.txtGmailClientId.Location = new System.Drawing.Point(134, 26);
            this.txtGmailClientId.Name = "txtGmailClientId";
            this.txtGmailClientId.Size = new System.Drawing.Size(234, 22);
            this.txtGmailClientId.TabIndex = 1;
            //
            // lblGmailClientSecret
            //
            this.lblGmailClientSecret.AutoSize = true;
            this.lblGmailClientSecret.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblGmailClientSecret.Location = new System.Drawing.Point(10, 59);
            this.lblGmailClientSecret.Name = "lblGmailClientSecret";
            this.lblGmailClientSecret.Size = new System.Drawing.Size(79, 13);
            this.lblGmailClientSecret.TabIndex = 2;
            this.lblGmailClientSecret.Text = "Client Secret:";
            //
            // txtGmailClientSecret
            //
            this.txtGmailClientSecret.Location = new System.Drawing.Point(134, 56);
            this.txtGmailClientSecret.Name = "txtGmailClientSecret";
            this.txtGmailClientSecret.Size = new System.Drawing.Size(170, 22);
            this.txtGmailClientSecret.TabIndex = 3;
            this.txtGmailClientSecret.UseSystemPasswordChar = true;
            //
            // chkShowGmailClientSecret
            //
            this.chkShowGmailClientSecret.AutoSize = true;
            this.chkShowGmailClientSecret.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkShowGmailClientSecret.Location = new System.Drawing.Point(314, 58);
            this.chkShowGmailClientSecret.Name = "chkShowGmailClientSecret";
            this.chkShowGmailClientSecret.Size = new System.Drawing.Size(54, 17);
            this.chkShowGmailClientSecret.TabIndex = 4;
            this.chkShowGmailClientSecret.Text = "Show";
            this.chkShowGmailClientSecret.UseVisualStyleBackColor = true;
            this.chkShowGmailClientSecret.CheckedChanged += new System.EventHandler(this.chkShowGmailClientSecret_CheckedChanged);
            //
            // lblGmailRefreshToken
            //
            this.lblGmailRefreshToken.AutoSize = true;
            this.lblGmailRefreshToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblGmailRefreshToken.Location = new System.Drawing.Point(10, 89);
            this.lblGmailRefreshToken.Name = "lblGmailRefreshToken";
            this.lblGmailRefreshToken.Size = new System.Drawing.Size(87, 13);
            this.lblGmailRefreshToken.TabIndex = 5;
            this.lblGmailRefreshToken.Text = "Refresh Token:";
            //
            // txtGmailRefreshToken
            //
            this.txtGmailRefreshToken.Location = new System.Drawing.Point(134, 86);
            this.txtGmailRefreshToken.Name = "txtGmailRefreshToken";
            this.txtGmailRefreshToken.Size = new System.Drawing.Size(170, 22);
            this.txtGmailRefreshToken.TabIndex = 6;
            this.txtGmailRefreshToken.UseSystemPasswordChar = true;
            //
            // chkShowGmailRefreshToken
            //
            this.chkShowGmailRefreshToken.AutoSize = true;
            this.chkShowGmailRefreshToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkShowGmailRefreshToken.Location = new System.Drawing.Point(314, 88);
            this.chkShowGmailRefreshToken.Name = "chkShowGmailRefreshToken";
            this.chkShowGmailRefreshToken.Size = new System.Drawing.Size(54, 17);
            this.chkShowGmailRefreshToken.TabIndex = 7;
            this.chkShowGmailRefreshToken.Text = "Show";
            this.chkShowGmailRefreshToken.UseVisualStyleBackColor = true;
            this.chkShowGmailRefreshToken.CheckedChanged += new System.EventHandler(this.chkShowGmailRefreshToken_CheckedChanged);
            //
            // lblGmailSender
            //
            this.lblGmailSender.AutoSize = true;
            this.lblGmailSender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblGmailSender.Location = new System.Drawing.Point(10, 119);
            this.lblGmailSender.Name = "lblGmailSender";
            this.lblGmailSender.Size = new System.Drawing.Size(89, 13);
            this.lblGmailSender.TabIndex = 8;
            this.lblGmailSender.Text = "Sender Address:";
            //
            // txtGmailSender
            //
            this.txtGmailSender.Location = new System.Drawing.Point(134, 116);
            this.txtGmailSender.Name = "txtGmailSender";
            this.txtGmailSender.Size = new System.Drawing.Size(234, 22);
            this.txtGmailSender.TabIndex = 9;
            //
            // lblGmailNote
            //
            this.lblGmailNote.AutoSize = true;
            this.lblGmailNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblGmailNote.ForeColor = System.Drawing.Color.Gray;
            this.lblGmailNote.Location = new System.Drawing.Point(10, 146);
            this.lblGmailNote.MaximumSize = new System.Drawing.Size(360, 0);
            this.lblGmailNote.Name = "lblGmailNote";
            this.lblGmailNote.Size = new System.Drawing.Size(275, 24);
            this.lblGmailNote.TabIndex = 10;
            this.lblGmailNote.Text = "Client ID/Secret and Refresh Token come from a Google Cloud OAuth 2.0 Desktop client with the Gmail API enabled.";
            //
            // gbxSmtp
            //
            this.gbxSmtp.BackColor = System.Drawing.Color.MintCream;
            this.gbxSmtp.Controls.Add(this.lblSmtpNote);
            this.gbxSmtp.Controls.Add(this.chkShowSmtpPassword);
            this.gbxSmtp.Controls.Add(this.txtSmtpPassword);
            this.gbxSmtp.Controls.Add(this.lblSmtpPassword);
            this.gbxSmtp.Controls.Add(this.txtSmtpUser);
            this.gbxSmtp.Controls.Add(this.lblSmtpUser);
            this.gbxSmtp.Controls.Add(this.txtSmtpFrom);
            this.gbxSmtp.Controls.Add(this.lblSmtpFrom);
            this.gbxSmtp.Controls.Add(this.chkSmtpUseSsl);
            this.gbxSmtp.Controls.Add(this.nud_smtpPort);
            this.gbxSmtp.Controls.Add(this.lblSmtpPort);
            this.gbxSmtp.Controls.Add(this.txtSmtpHost);
            this.gbxSmtp.Controls.Add(this.lblSmtpHost);
            this.gbxSmtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbxSmtp.Location = new System.Drawing.Point(6, 192);
            this.gbxSmtp.Name = "gbxSmtp";
            this.gbxSmtp.Size = new System.Drawing.Size(380, 192);
            this.gbxSmtp.TabIndex = 5;
            this.gbxSmtp.TabStop = false;
            this.gbxSmtp.Text = "Outgoing Mail (SMTP)";
            //
            // lblSmtpHost
            //
            this.lblSmtpHost.AutoSize = true;
            this.lblSmtpHost.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSmtpHost.Location = new System.Drawing.Point(10, 29);
            this.lblSmtpHost.Name = "lblSmtpHost";
            this.lblSmtpHost.Size = new System.Drawing.Size(38, 13);
            this.lblSmtpHost.TabIndex = 0;
            this.lblSmtpHost.Text = "Host:";
            //
            // txtSmtpHost
            //
            this.txtSmtpHost.Location = new System.Drawing.Point(134, 26);
            this.txtSmtpHost.Name = "txtSmtpHost";
            this.txtSmtpHost.Size = new System.Drawing.Size(234, 22);
            this.txtSmtpHost.TabIndex = 1;
            //
            // lblSmtpPort
            //
            this.lblSmtpPort.AutoSize = true;
            this.lblSmtpPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSmtpPort.Location = new System.Drawing.Point(10, 59);
            this.lblSmtpPort.Name = "lblSmtpPort";
            this.lblSmtpPort.Size = new System.Drawing.Size(35, 13);
            this.lblSmtpPort.TabIndex = 2;
            this.lblSmtpPort.Text = "Port:";
            //
            // nud_smtpPort
            //
            this.nud_smtpPort.Location = new System.Drawing.Point(134, 56);
            this.nud_smtpPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.nud_smtpPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nud_smtpPort.Name = "nud_smtpPort";
            this.nud_smtpPort.Size = new System.Drawing.Size(70, 22);
            this.nud_smtpPort.TabIndex = 3;
            this.nud_smtpPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_smtpPort.Value = new decimal(new int[] { 587, 0, 0, 0 });
            //
            // chkSmtpUseSsl
            //
            this.chkSmtpUseSsl.AutoSize = true;
            this.chkSmtpUseSsl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkSmtpUseSsl.Location = new System.Drawing.Point(220, 58);
            this.chkSmtpUseSsl.Name = "chkSmtpUseSsl";
            this.chkSmtpUseSsl.Size = new System.Drawing.Size(107, 17);
            this.chkSmtpUseSsl.TabIndex = 4;
            this.chkSmtpUseSsl.Text = "Use SSL / TLS";
            this.chkSmtpUseSsl.UseVisualStyleBackColor = true;
            //
            // lblSmtpFrom
            //
            this.lblSmtpFrom.AutoSize = true;
            this.lblSmtpFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSmtpFrom.Location = new System.Drawing.Point(10, 89);
            this.lblSmtpFrom.Name = "lblSmtpFrom";
            this.lblSmtpFrom.Size = new System.Drawing.Size(38, 13);
            this.lblSmtpFrom.TabIndex = 5;
            this.lblSmtpFrom.Text = "From:";
            //
            // txtSmtpFrom
            //
            this.txtSmtpFrom.Location = new System.Drawing.Point(134, 86);
            this.txtSmtpFrom.Name = "txtSmtpFrom";
            this.txtSmtpFrom.Size = new System.Drawing.Size(234, 22);
            this.txtSmtpFrom.TabIndex = 6;
            //
            // lblSmtpUser
            //
            this.lblSmtpUser.AutoSize = true;
            this.lblSmtpUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSmtpUser.Location = new System.Drawing.Point(10, 119);
            this.lblSmtpUser.Name = "lblSmtpUser";
            this.lblSmtpUser.Size = new System.Drawing.Size(32, 13);
            this.lblSmtpUser.TabIndex = 7;
            this.lblSmtpUser.Text = "User:";
            //
            // txtSmtpUser
            //
            this.txtSmtpUser.Location = new System.Drawing.Point(134, 116);
            this.txtSmtpUser.Name = "txtSmtpUser";
            this.txtSmtpUser.Size = new System.Drawing.Size(234, 22);
            this.txtSmtpUser.TabIndex = 8;
            //
            // lblSmtpPassword
            //
            this.lblSmtpPassword.AutoSize = true;
            this.lblSmtpPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSmtpPassword.Location = new System.Drawing.Point(10, 149);
            this.lblSmtpPassword.Name = "lblSmtpPassword";
            this.lblSmtpPassword.Size = new System.Drawing.Size(57, 13);
            this.lblSmtpPassword.TabIndex = 9;
            this.lblSmtpPassword.Text = "Password:";
            //
            // txtSmtpPassword
            //
            this.txtSmtpPassword.Location = new System.Drawing.Point(134, 146);
            this.txtSmtpPassword.Name = "txtSmtpPassword";
            this.txtSmtpPassword.Size = new System.Drawing.Size(170, 22);
            this.txtSmtpPassword.TabIndex = 10;
            this.txtSmtpPassword.UseSystemPasswordChar = true;
            //
            // chkShowSmtpPassword
            //
            this.chkShowSmtpPassword.AutoSize = true;
            this.chkShowSmtpPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkShowSmtpPassword.Location = new System.Drawing.Point(314, 148);
            this.chkShowSmtpPassword.Name = "chkShowSmtpPassword";
            this.chkShowSmtpPassword.Size = new System.Drawing.Size(54, 17);
            this.chkShowSmtpPassword.TabIndex = 11;
            this.chkShowSmtpPassword.Text = "Show";
            this.chkShowSmtpPassword.UseVisualStyleBackColor = true;
            this.chkShowSmtpPassword.CheckedChanged += new System.EventHandler(this.chkShowSmtpPassword_CheckedChanged);
            //
            // lblSmtpNote
            //
            this.lblSmtpNote.AutoSize = true;
            this.lblSmtpNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblSmtpNote.ForeColor = System.Drawing.Color.Gray;
            this.lblSmtpNote.Location = new System.Drawing.Point(10, 172);
            this.lblSmtpNote.MaximumSize = new System.Drawing.Size(360, 0);
            this.lblSmtpNote.Name = "lblSmtpNote";
            this.lblSmtpNote.Size = new System.Drawing.Size(275, 12);
            this.lblSmtpNote.TabIndex = 12;
            this.lblSmtpNote.Text = "Used for both scheduled and manual report emails - no restart needed.";
            //
            // tabPageDataSync
            //
            this.tabPageDataSync.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageDataSync.Controls.Add(this.grpUpload);
            this.tabPageDataSync.Controls.Add(this.lblLastSync);
            this.tabPageDataSync.Controls.Add(this.lblSyncStatus);
            this.tabPageDataSync.Controls.Add(this.btnSyncNow);
            this.tabPageDataSync.Controls.Add(this.btnBrowseAccessDbPath);
            this.tabPageDataSync.Controls.Add(this.txtAccessDbPath);
            this.tabPageDataSync.Controls.Add(this.lblAccessPath);
            this.tabPageDataSync.Controls.Add(this.lblSyncHint);
            this.tabPageDataSync.Location = new System.Drawing.Point(4, 22);
            this.tabPageDataSync.Name = "tabPageDataSync";
            this.tabPageDataSync.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDataSync.Size = new System.Drawing.Size(392, 400);
            this.tabPageDataSync.TabIndex = 6;
            this.tabPageDataSync.Text = "Data Sync";
            //
            // lblSyncHint
            //
            this.lblSyncHint.AutoSize = true;
            this.lblSyncHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSyncHint.Location = new System.Drawing.Point(6, 6);
            this.lblSyncHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblSyncHint.Name = "lblSyncHint";
            this.lblSyncHint.Size = new System.Drawing.Size(370, 84);
            this.lblSyncHint.TabIndex = 0;
            this.lblSyncHint.Text = "\"Sync to Access\" copies items, users, sales, loyalty, petty cash, and activity-log data from the online database into a local Microsoft Access file for offline viewing or reporting; each run replaces the file\'s contents. \"Push Offline Items to Online\" sends item rows the other way: any serial already online gets its data updated, and any new serial gets added.";
            //
            // lblAccessPath
            //
            this.lblAccessPath.AutoSize = true;
            this.lblAccessPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblAccessPath.Location = new System.Drawing.Point(6, 95);
            this.lblAccessPath.Name = "lblAccessPath";
            this.lblAccessPath.Size = new System.Drawing.Size(66, 13);
            this.lblAccessPath.TabIndex = 1;
            this.lblAccessPath.Text = "Access File:";
            //
            // txtAccessDbPath
            //
            this.txtAccessDbPath.Location = new System.Drawing.Point(6, 112);
            this.txtAccessDbPath.Name = "txtAccessDbPath";
            this.txtAccessDbPath.Size = new System.Drawing.Size(300, 22);
            this.txtAccessDbPath.TabIndex = 2;
            //
            // btnBrowseAccessDbPath
            //
            this.btnBrowseAccessDbPath.Location = new System.Drawing.Point(312, 111);
            this.btnBrowseAccessDbPath.Name = "btnBrowseAccessDbPath";
            this.btnBrowseAccessDbPath.Size = new System.Drawing.Size(74, 24);
            this.btnBrowseAccessDbPath.TabIndex = 3;
            this.btnBrowseAccessDbPath.Text = "Browse...";
            this.btnBrowseAccessDbPath.UseVisualStyleBackColor = true;
            this.btnBrowseAccessDbPath.Click += new System.EventHandler(this.btnBrowseAccessDbPath_Click);
            //
            // btnSyncNow
            //
            this.btnSyncNow.Location = new System.Drawing.Point(6, 147);
            this.btnSyncNow.Name = "btnSyncNow";
            this.btnSyncNow.Size = new System.Drawing.Size(180, 32);
            this.btnSyncNow.TabIndex = 4;
            this.btnSyncNow.Text = "Sync to Access";
            this.btnSyncNow.UseVisualStyleBackColor = true;
            this.btnSyncNow.Click += new System.EventHandler(this.btnSyncNow_Click);
            //
            // lblSyncStatus
            //
            this.lblSyncStatus.AutoSize = true;
            this.lblSyncStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblSyncStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblSyncStatus.Location = new System.Drawing.Point(6, 187);
            this.lblSyncStatus.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblSyncStatus.Name = "lblSyncStatus";
            this.lblSyncStatus.Size = new System.Drawing.Size(0, 12);
            this.lblSyncStatus.TabIndex = 5;
            //
            // lblLastSync
            //
            this.lblLastSync.AutoSize = true;
            this.lblLastSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblLastSync.ForeColor = System.Drawing.Color.Gray;
            this.lblLastSync.Location = new System.Drawing.Point(6, 207);
            this.lblLastSync.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblLastSync.Name = "lblLastSync";
            this.lblLastSync.Size = new System.Drawing.Size(101, 12);
            this.lblLastSync.TabIndex = 6;
            this.lblLastSync.Text = "Last sync: never";
            //
            // btnPushToOnline
            //
            this.btnPushToOnline.Location = new System.Drawing.Point(10, 25);
            this.btnPushToOnline.Name = "btnPushToOnline";
            this.btnPushToOnline.Size = new System.Drawing.Size(220, 32);
            this.btnPushToOnline.TabIndex = 0;
            this.btnPushToOnline.Text = "Push Offline Items to Online";
            this.btnPushToOnline.UseVisualStyleBackColor = true;
            this.btnPushToOnline.Click += new System.EventHandler(this.btnPushToOnline_Click);
            //
            // lblPushStatus
            //
            this.lblPushStatus.AutoSize = true;
            this.lblPushStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblPushStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblPushStatus.Location = new System.Drawing.Point(10, 63);
            this.lblPushStatus.MaximumSize = new System.Drawing.Size(360, 0);
            this.lblPushStatus.Name = "lblPushStatus";
            this.lblPushStatus.Size = new System.Drawing.Size(0, 12);
            this.lblPushStatus.TabIndex = 1;
            //
            // lblLastPush
            //
            this.lblLastPush.AutoSize = true;
            this.lblLastPush.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblLastPush.ForeColor = System.Drawing.Color.Gray;
            this.lblLastPush.Location = new System.Drawing.Point(10, 83);
            this.lblLastPush.MaximumSize = new System.Drawing.Size(360, 0);
            this.lblLastPush.Name = "lblLastPush";
            this.lblLastPush.Size = new System.Drawing.Size(101, 12);
            this.lblLastPush.TabIndex = 2;
            this.lblLastPush.Text = "Last push: never";
            //
            // grpUpload
            //
            this.grpUpload.BackColor = System.Drawing.Color.White;
            this.grpUpload.Controls.Add(this.btnPushToOnline);
            this.grpUpload.Controls.Add(this.lblPushStatus);
            this.grpUpload.Controls.Add(this.lblLastPush);
            this.grpUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.grpUpload.Location = new System.Drawing.Point(6, 225);
            this.grpUpload.Name = "grpUpload";
            this.grpUpload.Size = new System.Drawing.Size(380, 108);
            this.grpUpload.TabIndex = 7;
            this.grpUpload.TabStop = false;
            this.grpUpload.Text = "Upload to Online Database";
            //
            // tabPageTrial
            //
            this.tabPageTrial.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageTrial.Controls.Add(this.lblTrialAutoCloseHint);
            this.tabPageTrial.Controls.Add(this.chkTrialAutoClose);
            this.tabPageTrial.Controls.Add(this.btnImportLicenseFile);
            this.tabPageTrial.Controls.Add(this.btnActivateLicense);
            this.tabPageTrial.Controls.Add(this.cmbSubscriptionTier);
            this.tabPageTrial.Controls.Add(this.lblSubscriptionTier);
            this.tabPageTrial.Controls.Add(this.txtLicenseKey);
            this.tabPageTrial.Controls.Add(this.lblLicenseKey);
            this.tabPageTrial.Controls.Add(this.lblTrialStatus);
            this.tabPageTrial.Controls.Add(this.nud_trialDays);
            this.tabPageTrial.Controls.Add(this.lblTrialDays);
            this.tabPageTrial.Controls.Add(this.chkTrialEnabled);
            this.tabPageTrial.Controls.Add(this.lblTrialHint);
            this.tabPageTrial.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrial.Name = "tabPageTrial";
            this.tabPageTrial.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTrial.Size = new System.Drawing.Size(392, 400);
            this.tabPageTrial.TabIndex = 7;
            this.tabPageTrial.Text = "Trial / Licensing";
            //
            // lblTrialHint
            //
            this.lblTrialHint.AutoSize = true;
            this.lblTrialHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblTrialHint.Location = new System.Drawing.Point(6, 6);
            this.lblTrialHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblTrialHint.Name = "lblTrialHint";
            this.lblTrialHint.Size = new System.Drawing.Size(370, 52);
            this.lblTrialHint.TabIndex = 0;
            this.lblTrialHint.Text = "Gates new installs behind a time-limited trial that starts the first time the a" +
    "pp runs on a machine. Enter the license key issued for this store\'s Store Cod" +
    "e to remove the gate permanently.";
            //
            // chkTrialEnabled
            //
            this.chkTrialEnabled.AutoSize = true;
            this.chkTrialEnabled.Location = new System.Drawing.Point(6, 64);
            this.chkTrialEnabled.Name = "chkTrialEnabled";
            this.chkTrialEnabled.Size = new System.Drawing.Size(150, 17);
            this.chkTrialEnabled.TabIndex = 1;
            this.chkTrialEnabled.Text = "Enable trial gate";
            this.chkTrialEnabled.UseVisualStyleBackColor = true;
            this.chkTrialEnabled.CheckedChanged += new System.EventHandler(this.chkTrialEnabled_CheckedChanged);
            //
            // lblTrialDays
            //
            this.lblTrialDays.AutoSize = true;
            this.lblTrialDays.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblTrialDays.Location = new System.Drawing.Point(6, 91);
            this.lblTrialDays.Name = "lblTrialDays";
            this.lblTrialDays.Size = new System.Drawing.Size(96, 13);
            this.lblTrialDays.TabIndex = 2;
            this.lblTrialDays.Text = "Trial length (days):";
            //
            // nud_trialDays
            //
            this.nud_trialDays.Location = new System.Drawing.Point(140, 89);
            this.nud_trialDays.Maximum = new decimal(new int[] {
            3650,
            0,
            0,
            0});
            this.nud_trialDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_trialDays.Name = "nud_trialDays";
            this.nud_trialDays.Size = new System.Drawing.Size(70, 22);
            this.nud_trialDays.TabIndex = 3;
            this.nud_trialDays.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            //
            // lblTrialStatus
            //
            this.lblTrialStatus.AutoSize = true;
            this.lblTrialStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblTrialStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblTrialStatus.Location = new System.Drawing.Point(6, 122);
            this.lblTrialStatus.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblTrialStatus.Name = "lblTrialStatus";
            this.lblTrialStatus.Size = new System.Drawing.Size(101, 24);
            this.lblTrialStatus.TabIndex = 4;
            //
            // lblLicenseKey
            //
            this.lblLicenseKey.AutoSize = true;
            this.lblLicenseKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblLicenseKey.Location = new System.Drawing.Point(6, 166);
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.lblLicenseKey.Size = new System.Drawing.Size(69, 13);
            this.lblLicenseKey.TabIndex = 5;
            this.lblLicenseKey.Text = "License key:";
            //
            // txtLicenseKey
            //
            this.txtLicenseKey.Location = new System.Drawing.Point(6, 183);
            this.txtLicenseKey.Name = "txtLicenseKey";
            this.txtLicenseKey.Size = new System.Drawing.Size(300, 22);
            this.txtLicenseKey.TabIndex = 6;
            //
            // lblSubscriptionTier
            //
            this.lblSubscriptionTier.AutoSize = true;
            this.lblSubscriptionTier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblSubscriptionTier.Location = new System.Drawing.Point(6, 212);
            this.lblSubscriptionTier.Name = "lblSubscriptionTier";
            this.lblSubscriptionTier.Size = new System.Drawing.Size(96, 13);
            this.lblSubscriptionTier.TabIndex = 7;
            this.lblSubscriptionTier.Text = "Subscription Tier:";
            //
            // cmbSubscriptionTier
            //
            // Only used to pick which Documents\App Inventory\Licenses\<Tier> subfolder
            // btnImportLicenseFile_Click opens into below - the tier itself comes from the
            // imported file's own Tier field (see TrialLicense.TryActivateFromFile).
            this.cmbSubscriptionTier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubscriptionTier.Items.AddRange(new object[] { "Basic", "Pro", "Enterprise" });
            this.cmbSubscriptionTier.Location = new System.Drawing.Point(120, 208);
            this.cmbSubscriptionTier.Name = "cmbSubscriptionTier";
            this.cmbSubscriptionTier.Size = new System.Drawing.Size(150, 23);
            this.cmbSubscriptionTier.TabIndex = 8;
            //
            // btnActivateLicense
            //
            this.btnActivateLicense.Location = new System.Drawing.Point(6, 245);
            this.btnActivateLicense.Name = "btnActivateLicense";
            this.btnActivateLicense.Size = new System.Drawing.Size(180, 30);
            this.btnActivateLicense.TabIndex = 9;
            this.btnActivateLicense.Text = "Activate";
            this.btnActivateLicense.UseVisualStyleBackColor = true;
            this.btnActivateLicense.Click += new System.EventHandler(this.btnActivateLicense_Click);
            //
            // btnImportLicenseFile
            //
            this.btnImportLicenseFile.Location = new System.Drawing.Point(192, 245);
            this.btnImportLicenseFile.Name = "btnImportLicenseFile";
            this.btnImportLicenseFile.Size = new System.Drawing.Size(180, 30);
            this.btnImportLicenseFile.TabIndex = 10;
            this.btnImportLicenseFile.Text = "Import License File...";
            this.btnImportLicenseFile.UseVisualStyleBackColor = true;
            this.btnImportLicenseFile.Click += new System.EventHandler(this.btnImportLicenseFile_Click);
            //
            // openFileDialogLicense
            //
            this.openFileDialogLicense.DefaultExt = "tmrgo";
            this.openFileDialogLicense.Filter = "License Key File (*.tmrgo)|*.tmrgo|All files (*.*)|*.*";
            this.openFileDialogLicense.Title = "Choose License Key File";
            //
            // chkTrialAutoClose
            //
            this.chkTrialAutoClose.AutoSize = true;
            this.chkTrialAutoClose.Location = new System.Drawing.Point(6, 285);
            this.chkTrialAutoClose.Name = "chkTrialAutoClose";
            this.chkTrialAutoClose.Size = new System.Drawing.Size(150, 17);
            this.chkTrialAutoClose.TabIndex = 11;
            this.chkTrialAutoClose.Text = "Auto-close app on expiry";
            this.chkTrialAutoClose.UseVisualStyleBackColor = true;
            //
            // lblTrialAutoCloseHint
            //
            this.lblTrialAutoCloseHint.AutoSize = true;
            this.lblTrialAutoCloseHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblTrialAutoCloseHint.ForeColor = System.Drawing.Color.Gray;
            this.lblTrialAutoCloseHint.Location = new System.Drawing.Point(6, 305);
            this.lblTrialAutoCloseHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblTrialAutoCloseHint.Name = "lblTrialAutoCloseHint";
            this.lblTrialAutoCloseHint.Size = new System.Drawing.Size(101, 24);
            this.lblTrialAutoCloseHint.TabIndex = 12;
            this.lblTrialAutoCloseHint.Text = "If checked, a terminal already running will close itself as soon as the trial o" +
    "r license expires, instead of waiting for the next restart.";
            //
            // tabPageDbSetup
            //
            this.tabPageDbSetup.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageDbSetup.Controls.Add(this.lblDbSetupStatus);
            this.tabPageDbSetup.Controls.Add(this.btnCreateDatabase);
            this.tabPageDbSetup.Controls.Add(this.chkSwitchToNewDb);
            this.tabPageDbSetup.Controls.Add(this.txtNewDbName);
            this.tabPageDbSetup.Controls.Add(this.lblNewDbName);
            this.tabPageDbSetup.Controls.Add(this.lblDbSetupHint);
            this.tabPageDbSetup.Location = new System.Drawing.Point(4, 22);
            this.tabPageDbSetup.Name = "tabPageDbSetup";
            this.tabPageDbSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDbSetup.Size = new System.Drawing.Size(392, 400);
            this.tabPageDbSetup.TabIndex = 8;
            this.tabPageDbSetup.Text = "Database Setup";
            //
            // lblDbSetupHint
            //
            this.lblDbSetupHint.AutoSize = true;
            this.lblDbSetupHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblDbSetupHint.Location = new System.Drawing.Point(6, 6);
            this.lblDbSetupHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblDbSetupHint.Name = "lblDbSetupHint";
            this.lblDbSetupHint.Size = new System.Drawing.Size(370, 91);
            this.lblDbSetupHint.TabIndex = 0;
            this.lblDbSetupHint.Text = "Creates the database (if it doesn\'t already exist) and every table this app ne" +
    "eds - items_, user_, sales_, customers_, loyalty_transactions_, pettycash_, an" +
    "d logs_ - on the MySQL server configured in the Settings tab. Existing tables" +
    " and data are left untouched, so it\'s safe to run against a database that\'s a" +
    "lready partially set up.";
            //
            // lblNewDbName
            //
            this.lblNewDbName.AutoSize = true;
            this.lblNewDbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblNewDbName.Location = new System.Drawing.Point(6, 104);
            this.lblNewDbName.Name = "lblNewDbName";
            this.lblNewDbName.Size = new System.Drawing.Size(120, 13);
            this.lblNewDbName.TabIndex = 1;
            this.lblNewDbName.Text = "Database name to create:";
            //
            // txtNewDbName
            //
            this.txtNewDbName.Location = new System.Drawing.Point(6, 121);
            this.txtNewDbName.Name = "txtNewDbName";
            this.txtNewDbName.Size = new System.Drawing.Size(300, 22);
            this.txtNewDbName.TabIndex = 2;
            //
            // chkSwitchToNewDb
            //
            this.chkSwitchToNewDb.AutoSize = true;
            this.chkSwitchToNewDb.Checked = true;
            this.chkSwitchToNewDb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSwitchToNewDb.Location = new System.Drawing.Point(6, 153);
            this.chkSwitchToNewDb.Name = "chkSwitchToNewDb";
            this.chkSwitchToNewDb.Size = new System.Drawing.Size(260, 17);
            this.chkSwitchToNewDb.TabIndex = 3;
            this.chkSwitchToNewDb.Text = "Switch this app to use it after creating";
            this.chkSwitchToNewDb.UseVisualStyleBackColor = true;
            //
            // btnCreateDatabase
            //
            this.btnCreateDatabase.Location = new System.Drawing.Point(6, 185);
            this.btnCreateDatabase.Name = "btnCreateDatabase";
            this.btnCreateDatabase.Size = new System.Drawing.Size(220, 32);
            this.btnCreateDatabase.TabIndex = 4;
            this.btnCreateDatabase.Text = "Create Database & Tables";
            this.btnCreateDatabase.UseVisualStyleBackColor = true;
            this.btnCreateDatabase.Click += new System.EventHandler(this.btnCreateDatabase_Click);
            //
            // lblDbSetupStatus
            //
            this.lblDbSetupStatus.AutoSize = true;
            this.lblDbSetupStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblDbSetupStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblDbSetupStatus.Location = new System.Drawing.Point(6, 225);
            this.lblDbSetupStatus.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblDbSetupStatus.Name = "lblDbSetupStatus";
            this.lblDbSetupStatus.Size = new System.Drawing.Size(0, 12);
            this.lblDbSetupStatus.TabIndex = 5;
            //
            // tabPageRemoteControl
            //
            this.tabPageRemoteControl.AutoScroll = true;
            this.tabPageRemoteControl.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageRemoteControl.Controls.Add(this.lblBluetoothRadioInfo);
            this.tabPageRemoteControl.Controls.Add(this.chkBluetoothEnabled);
            this.tabPageRemoteControl.Controls.Add(this.lblBluetoothHeader);
            this.tabPageRemoteControl.Controls.Add(this.btnCheckPublicIp);
            this.tabPageRemoteControl.Controls.Add(this.txtPublicIp);
            this.tabPageRemoteControl.Controls.Add(this.lblPublicIp);
            this.tabPageRemoteControl.Controls.Add(this.btnCopyThumbprint);
            this.tabPageRemoteControl.Controls.Add(this.txtCertThumbprint);
            this.tabPageRemoteControl.Controls.Add(this.lblCertThumbprintHeader);
            this.tabPageRemoteControl.Controls.Add(this.btnRefreshIp);
            this.tabPageRemoteControl.Controls.Add(this.lblRemoteIpAddress);
            this.tabPageRemoteControl.Controls.Add(this.lblRemoteStatus);
            this.tabPageRemoteControl.Controls.Add(this.btnGenerateRemoteToken);
            this.tabPageRemoteControl.Controls.Add(this.chkShowRemoteToken);
            this.tabPageRemoteControl.Controls.Add(this.txtRemoteToken);
            this.tabPageRemoteControl.Controls.Add(this.lblRemoteToken);
            this.tabPageRemoteControl.Controls.Add(this.nud_remotePort);
            this.tabPageRemoteControl.Controls.Add(this.lblRemotePort);
            this.tabPageRemoteControl.Controls.Add(this.chkRemoteControlEnabled);
            this.tabPageRemoteControl.Controls.Add(this.lblRemoteHint);
            this.tabPageRemoteControl.Location = new System.Drawing.Point(4, 22);
            this.tabPageRemoteControl.Name = "tabPageRemoteControl";
            this.tabPageRemoteControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRemoteControl.Size = new System.Drawing.Size(392, 400);
            this.tabPageRemoteControl.TabIndex = 9;
            this.tabPageRemoteControl.Text = "Remote Control";
            //
            // lblRemoteHint
            //
            this.lblRemoteHint.AutoSize = true;
            this.lblRemoteHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblRemoteHint.Location = new System.Drawing.Point(6, 6);
            this.lblRemoteHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblRemoteHint.Name = "lblRemoteHint";
            this.lblRemoteHint.Size = new System.Drawing.Size(370, 91);
            this.lblRemoteHint.TabIndex = 0;
            this.lblRemoteHint.Text = "Lets the standalone RemoteController companion app check this terminal\'s statu" +
    "s and push a license key to it over the network - a health check and license " +
    "activation only, nothing that touches sales or item data. Intended for a trus" +
    "ted local network only; do not expose this port to the open internet.";
            //
            // lblRemoteIpAddress
            //
            this.lblRemoteIpAddress.AutoSize = true;
            this.lblRemoteIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblRemoteIpAddress.Location = new System.Drawing.Point(6, 101);
            this.lblRemoteIpAddress.MaximumSize = new System.Drawing.Size(300, 0);
            this.lblRemoteIpAddress.Name = "lblRemoteIpAddress";
            this.lblRemoteIpAddress.Size = new System.Drawing.Size(101, 26);
            this.lblRemoteIpAddress.TabIndex = 1;
            //
            // btnRefreshIp
            //
            this.btnRefreshIp.Location = new System.Drawing.Point(312, 98);
            this.btnRefreshIp.Name = "btnRefreshIp";
            this.btnRefreshIp.Size = new System.Drawing.Size(70, 24);
            this.btnRefreshIp.TabIndex = 2;
            this.btnRefreshIp.Text = "Refresh";
            this.btnRefreshIp.UseVisualStyleBackColor = true;
            this.btnRefreshIp.Click += new System.EventHandler(this.btnRefreshIp_Click);
            //
            // chkRemoteControlEnabled
            //
            this.chkRemoteControlEnabled.AutoSize = true;
            this.chkRemoteControlEnabled.Location = new System.Drawing.Point(6, 140);
            this.chkRemoteControlEnabled.Name = "chkRemoteControlEnabled";
            this.chkRemoteControlEnabled.Size = new System.Drawing.Size(150, 17);
            this.chkRemoteControlEnabled.TabIndex = 3;
            this.chkRemoteControlEnabled.Text = "Enable remote control";
            this.chkRemoteControlEnabled.UseVisualStyleBackColor = true;
            //
            // lblRemotePort
            //
            this.lblRemotePort.AutoSize = true;
            this.lblRemotePort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblRemotePort.Location = new System.Drawing.Point(6, 167);
            this.lblRemotePort.Name = "lblRemotePort";
            this.lblRemotePort.Size = new System.Drawing.Size(32, 13);
            this.lblRemotePort.TabIndex = 4;
            this.lblRemotePort.Text = "Port:";
            //
            // nud_remotePort
            //
            this.nud_remotePort.Location = new System.Drawing.Point(140, 165);
            this.nud_remotePort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nud_remotePort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nud_remotePort.Name = "nud_remotePort";
            this.nud_remotePort.Size = new System.Drawing.Size(80, 22);
            this.nud_remotePort.TabIndex = 5;
            this.nud_remotePort.Value = new decimal(new int[] {
            51820,
            0,
            0,
            0});
            this.nud_remotePort.ValueChanged += new System.EventHandler(this.nud_remotePort_ValueChanged);
            //
            // lblRemoteToken
            //
            this.lblRemoteToken.AutoSize = true;
            this.lblRemoteToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblRemoteToken.Location = new System.Drawing.Point(6, 200);
            this.lblRemoteToken.Name = "lblRemoteToken";
            this.lblRemoteToken.Size = new System.Drawing.Size(76, 13);
            this.lblRemoteToken.TabIndex = 6;
            this.lblRemoteToken.Text = "Shared token:";
            //
            // txtRemoteToken
            //
            this.txtRemoteToken.Location = new System.Drawing.Point(6, 217);
            this.txtRemoteToken.Name = "txtRemoteToken";
            this.txtRemoteToken.Size = new System.Drawing.Size(220, 22);
            this.txtRemoteToken.TabIndex = 7;
            this.txtRemoteToken.UseSystemPasswordChar = true;
            //
            // chkShowRemoteToken
            //
            this.chkShowRemoteToken.AutoSize = true;
            this.chkShowRemoteToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.chkShowRemoteToken.Location = new System.Drawing.Point(232, 220);
            this.chkShowRemoteToken.Name = "chkShowRemoteToken";
            this.chkShowRemoteToken.Size = new System.Drawing.Size(54, 17);
            this.chkShowRemoteToken.TabIndex = 8;
            this.chkShowRemoteToken.Text = "Show";
            this.chkShowRemoteToken.UseVisualStyleBackColor = true;
            this.chkShowRemoteToken.CheckedChanged += new System.EventHandler(this.chkShowRemoteToken_CheckedChanged);
            //
            // btnGenerateRemoteToken
            //
            this.btnGenerateRemoteToken.Location = new System.Drawing.Point(6, 250);
            this.btnGenerateRemoteToken.Name = "btnGenerateRemoteToken";
            this.btnGenerateRemoteToken.Size = new System.Drawing.Size(160, 30);
            this.btnGenerateRemoteToken.TabIndex = 9;
            this.btnGenerateRemoteToken.Text = "Generate Token";
            this.btnGenerateRemoteToken.UseVisualStyleBackColor = true;
            this.btnGenerateRemoteToken.Click += new System.EventHandler(this.btnGenerateRemoteToken_Click);
            //
            // lblRemoteStatus
            //
            this.lblRemoteStatus.AutoSize = true;
            this.lblRemoteStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblRemoteStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblRemoteStatus.Location = new System.Drawing.Point(6, 290);
            this.lblRemoteStatus.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblRemoteStatus.Name = "lblRemoteStatus";
            this.lblRemoteStatus.Size = new System.Drawing.Size(101, 12);
            this.lblRemoteStatus.TabIndex = 10;
            //
            // lblCertThumbprintHeader
            //
            this.lblCertThumbprintHeader.AutoSize = true;
            this.lblCertThumbprintHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblCertThumbprintHeader.Location = new System.Drawing.Point(6, 315);
            this.lblCertThumbprintHeader.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblCertThumbprintHeader.Name = "lblCertThumbprintHeader";
            this.lblCertThumbprintHeader.Size = new System.Drawing.Size(300, 13);
            this.lblCertThumbprintHeader.TabIndex = 11;
            this.lblCertThumbprintHeader.Text = "Certificate thumbprint (for pinning in RemoteController):";
            //
            // txtCertThumbprint
            //
            this.txtCertThumbprint.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtCertThumbprint.Location = new System.Drawing.Point(6, 332);
            this.txtCertThumbprint.Name = "txtCertThumbprint";
            this.txtCertThumbprint.ReadOnly = true;
            this.txtCertThumbprint.Size = new System.Drawing.Size(300, 21);
            this.txtCertThumbprint.TabIndex = 12;
            //
            // btnCopyThumbprint
            //
            this.btnCopyThumbprint.Location = new System.Drawing.Point(312, 331);
            this.btnCopyThumbprint.Name = "btnCopyThumbprint";
            this.btnCopyThumbprint.Size = new System.Drawing.Size(64, 23);
            this.btnCopyThumbprint.TabIndex = 13;
            this.btnCopyThumbprint.Text = "Copy";
            this.btnCopyThumbprint.UseVisualStyleBackColor = true;
            this.btnCopyThumbprint.Click += new System.EventHandler(this.btnCopyThumbprint_Click);
            //
            // lblPublicIp
            //
            this.lblPublicIp.AutoSize = true;
            this.lblPublicIp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblPublicIp.Location = new System.Drawing.Point(6, 365);
            this.lblPublicIp.Name = "lblPublicIp";
            this.lblPublicIp.Size = new System.Drawing.Size(58, 13);
            this.lblPublicIp.TabIndex = 14;
            this.lblPublicIp.Text = "Public IP:";
            //
            // txtPublicIp
            //
            this.txtPublicIp.Location = new System.Drawing.Point(70, 362);
            this.txtPublicIp.Name = "txtPublicIp";
            this.txtPublicIp.ReadOnly = true;
            this.txtPublicIp.Size = new System.Drawing.Size(220, 22);
            this.txtPublicIp.TabIndex = 15;
            //
            // btnCheckPublicIp
            //
            this.btnCheckPublicIp.Location = new System.Drawing.Point(298, 361);
            this.btnCheckPublicIp.Name = "btnCheckPublicIp";
            this.btnCheckPublicIp.Size = new System.Drawing.Size(78, 24);
            this.btnCheckPublicIp.TabIndex = 16;
            this.btnCheckPublicIp.Text = "Check";
            this.btnCheckPublicIp.UseVisualStyleBackColor = true;
            this.btnCheckPublicIp.Click += new System.EventHandler(this.btnCheckPublicIp_Click);
            //
            // lblBluetoothHeader
            //
            this.lblBluetoothHeader.AutoSize = true;
            this.lblBluetoothHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblBluetoothHeader.Location = new System.Drawing.Point(6, 400);
            this.lblBluetoothHeader.Name = "lblBluetoothHeader";
            this.lblBluetoothHeader.Size = new System.Drawing.Size(66, 15);
            this.lblBluetoothHeader.TabIndex = 17;
            this.lblBluetoothHeader.Text = "Bluetooth";
            //
            // chkBluetoothEnabled
            //
            this.chkBluetoothEnabled.AutoSize = true;
            this.chkBluetoothEnabled.Location = new System.Drawing.Point(6, 422);
            this.chkBluetoothEnabled.Name = "chkBluetoothEnabled";
            this.chkBluetoothEnabled.Size = new System.Drawing.Size(160, 17);
            this.chkBluetoothEnabled.TabIndex = 18;
            this.chkBluetoothEnabled.Text = "Enable Bluetooth control";
            this.chkBluetoothEnabled.UseVisualStyleBackColor = true;
            //
            // lblBluetoothRadioInfo
            //
            this.lblBluetoothRadioInfo.AutoSize = true;
            this.lblBluetoothRadioInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblBluetoothRadioInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblBluetoothRadioInfo.Location = new System.Drawing.Point(6, 445);
            this.lblBluetoothRadioInfo.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblBluetoothRadioInfo.Name = "lblBluetoothRadioInfo";
            this.lblBluetoothRadioInfo.Size = new System.Drawing.Size(101, 12);
            this.lblBluetoothRadioInfo.TabIndex = 19;
            //
            // tabPageMaintenance
            //
            this.tabPageMaintenance.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageMaintenance.Controls.Add(this.lblMaintenanceStatus);
            this.tabPageMaintenance.Controls.Add(this.btnRefreshMaintenance);
            this.tabPageMaintenance.Controls.Add(this.btnSaveMaintenance);
            this.tabPageMaintenance.Controls.Add(this.txtMaintenanceMessage);
            this.tabPageMaintenance.Controls.Add(this.lblMaintenanceMessage);
            this.tabPageMaintenance.Controls.Add(this.chkMaintenanceEnabled);
            this.tabPageMaintenance.Controls.Add(this.lblMaintenanceHint);
            this.tabPageMaintenance.Location = new System.Drawing.Point(4, 22);
            this.tabPageMaintenance.Name = "tabPageMaintenance";
            this.tabPageMaintenance.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaintenance.Size = new System.Drawing.Size(392, 400);
            this.tabPageMaintenance.TabIndex = 10;
            this.tabPageMaintenance.Text = "Maintenance";
            //
            // lblMaintenanceHint
            //
            this.lblMaintenanceHint.AutoSize = true;
            this.lblMaintenanceHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblMaintenanceHint.Location = new System.Drawing.Point(6, 6);
            this.lblMaintenanceHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblMaintenanceHint.Name = "lblMaintenanceHint";
            this.lblMaintenanceHint.Size = new System.Drawing.Size(370, 52);
            this.lblMaintenanceHint.TabIndex = 0;
            this.lblMaintenanceHint.Text = "When enabled, cashiers are blocked at login with a maintenance notice on every " +
    "terminal that shares this database - including ones already running. A System" +
    " Admin PIN can still get in.";
            //
            // chkMaintenanceEnabled
            //
            this.chkMaintenanceEnabled.AutoSize = true;
            this.chkMaintenanceEnabled.Location = new System.Drawing.Point(6, 70);
            this.chkMaintenanceEnabled.Name = "chkMaintenanceEnabled";
            this.chkMaintenanceEnabled.Size = new System.Drawing.Size(160, 17);
            this.chkMaintenanceEnabled.TabIndex = 1;
            this.chkMaintenanceEnabled.Text = "Enable maintenance mode";
            this.chkMaintenanceEnabled.UseVisualStyleBackColor = true;
            //
            // lblMaintenanceMessage
            //
            this.lblMaintenanceMessage.AutoSize = true;
            this.lblMaintenanceMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblMaintenanceMessage.Location = new System.Drawing.Point(6, 97);
            this.lblMaintenanceMessage.Name = "lblMaintenanceMessage";
            this.lblMaintenanceMessage.Size = new System.Drawing.Size(150, 13);
            this.lblMaintenanceMessage.TabIndex = 2;
            this.lblMaintenanceMessage.Text = "Message shown to cashiers:";
            //
            // txtMaintenanceMessage
            //
            this.txtMaintenanceMessage.Location = new System.Drawing.Point(6, 114);
            this.txtMaintenanceMessage.Multiline = true;
            this.txtMaintenanceMessage.Name = "txtMaintenanceMessage";
            this.txtMaintenanceMessage.Size = new System.Drawing.Size(370, 70);
            this.txtMaintenanceMessage.TabIndex = 3;
            //
            // btnSaveMaintenance
            //
            this.btnSaveMaintenance.Location = new System.Drawing.Point(6, 192);
            this.btnSaveMaintenance.Name = "btnSaveMaintenance";
            this.btnSaveMaintenance.Size = new System.Drawing.Size(180, 30);
            this.btnSaveMaintenance.TabIndex = 4;
            this.btnSaveMaintenance.Text = "Save Maintenance Setting";
            this.btnSaveMaintenance.UseVisualStyleBackColor = true;
            this.btnSaveMaintenance.Click += new System.EventHandler(this.btnSaveMaintenance_Click);
            //
            // btnRefreshMaintenance
            //
            this.btnRefreshMaintenance.Location = new System.Drawing.Point(192, 192);
            this.btnRefreshMaintenance.Name = "btnRefreshMaintenance";
            this.btnRefreshMaintenance.Size = new System.Drawing.Size(180, 30);
            this.btnRefreshMaintenance.TabIndex = 5;
            this.btnRefreshMaintenance.Text = "Refresh Status";
            this.btnRefreshMaintenance.UseVisualStyleBackColor = true;
            this.btnRefreshMaintenance.Click += new System.EventHandler(this.btnRefreshMaintenance_Click);
            //
            // lblMaintenanceStatus
            //
            this.lblMaintenanceStatus.AutoSize = true;
            this.lblMaintenanceStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblMaintenanceStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblMaintenanceStatus.Location = new System.Drawing.Point(6, 230);
            this.lblMaintenanceStatus.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblMaintenanceStatus.Name = "lblMaintenanceStatus";
            this.lblMaintenanceStatus.Size = new System.Drawing.Size(101, 24);
            this.lblMaintenanceStatus.TabIndex = 6;
            //
            // tabPageLowStock
            //
            this.tabPageLowStock.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageLowStock.Controls.Add(this.lblLowStockStatus);
            this.tabPageLowStock.Controls.Add(this.btnRefreshLowStock);
            this.tabPageLowStock.Controls.Add(this.btnSaveLowStock);
            this.tabPageLowStock.Controls.Add(this.nudLowStockInterval);
            this.tabPageLowStock.Controls.Add(this.lblLowStockInterval);
            this.tabPageLowStock.Controls.Add(this.nudLowStockThreshold);
            this.tabPageLowStock.Controls.Add(this.lblLowStockThreshold);
            this.tabPageLowStock.Controls.Add(this.chkLowStockEnabled);
            this.tabPageLowStock.Controls.Add(this.lblLowStockHint);
            this.tabPageLowStock.Location = new System.Drawing.Point(4, 22);
            this.tabPageLowStock.Name = "tabPageLowStock";
            this.tabPageLowStock.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLowStock.Size = new System.Drawing.Size(392, 400);
            this.tabPageLowStock.TabIndex = 11;
            this.tabPageLowStock.Text = "Low Stock Reminder";
            //
            // lblLowStockHint
            //
            this.lblLowStockHint.AutoSize = true;
            this.lblLowStockHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblLowStockHint.Location = new System.Drawing.Point(6, 6);
            this.lblLowStockHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblLowStockHint.Name = "lblLowStockHint";
            this.lblLowStockHint.Size = new System.Drawing.Size(370, 52);
            this.lblLowStockHint.TabIndex = 0;
            this.lblLowStockHint.Text = "When enabled, a centered popup lists items at or below the threshold below, on " +
    "a timer, on every terminal that shares this database.";
            //
            // chkLowStockEnabled
            //
            this.chkLowStockEnabled.AutoSize = true;
            this.chkLowStockEnabled.Location = new System.Drawing.Point(6, 70);
            this.chkLowStockEnabled.Name = "chkLowStockEnabled";
            this.chkLowStockEnabled.Size = new System.Drawing.Size(160, 17);
            this.chkLowStockEnabled.TabIndex = 1;
            this.chkLowStockEnabled.Text = "Enable low stock reminder";
            this.chkLowStockEnabled.UseVisualStyleBackColor = true;
            //
            // lblLowStockThreshold
            //
            this.lblLowStockThreshold.AutoSize = true;
            this.lblLowStockThreshold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblLowStockThreshold.Location = new System.Drawing.Point(6, 97);
            this.lblLowStockThreshold.Name = "lblLowStockThreshold";
            this.lblLowStockThreshold.Size = new System.Drawing.Size(150, 13);
            this.lblLowStockThreshold.TabIndex = 2;
            this.lblLowStockThreshold.Text = "Remind when quantity is at or below:";
            //
            // nudLowStockThreshold
            //
            this.nudLowStockThreshold.Location = new System.Drawing.Point(6, 114);
            this.nudLowStockThreshold.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudLowStockThreshold.Name = "nudLowStockThreshold";
            this.nudLowStockThreshold.Size = new System.Drawing.Size(120, 20);
            this.nudLowStockThreshold.TabIndex = 3;
            this.nudLowStockThreshold.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            //
            // lblLowStockInterval
            //
            this.lblLowStockInterval.AutoSize = true;
            this.lblLowStockInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblLowStockInterval.Location = new System.Drawing.Point(6, 145);
            this.lblLowStockInterval.Name = "lblLowStockInterval";
            this.lblLowStockInterval.Size = new System.Drawing.Size(150, 13);
            this.lblLowStockInterval.TabIndex = 4;
            this.lblLowStockInterval.Text = "Remind again every (minutes):";
            //
            // nudLowStockInterval
            //
            this.nudLowStockInterval.Location = new System.Drawing.Point(6, 162);
            this.nudLowStockInterval.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.nudLowStockInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowStockInterval.Name = "nudLowStockInterval";
            this.nudLowStockInterval.Size = new System.Drawing.Size(120, 20);
            this.nudLowStockInterval.TabIndex = 5;
            this.nudLowStockInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            //
            // btnSaveLowStock
            //
            this.btnSaveLowStock.Location = new System.Drawing.Point(6, 192);
            this.btnSaveLowStock.Name = "btnSaveLowStock";
            this.btnSaveLowStock.Size = new System.Drawing.Size(180, 30);
            this.btnSaveLowStock.TabIndex = 6;
            this.btnSaveLowStock.Text = "Save Low Stock Setting";
            this.btnSaveLowStock.UseVisualStyleBackColor = true;
            this.btnSaveLowStock.Click += new System.EventHandler(this.btnSaveLowStock_Click);
            //
            // btnRefreshLowStock
            //
            this.btnRefreshLowStock.Location = new System.Drawing.Point(192, 192);
            this.btnRefreshLowStock.Name = "btnRefreshLowStock";
            this.btnRefreshLowStock.Size = new System.Drawing.Size(180, 30);
            this.btnRefreshLowStock.TabIndex = 7;
            this.btnRefreshLowStock.Text = "Refresh Status";
            this.btnRefreshLowStock.UseVisualStyleBackColor = true;
            this.btnRefreshLowStock.Click += new System.EventHandler(this.btnRefreshLowStock_Click);
            //
            // lblLowStockStatus
            //
            this.lblLowStockStatus.AutoSize = true;
            this.lblLowStockStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblLowStockStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblLowStockStatus.Location = new System.Drawing.Point(6, 230);
            this.lblLowStockStatus.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblLowStockStatus.Name = "lblLowStockStatus";
            this.lblLowStockStatus.Size = new System.Drawing.Size(101, 24);
            this.lblLowStockStatus.TabIndex = 8;
            //
            // tabPageReceiptTesting
            //
            this.tabPageReceiptTesting.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageReceiptTesting.Controls.Add(this.btnPreviewReceipt);
            this.tabPageReceiptTesting.Controls.Add(this.lblPriceOverrideValue);
            this.tabPageReceiptTesting.Controls.Add(this.tbPriceOverride);
            this.tabPageReceiptTesting.Controls.Add(this.chkPriceOverrideEnabled);
            this.tabPageReceiptTesting.Controls.Add(this.lblReceiptTestingHint);
            this.tabPageReceiptTesting.Location = new System.Drawing.Point(4, 22);
            this.tabPageReceiptTesting.Name = "tabPageReceiptTesting";
            this.tabPageReceiptTesting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReceiptTesting.Size = new System.Drawing.Size(392, 400);
            this.tabPageReceiptTesting.TabIndex = 11;
            this.tabPageReceiptTesting.Text = "Receipt Testing";
            //
            // lblReceiptTestingHint
            //
            this.lblReceiptTestingHint.AutoSize = true;
            this.lblReceiptTestingHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblReceiptTestingHint.Location = new System.Drawing.Point(6, 6);
            this.lblReceiptTestingHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblReceiptTestingHint.Name = "lblReceiptTestingHint";
            this.lblReceiptTestingHint.Size = new System.Drawing.Size(370, 39);
            this.lblReceiptTestingHint.TabIndex = 0;
            this.lblReceiptTestingHint.Text = "Testing-only, not saved: while enabled, every item on a printed or previewed r" +
    "eceipt shows this price instead of its real one, so you can check how the lay" +
    "out holds up with very small or very large numbers.";
            //
            // chkPriceOverrideEnabled
            //
            this.chkPriceOverrideEnabled.AutoSize = true;
            this.chkPriceOverrideEnabled.Location = new System.Drawing.Point(6, 60);
            this.chkPriceOverrideEnabled.Name = "chkPriceOverrideEnabled";
            this.chkPriceOverrideEnabled.Size = new System.Drawing.Size(230, 17);
            this.chkPriceOverrideEnabled.TabIndex = 1;
            this.chkPriceOverrideEnabled.Text = "Override printed item price for testing";
            this.chkPriceOverrideEnabled.UseVisualStyleBackColor = true;
            this.chkPriceOverrideEnabled.CheckedChanged += new System.EventHandler(this.chkPriceOverrideEnabled_CheckedChanged);
            //
            // tbPriceOverride
            //
            this.tbPriceOverride.LargeChange = 10000;
            this.tbPriceOverride.Location = new System.Drawing.Point(6, 86);
            this.tbPriceOverride.Maximum = 999999;
            this.tbPriceOverride.Minimum = 1;
            this.tbPriceOverride.Name = "tbPriceOverride";
            this.tbPriceOverride.Size = new System.Drawing.Size(370, 45);
            this.tbPriceOverride.SmallChange = 1;
            this.tbPriceOverride.TabIndex = 2;
            this.tbPriceOverride.TickFrequency = 50000;
            this.tbPriceOverride.Value = 1;
            this.tbPriceOverride.Scroll += new System.EventHandler(this.tbPriceOverride_Scroll);
            //
            // lblPriceOverrideValue
            //
            this.lblPriceOverrideValue.AutoSize = true;
            this.lblPriceOverrideValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPriceOverrideValue.Location = new System.Drawing.Point(6, 134);
            this.lblPriceOverrideValue.Name = "lblPriceOverrideValue";
            this.lblPriceOverrideValue.Size = new System.Drawing.Size(120, 13);
            this.lblPriceOverrideValue.TabIndex = 3;
            this.lblPriceOverrideValue.Text = "Override price: 1.00";
            //
            // btnPreviewReceipt
            //
            this.btnPreviewReceipt.Location = new System.Drawing.Point(6, 160);
            this.btnPreviewReceipt.Name = "btnPreviewReceipt";
            this.btnPreviewReceipt.Size = new System.Drawing.Size(180, 32);
            this.btnPreviewReceipt.TabIndex = 4;
            this.btnPreviewReceipt.Text = "Preview Receipt...";
            this.btnPreviewReceipt.UseVisualStyleBackColor = true;
            this.btnPreviewReceipt.Click += new System.EventHandler(this.btnPreviewReceipt_Click);
            //
            // tabPageItemSearch
            //
            this.tabPageItemSearch.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageItemSearch.Controls.Add(this.clbItemSearchColumns);
            this.tabPageItemSearch.Controls.Add(this.btnItemSearchColumnUp);
            this.tabPageItemSearch.Controls.Add(this.btnItemSearchColumnDown);
            this.tabPageItemSearch.Controls.Add(this.chkItemSearchDateSortDescending);
            this.tabPageItemSearch.Controls.Add(this.chkItemSearchNameSortDescending);
            this.tabPageItemSearch.Controls.Add(this.lblItemSearchHint);
            this.tabPageItemSearch.Location = new System.Drawing.Point(4, 22);
            this.tabPageItemSearch.Name = "tabPageItemSearch";
            this.tabPageItemSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageItemSearch.Size = new System.Drawing.Size(392, 400);
            this.tabPageItemSearch.TabIndex = 12;
            this.tabPageItemSearch.Text = "Item Search";
            //
            // lblItemSearchHint
            //
            this.lblItemSearchHint.AutoSize = true;
            this.lblItemSearchHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblItemSearchHint.Location = new System.Drawing.Point(6, 6);
            this.lblItemSearchHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblItemSearchHint.Name = "lblItemSearchHint";
            this.lblItemSearchHint.Size = new System.Drawing.Size(300, 26);
            this.lblItemSearchHint.TabIndex = 0;
            this.lblItemSearchHint.Text = "Uncheck a column to hide it, or select one and use Move Up/Down to reorder it. " +
    "The checkboxes below set sort direction for the item search results on the mai" +
    "n POS screen.";
            //
            // clbItemSearchColumns
            //
            this.clbItemSearchColumns.CheckOnClick = true;
            this.clbItemSearchColumns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.clbItemSearchColumns.FormattingEnabled = true;
            this.clbItemSearchColumns.IntegralHeight = false;
            this.clbItemSearchColumns.Location = new System.Drawing.Point(6, 40);
            this.clbItemSearchColumns.Name = "clbItemSearchColumns";
            this.clbItemSearchColumns.Size = new System.Drawing.Size(300, 290);
            this.clbItemSearchColumns.TabIndex = 1;
            //
            // btnItemSearchColumnUp
            //
            this.btnItemSearchColumnUp.Location = new System.Drawing.Point(312, 40);
            this.btnItemSearchColumnUp.Name = "btnItemSearchColumnUp";
            this.btnItemSearchColumnUp.Size = new System.Drawing.Size(74, 30);
            this.btnItemSearchColumnUp.TabIndex = 2;
            this.btnItemSearchColumnUp.Text = "Move Up";
            this.btnItemSearchColumnUp.UseVisualStyleBackColor = true;
            this.btnItemSearchColumnUp.Click += new System.EventHandler(this.btnItemSearchColumnUp_Click);
            //
            // btnItemSearchColumnDown
            //
            this.btnItemSearchColumnDown.Location = new System.Drawing.Point(312, 76);
            this.btnItemSearchColumnDown.Name = "btnItemSearchColumnDown";
            this.btnItemSearchColumnDown.Size = new System.Drawing.Size(74, 30);
            this.btnItemSearchColumnDown.TabIndex = 3;
            this.btnItemSearchColumnDown.Text = "Move Down";
            this.btnItemSearchColumnDown.UseVisualStyleBackColor = true;
            this.btnItemSearchColumnDown.Click += new System.EventHandler(this.btnItemSearchColumnDown_Click);
            //
            // chkItemSearchDateSortDescending
            //
            this.chkItemSearchDateSortDescending.AutoSize = true;
            this.chkItemSearchDateSortDescending.Location = new System.Drawing.Point(6, 336);
            this.chkItemSearchDateSortDescending.Name = "chkItemSearchDateSortDescending";
            this.chkItemSearchDateSortDescending.Size = new System.Drawing.Size(300, 17);
            this.chkItemSearchDateSortDescending.TabIndex = 4;
            this.chkItemSearchDateSortDescending.Text = "Enter-to-search results: newest first (descending)";
            this.chkItemSearchDateSortDescending.UseVisualStyleBackColor = true;
            //
            // chkItemSearchNameSortDescending
            //
            this.chkItemSearchNameSortDescending.AutoSize = true;
            this.chkItemSearchNameSortDescending.Location = new System.Drawing.Point(6, 360);
            this.chkItemSearchNameSortDescending.Name = "chkItemSearchNameSortDescending";
            this.chkItemSearchNameSortDescending.Size = new System.Drawing.Size(300, 17);
            this.chkItemSearchNameSortDescending.TabIndex = 5;
            this.chkItemSearchNameSortDescending.Text = "Browse/refresh results: descending (Z-A)";
            this.chkItemSearchNameSortDescending.UseVisualStyleBackColor = true;
            //
            // tabPageCartColumns
            //
            this.tabPageCartColumns.BackColor = System.Drawing.Color.FloralWhite;
            this.tabPageCartColumns.Controls.Add(this.clbCartColumns);
            this.tabPageCartColumns.Controls.Add(this.btnCartColumnUp);
            this.tabPageCartColumns.Controls.Add(this.btnCartColumnDown);
            this.tabPageCartColumns.Controls.Add(this.lblCartColumnsHint);
            this.tabPageCartColumns.Location = new System.Drawing.Point(4, 22);
            this.tabPageCartColumns.Name = "tabPageCartColumns";
            this.tabPageCartColumns.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCartColumns.Size = new System.Drawing.Size(392, 372);
            this.tabPageCartColumns.TabIndex = 13;
            this.tabPageCartColumns.Text = "Cart Board";
            //
            // lblCartColumnsHint
            //
            this.lblCartColumnsHint.AutoSize = true;
            this.lblCartColumnsHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblCartColumnsHint.Location = new System.Drawing.Point(6, 6);
            this.lblCartColumnsHint.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblCartColumnsHint.Name = "lblCartColumnsHint";
            this.lblCartColumnsHint.Size = new System.Drawing.Size(300, 26);
            this.lblCartColumnsHint.TabIndex = 0;
            this.lblCartColumnsHint.Text = "Uncheck a column to hide it, or select one and use Move Up/Down to reorder it -" +
    " applies to the cart board on the main POS screen.";
            //
            // clbCartColumns
            //
            this.clbCartColumns.CheckOnClick = true;
            this.clbCartColumns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.clbCartColumns.FormattingEnabled = true;
            this.clbCartColumns.IntegralHeight = false;
            this.clbCartColumns.Location = new System.Drawing.Point(6, 40);
            this.clbCartColumns.Name = "clbCartColumns";
            this.clbCartColumns.Size = new System.Drawing.Size(300, 326);
            this.clbCartColumns.TabIndex = 1;
            //
            // btnCartColumnUp
            //
            this.btnCartColumnUp.Location = new System.Drawing.Point(312, 40);
            this.btnCartColumnUp.Name = "btnCartColumnUp";
            this.btnCartColumnUp.Size = new System.Drawing.Size(74, 30);
            this.btnCartColumnUp.TabIndex = 2;
            this.btnCartColumnUp.Text = "Move Up";
            this.btnCartColumnUp.UseVisualStyleBackColor = true;
            this.btnCartColumnUp.Click += new System.EventHandler(this.btnCartColumnUp_Click);
            //
            // btnCartColumnDown
            //
            this.btnCartColumnDown.Location = new System.Drawing.Point(312, 76);
            this.btnCartColumnDown.Name = "btnCartColumnDown";
            this.btnCartColumnDown.Size = new System.Drawing.Size(74, 30);
            this.btnCartColumnDown.TabIndex = 3;
            this.btnCartColumnDown.Text = "Move Down";
            this.btnCartColumnDown.UseVisualStyleBackColor = true;
            this.btnCartColumnDown.Click += new System.EventHandler(this.btnCartColumnDown_Click);
            //
            // openFileDialogAccess
            //
            this.openFileDialogAccess.CheckFileExists = false;
            this.openFileDialogAccess.DefaultExt = "accdb";
            this.openFileDialogAccess.Filter = "Access Database (*.accdb)|*.accdb|All files (*.*)|*.*";
            this.openFileDialogAccess.Title = "Choose Access File Location";
            //
            // folderBrowserDialogCounterReceipt
            //
            this.folderBrowserDialogCounterReceipt.Description = "Choose a folder to save the counter\'s copy JPEG receipts";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Tomato;
            this.lblStatus.Location = new System.Drawing.Point(16, 518);
            this.lblStatus.MaximumSize = new System.Drawing.Size(396, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 1;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(16, 538);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(136, 538);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // SystemConfigForm
            //
            this.AcceptButton = this.btnSave;
            this.BackColor = System.Drawing.Color.FloralWhite;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(564, 582);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlConfigHost);
            this.Controls.Add(this.lstConfigSections);
            this.Controls.Add(this.lblWarningBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SystemConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "System Configuration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SystemConfigForm_FormClosed);
            this.Load += new System.EventHandler(this.SystemConfigForm_Load);
            this.pnlConfigHost.ResumeLayout(false);
            this.tabControlConfig.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.gbxStoreInfo.ResumeLayout(false);
            this.gbxStoreInfo.PerformLayout();
            this.gbxDatabase.ResumeLayout(false);
            this.gbxDatabase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_connectionTimeout)).EndInit();
            this.gbxSecurity.ResumeLayout(false);
            this.gbxSecurity.PerformLayout();
            this.gbxSalesDefaults.ResumeLayout(false);
            this.gbxSalesDefaults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_autoHoldMinutes)).EndInit();
            this.gbxAlerts.ResumeLayout(false);
            this.gbxAlerts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_alertAutoDismissSeconds)).EndInit();
            this.gbxStartup.ResumeLayout(false);
            this.gbxStartup.PerformLayout();
            this.tabPageNetwork.ResumeLayout(false);
            this.tabPageNetwork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartNetwork)).EndInit();
            this.tabPageMenus.ResumeLayout(false);
            this.tabPageMenus.PerformLayout();
            this.tabPagePrinter.ResumeLayout(false);
            this.tabPagePrinter.PerformLayout();
            this.tabPageAppearance.ResumeLayout(false);
            this.tabPageAppearance.PerformLayout();
            this.tabPageLayout.ResumeLayout(false);
            this.tabPageLayout.PerformLayout();
            this.tabPageSchedule.ResumeLayout(false);
            this.tabPageSchedule.PerformLayout();
            this.gbxGmail.ResumeLayout(false);
            this.gbxGmail.PerformLayout();
            this.gbxSmtp.ResumeLayout(false);
            this.gbxSmtp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_smtpPort)).EndInit();
            this.tabPageDataSync.ResumeLayout(false);
            this.tabPageDataSync.PerformLayout();
            this.grpUpload.ResumeLayout(false);
            this.grpUpload.PerformLayout();
            this.tabPageTrial.ResumeLayout(false);
            this.tabPageTrial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_trialDays)).EndInit();
            this.tabPageDbSetup.ResumeLayout(false);
            this.tabPageDbSetup.PerformLayout();
            this.tabPageRemoteControl.ResumeLayout(false);
            this.tabPageRemoteControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_fontScalePt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_remotePort)).EndInit();
            this.tabPageMaintenance.ResumeLayout(false);
            this.tabPageMaintenance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowStockThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowStockInterval)).EndInit();
            this.tabPageLowStock.ResumeLayout(false);
            this.tabPageLowStock.PerformLayout();
            this.tabPageReceiptTesting.ResumeLayout(false);
            this.tabPageReceiptTesting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPriceOverride)).EndInit();
            this.tabPageItemSearch.ResumeLayout(false);
            this.tabPageItemSearch.PerformLayout();
            this.tabPageCartColumns.ResumeLayout(false);
            this.tabPageCartColumns.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstConfigSections;
        private System.Windows.Forms.Panel pnlConfigHost;
        private System.Windows.Forms.TabControl tabControlConfig;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.GroupBox gbxStoreInfo;
        private System.Windows.Forms.Label lblStoreName;
        private System.Windows.Forms.TextBox txtStoreName;
        private System.Windows.Forms.Label lblStoreCode;
        private System.Windows.Forms.TextBox txtStoreCode;
        private System.Windows.Forms.Label lblStoreEmail;
        private System.Windows.Forms.TextBox txtStoreEmail;
        private System.Windows.Forms.GroupBox gbxDatabase;
        private System.Windows.Forms.Label lblDbServer;
        private System.Windows.Forms.TextBox txtDbServer;
        private System.Windows.Forms.Label lblDbName;
        private System.Windows.Forms.TextBox txtDbName;
        private System.Windows.Forms.Label lblDbUser;
        private System.Windows.Forms.TextBox txtDbUser;
        private System.Windows.Forms.Label lblDbPassword;
        private System.Windows.Forms.TextBox txtDbPassword;
        private System.Windows.Forms.CheckBox chkShowDbPassword;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.NumericUpDown nud_connectionTimeout;
        private System.Windows.Forms.Label lblDbNote;
        private System.Windows.Forms.GroupBox gbxSecurity;
        private System.Windows.Forms.Label lblManagerPin;
        private System.Windows.Forms.TextBox txtManagerPin;
        private System.Windows.Forms.CheckBox chkShowPin;
        private System.Windows.Forms.Label lblAdminPin;
        private System.Windows.Forms.TextBox txtAdminPin;
        private System.Windows.Forms.CheckBox chkShowAdminPin;
        private System.Windows.Forms.GroupBox gbxSalesDefaults;
        private System.Windows.Forms.CheckBox chkDefaultSelltype;
        private System.Windows.Forms.ComboBox cbxDefaultSelltype;
        private System.Windows.Forms.CheckBox chkAutoHoldEnabled;
        private System.Windows.Forms.NumericUpDown nud_autoHoldMinutes;
        private System.Windows.Forms.Label lblAutoHoldMinutes;
        private System.Windows.Forms.CheckBox chkBlockZeroPriceItems;
        private System.Windows.Forms.GroupBox gbxAlerts;
        private System.Windows.Forms.CheckBox chkAlertAutoDismissEnabled;
        private System.Windows.Forms.NumericUpDown nud_alertAutoDismissSeconds;
        private System.Windows.Forms.Label lblAlertAutoDismissSeconds;
        private System.Windows.Forms.GroupBox gbxStartup;
        private System.Windows.Forms.CheckBox chkAutoStartWithWindows;
        private System.Windows.Forms.TabPage tabPageNetwork;
        private System.Windows.Forms.Button btnToggleMonitor;
        private System.Windows.Forms.Label lblNetworkStatus;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartNetwork;
        private System.Windows.Forms.Label lblNetworkHint;
        private System.Windows.Forms.TabPage tabPageMenus;
        private System.Windows.Forms.CheckedListBox clbMenus;
        private System.Windows.Forms.Label lblMenusHint;
        private System.Windows.Forms.TabPage tabPagePrinter;
        private System.Windows.Forms.Label lblPrinterHint;
        private System.Windows.Forms.Label lblReceiptPrinter;
        private System.Windows.Forms.ComboBox cbxReceiptPrinter;
        private System.Windows.Forms.Label lblPaperSizeInfo;
        private System.Windows.Forms.Label lblPaperWidth;
        private System.Windows.Forms.ComboBox cbxPaperWidth;
        private System.Windows.Forms.CheckBox chkPaperWidthIsDefault;
        private System.Windows.Forms.Label lblCounterReceiptPath;
        private System.Windows.Forms.TextBox txtCounterReceiptPath;
        private System.Windows.Forms.Button btnBrowseCounterReceiptPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogCounterReceipt;
        private System.Windows.Forms.CheckBox chkSetAsSystemDefaultPrinter;
        private System.Windows.Forms.CheckBox chkSaveCounterReceiptAsJpeg;
        private System.Windows.Forms.Label lblCounterReceiptHint;
        private System.Windows.Forms.TabPage tabPageAppearance;
        private System.Windows.Forms.Label lblAppearanceHint;
        private System.Windows.Forms.CheckBox chkEnableAccent;
        private System.Windows.Forms.Label lblBgColor;
        private System.Windows.Forms.Panel pnlBgSwatch;
        private System.Windows.Forms.Button btnPickBgColor;
        private System.Windows.Forms.Label lblFgColor;
        private System.Windows.Forms.Panel pnlFgSwatch;
        private System.Windows.Forms.Button btnPickFgColor;
        private System.Windows.Forms.Label lblAppearanceNote;
        private System.Windows.Forms.Label lblFontScale;
        private System.Windows.Forms.NumericUpDown nud_fontScalePt;
        private System.Windows.Forms.Label lblFontScaleUnit;
        private System.Windows.Forms.Label lblFontScaleHint;
        private System.Windows.Forms.Label lblLogoUrl;
        private System.Windows.Forms.TextBox txtLogoUrl;
        private System.Windows.Forms.Label lblLogoUrlHint;
        private System.Windows.Forms.CheckBox chkLayoutEditEnabled;
        private System.Windows.Forms.Label lblLayoutEditHint;
        private System.Windows.Forms.ColorDialog colorDialogAccent;
        private System.Windows.Forms.TabPage tabPageLayout;
        private System.Windows.Forms.Label lblLayoutRevisionHint;
        private System.Windows.Forms.TextBox txtLayoutRevision;
        private System.Windows.Forms.Button btnRefreshLayoutText;
        private System.Windows.Forms.Button btnApplyLayoutText;
        private System.Windows.Forms.Button btnLoadLayoutFile;
        private System.Windows.Forms.Button btnSaveLayoutFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogLayout;
        private System.Windows.Forms.SaveFileDialog saveFileDialogLayout;
        private System.Windows.Forms.TabPage tabPageSchedule;
        private System.Windows.Forms.Label lblScheduleHint;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.ComboBox cbxReportFrequency;
        private System.Windows.Forms.Label lblScheduleStatus;
        private System.Windows.Forms.Button btnSendReportNow;
        private System.Windows.Forms.Label lblSendVia;
        private System.Windows.Forms.RadioButton rdoProviderSmtp;
        private System.Windows.Forms.RadioButton rdoProviderGmail;
        private System.Windows.Forms.GroupBox gbxGmail;
        private System.Windows.Forms.Label lblGmailClientId;
        private System.Windows.Forms.TextBox txtGmailClientId;
        private System.Windows.Forms.Label lblGmailClientSecret;
        private System.Windows.Forms.TextBox txtGmailClientSecret;
        private System.Windows.Forms.CheckBox chkShowGmailClientSecret;
        private System.Windows.Forms.Label lblGmailRefreshToken;
        private System.Windows.Forms.TextBox txtGmailRefreshToken;
        private System.Windows.Forms.CheckBox chkShowGmailRefreshToken;
        private System.Windows.Forms.Label lblGmailSender;
        private System.Windows.Forms.TextBox txtGmailSender;
        private System.Windows.Forms.Label lblGmailNote;
        private System.Windows.Forms.GroupBox gbxSmtp;
        private System.Windows.Forms.Label lblSmtpHost;
        private System.Windows.Forms.TextBox txtSmtpHost;
        private System.Windows.Forms.Label lblSmtpPort;
        private System.Windows.Forms.NumericUpDown nud_smtpPort;
        private System.Windows.Forms.CheckBox chkSmtpUseSsl;
        private System.Windows.Forms.Label lblSmtpFrom;
        private System.Windows.Forms.TextBox txtSmtpFrom;
        private System.Windows.Forms.Label lblSmtpUser;
        private System.Windows.Forms.TextBox txtSmtpUser;
        private System.Windows.Forms.Label lblSmtpPassword;
        private System.Windows.Forms.TextBox txtSmtpPassword;
        private System.Windows.Forms.CheckBox chkShowSmtpPassword;
        private System.Windows.Forms.Label lblSmtpNote;
        private System.Windows.Forms.TabPage tabPageDataSync;
        private System.Windows.Forms.Label lblSyncHint;
        private System.Windows.Forms.Label lblAccessPath;
        private System.Windows.Forms.TextBox txtAccessDbPath;
        private System.Windows.Forms.Button btnBrowseAccessDbPath;
        private System.Windows.Forms.Button btnSyncNow;
        private System.Windows.Forms.Label lblSyncStatus;
        private System.Windows.Forms.Label lblLastSync;
        private System.Windows.Forms.Button btnPushToOnline;
        private System.Windows.Forms.Label lblPushStatus;
        private System.Windows.Forms.Label lblLastPush;
        private System.Windows.Forms.GroupBox grpUpload;
        private System.Windows.Forms.TabPage tabPageTrial;
        private System.Windows.Forms.Label lblTrialHint;
        private System.Windows.Forms.CheckBox chkTrialEnabled;
        private System.Windows.Forms.Label lblTrialDays;
        private System.Windows.Forms.NumericUpDown nud_trialDays;
        private System.Windows.Forms.Label lblTrialStatus;
        private System.Windows.Forms.Label lblLicenseKey;
        private System.Windows.Forms.TextBox txtLicenseKey;
        private System.Windows.Forms.Label lblSubscriptionTier;
        private System.Windows.Forms.ComboBox cmbSubscriptionTier;
        private System.Windows.Forms.Button btnActivateLicense;
        private System.Windows.Forms.Button btnImportLicenseFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogLicense;
        private System.Windows.Forms.CheckBox chkTrialAutoClose;
        private System.Windows.Forms.Label lblTrialAutoCloseHint;
        private System.Windows.Forms.TabPage tabPageDbSetup;
        private System.Windows.Forms.Label lblDbSetupHint;
        private System.Windows.Forms.Label lblNewDbName;
        private System.Windows.Forms.TextBox txtNewDbName;
        private System.Windows.Forms.CheckBox chkSwitchToNewDb;
        private System.Windows.Forms.Button btnCreateDatabase;
        private System.Windows.Forms.Label lblDbSetupStatus;
        private System.Windows.Forms.TabPage tabPageRemoteControl;
        private System.Windows.Forms.Label lblRemoteHint;
        private System.Windows.Forms.Label lblRemoteIpAddress;
        private System.Windows.Forms.Button btnRefreshIp;
        private System.Windows.Forms.CheckBox chkRemoteControlEnabled;
        private System.Windows.Forms.Label lblRemotePort;
        private System.Windows.Forms.NumericUpDown nud_remotePort;
        private System.Windows.Forms.Label lblRemoteToken;
        private System.Windows.Forms.TextBox txtRemoteToken;
        private System.Windows.Forms.CheckBox chkShowRemoteToken;
        private System.Windows.Forms.Button btnGenerateRemoteToken;
        private System.Windows.Forms.Label lblRemoteStatus;
        private System.Windows.Forms.Label lblCertThumbprintHeader;
        private System.Windows.Forms.TextBox txtCertThumbprint;
        private System.Windows.Forms.Button btnCopyThumbprint;
        private System.Windows.Forms.Label lblPublicIp;
        private System.Windows.Forms.TextBox txtPublicIp;
        private System.Windows.Forms.Button btnCheckPublicIp;
        private System.Windows.Forms.Label lblBluetoothHeader;
        private System.Windows.Forms.CheckBox chkBluetoothEnabled;
        private System.Windows.Forms.Label lblBluetoothRadioInfo;
        private System.Windows.Forms.TabPage tabPageMaintenance;
        private System.Windows.Forms.Label lblMaintenanceHint;
        private System.Windows.Forms.CheckBox chkMaintenanceEnabled;
        private System.Windows.Forms.Label lblMaintenanceMessage;
        private System.Windows.Forms.TextBox txtMaintenanceMessage;
        private System.Windows.Forms.Button btnSaveMaintenance;
        private System.Windows.Forms.Button btnRefreshMaintenance;
        private System.Windows.Forms.Label lblMaintenanceStatus;
        private System.Windows.Forms.TabPage tabPageLowStock;
        private System.Windows.Forms.Label lblLowStockHint;
        private System.Windows.Forms.CheckBox chkLowStockEnabled;
        private System.Windows.Forms.Label lblLowStockThreshold;
        private System.Windows.Forms.NumericUpDown nudLowStockThreshold;
        private System.Windows.Forms.Label lblLowStockInterval;
        private System.Windows.Forms.NumericUpDown nudLowStockInterval;
        private System.Windows.Forms.Button btnSaveLowStock;
        private System.Windows.Forms.Button btnRefreshLowStock;
        private System.Windows.Forms.Label lblLowStockStatus;
        private System.Windows.Forms.TabPage tabPageReceiptTesting;
        private System.Windows.Forms.Label lblReceiptTestingHint;
        private System.Windows.Forms.CheckBox chkPriceOverrideEnabled;
        private System.Windows.Forms.TrackBar tbPriceOverride;
        private System.Windows.Forms.Label lblPriceOverrideValue;
        private System.Windows.Forms.Button btnPreviewReceipt;
        private System.Windows.Forms.TabPage tabPageItemSearch;
        private System.Windows.Forms.CheckedListBox clbItemSearchColumns;
        private System.Windows.Forms.Label lblItemSearchHint;
        private System.Windows.Forms.Button btnItemSearchColumnUp;
        private System.Windows.Forms.Button btnItemSearchColumnDown;
        private System.Windows.Forms.CheckBox chkItemSearchDateSortDescending;
        private System.Windows.Forms.CheckBox chkItemSearchNameSortDescending;
        private System.Windows.Forms.TabPage tabPageCartColumns;
        private System.Windows.Forms.CheckedListBox clbCartColumns;
        private System.Windows.Forms.Label lblCartColumnsHint;
        private System.Windows.Forms.Button btnCartColumnUp;
        private System.Windows.Forms.Button btnCartColumnDown;
        private System.Windows.Forms.OpenFileDialog openFileDialogAccess;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblWarningBar;
        private System.Windows.Forms.ToolTip tooltipWarnings;
    }
}
