namespace Andrea_NameSpace
{
    partial class DashBoardEmulEP
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashBoardEmulEP));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.interfacciaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FTDIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTxtBoxDescrittore = new System.Windows.Forms.ToolStripTextBox();
            this.uSBPortLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFTDIlocation = new System.Windows.Forms.ToolStripTextBox();
            this.setPreferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPreferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setPreferenceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.salvaPreferenceOnExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.istruzioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.istruzioniToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.schemaElettricoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrInterfaccia = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabEmulEP = new System.Windows.Forms.TabPage();
            this.lblStatoCPU = new System.Windows.Forms.Label();
            this.btnResetRun = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.nudStopBin = new System.Windows.Forms.NumericUpDown();
            this.nudStartBin = new System.Windows.Forms.NumericUpDown();
            this.btnSalvaBin = new System.Windows.Forms.Button();
            this.btnSalvaHex = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblRangeEprom = new System.Windows.Forms.Label();
            this.ckbLoadAllRam = new System.Windows.Forms.CheckBox();
            this.ckbVerifyRam = new System.Windows.Forms.CheckBox();
            this.cbSizeEP = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nudBaseEP = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.btnLoadAndRun = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbBancoRamLow = new System.Windows.Forms.RadioButton();
            this.rbBancoRamHigh = new System.Windows.Forms.RadioButton();
            this.btnEditMem = new System.Windows.Forms.Button();
            this.nudBaseBin = new System.Windows.Forms.NumericUpDown();
            this.chbAutoLoad = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txbBinFile = new System.Windows.Forms.TextBox();
            this.ckbResetOnLoad = new System.Windows.Forms.CheckBox();
            this.nudResetValue = new System.Windows.Forms.NumericUpDown();
            this.btnResetMem = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txbHexFile = new System.Windows.Forms.TextBox();
            this.chbAutoAssembler = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtbArgomentiZasm = new System.Windows.Forms.TextBox();
            this.btnCompilaZasm = new System.Windows.Forms.Button();
            this.txbAsmFile = new System.Windows.Forms.TextBox();
            this.btnLoop = new System.Windows.Forms.Button();
            this.tabFirmware = new System.Windows.Forms.TabPage();
            this.btnVerifyFirmware = new System.Windows.Forms.Button();
            this.btnStartFirmware = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblPicTypeDeviceOnPC = new System.Windows.Forms.Label();
            this.lblNomeFirmwareOnPC = new System.Windows.Forms.Label();
            this.lblVerDeviceOnPC = new System.Windows.Forms.Label();
            this.btnUploadFirmware = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPicTypeDevice = new System.Windows.Forms.Label();
            this.lblNomeFirmware = new System.Windows.Forms.Label();
            this.lblVerDevice = new System.Windows.Forms.Label();
            this.btnLeggiFirmware = new System.Windows.Forms.Button();
            this.btnEnterBootLoader = new System.Windows.Forms.Button();
            this.monitor = new Andrea_NameSpace.Monitor();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabEmulEP.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStopBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartBin)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBaseEP)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBaseBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResetValue)).BeginInit();
            this.tabFirmware.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.interfacciaToolStripMenuItem,
            this.setPreferenceToolStripMenuItem,
            this.istruzioniToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(827, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // interfacciaToolStripMenuItem
            // 
            this.interfacciaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FTDIToolStripMenuItem,
            this.uSBPortLocationToolStripMenuItem});
            this.interfacciaToolStripMenuItem.Name = "interfacciaToolStripMenuItem";
            this.interfacciaToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.interfacciaToolStripMenuItem.Text = "Interface USB";
            // 
            // FTDIToolStripMenuItem
            // 
            this.FTDIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTxtBoxDescrittore});
            this.FTDIToolStripMenuItem.Name = "FTDIToolStripMenuItem";
            this.FTDIToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.FTDIToolStripMenuItem.Text = "Descrittore FTDI";
            this.FTDIToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.FTDIToolStripMenuItem_CheckStateChanged);
            // 
            // toolStripTxtBoxDescrittore
            // 
            this.toolStripTxtBoxDescrittore.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTxtBoxDescrittore.Name = "toolStripTxtBoxDescrittore";
            this.toolStripTxtBoxDescrittore.Size = new System.Drawing.Size(100, 23);
            this.toolStripTxtBoxDescrittore.Text = "FT232R USB UART";
            this.toolStripTxtBoxDescrittore.TextChanged += new System.EventHandler(this.toolStripTxtBoxDescrittore_TextChanged);
            // 
            // uSBPortLocationToolStripMenuItem
            // 
            this.uSBPortLocationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFTDIlocation});
            this.uSBPortLocationToolStripMenuItem.Name = "uSBPortLocationToolStripMenuItem";
            this.uSBPortLocationToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.uSBPortLocationToolStripMenuItem.Text = "USB port (Location)";
            // 
            // toolStripFTDIlocation
            // 
            this.toolStripFTDIlocation.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripFTDIlocation.Name = "toolStripFTDIlocation";
            this.toolStripFTDIlocation.Size = new System.Drawing.Size(100, 23);
            this.toolStripFTDIlocation.Text = "0";
            this.toolStripFTDIlocation.TextChanged += new System.EventHandler(this.toolStripFTDIlocation_TextChanged);
            // 
            // setPreferenceToolStripMenuItem
            // 
            this.setPreferenceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPreferenceToolStripMenuItem,
            this.setPreferenceToolStripMenuItem1,
            this.salvaPreferenceOnExitToolStripMenuItem});
            this.setPreferenceToolStripMenuItem.Name = "setPreferenceToolStripMenuItem";
            this.setPreferenceToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.setPreferenceToolStripMenuItem.Text = "Preference";
            // 
            // loadPreferenceToolStripMenuItem
            // 
            this.loadPreferenceToolStripMenuItem.Name = "loadPreferenceToolStripMenuItem";
            this.loadPreferenceToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.loadPreferenceToolStripMenuItem.Text = "Load Preference";
            this.loadPreferenceToolStripMenuItem.Click += new System.EventHandler(this.loadPreferenceToolStripMenuItem_Click);
            // 
            // setPreferenceToolStripMenuItem1
            // 
            this.setPreferenceToolStripMenuItem1.Name = "setPreferenceToolStripMenuItem1";
            this.setPreferenceToolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
            this.setPreferenceToolStripMenuItem1.Text = "Save Preference";
            this.setPreferenceToolStripMenuItem1.Click += new System.EventHandler(this.setPreferenceToolStripMenuItem_Click);
            // 
            // salvaPreferenceOnExitToolStripMenuItem
            // 
            this.salvaPreferenceOnExitToolStripMenuItem.CheckOnClick = true;
            this.salvaPreferenceOnExitToolStripMenuItem.Name = "salvaPreferenceOnExitToolStripMenuItem";
            this.salvaPreferenceOnExitToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.salvaPreferenceOnExitToolStripMenuItem.Text = "Save Preference on exit";
            // 
            // istruzioniToolStripMenuItem
            // 
            this.istruzioniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.istruzioniToolStripMenuItem1,
            this.schemaElettricoToolStripMenuItem});
            this.istruzioniToolStripMenuItem.Name = "istruzioniToolStripMenuItem";
            this.istruzioniToolStripMenuItem.Size = new System.Drawing.Size(24, 20);
            this.istruzioniToolStripMenuItem.Text = "?";
            // 
            // istruzioniToolStripMenuItem1
            // 
            this.istruzioniToolStripMenuItem1.Name = "istruzioniToolStripMenuItem1";
            this.istruzioniToolStripMenuItem1.Size = new System.Drawing.Size(162, 22);
            this.istruzioniToolStripMenuItem1.Text = "Istruzioni";
            this.istruzioniToolStripMenuItem1.Click += new System.EventHandler(this.istruzioniToolStripMenuItem1_Click);
            // 
            // schemaElettricoToolStripMenuItem
            // 
            this.schemaElettricoToolStripMenuItem.Name = "schemaElettricoToolStripMenuItem";
            this.schemaElettricoToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.schemaElettricoToolStripMenuItem.Text = "Schema elettrico";
            this.schemaElettricoToolStripMenuItem.Click += new System.EventHandler(this.schemaElettricoToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 628);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(827, 22);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(250, 17);
            this.toolStripStatusLabel1.Text = "Stato interfaccia ?";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.AutoSize = false;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(90, 17);
            this.toolStripStatusLabel3.Text = "Baud:";
            this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(472, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "Errori :";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tmrInterfaccia
            // 
            this.tmrInterfaccia.Interval = 800;
            this.tmrInterfaccia.Tick += new System.EventHandler(this.tmrInterfaccia_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.monitor);
            this.splitContainer1.Size = new System.Drawing.Size(827, 604);
            this.splitContainer1.SplitterDistance = 353;
            this.splitContainer1.TabIndex = 18;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabEmulEP);
            this.tabControl.Controls.Add(this.tabFirmware);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(827, 353);
            this.tabControl.TabIndex = 17;
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            this.tabControl.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Deselecting);
            // 
            // tabEmulEP
            // 
            this.tabEmulEP.BackColor = System.Drawing.Color.Transparent;
            this.tabEmulEP.Controls.Add(this.lblStatoCPU);
            this.tabEmulEP.Controls.Add(this.btnResetRun);
            this.tabEmulEP.Controls.Add(this.groupBox5);
            this.tabEmulEP.Controls.Add(this.groupBox4);
            this.tabEmulEP.Controls.Add(this.btnLoadAndRun);
            this.tabEmulEP.Controls.Add(this.groupBox3);
            this.tabEmulEP.Controls.Add(this.btnEditMem);
            this.tabEmulEP.Controls.Add(this.nudBaseBin);
            this.tabEmulEP.Controls.Add(this.chbAutoLoad);
            this.tabEmulEP.Controls.Add(this.label3);
            this.tabEmulEP.Controls.Add(this.label1);
            this.tabEmulEP.Controls.Add(this.txbBinFile);
            this.tabEmulEP.Controls.Add(this.ckbResetOnLoad);
            this.tabEmulEP.Controls.Add(this.nudResetValue);
            this.tabEmulEP.Controls.Add(this.btnResetMem);
            this.tabEmulEP.Controls.Add(this.label7);
            this.tabEmulEP.Controls.Add(this.txbHexFile);
            this.tabEmulEP.Controls.Add(this.chbAutoAssembler);
            this.tabEmulEP.Controls.Add(this.label6);
            this.tabEmulEP.Controls.Add(this.txtbArgomentiZasm);
            this.tabEmulEP.Controls.Add(this.btnCompilaZasm);
            this.tabEmulEP.Controls.Add(this.txbAsmFile);
            this.tabEmulEP.Controls.Add(this.btnLoop);
            this.tabEmulEP.Location = new System.Drawing.Point(4, 22);
            this.tabEmulEP.Name = "tabEmulEP";
            this.tabEmulEP.Padding = new System.Windows.Forms.Padding(3);
            this.tabEmulEP.Size = new System.Drawing.Size(819, 327);
            this.tabEmulEP.TabIndex = 0;
            this.tabEmulEP.Text = "Eprom Emulator";
            // 
            // lblStatoCPU
            // 
            this.lblStatoCPU.AutoSize = true;
            this.lblStatoCPU.BackColor = System.Drawing.Color.Transparent;
            this.lblStatoCPU.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatoCPU.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStatoCPU.Location = new System.Drawing.Point(502, 33);
            this.lblStatoCPU.Name = "lblStatoCPU";
            this.lblStatoCPU.Size = new System.Drawing.Size(276, 44);
            this.lblStatoCPU.TabIndex = 149;
            this.lblStatoCPU.Text = "CPU in RESET";
            this.lblStatoCPU.Visible = false;
            // 
            // btnResetRun
            // 
            this.btnResetRun.Enabled = false;
            this.btnResetRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetRun.Location = new System.Drawing.Point(591, 175);
            this.btnResetRun.Name = "btnResetRun";
            this.btnResetRun.Size = new System.Drawing.Size(220, 68);
            this.btnResetRun.TabIndex = 148;
            this.btnResetRun.Text = "RUN CPU";
            this.btnResetRun.UseVisualStyleBackColor = true;
            this.btnResetRun.Click += new System.EventHandler(this.btnResetRun_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.nudStopBin);
            this.groupBox5.Controls.Add(this.nudStartBin);
            this.groupBox5.Controls.Add(this.btnSalvaBin);
            this.groupBox5.Controls.Add(this.btnSalvaHex);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Location = new System.Drawing.Point(11, 157);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(348, 86);
            this.groupBox5.TabIndex = 147;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Codici in memoria PC 64K";
            // 
            // nudStopBin
            // 
            this.nudStopBin.Hexadecimal = true;
            this.nudStopBin.Location = new System.Drawing.Point(258, 38);
            this.nudStopBin.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudStopBin.Name = "nudStopBin";
            this.nudStopBin.Size = new System.Drawing.Size(67, 20);
            this.nudStopBin.TabIndex = 133;
            this.nudStopBin.Value = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            // 
            // nudStartBin
            // 
            this.nudStartBin.Hexadecimal = true;
            this.nudStartBin.Location = new System.Drawing.Point(141, 38);
            this.nudStartBin.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudStartBin.Name = "nudStartBin";
            this.nudStartBin.Size = new System.Drawing.Size(67, 20);
            this.nudStartBin.TabIndex = 134;
            // 
            // btnSalvaBin
            // 
            this.btnSalvaBin.Location = new System.Drawing.Point(6, 52);
            this.btnSalvaBin.Name = "btnSalvaBin";
            this.btnSalvaBin.Size = new System.Drawing.Size(87, 23);
            this.btnSalvaBin.TabIndex = 132;
            this.btnSalvaBin.Text = "Salva Bin";
            this.btnSalvaBin.UseVisualStyleBackColor = true;
            this.btnSalvaBin.Click += new System.EventHandler(this.btnSalvaBin_Click);
            // 
            // btnSalvaHex
            // 
            this.btnSalvaHex.Location = new System.Drawing.Point(8, 18);
            this.btnSalvaHex.Name = "btnSalvaHex";
            this.btnSalvaHex.Size = new System.Drawing.Size(87, 23);
            this.btnSalvaHex.TabIndex = 137;
            this.btnSalvaHex.Text = "Salva Hex";
            this.btnSalvaHex.UseVisualStyleBackColor = true;
            this.btnSalvaHex.Click += new System.EventHandler(this.btnSalvaHex_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(107, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 135;
            this.label4.Text = "Inizio";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(228, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 136;
            this.label2.Text = "Fine";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblRangeEprom);
            this.groupBox4.Controls.Add(this.ckbLoadAllRam);
            this.groupBox4.Controls.Add(this.ckbVerifyRam);
            this.groupBox4.Controls.Add(this.cbSizeEP);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.nudBaseEP);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(365, 109);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(220, 133);
            this.groupBox4.TabIndex = 146;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Carica su emulatore Eprom";
            // 
            // lblRangeEprom
            // 
            this.lblRangeEprom.AutoSize = true;
            this.lblRangeEprom.Location = new System.Drawing.Point(3, 110);
            this.lblRangeEprom.Name = "lblRangeEprom";
            this.lblRangeEprom.Size = new System.Drawing.Size(72, 13);
            this.lblRangeEprom.TabIndex = 148;
            this.lblRangeEprom.Text = "Range Eprom";
            // 
            // ckbLoadAllRam
            // 
            this.ckbLoadAllRam.AutoSize = true;
            this.ckbLoadAllRam.Checked = true;
            this.ckbLoadAllRam.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbLoadAllRam.Location = new System.Drawing.Point(6, 52);
            this.ckbLoadAllRam.Name = "ckbLoadAllRam";
            this.ckbLoadAllRam.Size = new System.Drawing.Size(83, 17);
            this.ckbLoadAllRam.TabIndex = 148;
            this.ckbLoadAllRam.Text = "Load all ram";
            this.ckbLoadAllRam.UseVisualStyleBackColor = true;
            // 
            // ckbVerifyRam
            // 
            this.ckbVerifyRam.AutoSize = true;
            this.ckbVerifyRam.Checked = true;
            this.ckbVerifyRam.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbVerifyRam.Location = new System.Drawing.Point(134, 52);
            this.ckbVerifyRam.Name = "ckbVerifyRam";
            this.ckbVerifyRam.Size = new System.Drawing.Size(77, 17);
            this.ckbVerifyRam.TabIndex = 145;
            this.ckbVerifyRam.Text = "Verify Ram";
            this.ckbVerifyRam.UseVisualStyleBackColor = true;
            // 
            // cbSizeEP
            // 
            this.cbSizeEP.FormattingEnabled = true;
            this.cbSizeEP.Location = new System.Drawing.Point(87, 22);
            this.cbSizeEP.Name = "cbSizeEP";
            this.cbSizeEP.Size = new System.Drawing.Size(124, 21);
            this.cbSizeEP.TabIndex = 143;
            this.cbSizeEP.SelectedIndexChanged += new System.EventHandler(this.cbSizeEP_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 142;
            this.label8.Text = "Size Eprom";
            // 
            // nudBaseEP
            // 
            this.nudBaseEP.Hexadecimal = true;
            this.nudBaseEP.Increment = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudBaseEP.Location = new System.Drawing.Point(87, 79);
            this.nudBaseEP.Maximum = new decimal(new int[] {
            64512,
            0,
            0,
            0});
            this.nudBaseEP.Name = "nudBaseEP";
            this.nudBaseEP.Size = new System.Drawing.Size(67, 20);
            this.nudBaseEP.TabIndex = 139;
            this.nudBaseEP.ValueChanged += new System.EventHandler(this.nudBaseEP_ValueChanged);
            this.nudBaseEP.Validated += new System.EventHandler(this.nudBaseEP_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 140;
            this.label5.Text = "Address Eprom";
            // 
            // btnLoadAndRun
            // 
            this.btnLoadAndRun.Enabled = false;
            this.btnLoadAndRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadAndRun.Location = new System.Drawing.Point(591, 250);
            this.btnLoadAndRun.Name = "btnLoadAndRun";
            this.btnLoadAndRun.Size = new System.Drawing.Size(220, 68);
            this.btnLoadAndRun.TabIndex = 138;
            this.btnLoadAndRun.Text = "LOAD AND RUN CPU";
            this.btnLoadAndRun.UseVisualStyleBackColor = true;
            this.btnLoadAndRun.Click += new System.EventHandler(this.btnLoadEmul_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbBancoRamLow);
            this.groupBox3.Controls.Add(this.rbBancoRamHigh);
            this.groupBox3.Location = new System.Drawing.Point(365, 249);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(220, 69);
            this.groupBox3.TabIndex = 144;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Selezione Banco RAM 628128";
            // 
            // rbBancoRamLow
            // 
            this.rbBancoRamLow.AutoSize = true;
            this.rbBancoRamLow.Checked = true;
            this.rbBancoRamLow.Location = new System.Drawing.Point(6, 42);
            this.rbBancoRamLow.Name = "rbBancoRamLow";
            this.rbBancoRamLow.Size = new System.Drawing.Size(104, 17);
            this.rbBancoRamLow.TabIndex = 1;
            this.rbBancoRamLow.TabStop = true;
            this.rbBancoRamLow.Text = "Banco Ram Low";
            this.rbBancoRamLow.UseVisualStyleBackColor = true;
            this.rbBancoRamLow.CheckedChanged += new System.EventHandler(this.rbBancoRamLow_CheckedChanged);
            // 
            // rbBancoRamHigh
            // 
            this.rbBancoRamHigh.AutoSize = true;
            this.rbBancoRamHigh.Location = new System.Drawing.Point(6, 19);
            this.rbBancoRamHigh.Name = "rbBancoRamHigh";
            this.rbBancoRamHigh.Size = new System.Drawing.Size(106, 17);
            this.rbBancoRamHigh.TabIndex = 0;
            this.rbBancoRamHigh.Text = "Banco Ram High";
            this.rbBancoRamHigh.UseVisualStyleBackColor = true;
            // 
            // btnEditMem
            // 
            this.btnEditMem.Location = new System.Drawing.Point(11, 296);
            this.btnEditMem.Name = "btnEditMem";
            this.btnEditMem.Size = new System.Drawing.Size(102, 23);
            this.btnEditMem.TabIndex = 131;
            this.btnEditMem.Text = "Edit Mem 64K";
            this.btnEditMem.UseVisualStyleBackColor = true;
            this.btnEditMem.Click += new System.EventHandler(this.btnEditMem_Click);
            // 
            // nudBaseBin
            // 
            this.nudBaseBin.Hexadecimal = true;
            this.nudBaseBin.Location = new System.Drawing.Point(127, 109);
            this.nudBaseBin.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudBaseBin.Name = "nudBaseBin";
            this.nudBaseBin.Size = new System.Drawing.Size(67, 20);
            this.nudBaseBin.TabIndex = 129;
            // 
            // chbAutoLoad
            // 
            this.chbAutoLoad.AutoSize = true;
            this.chbAutoLoad.Location = new System.Drawing.Point(365, 72);
            this.chbAutoLoad.Name = "chbAutoLoad";
            this.chbAutoLoad.Size = new System.Drawing.Size(115, 17);
            this.chbAutoLoad.TabIndex = 126;
            this.chbAutoLoad.Text = "Auto load on Emul.";
            this.chbAutoLoad.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 130;
            this.label3.Text = "Base File binario:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 128;
            this.label1.Text = "File Binario (*.bin):";
            // 
            // txbBinFile
            // 
            this.txbBinFile.Location = new System.Drawing.Point(127, 83);
            this.txbBinFile.Name = "txbBinFile";
            this.txbBinFile.ReadOnly = true;
            this.txbBinFile.Size = new System.Drawing.Size(232, 20);
            this.txbBinFile.TabIndex = 127;
            this.txbBinFile.Click += new System.EventHandler(this.txbBinFile_Click);
            // 
            // ckbResetOnLoad
            // 
            this.ckbResetOnLoad.AutoSize = true;
            this.ckbResetOnLoad.Checked = true;
            this.ckbResetOnLoad.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbResetOnLoad.Location = new System.Drawing.Point(152, 301);
            this.ckbResetOnLoad.Name = "ckbResetOnLoad";
            this.ckbResetOnLoad.Size = new System.Drawing.Size(96, 17);
            this.ckbResetOnLoad.TabIndex = 125;
            this.ckbResetOnLoad.Text = "Reset on Load";
            this.ckbResetOnLoad.UseVisualStyleBackColor = true;
            // 
            // nudResetValue
            // 
            this.nudResetValue.Hexadecimal = true;
            this.nudResetValue.Location = new System.Drawing.Point(152, 275);
            this.nudResetValue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudResetValue.Name = "nudResetValue";
            this.nudResetValue.Size = new System.Drawing.Size(96, 20);
            this.nudResetValue.TabIndex = 124;
            this.nudResetValue.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // btnResetMem
            // 
            this.btnResetMem.Location = new System.Drawing.Point(152, 249);
            this.btnResetMem.Name = "btnResetMem";
            this.btnResetMem.Size = new System.Drawing.Size(96, 23);
            this.btnResetMem.TabIndex = 123;
            this.btnResetMem.Text = "ResetMem";
            this.btnResetMem.UseVisualStyleBackColor = true;
            this.btnResetMem.Click += new System.EventHandler(this.btnResetMem_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 122;
            this.label7.Text = "File Intel (*.hex):";
            // 
            // txbHexFile
            // 
            this.txbHexFile.Location = new System.Drawing.Point(127, 57);
            this.txbHexFile.Name = "txbHexFile";
            this.txbHexFile.ReadOnly = true;
            this.txbHexFile.Size = new System.Drawing.Size(232, 20);
            this.txbHexFile.TabIndex = 121;
            this.txbHexFile.Click += new System.EventHandler(this.txbHexFile_Click);
            // 
            // chbAutoAssembler
            // 
            this.chbAutoAssembler.AutoSize = true;
            this.chbAutoAssembler.Location = new System.Drawing.Point(365, 9);
            this.chbAutoAssembler.Name = "chbAutoAssembler";
            this.chbAutoAssembler.Size = new System.Drawing.Size(98, 17);
            this.chbAutoAssembler.TabIndex = 120;
            this.chbAutoAssembler.Text = "Auto assembler";
            this.chbAutoAssembler.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 119;
            this.label6.Text = "File Assembly *.asm";
            // 
            // txtbArgomentiZasm
            // 
            this.txtbArgomentiZasm.Location = new System.Drawing.Point(127, 32);
            this.txtbArgomentiZasm.Name = "txtbArgomentiZasm";
            this.txtbArgomentiZasm.Size = new System.Drawing.Size(232, 20);
            this.txtbArgomentiZasm.TabIndex = 118;
            this.txtbArgomentiZasm.Text = "--z80 -xu --target=ram -w";
            // 
            // btnCompilaZasm
            // 
            this.btnCompilaZasm.Location = new System.Drawing.Point(11, 30);
            this.btnCompilaZasm.Name = "btnCompilaZasm";
            this.btnCompilaZasm.Size = new System.Drawing.Size(109, 23);
            this.btnCompilaZasm.TabIndex = 117;
            this.btnCompilaZasm.Text = "Compila con zasm";
            this.btnCompilaZasm.UseVisualStyleBackColor = true;
            this.btnCompilaZasm.Click += new System.EventHandler(this.btnCompilaZasm_Click);
            // 
            // txbAsmFile
            // 
            this.txbAsmFile.Location = new System.Drawing.Point(127, 6);
            this.txbAsmFile.Name = "txbAsmFile";
            this.txbAsmFile.ReadOnly = true;
            this.txbAsmFile.Size = new System.Drawing.Size(232, 20);
            this.txbAsmFile.TabIndex = 116;
            this.txbAsmFile.Click += new System.EventHandler(this.txbAsmFile_Click);
            // 
            // btnLoop
            // 
            this.btnLoop.Location = new System.Drawing.Point(11, 250);
            this.btnLoop.Name = "btnLoop";
            this.btnLoop.Size = new System.Drawing.Size(102, 22);
            this.btnLoop.TabIndex = 0;
            this.btnLoop.Text = "Test Link";
            this.btnLoop.UseVisualStyleBackColor = true;
            this.btnLoop.Click += new System.EventHandler(this.btnLoop_Click);
            // 
            // tabFirmware
            // 
            this.tabFirmware.Controls.Add(this.btnVerifyFirmware);
            this.tabFirmware.Controls.Add(this.btnStartFirmware);
            this.tabFirmware.Controls.Add(this.groupBox2);
            this.tabFirmware.Controls.Add(this.btnUploadFirmware);
            this.tabFirmware.Controls.Add(this.groupBox1);
            this.tabFirmware.Controls.Add(this.btnLeggiFirmware);
            this.tabFirmware.Controls.Add(this.btnEnterBootLoader);
            this.tabFirmware.Location = new System.Drawing.Point(4, 22);
            this.tabFirmware.Name = "tabFirmware";
            this.tabFirmware.Padding = new System.Windows.Forms.Padding(3);
            this.tabFirmware.Size = new System.Drawing.Size(819, 327);
            this.tabFirmware.TabIndex = 1;
            this.tabFirmware.Text = "Firmware";
            this.tabFirmware.UseVisualStyleBackColor = true;
            // 
            // btnVerifyFirmware
            // 
            this.btnVerifyFirmware.Enabled = false;
            this.btnVerifyFirmware.Location = new System.Drawing.Point(8, 90);
            this.btnVerifyFirmware.Name = "btnVerifyFirmware";
            this.btnVerifyFirmware.Size = new System.Drawing.Size(124, 22);
            this.btnVerifyFirmware.TabIndex = 9;
            this.btnVerifyFirmware.Text = "Verify Firmware";
            this.btnVerifyFirmware.UseVisualStyleBackColor = true;
            this.btnVerifyFirmware.Click += new System.EventHandler(this.btnVerifyFirmware_Click);
            // 
            // btnStartFirmware
            // 
            this.btnStartFirmware.Enabled = false;
            this.btnStartFirmware.Location = new System.Drawing.Point(8, 118);
            this.btnStartFirmware.Name = "btnStartFirmware";
            this.btnStartFirmware.Size = new System.Drawing.Size(124, 22);
            this.btnStartFirmware.TabIndex = 8;
            this.btnStartFirmware.Text = "Start Firmware";
            this.btnStartFirmware.UseVisualStyleBackColor = true;
            this.btnStartFirmware.Click += new System.EventHandler(this.btnStartFirmware_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblPicTypeDeviceOnPC);
            this.groupBox2.Controls.Add(this.lblNomeFirmwareOnPC);
            this.groupBox2.Controls.Add(this.lblVerDeviceOnPC);
            this.groupBox2.Location = new System.Drawing.Point(156, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(209, 68);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sul PC";
            // 
            // lblPicTypeDeviceOnPC
            // 
            this.lblPicTypeDeviceOnPC.AutoSize = true;
            this.lblPicTypeDeviceOnPC.Location = new System.Drawing.Point(6, 41);
            this.lblPicTypeDeviceOnPC.Name = "lblPicTypeDeviceOnPC";
            this.lblPicTypeDeviceOnPC.Size = new System.Drawing.Size(58, 13);
            this.lblPicTypeDeviceOnPC.TabIndex = 3;
            this.lblPicTypeDeviceOnPC.Text = "PicType: --";
            // 
            // lblNomeFirmwareOnPC
            // 
            this.lblNomeFirmwareOnPC.AutoSize = true;
            this.lblNomeFirmwareOnPC.Location = new System.Drawing.Point(6, 15);
            this.lblNomeFirmwareOnPC.Name = "lblNomeFirmwareOnPC";
            this.lblNomeFirmwareOnPC.Size = new System.Drawing.Size(47, 13);
            this.lblNomeFirmwareOnPC.TabIndex = 4;
            this.lblNomeFirmwareOnPC.Text = "Nome: --";
            // 
            // lblVerDeviceOnPC
            // 
            this.lblVerDeviceOnPC.AutoSize = true;
            this.lblVerDeviceOnPC.Location = new System.Drawing.Point(6, 28);
            this.lblVerDeviceOnPC.Name = "lblVerDeviceOnPC";
            this.lblVerDeviceOnPC.Size = new System.Drawing.Size(69, 13);
            this.lblVerDeviceOnPC.TabIndex = 2;
            this.lblVerDeviceOnPC.Text = "Versione: --.--";
            // 
            // btnUploadFirmware
            // 
            this.btnUploadFirmware.Enabled = false;
            this.btnUploadFirmware.Location = new System.Drawing.Point(8, 62);
            this.btnUploadFirmware.Name = "btnUploadFirmware";
            this.btnUploadFirmware.Size = new System.Drawing.Size(124, 22);
            this.btnUploadFirmware.TabIndex = 6;
            this.btnUploadFirmware.Text = "Upload Firmware";
            this.btnUploadFirmware.UseVisualStyleBackColor = true;
            this.btnUploadFirmware.Click += new System.EventHandler(this.btnUploadFirmware_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblPicTypeDevice);
            this.groupBox1.Controls.Add(this.lblNomeFirmware);
            this.groupBox1.Controls.Add(this.lblVerDevice);
            this.groupBox1.Location = new System.Drawing.Point(390, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 68);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sul Device";
            // 
            // lblPicTypeDevice
            // 
            this.lblPicTypeDevice.AutoSize = true;
            this.lblPicTypeDevice.Location = new System.Drawing.Point(6, 41);
            this.lblPicTypeDevice.Name = "lblPicTypeDevice";
            this.lblPicTypeDevice.Size = new System.Drawing.Size(58, 13);
            this.lblPicTypeDevice.TabIndex = 3;
            this.lblPicTypeDevice.Text = "PicType: --";
            // 
            // lblNomeFirmware
            // 
            this.lblNomeFirmware.AutoSize = true;
            this.lblNomeFirmware.Location = new System.Drawing.Point(6, 15);
            this.lblNomeFirmware.Name = "lblNomeFirmware";
            this.lblNomeFirmware.Size = new System.Drawing.Size(47, 13);
            this.lblNomeFirmware.TabIndex = 4;
            this.lblNomeFirmware.Text = "Nome: --";
            // 
            // lblVerDevice
            // 
            this.lblVerDevice.AutoSize = true;
            this.lblVerDevice.Location = new System.Drawing.Point(6, 28);
            this.lblVerDevice.Name = "lblVerDevice";
            this.lblVerDevice.Size = new System.Drawing.Size(69, 13);
            this.lblVerDevice.TabIndex = 2;
            this.lblVerDevice.Text = "Versione: --.--";
            // 
            // btnLeggiFirmware
            // 
            this.btnLeggiFirmware.Enabled = false;
            this.btnLeggiFirmware.Location = new System.Drawing.Point(8, 34);
            this.btnLeggiFirmware.Name = "btnLeggiFirmware";
            this.btnLeggiFirmware.Size = new System.Drawing.Size(124, 22);
            this.btnLeggiFirmware.TabIndex = 1;
            this.btnLeggiFirmware.Text = "File Firmware";
            this.btnLeggiFirmware.UseVisualStyleBackColor = true;
            this.btnLeggiFirmware.Click += new System.EventHandler(this.btnUploadFirm_Click);
            // 
            // btnEnterBootLoader
            // 
            this.btnEnterBootLoader.Location = new System.Drawing.Point(8, 6);
            this.btnEnterBootLoader.Name = "btnEnterBootLoader";
            this.btnEnterBootLoader.Size = new System.Drawing.Size(124, 22);
            this.btnEnterBootLoader.TabIndex = 0;
            this.btnEnterBootLoader.Text = "Entra in bootloader";
            this.btnEnterBootLoader.UseVisualStyleBackColor = true;
            this.btnEnterBootLoader.Click += new System.EventHandler(this.btnEnterBootLoader_Click);
            // 
            // monitor
            // 
            this.monitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitor.Location = new System.Drawing.Point(0, 0);
            this.monitor.Name = "monitor";
            this.monitor.Size = new System.Drawing.Size(827, 247);
            this.monitor.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DashBoardEmulEP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 650);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DashBoardEmulEP";
            this.Text = "Dashboard EmulEP Ver 2.15";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DashBoardEmulEP_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabEmulEP.ResumeLayout(false);
            this.tabEmulEP.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStopBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartBin)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBaseEP)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBaseBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResetValue)).EndInit();
            this.tabFirmware.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
               
        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Timer tmrInterfaccia;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabEmulEP;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private Monitor monitor;
        private System.Windows.Forms.ToolStripMenuItem interfacciaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FTDIToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTxtBoxDescrittore;
        private System.Windows.Forms.Button btnLoop;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tabFirmware;
        private System.Windows.Forms.Button btnEnterBootLoader;
        private System.Windows.Forms.Button btnLeggiFirmware;
        private System.Windows.Forms.Label lblVerDevice;
        private System.Windows.Forms.Label lblPicTypeDevice;
        private System.Windows.Forms.Label lblNomeFirmware;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnUploadFirmware;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblPicTypeDeviceOnPC;
        private System.Windows.Forms.Label lblNomeFirmwareOnPC;
        private System.Windows.Forms.Label lblVerDeviceOnPC;
        private System.Windows.Forms.Button btnStartFirmware;
        private System.Windows.Forms.CheckBox chbAutoAssembler;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtbArgomentiZasm;
        private System.Windows.Forms.Button btnCompilaZasm;
        private System.Windows.Forms.TextBox txbAsmFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txbHexFile;
        private System.Windows.Forms.CheckBox ckbResetOnLoad;
        public System.Windows.Forms.NumericUpDown nudResetValue;
        public System.Windows.Forms.Button btnResetMem;
        private System.Windows.Forms.CheckBox chbAutoLoad;
        public System.Windows.Forms.NumericUpDown nudBaseBin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbBinFile;
        private System.Windows.Forms.Button btnEditMem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSalvaBin;
        public System.Windows.Forms.NumericUpDown nudStartBin;
        public System.Windows.Forms.NumericUpDown nudStopBin;
        private System.Windows.Forms.Button btnSalvaHex;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown nudBaseEP;
        private System.Windows.Forms.Button btnLoadAndRun;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbSizeEP;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbBancoRamLow;
        private System.Windows.Forms.RadioButton rbBancoRamHigh;
        private System.Windows.Forms.ToolStripMenuItem setPreferenceToolStripMenuItem;
        private System.Windows.Forms.CheckBox ckbVerifyRam;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ToolStripMenuItem setPreferenceToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem salvaPreferenceOnExitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ToolStripMenuItem loadPreferenceToolStripMenuItem;
        private System.Windows.Forms.Button btnVerifyFirmware;
        private System.Windows.Forms.CheckBox ckbLoadAllRam;
        private System.Windows.Forms.Label lblRangeEprom;
        private System.Windows.Forms.ToolStripMenuItem istruzioniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem istruzioniToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem schemaElettricoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uSBPortLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripFTDIlocation;
        private System.Windows.Forms.Button btnResetRun;
        private System.Windows.Forms.Label lblStatoCPU;
    }
}

