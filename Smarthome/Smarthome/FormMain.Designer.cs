namespace Smarthome
{
    partial class FormMain
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            hTToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            menuLogout = new ToolStripMenuItem();
            menuExit = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            menuConnect = new ToolStripMenuItem();
            menuDisconnect = new ToolStripMenuItem();
            menuStatus = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            menuSensorData = new ToolStripMenuItem();
            menuAlert = new ToolStripMenuItem();
            menuCommand = new ToolStripMenuItem();
            menuAccess = new ToolStripMenuItem();
            toolStripMenuItem13 = new ToolStripMenuItem();
            menuUser = new ToolStripMenuItem();
            menuConfig = new ToolStripMenuItem();
            trợGiúpToolStripMenuItem = new ToolStripMenuItem();
            menuAbout = new ToolStripMenuItem();
            pnlHeader = new Panel();
            lblCurrentUser = new Label();
            lblRole = new Label();
            lblClock = new Label();
            lblAppTitle = new Label();
            tabLog = new TabPage();
            tabConfig = new TabPage();
            tabUser = new TabPage();
            tabAccess = new TabPage();
            tabSensorData = new TabPage();
            grpSensorFilter = new GroupBox();
            lblFromDate = new Label();
            lblToDate = new Label();
            dtpToDate = new DateTimePicker();
            btnRefreshSensor = new Button();
            dateTimePicker4 = new DateTimePicker();
            dataGridView3 = new DataGridView();
            colDoor = new DataGridViewTextBoxColumn();
            colFan = new DataGridViewTextBoxColumn();
            colPir = new DataGridViewTextBoxColumn();
            colRain = new DataGridViewTextBoxColumn();
            colTemp = new DataGridViewTextBoxColumn();
            colGas = new DataGridViewTextBoxColumn();
            colTime1 = new DataGridViewTextBoxColumn();
            colId1 = new DataGridViewTextBoxColumn();
            tabAlert = new TabPage();
            grpCurrentAlert = new GroupBox();
            pnlAlertLevel = new Panel();
            lblCurrentLevel = new Label();
            lblCurrentAlertTitle = new Label();
            lblCurrentAlertMessage = new Label();
            lblCurrentTimeTitle = new Label();
            lblCurrentTime = new Label();
            grpAlertFilter = new GroupBox();
            labelFrom = new Label();
            labelType = new Label();
            dateTimePicker1 = new DateTimePicker();
            dtpAlertFrom = new Label();
            dateTimePicker2 = new DateTimePicker();
            labelLevel = new Label();
            comboBox1 = new ComboBox();
            cboAlertType = new ComboBox();
            btnSearchAlert = new Button();
            btnRefreshAlert = new Button();
            grpAlertList = new GroupBox();
            dataGridView2 = new DataGridView();
            colHandleTime = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colLevel = new DataGridViewTextBoxColumn();
            colType = new DataGridViewTextBoxColumn();
            colTime = new DataGridViewTextBoxColumn();
            colId = new DataGridViewTextBoxColumn();
            btnResolveAlert = new Button();
            btnDeleteAlert = new Button();
            tabControl = new TabPage();
            grpConnection = new GroupBox();
            cboCom = new ComboBox();
            btnRefreshCom = new Button();
            btnDisconnect = new Button();
            btnConnect = new Button();
            lblComStatus = new Label();
            label34 = new Label();
            label35 = new Label();
            grpDeviceControl = new GroupBox();
            btnFanOn = new Button();
            btnFanOff = new Button();
            button1 = new Button();
            btnResetAlarm = new Button();
            btnWindowOpen = new Button();
            btnWindowClose = new Button();
            btnDoorClose = new Button();
            btnDoorOpen = new Button();
            btnWindowStop = new Button();
            btnDoorStop = new Button();
            btnBuzzerOff = new Button();
            btnBuzzerOn = new Button();
            grpMode = new GroupBox();
            btnModeAuto = new Button();
            btnModeManual = new Button();
            btnStatus = new Button();
            grpCommandStatus = new GroupBox();
            lblLastCommand = new Label();
            lblCommandResult = new Label();
            lblLastCommandTitle = new Label();
            lblCommandResultTitle = new Label();
            tabDashboard = new TabPage();
            grpSystemStatus = new GroupBox();
            pnlSystemStatus = new Panel();
            lblLevel = new Label();
            lblMessage = new Label();
            lblMessageTitle = new Label();
            label1 = new Label();
            grpSensor = new GroupBox();
            label2 = new Label();
            label3 = new Label();
            label5 = new Label();
            lblRain = new Label();
            label7 = new Label();
            lblTemp = new Label();
            label9 = new Label();
            lblGas = new Label();
            grpRealtime = new GroupBox();
            dataGridView1 = new DataGridView();
            grpDevice = new GroupBox();
            label4 = new Label();
            lblFan = new Label();
            label8 = new Label();
            lblWindow = new Label();
            label11 = new Label();
            lblDoor = new Label();
            label13 = new Label();
            lblBuzzer = new Label();
            label15 = new Label();
            lblMode = new Label();
            tabMain = new TabControl();
            grpAccessFilter = new GroupBox();
            label6 = new Label();
            cboAccessStatus = new ComboBox();
            btnRefreshAccess = new Button();
            dgvAccess = new DataGridView();
            colTime2 = new DataGridViewTextBoxColumn();
            colPassword = new DataGridViewTextBoxColumn();
            colStatus2 = new DataGridViewTextBoxColumn();
            colMessage = new DataGridViewTextBoxColumn();
            grpUserInfo = new GroupBox();
            label10 = new Label();
            label12 = new Label();
            label14 = new Label();
            textBox1 = new TextBox();
            txtPassword = new TextBox();
            cboRole = new ComboBox();
            btnAddUser = new Button();
            button3 = new Button();
            btnRefreshUser = new Button();
            btnDeleteUser = new Button();
            btnEditUser = new Button();
            dgvUser = new DataGridView();
            colId3 = new DataGridViewTextBoxColumn();
            colUsername = new DataGridViewTextBoxColumn();
            colPassword3 = new DataGridViewTextBoxColumn();
            colRole = new DataGridViewTextBoxColumn();
            grpConfig = new GroupBox();
            label16 = new Label();
            label17 = new Label();
            label18 = new Label();
            label19 = new Label();
            label20 = new Label();
            label21 = new Label();
            label22 = new Label();
            numGasWarning = new NumericUpDown();
            numTempDanger = new NumericUpDown();
            numTempWarning = new NumericUpDown();
            numGasDanger = new NumericUpDown();
            boBaudrate = new ComboBox();
            cboDefaultMode = new ComboBox();
            cboComPort = new ComboBox();
            btnSaveConfig = new Button();
            btnResetConfig = new Button();
            textBox2 = new TextBox();
            panel1 = new Panel();
            btnClearLog = new Button();
            menuStrip1.SuspendLayout();
            pnlHeader.SuspendLayout();
            tabLog.SuspendLayout();
            tabConfig.SuspendLayout();
            tabUser.SuspendLayout();
            tabAccess.SuspendLayout();
            tabSensorData.SuspendLayout();
            grpSensorFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            tabAlert.SuspendLayout();
            grpCurrentAlert.SuspendLayout();
            pnlAlertLevel.SuspendLayout();
            grpAlertFilter.SuspendLayout();
            grpAlertList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            tabControl.SuspendLayout();
            grpConnection.SuspendLayout();
            grpDeviceControl.SuspendLayout();
            grpMode.SuspendLayout();
            grpCommandStatus.SuspendLayout();
            tabDashboard.SuspendLayout();
            grpSystemStatus.SuspendLayout();
            pnlSystemStatus.SuspendLayout();
            grpSensor.SuspendLayout();
            grpRealtime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            grpDevice.SuspendLayout();
            tabMain.SuspendLayout();
            grpAccessFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAccess).BeginInit();
            grpUserInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUser).BeginInit();
            grpConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numGasWarning).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTempDanger).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTempWarning).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numGasDanger).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { hTToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1234, 33);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // hTToolStripMenuItem
            // 
            hTToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3, toolStripMenuItem13, trợGiúpToolStripMenuItem });
            hTToolStripMenuItem.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
            hTToolStripMenuItem.Name = "hTToolStripMenuItem";
            hTToolStripMenuItem.Size = new Size(78, 29);
            hTToolStripMenuItem.Text = "MENU";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { menuLogout, menuExit });
            toolStripMenuItem1.Font = new Font("Segoe UI", 11.25F);
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(143, 24);
            toolStripMenuItem1.Text = "Hệ thống";
            // 
            // menuLogout
            // 
            menuLogout.Name = "menuLogout";
            menuLogout.Size = new Size(146, 24);
            menuLogout.Text = "Đăng xuất";
            // 
            // menuExit
            // 
            menuExit.Name = "menuExit";
            menuExit.Size = new Size(146, 24);
            menuExit.Text = "Thoát";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { menuConnect, menuDisconnect, menuStatus });
            toolStripMenuItem2.Font = new Font("Segoe UI", 11.25F);
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(143, 24);
            toolStripMenuItem2.Text = "Thiết bị";
            // 
            // menuConnect
            // 
            menuConnect.Name = "menuConnect";
            menuConnect.Size = new Size(168, 44);
            menuConnect.Text = "Kết nối COM\n";
            // 
            // menuDisconnect
            // 
            menuDisconnect.Name = "menuDisconnect";
            menuDisconnect.Size = new Size(168, 44);
            menuDisconnect.Text = "Ngắt kết nối";
            // 
            // menuStatus
            // 
            menuStatus.Name = "menuStatus";
            menuStatus.Size = new Size(168, 44);
            menuStatus.Text = "Lấy trạng thái";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.DropDownItems.AddRange(new ToolStripItem[] { menuSensorData, menuAlert, menuCommand, menuAccess });
            toolStripMenuItem3.Font = new Font("Segoe UI", 11.25F);
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(143, 24);
            toolStripMenuItem3.Text = "Dữ liệu";
            // 
            // menuSensorData
            // 
            menuSensorData.Name = "menuSensorData";
            menuSensorData.Size = new Size(191, 24);
            menuSensorData.Text = "Dữ liệu cảm biến";
            // 
            // menuAlert
            // 
            menuAlert.Name = "menuAlert";
            menuAlert.Size = new Size(191, 24);
            menuAlert.Text = "Cảnh báo";
            // 
            // menuCommand
            // 
            menuCommand.Name = "menuCommand";
            menuCommand.Size = new Size(191, 24);
            menuCommand.Text = "Lệnh điều khiển";
            // 
            // menuAccess
            // 
            menuAccess.Name = "menuAccess";
            menuAccess.Size = new Size(191, 24);
            menuAccess.Text = "Truy cập cửa";
            // 
            // toolStripMenuItem13
            // 
            toolStripMenuItem13.DropDownItems.AddRange(new ToolStripItem[] { menuUser, menuConfig });
            toolStripMenuItem13.Font = new Font("Segoe UI", 11.25F);
            toolStripMenuItem13.Name = "toolStripMenuItem13";
            toolStripMenuItem13.Size = new Size(143, 24);
            toolStripMenuItem13.Text = "Quản trị";
            // 
            // menuUser
            // 
            menuUser.Name = "menuUser";
            menuUser.Size = new Size(158, 24);
            menuUser.Text = "Người dùng";
            // 
            // menuConfig
            // 
            menuConfig.Name = "menuConfig";
            menuConfig.Size = new Size(158, 24);
            menuConfig.Text = "Cấu hình";
            // 
            // trợGiúpToolStripMenuItem
            // 
            trợGiúpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuAbout });
            trợGiúpToolStripMenuItem.Font = new Font("Segoe UI", 11.25F);
            trợGiúpToolStripMenuItem.Name = "trợGiúpToolStripMenuItem";
            trợGiúpToolStripMenuItem.Size = new Size(143, 24);
            trợGiúpToolStripMenuItem.Text = "Trợ giúp";
            // 
            // menuAbout
            // 
            menuAbout.Name = "menuAbout";
            menuAbout.Size = new Size(142, 24);
            menuAbout.Text = "Giới thiệu";
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(lblCurrentUser);
            pnlHeader.Controls.Add(lblRole);
            pnlHeader.Controls.Add(lblClock);
            pnlHeader.Controls.Add(lblAppTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 33);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1234, 80);
            pnlHeader.TabIndex = 1;
            // 
            // lblCurrentUser
            // 
            lblCurrentUser.AutoSize = true;
            lblCurrentUser.Font = new Font("Microsoft Sans Serif", 12F);
            lblCurrentUser.Location = new Point(12, 46);
            lblCurrentUser.Name = "lblCurrentUser";
            lblCurrentUser.Size = new Size(94, 20);
            lblCurrentUser.TabIndex = 3;
            lblCurrentUser.Text = "User: admin";
            // 
            // lblRole
            // 
            lblRole.AutoSize = true;
            lblRole.Font = new Font("Microsoft Sans Serif", 12F);
            lblRole.Location = new Point(130, 46);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(95, 20);
            lblRole.TabIndex = 2;
            lblRole.Text = "Role: Admin";
            // 
            // lblClock
            // 
            lblClock.AutoSize = true;
            lblClock.Font = new Font("Microsoft Sans Serif", 12F);
            lblClock.Location = new Point(261, 46);
            lblClock.Name = "lblClock";
            lblClock.Size = new Size(89, 20);
            lblClock.TabIndex = 1;
            lblClock.Text = "Time: --:--:--";
            // 
            // lblAppTitle
            // 
            lblAppTitle.AutoSize = true;
            lblAppTitle.Font = new Font("Microsoft Sans Serif", 12F);
            lblAppTitle.Location = new Point(12, 13);
            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.Size = new Size(361, 20);
            lblAppTitle.TabIndex = 0;
            lblAppTitle.Text = "SMART HOME SAFETY MONITORING SYSTEM";
            // 
            // tabLog
            // 
            tabLog.Controls.Add(panel1);
            tabLog.Controls.Add(textBox2);
            tabLog.Location = new Point(4, 24);
            tabLog.Name = "tabLog";
            tabLog.Padding = new Padding(3);
            tabLog.Size = new Size(1202, 479);
            tabLog.TabIndex = 8;
            tabLog.Text = "Log Serial";
            tabLog.UseVisualStyleBackColor = true;
            // 
            // tabConfig
            // 
            tabConfig.Controls.Add(grpConfig);
            tabConfig.Font = new Font("Segoe UI", 12F);
            tabConfig.Location = new Point(4, 24);
            tabConfig.Name = "tabConfig";
            tabConfig.Padding = new Padding(3);
            tabConfig.Size = new Size(1202, 479);
            tabConfig.TabIndex = 7;
            tabConfig.Text = "Cấu hình";
            tabConfig.UseVisualStyleBackColor = true;
            // 
            // tabUser
            // 
            tabUser.Controls.Add(dgvUser);
            tabUser.Controls.Add(grpUserInfo);
            tabUser.Location = new Point(4, 24);
            tabUser.Name = "tabUser";
            tabUser.Padding = new Padding(3);
            tabUser.Size = new Size(1202, 479);
            tabUser.TabIndex = 6;
            tabUser.Text = "Người dùng";
            tabUser.UseVisualStyleBackColor = true;
            // 
            // tabAccess
            // 
            tabAccess.Controls.Add(dgvAccess);
            tabAccess.Controls.Add(grpAccessFilter);
            tabAccess.Location = new Point(4, 24);
            tabAccess.Name = "tabAccess";
            tabAccess.Padding = new Padding(3);
            tabAccess.Size = new Size(1202, 479);
            tabAccess.TabIndex = 5;
            tabAccess.Text = "Truy cập cửa";
            tabAccess.UseVisualStyleBackColor = true;
            // 
            // tabSensorData
            // 
            tabSensorData.Controls.Add(dataGridView3);
            tabSensorData.Controls.Add(grpSensorFilter);
            tabSensorData.Location = new Point(4, 24);
            tabSensorData.Name = "tabSensorData";
            tabSensorData.Padding = new Padding(3);
            tabSensorData.Size = new Size(1202, 479);
            tabSensorData.TabIndex = 3;
            tabSensorData.Text = "Dữ liệu cảm biến";
            tabSensorData.UseVisualStyleBackColor = true;
            // 
            // grpSensorFilter
            // 
            grpSensorFilter.Controls.Add(dateTimePicker4);
            grpSensorFilter.Controls.Add(btnRefreshSensor);
            grpSensorFilter.Controls.Add(dtpToDate);
            grpSensorFilter.Controls.Add(lblToDate);
            grpSensorFilter.Controls.Add(lblFromDate);
            grpSensorFilter.Dock = DockStyle.Top;
            grpSensorFilter.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            grpSensorFilter.Location = new Point(3, 3);
            grpSensorFilter.Name = "grpSensorFilter";
            grpSensorFilter.Size = new Size(1196, 93);
            grpSensorFilter.TabIndex = 0;
            grpSensorFilter.TabStop = false;
            grpSensorFilter.Text = "Bộ lọc";
            // 
            // lblFromDate
            // 
            lblFromDate.AutoSize = true;
            lblFromDate.Location = new Point(36, 19);
            lblFromDate.Name = "lblFromDate";
            lblFromDate.Size = new Size(68, 21);
            lblFromDate.TabIndex = 0;
            lblFromDate.Text = "Từ ngày:";
            // 
            // lblToDate
            // 
            lblToDate.AutoSize = true;
            lblToDate.Location = new Point(29, 51);
            lblToDate.Name = "lblToDate";
            lblToDate.Size = new Size(79, 21);
            lblToDate.TabIndex = 1;
            lblToDate.Text = "Đến ngày:";
            // 
            // dtpToDate
            // 
            dtpToDate.Location = new Point(111, 16);
            dtpToDate.Name = "dtpToDate";
            dtpToDate.Size = new Size(200, 29);
            dtpToDate.TabIndex = 2;
            // 
            // btnRefreshSensor
            // 
            btnRefreshSensor.Location = new Point(477, 28);
            btnRefreshSensor.Name = "btnRefreshSensor";
            btnRefreshSensor.Size = new Size(153, 28);
            btnRefreshSensor.TabIndex = 3;
            btnRefreshSensor.Text = "Làm mới";
            btnRefreshSensor.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker4
            // 
            dateTimePicker4.Location = new Point(111, 51);
            dateTimePicker4.Name = "dateTimePicker4";
            dateTimePicker4.Size = new Size(200, 29);
            dateTimePicker4.TabIndex = 5;
            // 
            // dataGridView3
            // 
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView3.Columns.AddRange(new DataGridViewColumn[] { colId1, colTime1, colGas, colTemp, colRain, colPir, colFan, colDoor });
            dataGridView3.Dock = DockStyle.Fill;
            dataGridView3.Location = new Point(3, 96);
            dataGridView3.Name = "dataGridView3";
            dataGridView3.Size = new Size(1196, 380);
            dataGridView3.TabIndex = 1;
            // 
            // colDoor
            // 
            colDoor.HeaderText = "Cửa";
            colDoor.Name = "colDoor";
            // 
            // colFan
            // 
            colFan.HeaderText = "Quạt";
            colFan.Name = "colFan";
            // 
            // colPir
            // 
            colPir.HeaderText = "PIR";
            colPir.Name = "colPir";
            // 
            // colRain
            // 
            colRain.HeaderText = "Mưa";
            colRain.Name = "colRain";
            // 
            // colTemp
            // 
            colTemp.HeaderText = "Nhiệt độ";
            colTemp.Name = "colTemp";
            // 
            // colGas
            // 
            colGas.HeaderText = "Gas";
            colGas.Name = "colGas";
            // 
            // colTime1
            // 
            colTime1.HeaderText = "Thời gian";
            colTime1.Name = "colTime1";
            // 
            // colId1
            // 
            colId1.HeaderText = "ID";
            colId1.Name = "colId1";
            // 
            // tabAlert
            // 
            tabAlert.Controls.Add(btnDeleteAlert);
            tabAlert.Controls.Add(btnResolveAlert);
            tabAlert.Controls.Add(grpAlertList);
            tabAlert.Controls.Add(grpAlertFilter);
            tabAlert.Controls.Add(grpCurrentAlert);
            tabAlert.Font = new Font("Segoe UI", 12F);
            tabAlert.Location = new Point(4, 24);
            tabAlert.Name = "tabAlert";
            tabAlert.Padding = new Padding(3);
            tabAlert.Size = new Size(1202, 479);
            tabAlert.TabIndex = 2;
            tabAlert.Text = "Cảnh báo";
            tabAlert.UseVisualStyleBackColor = true;
            // 
            // grpCurrentAlert
            // 
            grpCurrentAlert.Controls.Add(lblCurrentTime);
            grpCurrentAlert.Controls.Add(lblCurrentTimeTitle);
            grpCurrentAlert.Controls.Add(lblCurrentAlertMessage);
            grpCurrentAlert.Controls.Add(lblCurrentAlertTitle);
            grpCurrentAlert.Controls.Add(pnlAlertLevel);
            grpCurrentAlert.Font = new Font("Segoe UI", 12F);
            grpCurrentAlert.Location = new Point(19, 6);
            grpCurrentAlert.Name = "grpCurrentAlert";
            grpCurrentAlert.Size = new Size(576, 149);
            grpCurrentAlert.TabIndex = 0;
            grpCurrentAlert.TabStop = false;
            grpCurrentAlert.Text = "Cảnh báo hiện tại\n";
            // 
            // pnlAlertLevel
            // 
            pnlAlertLevel.Controls.Add(lblCurrentLevel);
            pnlAlertLevel.Location = new Point(24, 28);
            pnlAlertLevel.Name = "pnlAlertLevel";
            pnlAlertLevel.Size = new Size(177, 48);
            pnlAlertLevel.TabIndex = 0;
            // 
            // lblCurrentLevel
            // 
            lblCurrentLevel.AutoSize = true;
            lblCurrentLevel.Location = new Point(41, 14);
            lblCurrentLevel.Name = "lblCurrentLevel";
            lblCurrentLevel.Size = new Size(94, 21);
            lblCurrentLevel.TabIndex = 0;
            lblCurrentLevel.Text = "NGUY HIEM";
            lblCurrentLevel.Click += lblCurrentLevel_Click;
            // 
            // lblCurrentAlertTitle
            // 
            lblCurrentAlertTitle.AutoSize = true;
            lblCurrentAlertTitle.Location = new Point(24, 90);
            lblCurrentAlertTitle.Name = "lblCurrentAlertTitle";
            lblCurrentAlertTitle.Size = new Size(145, 21);
            lblCurrentAlertTitle.TabIndex = 1;
            lblCurrentAlertTitle.Text = "Nội dung cảnh báo:";
            // 
            // lblCurrentAlertMessage
            // 
            lblCurrentAlertMessage.AutoSize = true;
            lblCurrentAlertMessage.Location = new Point(175, 90);
            lblCurrentAlertMessage.Name = "lblCurrentAlertMessage";
            lblCurrentAlertMessage.Size = new Size(185, 21);
            lblCurrentAlertMessage.TabIndex = 2;
            lblCurrentAlertMessage.Text = "KHI GAS VUOT NGUONG";
            // 
            // lblCurrentTimeTitle
            // 
            lblCurrentTimeTitle.AutoSize = true;
            lblCurrentTimeTitle.Location = new Point(81, 127);
            lblCurrentTimeTitle.Name = "lblCurrentTimeTitle";
            lblCurrentTimeTitle.Size = new Size(78, 21);
            lblCurrentTimeTitle.TabIndex = 3;
            lblCurrentTimeTitle.Text = "Thời gian:";
            // 
            // lblCurrentTime
            // 
            lblCurrentTime.AutoSize = true;
            lblCurrentTime.Location = new Point(168, 128);
            lblCurrentTime.Name = "lblCurrentTime";
            lblCurrentTime.Size = new Size(28, 21);
            lblCurrentTime.TabIndex = 4;
            lblCurrentTime.Text = "---";
            // 
            // grpAlertFilter
            // 
            grpAlertFilter.Controls.Add(btnRefreshAlert);
            grpAlertFilter.Controls.Add(btnSearchAlert);
            grpAlertFilter.Controls.Add(cboAlertType);
            grpAlertFilter.Controls.Add(comboBox1);
            grpAlertFilter.Controls.Add(labelLevel);
            grpAlertFilter.Controls.Add(dateTimePicker2);
            grpAlertFilter.Controls.Add(dtpAlertFrom);
            grpAlertFilter.Controls.Add(dateTimePicker1);
            grpAlertFilter.Controls.Add(labelType);
            grpAlertFilter.Controls.Add(labelFrom);
            grpAlertFilter.Font = new Font("Segoe UI", 12F);
            grpAlertFilter.Location = new Point(610, 6);
            grpAlertFilter.Name = "grpAlertFilter";
            grpAlertFilter.Size = new Size(586, 165);
            grpAlertFilter.TabIndex = 1;
            grpAlertFilter.TabStop = false;
            grpAlertFilter.Text = "Bộ lọc cảnh báo";
            // 
            // labelFrom
            // 
            labelFrom.AutoSize = true;
            labelFrom.Location = new Point(27, 38);
            labelFrom.Name = "labelFrom";
            labelFrom.Size = new Size(68, 21);
            labelFrom.TabIndex = 0;
            labelFrom.Text = "Từ ngày:";
            // 
            // labelType
            // 
            labelType.AutoSize = true;
            labelType.Location = new Point(28, 126);
            labelType.Name = "labelType";
            labelType.Size = new Size(109, 21);
            labelType.TabIndex = 1;
            labelType.Text = "Loại cảnh báo:";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(89, 36);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(200, 29);
            dateTimePicker1.TabIndex = 2;
            // 
            // dtpAlertFrom
            // 
            dtpAlertFrom.AutoSize = true;
            dtpAlertFrom.Location = new Point(295, 38);
            dtpAlertFrom.Name = "dtpAlertFrom";
            dtpAlertFrom.Size = new Size(79, 21);
            dtpAlertFrom.TabIndex = 3;
            dtpAlertFrom.Text = "Đến ngày:";
            dtpAlertFrom.Click += dtpAlertFrom_Click;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(374, 36);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(200, 29);
            dateTimePicker2.TabIndex = 4;
            // 
            // labelLevel
            // 
            labelLevel.AutoSize = true;
            labelLevel.Location = new Point(27, 80);
            labelLevel.Name = "labelLevel";
            labelLevel.Size = new Size(110, 21);
            labelLevel.TabIndex = 5;
            labelLevel.Text = "Mức cảnh báo:";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "AN TOAN", "CANH BAO", "N", "GUY HIEM" });
            comboBox1.Location = new Point(140, 77);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 29);
            comboBox1.TabIndex = 6;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // cboAlertType
            // 
            cboAlertType.FormattingEnabled = true;
            cboAlertType.Items.AddRange(new object[] { "GAS", "", "TEMP", "", "RAIN", "", "PIR", "", "SECURITY", "", "SYSTEM" });
            cboAlertType.Location = new Point(140, 126);
            cboAlertType.Name = "cboAlertType";
            cboAlertType.Size = new Size(121, 29);
            cboAlertType.TabIndex = 7;
            // 
            // btnSearchAlert
            // 
            btnSearchAlert.Location = new Point(295, 120);
            btnSearchAlert.Name = "btnSearchAlert";
            btnSearchAlert.Size = new Size(126, 34);
            btnSearchAlert.TabIndex = 8;
            btnSearchAlert.Text = "Tìm kiếm";
            btnSearchAlert.UseVisualStyleBackColor = true;
            // 
            // btnRefreshAlert
            // 
            btnRefreshAlert.Location = new Point(431, 120);
            btnRefreshAlert.Name = "btnRefreshAlert";
            btnRefreshAlert.Size = new Size(126, 34);
            btnRefreshAlert.TabIndex = 9;
            btnRefreshAlert.Text = "Làm mới";
            btnRefreshAlert.UseVisualStyleBackColor = true;
            // 
            // grpAlertList
            // 
            grpAlertList.Controls.Add(dataGridView2);
            grpAlertList.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            grpAlertList.Location = new Point(7, 177);
            grpAlertList.Name = "grpAlertList";
            grpAlertList.Size = new Size(698, 181);
            grpAlertList.TabIndex = 2;
            grpAlertList.TabStop = false;
            grpAlertList.Text = "Danh sách cảnh báo";
            // 
            // dataGridView2
            // 
            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { colId, colTime, colType, colLevel, colStatus, colHandleTime });
            dataGridView2.Location = new Point(6, 28);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(642, 133);
            dataGridView2.TabIndex = 0;
            // 
            // colHandleTime
            // 
            colHandleTime.HeaderText = "Xử lý lúc";
            colHandleTime.Name = "colHandleTime";
            // 
            // colStatus
            // 
            colStatus.HeaderText = "Trạng thái";
            colStatus.Name = "colStatus";
            // 
            // colLevel
            // 
            colLevel.HeaderText = "Mức cảnh báo";
            colLevel.Name = "colLevel";
            // 
            // colType
            // 
            colType.HeaderText = "Loại";
            colType.Name = "colType";
            // 
            // colTime
            // 
            colTime.HeaderText = "Thời gian";
            colTime.Name = "colTime";
            // 
            // colId
            // 
            colId.HeaderText = "ID";
            colId.Name = "colId";
            // 
            // btnResolveAlert
            // 
            btnResolveAlert.Font = new Font("Segoe UI", 12F);
            btnResolveAlert.Location = new Point(766, 210);
            btnResolveAlert.Name = "btnResolveAlert";
            btnResolveAlert.Size = new Size(133, 38);
            btnResolveAlert.TabIndex = 3;
            btnResolveAlert.Text = "Xác nhận xử lý";
            btnResolveAlert.UseVisualStyleBackColor = true;
            // 
            // btnDeleteAlert
            // 
            btnDeleteAlert.Font = new Font("Segoe UI", 12F);
            btnDeleteAlert.Location = new Point(949, 210);
            btnDeleteAlert.Name = "btnDeleteAlert";
            btnDeleteAlert.Size = new Size(133, 38);
            btnDeleteAlert.TabIndex = 6;
            btnDeleteAlert.Text = "Xóa";
            btnDeleteAlert.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(grpCommandStatus);
            tabControl.Controls.Add(grpMode);
            tabControl.Controls.Add(grpDeviceControl);
            tabControl.Controls.Add(grpConnection);
            tabControl.Location = new Point(4, 24);
            tabControl.Name = "tabControl";
            tabControl.Padding = new Padding(3);
            tabControl.Size = new Size(1202, 479);
            tabControl.TabIndex = 1;
            tabControl.Text = "Điều khiển";
            tabControl.UseVisualStyleBackColor = true;
            // 
            // grpConnection
            // 
            grpConnection.Controls.Add(label35);
            grpConnection.Controls.Add(label34);
            grpConnection.Controls.Add(lblComStatus);
            grpConnection.Controls.Add(btnConnect);
            grpConnection.Controls.Add(btnDisconnect);
            grpConnection.Controls.Add(btnRefreshCom);
            grpConnection.Controls.Add(cboCom);
            grpConnection.Font = new Font("Segoe UI", 12F);
            grpConnection.Location = new Point(22, 15);
            grpConnection.Name = "grpConnection";
            grpConnection.Size = new Size(514, 224);
            grpConnection.TabIndex = 0;
            grpConnection.TabStop = false;
            grpConnection.Text = "Kết nối thiết bị";
            // 
            // cboCom
            // 
            cboCom.Font = new Font("Segoe UI", 11.25F);
            cboCom.FormattingEnabled = true;
            cboCom.Location = new Point(102, 34);
            cboCom.Name = "cboCom";
            cboCom.Size = new Size(121, 28);
            cboCom.TabIndex = 0;
            // 
            // btnRefreshCom
            // 
            btnRefreshCom.Font = new Font("Segoe UI", 11.25F);
            btnRefreshCom.Location = new Point(30, 75);
            btnRefreshCom.Name = "btnRefreshCom";
            btnRefreshCom.Size = new Size(116, 33);
            btnRefreshCom.TabIndex = 1;
            btnRefreshCom.Text = "Tải COM";
            btnRefreshCom.UseVisualStyleBackColor = true;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Font = new Font("Segoe UI", 11.25F);
            btnDisconnect.Location = new Point(326, 75);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(116, 33);
            btnDisconnect.TabIndex = 2;
            btnDisconnect.Text = "Ngắt kết nối";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnConnect
            // 
            btnConnect.Font = new Font("Segoe UI", 11.25F);
            btnConnect.Location = new Point(176, 75);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(116, 33);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Kết nối";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // lblComStatus
            // 
            lblComStatus.AutoSize = true;
            lblComStatus.Font = new Font("Segoe UI", 11.25F);
            lblComStatus.Location = new Point(110, 143);
            lblComStatus.Name = "lblComStatus";
            lblComStatus.Size = new Size(92, 20);
            lblComStatus.TabIndex = 4;
            lblComStatus.Text = "Chưa kết nối";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Font = new Font("Segoe UI", 11.25F);
            label34.Location = new Point(30, 143);
            label34.Name = "label34";
            label34.Size = new Size(78, 20);
            label34.TabIndex = 5;
            label34.Text = "Trạng thái:";
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new Point(6, 41);
            label35.Name = "label35";
            label35.Size = new Size(90, 21);
            label35.TabIndex = 6;
            label35.Text = "Cổng COM:";
            // 
            // grpDeviceControl
            // 
            grpDeviceControl.Controls.Add(btnBuzzerOn);
            grpDeviceControl.Controls.Add(btnBuzzerOff);
            grpDeviceControl.Controls.Add(btnDoorStop);
            grpDeviceControl.Controls.Add(btnWindowStop);
            grpDeviceControl.Controls.Add(btnDoorOpen);
            grpDeviceControl.Controls.Add(btnDoorClose);
            grpDeviceControl.Controls.Add(btnWindowClose);
            grpDeviceControl.Controls.Add(btnWindowOpen);
            grpDeviceControl.Controls.Add(btnResetAlarm);
            grpDeviceControl.Controls.Add(button1);
            grpDeviceControl.Controls.Add(btnFanOff);
            grpDeviceControl.Controls.Add(btnFanOn);
            grpDeviceControl.Font = new Font("Segoe UI", 12F);
            grpDeviceControl.Location = new Point(22, 260);
            grpDeviceControl.Name = "grpDeviceControl";
            grpDeviceControl.Size = new Size(514, 213);
            grpDeviceControl.TabIndex = 1;
            grpDeviceControl.TabStop = false;
            grpDeviceControl.Text = "Điều khiển thiết bị";
            // 
            // btnFanOn
            // 
            btnFanOn.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnFanOn.Location = new Point(15, 63);
            btnFanOn.Name = "btnFanOn";
            btnFanOn.Size = new Size(104, 29);
            btnFanOn.TabIndex = 0;
            btnFanOn.Text = "Bật quạt";
            btnFanOn.UseVisualStyleBackColor = true;
            // 
            // btnFanOff
            // 
            btnFanOff.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnFanOff.Location = new Point(15, 98);
            btnFanOff.Name = "btnFanOff";
            btnFanOff.Size = new Size(104, 29);
            btnFanOff.TabIndex = 2;
            btnFanOff.Text = "Tắt quạt";
            btnFanOff.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            button1.Location = new Point(167, 63);
            button1.Name = "button1";
            button1.Size = new Size(104, 29);
            button1.TabIndex = 3;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // btnResetAlarm
            // 
            btnResetAlarm.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnResetAlarm.Location = new Point(9, 28);
            btnResetAlarm.Name = "btnResetAlarm";
            btnResetAlarm.Size = new Size(110, 29);
            btnResetAlarm.TabIndex = 1;
            btnResetAlarm.Text = "Reset cảnh báo";
            btnResetAlarm.UseVisualStyleBackColor = true;
            // 
            // btnWindowOpen
            // 
            btnWindowOpen.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnWindowOpen.Location = new Point(136, 28);
            btnWindowOpen.Name = "btnWindowOpen";
            btnWindowOpen.Size = new Size(104, 29);
            btnWindowOpen.TabIndex = 4;
            btnWindowOpen.Text = "Mở cửa sổ";
            btnWindowOpen.UseVisualStyleBackColor = true;
            // 
            // btnWindowClose
            // 
            btnWindowClose.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnWindowClose.Location = new Point(136, 63);
            btnWindowClose.Name = "btnWindowClose";
            btnWindowClose.Size = new Size(104, 29);
            btnWindowClose.TabIndex = 5;
            btnWindowClose.Text = "Đóng cửa sổ";
            btnWindowClose.UseVisualStyleBackColor = true;
            // 
            // btnDoorClose
            // 
            btnDoorClose.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnDoorClose.Location = new Point(260, 63);
            btnDoorClose.Name = "btnDoorClose";
            btnDoorClose.Size = new Size(120, 29);
            btnDoorClose.TabIndex = 6;
            btnDoorClose.Text = "Đóng cửa chính";
            btnDoorClose.UseVisualStyleBackColor = true;
            // 
            // btnDoorOpen
            // 
            btnDoorOpen.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnDoorOpen.Location = new Point(260, 28);
            btnDoorOpen.Name = "btnDoorOpen";
            btnDoorOpen.Size = new Size(120, 29);
            btnDoorOpen.TabIndex = 7;
            btnDoorOpen.Text = "Mở cửa chính";
            btnDoorOpen.UseVisualStyleBackColor = true;
            // 
            // btnWindowStop
            // 
            btnWindowStop.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnWindowStop.Location = new Point(136, 98);
            btnWindowStop.Name = "btnWindowStop";
            btnWindowStop.Size = new Size(104, 29);
            btnWindowStop.TabIndex = 8;
            btnWindowStop.Text = "Dừng cửa sổ";
            btnWindowStop.UseVisualStyleBackColor = true;
            // 
            // btnDoorStop
            // 
            btnDoorStop.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnDoorStop.Location = new Point(260, 98);
            btnDoorStop.Name = "btnDoorStop";
            btnDoorStop.Size = new Size(120, 29);
            btnDoorStop.TabIndex = 9;
            btnDoorStop.Text = "Dừng cửa chính";
            btnDoorStop.UseVisualStyleBackColor = true;
            // 
            // btnBuzzerOff
            // 
            btnBuzzerOff.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnBuzzerOff.Location = new Point(404, 63);
            btnBuzzerOff.Name = "btnBuzzerOff";
            btnBuzzerOff.Size = new Size(104, 29);
            btnBuzzerOff.TabIndex = 10;
            btnBuzzerOff.Text = "Tắt còi";
            btnBuzzerOff.UseVisualStyleBackColor = true;
            // 
            // btnBuzzerOn
            // 
            btnBuzzerOn.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnBuzzerOn.Location = new Point(404, 28);
            btnBuzzerOn.Name = "btnBuzzerOn";
            btnBuzzerOn.Size = new Size(104, 29);
            btnBuzzerOn.TabIndex = 11;
            btnBuzzerOn.Text = "Bật còi";
            btnBuzzerOn.UseVisualStyleBackColor = true;
            // 
            // grpMode
            // 
            grpMode.Controls.Add(btnStatus);
            grpMode.Controls.Add(btnModeManual);
            grpMode.Controls.Add(btnModeAuto);
            grpMode.Font = new Font("Segoe UI", 12F);
            grpMode.Location = new Point(567, 15);
            grpMode.Name = "grpMode";
            grpMode.Size = new Size(614, 224);
            grpMode.TabIndex = 1;
            grpMode.TabStop = false;
            grpMode.Text = "Chế độ hệ thống";
            // 
            // btnModeAuto
            // 
            btnModeAuto.Location = new Point(35, 39);
            btnModeAuto.Name = "btnModeAuto";
            btnModeAuto.Size = new Size(96, 35);
            btnModeAuto.TabIndex = 0;
            btnModeAuto.Text = "AUTO";
            btnModeAuto.UseVisualStyleBackColor = true;
            // 
            // btnModeManual
            // 
            btnModeManual.Location = new Point(234, 41);
            btnModeManual.Name = "btnModeManual";
            btnModeManual.Size = new Size(96, 35);
            btnModeManual.TabIndex = 1;
            btnModeManual.Text = "MANUAL";
            btnModeManual.UseVisualStyleBackColor = true;
            // 
            // btnStatus
            // 
            btnStatus.Location = new Point(433, 41);
            btnStatus.Name = "btnStatus";
            btnStatus.Size = new Size(137, 35);
            btnStatus.TabIndex = 2;
            btnStatus.Text = "Lấy trạng thái";
            btnStatus.UseVisualStyleBackColor = true;
            // 
            // grpCommandStatus
            // 
            grpCommandStatus.Controls.Add(lblCommandResultTitle);
            grpCommandStatus.Controls.Add(lblLastCommandTitle);
            grpCommandStatus.Controls.Add(lblCommandResult);
            grpCommandStatus.Controls.Add(lblLastCommand);
            grpCommandStatus.Font = new Font("Segoe UI", 12F);
            grpCommandStatus.Location = new Point(567, 260);
            grpCommandStatus.Name = "grpCommandStatus";
            grpCommandStatus.Size = new Size(614, 213);
            grpCommandStatus.TabIndex = 1;
            grpCommandStatus.TabStop = false;
            grpCommandStatus.Text = "Trạng thái lệnh";
            // 
            // lblLastCommand
            // 
            lblLastCommand.AutoSize = true;
            lblLastCommand.Location = new Point(153, 66);
            lblLastCommand.Name = "lblLastCommand";
            lblLastCommand.Size = new Size(28, 21);
            lblLastCommand.TabIndex = 0;
            lblLastCommand.Text = "---";
            lblLastCommand.Click += lblLastCommand_Click;
            // 
            // lblCommandResult
            // 
            lblCommandResult.AutoSize = true;
            lblCommandResult.Location = new Point(153, 101);
            lblCommandResult.Name = "lblCommandResult";
            lblCommandResult.Size = new Size(28, 21);
            lblCommandResult.TabIndex = 1;
            lblCommandResult.Text = "---";
            // 
            // lblLastCommandTitle
            // 
            lblLastCommandTitle.AutoSize = true;
            lblLastCommandTitle.Location = new Point(35, 63);
            lblLastCommandTitle.Name = "lblLastCommandTitle";
            lblLastCommandTitle.Size = new Size(112, 21);
            lblLastCommandTitle.TabIndex = 2;
            lblLastCommandTitle.Text = "Lệnh gần nhất:";
            // 
            // lblCommandResultTitle
            // 
            lblCommandResultTitle.AutoSize = true;
            lblCommandResultTitle.Location = new Point(82, 98);
            lblCommandResultTitle.Name = "lblCommandResultTitle";
            lblCommandResultTitle.Size = new Size(65, 21);
            lblCommandResultTitle.TabIndex = 3;
            lblCommandResultTitle.Text = "Kết quả:";
            // 
            // tabDashboard
            // 
            tabDashboard.Controls.Add(grpDevice);
            tabDashboard.Controls.Add(grpRealtime);
            tabDashboard.Controls.Add(grpSensor);
            tabDashboard.Controls.Add(grpSystemStatus);
            tabDashboard.Location = new Point(4, 24);
            tabDashboard.Name = "tabDashboard";
            tabDashboard.Padding = new Padding(3);
            tabDashboard.Size = new Size(1202, 479);
            tabDashboard.TabIndex = 0;
            tabDashboard.Text = "Dashboard";
            tabDashboard.UseVisualStyleBackColor = true;
            tabDashboard.Click += tabDashboard_Click;
            // 
            // grpSystemStatus
            // 
            grpSystemStatus.Controls.Add(pnlSystemStatus);
            grpSystemStatus.Font = new Font("Segoe UI", 12F);
            grpSystemStatus.Location = new Point(36, 15);
            grpSystemStatus.Name = "grpSystemStatus";
            grpSystemStatus.Size = new Size(389, 221);
            grpSystemStatus.TabIndex = 0;
            grpSystemStatus.TabStop = false;
            grpSystemStatus.Text = "Trạng thái hệ thống";
            // 
            // pnlSystemStatus
            // 
            pnlSystemStatus.Controls.Add(label1);
            pnlSystemStatus.Controls.Add(lblMessageTitle);
            pnlSystemStatus.Controls.Add(lblMessage);
            pnlSystemStatus.Controls.Add(lblLevel);
            pnlSystemStatus.Location = new Point(6, 22);
            pnlSystemStatus.Name = "pnlSystemStatus";
            pnlSystemStatus.Size = new Size(362, 178);
            pnlSystemStatus.TabIndex = 0;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Font = new Font("Segoe UI", 11.25F);
            lblLevel.Location = new Point(111, 13);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(73, 20);
            lblLevel.TabIndex = 0;
            lblLevel.Text = "AN TOAN";
            lblLevel.Click += lblLevel_Click;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI", 11.25F);
            lblMessage.Location = new Point(111, 38);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(151, 20);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "HE THONG AN TOAN";
            lblMessage.Click += lblMessage_Click;
            // 
            // lblMessageTitle
            // 
            lblMessageTitle.AutoSize = true;
            lblMessageTitle.Font = new Font("Segoe UI", 11.25F);
            lblMessageTitle.Location = new Point(21, 38);
            lblMessageTitle.Name = "lblMessageTitle";
            lblMessageTitle.Size = new Size(84, 20);
            lblMessageTitle.TabIndex = 2;
            lblMessageTitle.Text = "Thông báo:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F);
            label1.Location = new Point(3, 13);
            label1.Name = "label1";
            label1.Size = new Size(106, 20);
            label1.TabIndex = 3;
            label1.Text = "Mức cảnh báo:";
            // 
            // grpSensor
            // 
            grpSensor.Controls.Add(lblGas);
            grpSensor.Controls.Add(label9);
            grpSensor.Controls.Add(lblTemp);
            grpSensor.Controls.Add(label7);
            grpSensor.Controls.Add(lblRain);
            grpSensor.Controls.Add(label5);
            grpSensor.Controls.Add(label3);
            grpSensor.Controls.Add(label2);
            grpSensor.Font = new Font("Segoe UI", 12F);
            grpSensor.Location = new Point(452, 15);
            grpSensor.Name = "grpSensor";
            grpSensor.Size = new Size(381, 221);
            grpSensor.TabIndex = 1;
            grpSensor.TabStop = false;
            grpSensor.Text = "Cảm biến";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 20);
            label2.Name = "label2";
            label2.Size = new Size(66, 21);
            label2.TabIndex = 0;
            label2.Text = "Khí/cồn:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(31, 106);
            label3.Name = "label3";
            label3.Size = new Size(44, 21);
            label3.TabIndex = 1;
            label3.Text = "Mưa:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 76);
            label5.Name = "label5";
            label5.Size = new Size(77, 21);
            label5.TabIndex = 3;
            label5.Text = "Có người:";
            // 
            // lblRain
            // 
            lblRain.AutoSize = true;
            lblRain.Location = new Point(93, 106);
            lblRain.Name = "lblRain";
            lblRain.Size = new Size(90, 21);
            lblRain.TabIndex = 4;
            lblRain.Text = "Không mưa";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(93, 76);
            label7.Name = "label7";
            label7.Size = new Size(120, 21);
            label7.TabIndex = 5;
            label7.Text = "Không có người";
            // 
            // lblTemp
            // 
            lblTemp.AutoSize = true;
            lblTemp.Location = new Point(93, 51);
            lblTemp.Name = "lblTemp";
            lblTemp.Size = new Size(39, 21);
            lblTemp.TabIndex = 6;
            lblTemp.Text = "0 °C";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(11, 51);
            label9.Name = "label9";
            label9.Size = new Size(73, 21);
            label9.TabIndex = 7;
            label9.Text = "Nhiệt độ:";
            // 
            // lblGas
            // 
            lblGas.AutoSize = true;
            lblGas.Location = new Point(93, 20);
            lblGas.Name = "lblGas";
            lblGas.Size = new Size(19, 21);
            lblGas.TabIndex = 8;
            lblGas.Text = "0";
            // 
            // grpRealtime
            // 
            grpRealtime.Controls.Add(dataGridView1);
            grpRealtime.Font = new Font("Segoe UI", 12F);
            grpRealtime.Location = new Point(452, 259);
            grpRealtime.Name = "grpRealtime";
            grpRealtime.Size = new Size(381, 202);
            grpRealtime.TabIndex = 1;
            grpRealtime.TabStop = false;
            grpRealtime.Text = "Dữ liệu realtime";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(17, 30);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(240, 150);
            dataGridView1.TabIndex = 0;
            // 
            // grpDevice
            // 
            grpDevice.Controls.Add(lblMode);
            grpDevice.Controls.Add(label15);
            grpDevice.Controls.Add(lblBuzzer);
            grpDevice.Controls.Add(label13);
            grpDevice.Controls.Add(lblDoor);
            grpDevice.Controls.Add(label11);
            grpDevice.Controls.Add(lblWindow);
            grpDevice.Controls.Add(label8);
            grpDevice.Controls.Add(lblFan);
            grpDevice.Controls.Add(label4);
            grpDevice.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            grpDevice.Location = new Point(36, 259);
            grpDevice.Name = "grpDevice";
            grpDevice.Size = new Size(389, 202);
            grpDevice.TabIndex = 1;
            grpDevice.TabStop = false;
            grpDevice.Text = "Thiết bị";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 28);
            label4.Name = "label4";
            label4.Size = new Size(47, 21);
            label4.TabIndex = 0;
            label4.Text = "Quạt:";
            // 
            // lblFan
            // 
            lblFan.AutoSize = true;
            lblFan.Location = new Point(95, 28);
            lblFan.Name = "lblFan";
            lblFan.Size = new Size(34, 21);
            lblFan.TabIndex = 1;
            lblFan.Text = "TAT";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(20, 59);
            label8.Name = "label8";
            label8.Size = new Size(60, 21);
            label8.TabIndex = 2;
            label8.Text = "Cửa sổ:";
            // 
            // lblWindow
            // 
            lblWindow.AutoSize = true;
            lblWindow.Location = new Point(95, 59);
            lblWindow.Name = "lblWindow";
            lblWindow.Size = new Size(56, 21);
            lblWindow.TabIndex = 3;
            lblWindow.Text = "DONG";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(20, 91);
            label11.Name = "label11";
            label11.Size = new Size(82, 21);
            label11.TabIndex = 4;
            label11.Text = "Cửa chính:";
            // 
            // lblDoor
            // 
            lblDoor.AutoSize = true;
            lblDoor.Location = new Point(95, 91);
            lblDoor.Name = "lblDoor";
            lblDoor.Size = new Size(56, 21);
            lblDoor.TabIndex = 5;
            lblDoor.Text = "DONG";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(20, 122);
            label13.Name = "label13";
            label13.Size = new Size(66, 21);
            label13.TabIndex = 6;
            label13.Text = "Còi báo:";
            // 
            // lblBuzzer
            // 
            lblBuzzer.AutoSize = true;
            lblBuzzer.Location = new Point(95, 122);
            lblBuzzer.Name = "lblBuzzer";
            lblBuzzer.Size = new Size(34, 21);
            lblBuzzer.TabIndex = 7;
            lblBuzzer.Text = "TAT";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(20, 159);
            label15.Name = "label15";
            label15.Size = new Size(62, 21);
            label15.TabIndex = 8;
            label15.Text = "Chế độ:";
            // 
            // lblMode
            // 
            lblMode.AutoSize = true;
            lblMode.Location = new Point(95, 159);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(82, 21);
            lblMode.TabIndex = 9;
            lblMode.Text = "TU_DONG";
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabDashboard);
            tabMain.Controls.Add(tabControl);
            tabMain.Controls.Add(tabAlert);
            tabMain.Controls.Add(tabSensorData);
            tabMain.Controls.Add(tabAccess);
            tabMain.Controls.Add(tabUser);
            tabMain.Controls.Add(tabConfig);
            tabMain.Controls.Add(tabLog);
            tabMain.Location = new Point(12, 132);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1210, 507);
            tabMain.TabIndex = 2;
            // 
            // grpAccessFilter
            // 
            grpAccessFilter.Controls.Add(btnRefreshAccess);
            grpAccessFilter.Controls.Add(cboAccessStatus);
            grpAccessFilter.Controls.Add(label6);
            grpAccessFilter.Dock = DockStyle.Top;
            grpAccessFilter.Font = new Font("Segoe UI", 11.25F);
            grpAccessFilter.Location = new Point(3, 3);
            grpAccessFilter.Name = "grpAccessFilter";
            grpAccessFilter.Size = new Size(1196, 72);
            grpAccessFilter.TabIndex = 0;
            grpAccessFilter.TabStop = false;
            grpAccessFilter.Text = "Bộ lọc";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 11.25F);
            label6.Location = new Point(27, 32);
            label6.Name = "label6";
            label6.Size = new Size(75, 20);
            label6.TabIndex = 0;
            label6.Text = "Trạng thái";
            // 
            // cboAccessStatus
            // 
            cboAccessStatus.Font = new Font("Segoe UI", 11.25F);
            cboAccessStatus.FormattingEnabled = true;
            cboAccessStatus.Items.AddRange(new object[] { "THANH_CONG", "", "THAT_BAI", "", "KHOA_HE_THONG" });
            cboAccessStatus.Location = new Point(116, 28);
            cboAccessStatus.Name = "cboAccessStatus";
            cboAccessStatus.Size = new Size(188, 28);
            cboAccessStatus.TabIndex = 1;
            // 
            // btnRefreshAccess
            // 
            btnRefreshAccess.Font = new Font("Segoe UI", 11.25F);
            btnRefreshAccess.Location = new Point(343, 28);
            btnRefreshAccess.Name = "btnRefreshAccess";
            btnRefreshAccess.Size = new Size(99, 28);
            btnRefreshAccess.TabIndex = 2;
            btnRefreshAccess.Text = "Làm mới";
            btnRefreshAccess.UseVisualStyleBackColor = true;
            // 
            // dgvAccess
            // 
            dgvAccess.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAccess.Columns.AddRange(new DataGridViewColumn[] { colTime2, colPassword, colStatus2, colMessage });
            dgvAccess.Dock = DockStyle.Fill;
            dgvAccess.Location = new Point(3, 75);
            dgvAccess.Name = "dgvAccess";
            dgvAccess.Size = new Size(1196, 401);
            dgvAccess.TabIndex = 1;
            // 
            // colTime2
            // 
            colTime2.HeaderText = "Thời gian";
            colTime2.Name = "colTime2";
            // 
            // colPassword
            // 
            colPassword.HeaderText = "Mật khẩu";
            colPassword.Name = "colPassword";
            // 
            // colStatus2
            // 
            colStatus2.HeaderText = "Trạng thái";
            colStatus2.Name = "colStatus2";
            // 
            // colMessage
            // 
            colMessage.HeaderText = "Nội dung";
            colMessage.Name = "colMessage";
            // 
            // grpUserInfo
            // 
            grpUserInfo.Controls.Add(button3);
            grpUserInfo.Controls.Add(btnDeleteUser);
            grpUserInfo.Controls.Add(btnEditUser);
            grpUserInfo.Controls.Add(btnRefreshUser);
            grpUserInfo.Controls.Add(label10);
            grpUserInfo.Controls.Add(textBox1);
            grpUserInfo.Controls.Add(label14);
            grpUserInfo.Controls.Add(txtPassword);
            grpUserInfo.Controls.Add(btnAddUser);
            grpUserInfo.Controls.Add(label12);
            grpUserInfo.Controls.Add(cboRole);
            grpUserInfo.Dock = DockStyle.Top;
            grpUserInfo.Font = new Font("Segoe UI", 11.25F);
            grpUserInfo.Location = new Point(3, 3);
            grpUserInfo.Name = "grpUserInfo";
            grpUserInfo.Size = new Size(1196, 130);
            grpUserInfo.TabIndex = 0;
            grpUserInfo.TabStop = false;
            grpUserInfo.Text = "Thông tin người dùng";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 11.25F);
            label10.Location = new Point(9, 35);
            label10.Name = "label10";
            label10.Size = new Size(78, 20);
            label10.TabIndex = 1;
            label10.Text = "Username:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 11.25F);
            label12.Location = new Point(30, 94);
            label12.Name = "label12";
            label12.Size = new Size(54, 20);
            label12.TabIndex = 2;
            label12.Text = "Quyền:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 11.25F);
            label14.Location = new Point(11, 62);
            label14.Name = "label14";
            label14.Size = new Size(73, 20);
            label14.TabIndex = 3;
            label14.Text = "Password:";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 11.25F);
            textBox1.Location = new Point(90, 25);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 27);
            textBox1.TabIndex = 4;
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 11.25F);
            txtPassword.Location = new Point(90, 55);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(100, 27);
            txtPassword.TabIndex = 5;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // cboRole
            // 
            cboRole.Font = new Font("Segoe UI", 11.25F);
            cboRole.FormattingEnabled = true;
            cboRole.Items.AddRange(new object[] { "ADMIN", "", "USER" });
            cboRole.Location = new Point(85, 94);
            cboRole.Name = "cboRole";
            cboRole.Size = new Size(121, 28);
            cboRole.TabIndex = 6;
            // 
            // btnAddUser
            // 
            btnAddUser.Font = new Font("Segoe UI", 11.25F);
            btnAddUser.Location = new Point(263, 47);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(92, 31);
            btnAddUser.TabIndex = 7;
            btnAddUser.Text = "Thêm";
            btnAddUser.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 11.25F);
            button3.Location = new Point(660, 55);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 8;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // btnRefreshUser
            // 
            btnRefreshUser.Font = new Font("Segoe UI", 11.25F);
            btnRefreshUser.Location = new Point(555, 50);
            btnRefreshUser.Name = "btnRefreshUser";
            btnRefreshUser.Size = new Size(92, 31);
            btnRefreshUser.TabIndex = 9;
            btnRefreshUser.Text = "Làm mới";
            btnRefreshUser.UseVisualStyleBackColor = true;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.Font = new Font("Segoe UI", 11.25F);
            btnDeleteUser.Location = new Point(458, 47);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(91, 31);
            btnDeleteUser.TabIndex = 10;
            btnDeleteUser.Text = "Xóa";
            btnDeleteUser.UseVisualStyleBackColor = true;
            // 
            // btnEditUser
            // 
            btnEditUser.Font = new Font("Segoe UI", 11.25F);
            btnEditUser.Location = new Point(361, 47);
            btnEditUser.Name = "btnEditUser";
            btnEditUser.Size = new Size(91, 31);
            btnEditUser.TabIndex = 11;
            btnEditUser.Text = "Sửa";
            btnEditUser.UseVisualStyleBackColor = true;
            // 
            // dgvUser
            // 
            dgvUser.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUser.Columns.AddRange(new DataGridViewColumn[] { colId3, colUsername, colPassword3, colRole });
            dgvUser.Dock = DockStyle.Fill;
            dgvUser.Location = new Point(3, 133);
            dgvUser.Name = "dgvUser";
            dgvUser.Size = new Size(1196, 343);
            dgvUser.TabIndex = 1;
            // 
            // colId3
            // 
            colId3.HeaderText = "ID";
            colId3.Name = "colId3";
            // 
            // colUsername
            // 
            colUsername.HeaderText = "Username";
            colUsername.Name = "colUsername";
            // 
            // colPassword3
            // 
            colPassword3.HeaderText = "Password";
            colPassword3.Name = "colPassword3";
            // 
            // colRole
            // 
            colRole.HeaderText = "Quyền";
            colRole.Name = "colRole";
            // 
            // grpConfig
            // 
            grpConfig.Controls.Add(btnResetConfig);
            grpConfig.Controls.Add(btnSaveConfig);
            grpConfig.Controls.Add(cboComPort);
            grpConfig.Controls.Add(cboDefaultMode);
            grpConfig.Controls.Add(boBaudrate);
            grpConfig.Controls.Add(numGasDanger);
            grpConfig.Controls.Add(numTempWarning);
            grpConfig.Controls.Add(numTempDanger);
            grpConfig.Controls.Add(numGasWarning);
            grpConfig.Controls.Add(label22);
            grpConfig.Controls.Add(label21);
            grpConfig.Controls.Add(label20);
            grpConfig.Controls.Add(label19);
            grpConfig.Controls.Add(label18);
            grpConfig.Controls.Add(label17);
            grpConfig.Controls.Add(label16);
            grpConfig.Dock = DockStyle.Fill;
            grpConfig.Font = new Font("Segoe UI", 12F);
            grpConfig.Location = new Point(3, 3);
            grpConfig.Name = "grpConfig";
            grpConfig.Size = new Size(1196, 473);
            grpConfig.TabIndex = 0;
            grpConfig.TabStop = false;
            grpConfig.Text = "Cấu hình hệ thống";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(55, 38);
            label16.Name = "label16";
            label16.Size = new Size(106, 21);
            label16.TabIndex = 0;
            label16.Text = "Gas cảnh báo:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(85, 311);
            label17.Name = "label17";
            label17.Size = new Size(75, 21);
            label17.TabIndex = 1;
            label17.Text = "Baudrate:";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(21, 130);
            label18.Name = "label18";
            label18.Size = new Size(140, 21);
            label18.TabIndex = 2;
            label18.Text = "Nhiệt độ cảnh báo:";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(44, 86);
            label19.Name = "label19";
            label19.Size = new Size(117, 21);
            label19.TabIndex = 3;
            label19.Text = "Gas nguy hiểm:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(30, 219);
            label20.Name = "label20";
            label20.Size = new Size(130, 21);
            label20.TabIndex = 4;
            label20.Text = "Chế độ mặc định:";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(10, 171);
            label21.Name = "label21";
            label21.Size = new Size(151, 21);
            label21.TabIndex = 5;
            label21.Text = "Nhiệt độ nguy hiểm:";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(79, 267);
            label22.Name = "label22";
            label22.Size = new Size(81, 21);
            label22.TabIndex = 6;
            label22.Text = "COM Port:";
            // 
            // numGasWarning
            // 
            numGasWarning.Location = new Point(167, 33);
            numGasWarning.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numGasWarning.Name = "numGasWarning";
            numGasWarning.Size = new Size(120, 29);
            numGasWarning.TabIndex = 7;
            numGasWarning.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // numTempDanger
            // 
            numTempDanger.Location = new Point(167, 171);
            numTempDanger.Name = "numTempDanger";
            numTempDanger.Size = new Size(120, 29);
            numTempDanger.TabIndex = 9;
            numTempDanger.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // numTempWarning
            // 
            numTempWarning.Location = new Point(167, 128);
            numTempWarning.Name = "numTempWarning";
            numTempWarning.Size = new Size(120, 29);
            numTempWarning.TabIndex = 10;
            numTempWarning.Value = new decimal(new int[] { 35, 0, 0, 0 });
            // 
            // numGasDanger
            // 
            numGasDanger.Location = new Point(167, 84);
            numGasDanger.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numGasDanger.Name = "numGasDanger";
            numGasDanger.Size = new Size(120, 29);
            numGasDanger.TabIndex = 11;
            numGasDanger.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // boBaudrate
            // 
            boBaudrate.FormattingEnabled = true;
            boBaudrate.Items.AddRange(new object[] { "9600", "", "115200" });
            boBaudrate.Location = new Point(166, 308);
            boBaudrate.Name = "boBaudrate";
            boBaudrate.Size = new Size(121, 29);
            boBaudrate.TabIndex = 12;
            // 
            // cboDefaultMode
            // 
            cboDefaultMode.FormattingEnabled = true;
            cboDefaultMode.Items.AddRange(new object[] { "AUTO", "", "MANUAL" });
            cboDefaultMode.Location = new Point(166, 216);
            cboDefaultMode.Name = "cboDefaultMode";
            cboDefaultMode.Size = new Size(121, 29);
            cboDefaultMode.TabIndex = 13;
            // 
            // cboComPort
            // 
            cboComPort.FormattingEnabled = true;
            cboComPort.Items.AddRange(new object[] { "COM1", "", "COM2", "", "COM3", "", "COM4", "", "COM5" });
            cboComPort.Location = new Point(166, 267);
            cboComPort.Name = "cboComPort";
            cboComPort.Size = new Size(121, 29);
            cboComPort.TabIndex = 14;
            // 
            // btnSaveConfig
            // 
            btnSaveConfig.Location = new Point(198, 392);
            btnSaveConfig.Name = "btnSaveConfig";
            btnSaveConfig.Size = new Size(156, 37);
            btnSaveConfig.TabIndex = 15;
            btnSaveConfig.Text = "Lưu cấu hình";
            btnSaveConfig.UseVisualStyleBackColor = true;
            // 
            // btnResetConfig
            // 
            btnResetConfig.Location = new Point(360, 392);
            btnResetConfig.Name = "btnResetConfig";
            btnResetConfig.Size = new Size(156, 37);
            btnResetConfig.TabIndex = 16;
            btnResetConfig.Text = "Khôi phục mặc định";
            btnResetConfig.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Dock = DockStyle.Fill;
            textBox2.Location = new Point(3, 3);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.ScrollBars = ScrollBars.Vertical;
            textBox2.Size = new Size(1196, 473);
            textBox2.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnClearLog);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(3, 376);
            panel1.Name = "panel1";
            panel1.Size = new Size(1196, 100);
            panel1.TabIndex = 4;
            // 
            // btnClearLog
            // 
            btnClearLog.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
            btnClearLog.Location = new Point(21, 36);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new Size(100, 28);
            btnClearLog.TabIndex = 0;
            btnClearLog.Text = "Xóa log";
            btnClearLog.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1234, 721);
            Controls.Add(tabMain);
            Controls.Add(pnlHeader);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "FormMain";
            Text = "FormMain";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            tabLog.ResumeLayout(false);
            tabLog.PerformLayout();
            tabConfig.ResumeLayout(false);
            tabUser.ResumeLayout(false);
            tabAccess.ResumeLayout(false);
            tabSensorData.ResumeLayout(false);
            grpSensorFilter.ResumeLayout(false);
            grpSensorFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            tabAlert.ResumeLayout(false);
            grpCurrentAlert.ResumeLayout(false);
            grpCurrentAlert.PerformLayout();
            pnlAlertLevel.ResumeLayout(false);
            pnlAlertLevel.PerformLayout();
            grpAlertFilter.ResumeLayout(false);
            grpAlertFilter.PerformLayout();
            grpAlertList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            tabControl.ResumeLayout(false);
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            grpDeviceControl.ResumeLayout(false);
            grpMode.ResumeLayout(false);
            grpCommandStatus.ResumeLayout(false);
            grpCommandStatus.PerformLayout();
            tabDashboard.ResumeLayout(false);
            grpSystemStatus.ResumeLayout(false);
            pnlSystemStatus.ResumeLayout(false);
            pnlSystemStatus.PerformLayout();
            grpSensor.ResumeLayout(false);
            grpSensor.PerformLayout();
            grpRealtime.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            grpDevice.ResumeLayout(false);
            grpDevice.PerformLayout();
            tabMain.ResumeLayout(false);
            grpAccessFilter.ResumeLayout(false);
            grpAccessFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAccess).EndInit();
            grpUserInfo.ResumeLayout(false);
            grpUserInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUser).EndInit();
            grpConfig.ResumeLayout(false);
            grpConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numGasWarning).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTempDanger).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTempWarning).EndInit();
            ((System.ComponentModel.ISupportInitialize)numGasDanger).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem hTToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem menuLogout;
        private ToolStripMenuItem menuExit;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem menuConnect;
        private ToolStripMenuItem menuDisconnect;
        private ToolStripMenuItem menuStatus;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem menuSensorData;
        private ToolStripMenuItem menuAlert;
        private ToolStripMenuItem menuCommand;
        private ToolStripMenuItem menuAccess;
        private ToolStripMenuItem toolStripMenuItem13;
        private ToolStripMenuItem menuUser;
        private ToolStripMenuItem menuConfig;
        private ToolStripMenuItem trợGiúpToolStripMenuItem;
        private ToolStripMenuItem menuAbout;
        private Panel pnlHeader;
        private Label lblCurrentUser;
        private Label lblRole;
        private Label lblClock;
        private Label lblAppTitle;
        private TabPage tabLog;
        private TabPage tabConfig;
        private TabPage tabUser;
        private TabPage tabAccess;
        private TabPage tabSensorData;
        private DataGridView dataGridView3;
        private DataGridViewTextBoxColumn colId1;
        private DataGridViewTextBoxColumn colTime1;
        private DataGridViewTextBoxColumn colGas;
        private DataGridViewTextBoxColumn colTemp;
        private DataGridViewTextBoxColumn colRain;
        private DataGridViewTextBoxColumn colPir;
        private DataGridViewTextBoxColumn colFan;
        private DataGridViewTextBoxColumn colDoor;
        private GroupBox grpSensorFilter;
        private DateTimePicker dateTimePicker4;
        private Button btnRefreshSensor;
        private DateTimePicker dtpToDate;
        private Label lblToDate;
        private Label lblFromDate;
        private TabPage tabAlert;
        private Button btnDeleteAlert;
        private Button btnResolveAlert;
        private GroupBox grpAlertList;
        private DataGridView dataGridView2;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colLevel;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colHandleTime;
        private GroupBox grpAlertFilter;
        private Button btnRefreshAlert;
        private Button btnSearchAlert;
        private ComboBox cboAlertType;
        private ComboBox comboBox1;
        private Label labelLevel;
        private DateTimePicker dateTimePicker2;
        private Label dtpAlertFrom;
        private DateTimePicker dateTimePicker1;
        private Label labelType;
        private Label labelFrom;
        private GroupBox grpCurrentAlert;
        private Label lblCurrentTime;
        private Label lblCurrentTimeTitle;
        private Label lblCurrentAlertMessage;
        private Label lblCurrentAlertTitle;
        private Panel pnlAlertLevel;
        private Label lblCurrentLevel;
        private TabPage tabControl;
        private GroupBox grpCommandStatus;
        private Label lblCommandResultTitle;
        private Label lblLastCommandTitle;
        private Label lblCommandResult;
        private Label lblLastCommand;
        private GroupBox grpMode;
        private Button btnStatus;
        private Button btnModeManual;
        private Button btnModeAuto;
        private GroupBox grpDeviceControl;
        private Button btnBuzzerOn;
        private Button btnBuzzerOff;
        private Button btnDoorStop;
        private Button btnWindowStop;
        private Button btnDoorOpen;
        private Button btnDoorClose;
        private Button btnWindowClose;
        private Button btnWindowOpen;
        private Button btnResetAlarm;
        private Button button1;
        private Button btnFanOff;
        private Button btnFanOn;
        private GroupBox grpConnection;
        private Label label35;
        private Label label34;
        private Label lblComStatus;
        private Button btnConnect;
        private Button btnDisconnect;
        private Button btnRefreshCom;
        private ComboBox cboCom;
        private TabPage tabDashboard;
        private GroupBox grpDevice;
        private Label lblMode;
        private Label label15;
        private Label lblBuzzer;
        private Label label13;
        private Label lblDoor;
        private Label label11;
        private Label lblWindow;
        private Label label8;
        private Label lblFan;
        private Label label4;
        private GroupBox grpRealtime;
        private DataGridView dataGridView1;
        private GroupBox grpSensor;
        private Label lblGas;
        private Label label9;
        private Label lblTemp;
        private Label label7;
        private Label lblRain;
        private Label label5;
        private Label label3;
        private Label label2;
        private GroupBox grpSystemStatus;
        private Panel pnlSystemStatus;
        private Label label1;
        private Label lblMessageTitle;
        private Label lblMessage;
        private Label lblLevel;
        private TabControl tabMain;
        private GroupBox grpAccessFilter;
        private Button btnRefreshAccess;
        private ComboBox cboAccessStatus;
        private Label label6;
        private DataGridView dgvAccess;
        private DataGridViewTextBoxColumn colTime2;
        private DataGridViewTextBoxColumn colPassword;
        private DataGridViewTextBoxColumn colStatus2;
        private DataGridViewTextBoxColumn colMessage;
        private Button btnEditUser;
        private Button btnDeleteUser;
        private Button btnRefreshUser;
        private Button button3;
        private Button btnAddUser;
        private ComboBox cboRole;
        private GroupBox grpUserInfo;
        private Label label10;
        private TextBox textBox1;
        private Label label14;
        private TextBox txtPassword;
        private Label label12;
        private DataGridView dgvUser;
        private DataGridViewTextBoxColumn colId3;
        private DataGridViewTextBoxColumn colUsername;
        private DataGridViewTextBoxColumn colPassword3;
        private DataGridViewTextBoxColumn colRole;
        private GroupBox grpConfig;
        private Button btnSaveConfig;
        private ComboBox cboComPort;
        private ComboBox cboDefaultMode;
        private ComboBox boBaudrate;
        private NumericUpDown numGasDanger;
        private NumericUpDown numTempWarning;
        private NumericUpDown numTempDanger;
        private NumericUpDown numGasWarning;
        private Label label22;
        private Label label21;
        private Label label20;
        private Label label19;
        private Label label18;
        private Label label17;
        private Label label16;
        private Button btnResetConfig;
        private Panel panel1;
        private Button btnClearLog;
        private TextBox textBox2;
    }
}