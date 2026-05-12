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
    public partial class FrmLogin : Form
    {
        // Thong tin dang nhap duoc truyen sang FormMain
        public string LoggedInUsername { get; private set; } = "";
        public string LoggedInRole { get; private set; } = "";
        public int LoggedInAccountId { get; private set; } = 0;

        // Chuoi ket noi MySQL
        private const string ConnStr =
            "Server=localhost;Port=3306;Database=nha_thong_minh;" +
            "Uid=root;Pwd=123456789@;CharSet=utf8mb4;";

        public FrmLogin()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;

            btnLogin.Click += btnLogin_Click;
            btnExit.Click += btnExit_Click;

            // Cho phep nhan Enter de dang nhap
            this.AcceptButton = btnLogin;
        }

        private void lblTitle_Click(object sender, EventArgs e) { }
        private void lblSubTitle_Click(object sender, EventArgs e) { }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Vui lòng nhập đầy đủ thông tin";
                return;
            }

            try
            {
                using var conn = new MySqlConnection(ConnStr);
                conn.Open();

                // Lay tai khoan tu DB
                string sql = "SELECT `id`, `username`, `password_hash`, `role`, `is_active` " +
                             "FROM `accounts` WHERE `username` = @u LIMIT 1";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Sai tài khoản hoặc mật khẩu";
                    return;
                }

                int id = reader.GetInt32("id");
                string storedHash = reader.GetString("password_hash");
                string role = reader.GetString("role");
                bool isActive = reader.GetBoolean("is_active");

                if (!isActive)
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Tài khoản đã bị khóa";
                    return;
                }

                // B1: So sanh truc tiep plaintext (VD: nguoi dung nhap "1234", DB luu "1234")
                bool passwordOk = (password == storedHash);

                // B2: Neu khong khop, thu kiem tra bcrypt (truong hop DB luu hash)
                if (!passwordOk && storedHash.StartsWith("$2"))
                {
                    try
                    {
                        passwordOk = BCrypt.Net.BCrypt.Verify(password, storedHash);
                    }
                    catch { passwordOk = false; }
                }

                if (!passwordOk)
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Sai tài khoản hoặc mật khẩu";
                    return;
                }

                reader.Close();

                // Cap nhat last_login
                string updateSql = "UPDATE `accounts` SET `last_login` = NOW() WHERE `id` = @id";
                using var updateCmd = new MySqlCommand(updateSql, conn);
                updateCmd.Parameters.AddWithValue("@id", id);
                updateCmd.ExecuteNonQuery();

                // Luu thong tin dang nhap
                LoggedInUsername = username;
                LoggedInRole = role;
                LoggedInAccountId = id;

                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "Đăng nhập thành công!";

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (MySqlException ex)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Lỗi kết nối DB";
                MessageBox.Show($"Lỗi kết nối MySQL:\n{ex.Message}\n\nVui lòng kiểm tra:\n" +
                    "- MySQL đang chạy\n- Database 'nha_thong_minh' đã tạo\n" +
                    "- Đã chạy db_setup.py",
                    "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
