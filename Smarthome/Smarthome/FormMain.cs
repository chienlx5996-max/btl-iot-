using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Smarthome
{
    public partial class FormMain : Form
    {
        public string CurrentUsername { get; set; } = "";
        public string CurrentRole { get; set; } = "";
        public int CurrentAccountId { get; set; } = 0;

        private readonly DbHelper _db = new DbHelper();
        private int? _selectedUserId;
        private System.Windows.Forms.Timer? clockTimer;
        private SerialPort? _serialPort;

        public FormMain()
        {
            InitializeComponent();

            Load += FormMain_Load;

            btnRefreshSensor.Click += BtnRefreshSensor_Click;
            btnRefreshAlert.Click += BtnRefreshAlert_Click;
            btnRefreshAccess.Click += BtnRefreshAccess_Click;

            btnSearchAlert.Click += BtnSearchAlert_Click;
            btnResolveAlert.Click += BtnResolveAlert_Click;

            btnRefreshUser.Click += BtnRefreshUser_Click;
            btnAddUser.Click += BtnAddUser_Click;
            btnEditUser.Click += BtnEditUser_Click;
            btnDeleteUser.Click += BtnDeleteUser_Click;
            dgvUser.CellClick += DgvUser_CellClick;

            btnSaveConfig.Click += BtnSaveConfig_Click;
            btnResetConfig.Click += BtnResetConfig_Click;

            btnConnect.Click += BtnConnect_Click;
            btnDisconnect.Click += BtnDisconnect_Click;
            btnRefreshCom.Click += BtnRefreshCom_Click;
            btnStatus.Click += BtnStatus_Click;
            btnHelp.Click += BtnHelp_Click;
            menuStatus.Click += BtnStatus_Click;
            btnClearLog.Click += BtnClearLog_Click;

            btnFanOn.Click += (s, e) => SendDeviceCommand("FAN_ON", 1, null, null, null, "SAFE", null, "Quạt bật");
            btnFanOff.Click += (s, e) => SendDeviceCommand("FAN_OFF", 0, null, null, null, "SAFE", null, "Quạt tắt");
            btnWindowOpen.Click += (s, e) => SendDeviceCommand("WINDOW_OPEN", null, null, 1, null, null, null, "Cửa sổ mở");
            btnWindowClose.Click += (s, e) => SendDeviceCommand("WINDOW_CLOSE", null, null, 0, null, null, null, "Cửa sổ đóng");
            btnDoorOpen.Click += (s, e) => SendDeviceCommand("DOOR_OPEN", null, 1, null, null, null, null, "Cửa chính mở");
            btnDoorClose.Click += (s, e) => SendDeviceCommand("DOOR_CLOSE", null, 0, null, null, null, null, "Cửa chính đóng");
            btnBuzzerOn.Click += (s, e) => SendDeviceCommand("BUZZER_ON", null, null, null, 1, null, null, "Còi báo bật");
            btnBuzzerOff.Click += (s, e) => SendDeviceCommand("BUZZER_OFF", null, null, null, 0, null, null, "Còi báo tắt");
            btnModeAuto.Click += (s, e) => SendDeviceCommand("MODE_AUTO", null, null, null, null, null, 1, "Chế độ tự động");
            btnModeManual.Click += (s, e) => SendDeviceCommand("MODE_MANUAL", null, null, null, null, null, 0, "Chế độ thủ công");
            btnResetAlarm.Click += BtnResetAlarm_Click;

            cboAccessStatus.SelectedIndexChanged += CboAccessStatus_SelectedIndexChanged;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            lblCurrentUser.Text = $"User: {CurrentUsername}";
            lblRole.Text = $"Role: {CurrentRole}";
            lblClock.Text = $"Time: {DateTime.Now:HH:mm:ss}";

            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, ev) => lblClock.Text = $"Time: {DateTime.Now:HH:mm:ss}";
            clockTimer.Start();

            ApplyRolePermissions();
            RefreshComPorts();
            
            // Tự động kết nối COM2 @ 9600
            try
            {
                OpenSerialPort("COM2", 9600);
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Đã kết nối tự động COM2 @ 9600");
            }
            catch (Exception ex)
            {
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Lỗi kết nối tự động: {ex.Message}");
            }
            
            LoadSensorData();
            LoadAlertHistory();
            LoadDoorAccessLog();
            LoadUserList();
            LoadConfigValues();
            LoadDeviceStatus();
            LoadCommandStatus();
        }

        private void ApplyRolePermissions()
        {
            bool isAdmin = string.Equals(CurrentRole, "admin", StringComparison.OrdinalIgnoreCase);
            tabUser.Enabled = isAdmin;
            tabConfig.Enabled = isAdmin;
            btnAddUser.Enabled = isAdmin;
            btnEditUser.Enabled = isAdmin;
            btnDeleteUser.Enabled = isAdmin;
            btnSaveConfig.Enabled = isAdmin;
            btnResetConfig.Enabled = isAdmin;
        }

        private void RefreshComPorts()
        {
            try
            {
                var ports = new List<string>();

                // Danh sách port ảo VSPE mặc định
                var vspePorts = new[] { "VSPE1", "VSPE2", "VSPE3", "VSPE4", "VSPE5" };
                ports.AddRange(vspePorts);

                // Thêm port thực từ hệ thống
                var realPorts = SerialPort.GetPortNames();
                ports.AddRange(realPorts.Where(p => !ports.Contains(p)));

                cboComPort.Items.Clear();
                cboComPort.Items.AddRange(ports.ToArray());
                if (ports.Count > 0)
                {
                    // Ưu tiên chọn port ảo VSPE đầu tiên
                    var vspePort = ports.FirstOrDefault(p => p.StartsWith("VSPE"));
                    if (!string.IsNullOrEmpty(vspePort))
                    {
                        cboComPort.SelectedItem = vspePort;
                    }
                    else
                    {
                        cboComPort.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
                cboComPort.Items.Clear();
            }
        }

        private void BtnRefreshSensor_Click(object sender, EventArgs e)
        {
            LoadSensorData();
        }

        private void BtnRefreshAlert_Click(object sender, EventArgs e)
        {
            LoadAlertHistory();
        }

        private void BtnRefreshAccess_Click(object sender, EventArgs e)
        {
            LoadDoorAccessLog();
        }

        private void BtnSearchAlert_Click(object sender, EventArgs e)
        {
            LoadAlertHistory();
        }

        private void BtnResolveAlert_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một cảnh báo để xác nhận xử lý.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!long.TryParse(dataGridView2.CurrentRow.Cells["id"].Value?.ToString(), out var alertId))
            {
                MessageBox.Show("Không lấy được ID cảnh báo.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _db.ResolveAlert(alertId);
                LoadAlertHistory();
                MessageBox.Show("Đã xác nhận xử lý cảnh báo.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác nhận cảnh báo:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefreshUser_Click(object sender, EventArgs e)
        {
            LoadUserList();
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            var username = textBox1.Text.Trim();
            var password = txtPassword.Text.Trim();
            var role = cboRole.SelectedItem?.ToString()?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập, mật khẩu và quyền.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _db.AddAccount(username, password, role);
                LoadUserList();
                ClearUserForm();
                MessageBox.Show("Đã thêm người dùng mới.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm tài khoản:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditUser_Click(object sender, EventArgs e)
        {
            if (!_selectedUserId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn người dùng để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var password = txtPassword.Text.Trim();
            var role = cboRole.SelectedItem?.ToString()?.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Vui lòng chọn quyền.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _db.UpdateAccount(_selectedUserId.Value, string.IsNullOrWhiteSpace(password) ? null : password, role);
                LoadUserList();
                ClearUserForm();
                MessageBox.Show("Đã cập nhật tài khoản.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật tài khoản:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (!_selectedUserId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn người dùng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn khóa tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _db.DisableAccount(_selectedUserId.Value);
                LoadUserList();
                ClearUserForm();
                MessageBox.Show("Đã khóa tài khoản.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khóa tài khoản:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUser_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvUser.CurrentRow == null)
            {
                return;
            }

            _selectedUserId = int.TryParse(dgvUser.CurrentRow.Cells["id"].Value?.ToString(), out var id) ? id : (int?)null;
            textBox1.Text = dgvUser.CurrentRow.Cells["username"].Value?.ToString() ?? string.Empty;
            txtPassword.Text = string.Empty;
            var role = dgvUser.CurrentRow.Cells["role"].Value?.ToString() ?? string.Empty;
            cboRole.SelectedItem = role.ToUpper();
        }

        private void BtnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                _db.SetThreshold("GAS_WARNING", numGasWarning.Value.ToString(), "INT", CurrentAccountId);
                _db.SetThreshold("GAS_DANGER", numGasDanger.Value.ToString(), "INT", CurrentAccountId);
                _db.SetThreshold("TEMP_WARNING", numTempWarning.Value.ToString(), "FLOAT", CurrentAccountId);
                _db.SetThreshold("TEMP_DANGER", numTempDanger.Value.ToString(), "FLOAT", CurrentAccountId);

                var mode = cboDefaultMode.SelectedItem?.ToString() ?? "AUTO";
                _db.SetThreshold("AUTO_MODE", mode == "AUTO" ? "1" : "0", "BOOL", CurrentAccountId);
                var comPort = cboComPort.SelectedItem?.ToString() ?? string.Empty;
                _db.SetThreshold("COM_PORT", comPort, "STRING", CurrentAccountId);
                var baudRate = boBaudrate.SelectedItem?.ToString() ?? string.Empty;
                _db.SetThreshold("BAUDRATE", baudRate, "STRING", CurrentAccountId);

                MessageBox.Show("Đã lưu cấu hình.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu cấu hình:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnResetConfig_Click(object sender, EventArgs e)
        {
            numGasWarning.Value = 300;
            numGasDanger.Value = 600;
            numTempWarning.Value = 35;
            numTempDanger.Value = 40;
            cboDefaultMode.SelectedItem = "AUTO";
            boBaudrate.SelectedItem = "9600";
            if (cboComPort.Items.Count > 0)
            {
                cboComPort.SelectedIndex = 0;
            }
        }

        private void BtnConnect_Click(object? sender, EventArgs? e)
        {
            if (cboComPort.SelectedItem == null || boBaudrate.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn cổng COM và Baudrate.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var portName = cboComPort.SelectedItem.ToString() ?? string.Empty;
            if (!int.TryParse(boBaudrate.SelectedItem?.ToString(), out var baudRate))
            {
                MessageBox.Show("Baudrate không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                OpenSerialPort(portName, baudRate);
                MessageBox.Show($"Đã kết nối cổng {portName}@{baudRate}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cổng COM:\n{ex.Message}", "Lỗi COM", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDisconnect_Click(object? sender, EventArgs? e)
        {
            CloseSerialPort();
            MessageBox.Show("Đã ngắt kết nối COM.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRefreshCom_Click(object? sender, EventArgs? e)
        {
            RefreshComPorts();
        }

        private void BtnStatus_Click(object? sender, EventArgs? e)
        {
            if (!TrySendSerialCommand("STATUS"))
            {
                MessageBox.Show("Chưa kết nối COM. Vui lòng kết nối trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Đã gửi yêu cầu STATUS tới Arduino.");
            LoadDeviceStatus();
            LoadCommandStatus();
        }

        private void BtnHelp_Click(object? sender, EventArgs? e)
        {
            if (!TrySendSerialCommand("HELP"))
            {
                MessageBox.Show("Chưa kết nối COM. Vui lòng kết nối trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Đã gửi yêu cầu HELP tới Arduino.");
            MessageBox.Show("Đã gửi yêu cầu HELP. Kiểm tra tab Log Serial để xem danh sách lệnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClearLog_Click(object? sender, EventArgs? e)
        {
            textBox2.Clear();
        }

        private void OpenSerialPort(string portName, int baudRate)
        {
            CloseSerialPort();

            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 2000,
                WriteTimeout = 2000,
                NewLine = "\n",
                Encoding = Encoding.ASCII,
            };
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.ErrorReceived += SerialPort_ErrorReceived;
            _serialPort.Open();

            AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Mở cổng {portName}@{baudRate}");
            UpdateComStatus(true);
        }

        private void CloseSerialPort()
        {
            if (_serialPort == null)
            {
                UpdateComStatus(false);
                return;
            }

            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Lỗi khi đóng cổng COM: {ex.Message}");
            }
            finally
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.ErrorReceived -= SerialPort_ErrorReceived;
                _serialPort.Dispose();
                _serialPort = null;
            }

            AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Đã ngắt kết nối COM");
            UpdateComStatus(false);
        }

        private void UpdateComStatus(bool connected)
        {
            lblComStatus.Text = connected
                ? $"Kết nối: {cboComPort.SelectedItem}@{boBaudrate.SelectedItem}"
                : "Chưa kết nối";

            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;
            btnFanOn.Enabled = connected;
            btnFanOff.Enabled = connected;
            btnWindowOpen.Enabled = connected;
            btnWindowClose.Enabled = connected;
            btnDoorOpen.Enabled = connected;
            btnDoorClose.Enabled = connected;
            btnBuzzerOn.Enabled = connected;
            btnBuzzerOff.Enabled = connected;
            btnModeAuto.Enabled = connected;
            btnModeManual.Enabled = connected;
            btnHelp.Enabled = connected;
            btnResetAlarm.Enabled = connected;
        }

        private void SerialPort_DataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort == null)
            {
                return;
            }

            try
            {
                var response = _serialPort.ReadLine();
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] RX: {response}");
            }
            catch (TimeoutException)
            {
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] RX: Timeout");
            }
            catch (Exception ex)
            {
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Lỗi khi nhận dữ liệu COM: {ex.Message}");
            }
        }

        private void SerialPort_ErrorReceived(object? sender, SerialErrorReceivedEventArgs e)
        {
            AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Lỗi COM: {e.EventType}");
        }

        private void AppendSerialLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendSerialLog(message)));
                return;
            }

            textBox2.AppendText(message + Environment.NewLine);
        }

        private bool TrySendSerialCommand(string command)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
            {
                return false;
            }

            try
            {
                _serialPort.WriteLine(command);
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] TX: {command}");
                return true;
            }
            catch (Exception ex)
            {
                AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Lỗi khi gửi COM: {ex.Message}");
                return false;
            }
        }

        private string GetSerialCommandString(string dbCommand)
        {
            return dbCommand switch
            {
                "FAN_ON" => "FAN_ON",
                "FAN_OFF" => "FAN_OFF",
                "WINDOW_OPEN" => "WINDOW_OPEN",
                "WINDOW_CLOSE" => "WINDOW_CLOSE",
                "DOOR_OPEN" => "DOOR_OPEN",
                "DOOR_CLOSE" => "DOOR_CLOSE",
                "BUZZER_ON" => "BUZZER_ON",
                "BUZZER_OFF" => "BUZZER_OFF",
                "MODE_AUTO" => "MODE_AUTO",
                "MODE_MANUAL" => "MODE_MANUAL",
                _ => dbCommand,
            };
        }

        private void BtnResetAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                _db.UpdateDeviceStatus(lastMessage: "HỆ THỐNG AN TOÀN", ledState: "SAFE");
                LoadDeviceStatus();
                MessageBox.Show("Đã đặt lại trạng thái cảnh báo.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt lại cảnh báo:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboAccessStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDoorAccessLog();
        }

        private void LoadSensorData()
        {
            try
            {
                var table = _db.GetSensorData(100);
                dataGridView3.Columns.Clear();
                dataGridView3.AutoGenerateColumns = true;
                dataGridView3.DataSource = table;
                dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp dữ liệu cảm biến:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAlertHistory()
        {
            try
            {
                string alertType = null;
                if (cboAlertType.SelectedItem != null)
                {
                    var value = cboAlertType.SelectedItem.ToString();
                    if (!string.IsNullOrWhiteSpace(value)) alertType = value;
                }

                string level = null;
                if (comboBox1.SelectedItem != null)
                {
                    var value = comboBox1.SelectedItem.ToString();
                    if (!string.IsNullOrWhiteSpace(value)) level = value;
                }

                var table = _db.GetAlertHistory(100, alertType, level);
                dataGridView2.Columns.Clear();
                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.DataSource = table;
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp danh sách cảnh báo:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDoorAccessLog()
        {
            try
            {
                string filter = null;
                if (cboAccessStatus.SelectedItem != null)
                {
                    var value = cboAccessStatus.SelectedItem.ToString();
                    if (!string.IsNullOrWhiteSpace(value)) filter = value;
                }

                var table = _db.GetDoorAccessLog(100, filter);
                dgvAccess.Columns.Clear();
                dgvAccess.AutoGenerateColumns = true;
                dgvAccess.DataSource = table;
                dgvAccess.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp lịch sử truy cập:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserList()
        {
            try
            {
                var table = _db.GetAccounts();
                dgvUser.Columns.Clear();
                dgvUser.AutoGenerateColumns = true;
                dgvUser.DataSource = table;
                dgvUser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp danh sách người dùng:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadConfigValues()
        {
            try
            {
                var config = _db.GetThresholds(new[]
                {
                    "GAS_WARNING", "GAS_DANGER", "TEMP_WARNING", "TEMP_DANGER", "AUTO_MODE", "COM_PORT", "BAUDRATE"
                });

                if (config.TryGetValue("GAS_WARNING", out var gasWarning) && decimal.TryParse(gasWarning, out var gw))
                {
                    numGasWarning.Value = gw;
                }

                if (config.TryGetValue("GAS_DANGER", out var gasDanger) && decimal.TryParse(gasDanger, out var gd))
                {
                    numGasDanger.Value = gd;
                }

                if (config.TryGetValue("TEMP_WARNING", out var tempWarning) && decimal.TryParse(tempWarning, out var tw))
                {
                    numTempWarning.Value = tw;
                }

                if (config.TryGetValue("TEMP_DANGER", out var tempDanger) && decimal.TryParse(tempDanger, out var td))
                {
                    numTempDanger.Value = td;
                }

                if (config.TryGetValue("AUTO_MODE", out var autoMode))
                {
                    cboDefaultMode.SelectedItem = autoMode == "1" ? "AUTO" : "MANUAL";
                }
                else
                {
                    cboDefaultMode.SelectedItem = "AUTO";
                }

                if (config.TryGetValue("COM_PORT", out var comPort) && !string.IsNullOrWhiteSpace(comPort))
                {
                    if (cboComPort.Items.Contains(comPort))
                    {
                        cboComPort.SelectedItem = comPort;
                    }
                }

                if (config.TryGetValue("BAUDRATE", out var baudRate) && !string.IsNullOrWhiteSpace(baudRate))
                {
                    boBaudrate.SelectedItem = baudRate;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp cấu hình:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDeviceStatus()
        {
            try
            {
                var row = _db.GetDeviceStatus();
                if (row == null)
                {
                    return;
                }

                lblLevel.Text = row.Field<string>("system_level") ?? "N/A";
                lblMessage.Text = row.Field<string>("last_message") ?? "Không có";
                lblFan.Text = row.Field<bool?>("fan_status") == true ? "Bật" : "Tắt";
                lblDoor.Text = row.Field<bool?>("door_status") == true ? "Mở" : "Đóng";
                lblWindow.Text = row.Field<bool?>("window_status") == true ? "Mở" : "Đóng";
                lblBuzzer.Text = row.Field<bool?>("buzzer_status") == true ? "Bật" : "Tắt";
                lblMode.Text = row.Field<bool?>("auto_mode") == true ? "AUTO" : "MANUAL";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp trạng thái thiết bị:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCommandStatus()
        {
            try
            {
                var table = _db.GetRecentCommands(1);
                if (table.Rows.Count > 0)
                {
                    var row = table.Rows[0];
                    lblLastCommand.Text = row.Field<string>("command") ?? "-";
                    lblCommandResult.Text = row.Field<string>("status") ?? "-";
                }
                else
                {
                    lblLastCommand.Text = "-";
                    lblCommandResult.Text = "-";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp trạng thái lệnh:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendDeviceCommand(string dbCommand, int? fanStatus, int? doorStatus, int? windowStatus, int? buzzerStatus, string? ledState, int? autoMode, string userMessage)
        {
            try
            {
                var serialCommand = GetSerialCommandString(dbCommand);
                var serialSent = TrySendSerialCommand(serialCommand);

                if (!serialSent)
                {
                    AppendSerialLog($"[{DateTime.Now:HH:mm:ss}] Chưa gửi được lệnh COM, chỉ ghi DB: {serialCommand}");
                }

                _db.InsertCommand("WINFORM", dbCommand, CurrentAccountId, userMessage);
                _db.UpdateDeviceStatus(fanStatus, doorStatus, windowStatus, buzzerStatus, ledState, autoMode, null, userMessage);
                LoadDeviceStatus();
                LoadCommandStatus();
                MessageBox.Show($"Đã gửi lệnh: {userMessage}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi lệnh điều khiển:\n{ex.Message}", "Lỗi COM/DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearUserForm()
        {
            _selectedUserId = null;
            textBox1.Text = string.Empty;
            txtPassword.Text = string.Empty;
            cboRole.SelectedItem = null;
        }

        private void tabDashboard_Click(object sender, EventArgs e)
        {
        }

        private void lblLevel_Click(object sender, EventArgs e)
        {
        }

        private void lblMessage_Click(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void label15_Click(object sender, EventArgs e)
        {
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
        }

        private void lblLastCommand_Click(object sender, EventArgs e)
        {
        }

        private void lblCurrentLevel_Click(object sender, EventArgs e)
        {
        }

        private void dtpAlertFrom_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }
    }
}
