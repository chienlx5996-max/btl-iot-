using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private const string ConnStr =
            "Server=localhost;Port=3306;Database=nha_thong_minh;" +
            "Uid=root;Pwd=123456789@;CharSet=utf8mb4;";

        private System.Windows.Forms.Timer clockTimer;

        public FormMain()
        {
            InitializeComponent();

            Load += FormMain_Load;
            btnRefreshSensor.Click += BtnRefreshSensor_Click;
            btnRefreshAlert.Click += BtnRefreshAlert_Click;
            btnRefreshAccess.Click += BtnRefreshAccess_Click;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            lblCurrentUser.Text = $"User: {CurrentUsername}";
            lblRole.Text = $"Role: {CurrentRole}";
            lblClock.Text = $"Time: {DateTime.Now:HH:mm:ss}";

            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, ev) => lblClock.Text = $"Time: {DateTime.Now:HH:mm:ss}";
            clockTimer.Start();

            LoadSensorData();
            LoadAlertHistory();
            LoadDoorAccessLog();
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

        private void LoadSensorData()
        {
            try
            {
                using var conn = new MySqlConnection(ConnStr);
                conn.Open();

                string sql =
                    "SELECT `id`, `recorded_at`, `gas_value`, `temperature`, `pir_status`, " +
                    "`rain_status`, `system_level` FROM `sensor_data` " +
                    "ORDER BY `recorded_at` DESC LIMIT 100";

                using var adapter = new MySqlDataAdapter(sql, conn);
                var table = new DataTable();
                adapter.Fill(table);

                dataGridView3.AutoGenerateColumns = true;
                dataGridView3.DataSource = table;
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
                using var conn = new MySqlConnection(ConnStr);
                conn.Open();

                string sql =
                    "SELECT `id`, `alert_type`, `level`, `message`, `gas_value`, " +
                    "`temperature`, `pir_status`, `rain_status`, `created_at`, `resolved_at` " +
                    "FROM `alert_history` ORDER BY `created_at` DESC LIMIT 100";

                using var adapter = new MySqlDataAdapter(sql, conn);
                var table = new DataTable();
                adapter.Fill(table);

                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp dữ liệu cảnh báo:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDoorAccessLog()
        {
            try
            {
                using var conn = new MySqlConnection(ConnStr);
                conn.Open();

                string sql =
                    "SELECT `id`, `input_source`, `role_matched`, `result`, `wrong_count`, " +
                    "`note`, `opened_at`, `closed_at`, `duration_sec` " +
                    "FROM `door_access_log` ORDER BY `opened_at` DESC LIMIT 100";

                using var adapter = new MySqlDataAdapter(sql, conn);
                var table = new DataTable();
                adapter.Fill(table);

                dgvAccess.AutoGenerateColumns = true;
                dgvAccess.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp lịch sử truy cập cửa:\n{ex.Message}", "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
