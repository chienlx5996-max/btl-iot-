namespace Smarthome
{
    partial class FrmLogin
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
            lblTitle = new Label();
            lblSubTitle = new Label();
            label1 = new Label();
            txtUsername = new TextBox();
            label2 = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            btnExit = new Button();
            lblMessage = new Label();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
            lblTitle.Location = new Point(123, 21);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(236, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "SMART HOME IOT SYSTEM";
            lblTitle.Click += lblTitle_Click;
            // 
            // lblSubTitle
            // 
            lblSubTitle.AutoSize = true;
            lblSubTitle.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
            lblSubTitle.Location = new Point(75, 55);
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Size = new Size(336, 25);
            lblSubTitle.TabIndex = 1;
            lblSubTitle.Text = "Giám sát an toàn và thông gió tự động";
            lblSubTitle.Click += lblSubTitle_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label1.Location = new Point(70, 123);
            label1.Name = "label1";
            label1.Size = new Size(141, 25);
            label1.TabIndex = 2;
            label1.Text = "Tên đăng nhập:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(217, 125);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(164, 23);
            txtUsername.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label2.Location = new Point(116, 167);
            label2.Name = "label2";
            label2.Size = new Size(95, 25);
            label2.TabIndex = 4;
            label2.Text = "Mật khẩu:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(217, 172);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(164, 23);
            txtPassword.TabIndex = 5;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(116, 231);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(109, 36);
            btnLogin.TabIndex = 6;
            btnLogin.Text = "Đăng nhập";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(250, 231);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(109, 36);
            btnExit.TabIndex = 7;
            btnExit.Text = "Thoát";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
            lblMessage.Location = new Point(70, 287);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 8;
            // 
            // FrmLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 321);
            Controls.Add(lblMessage);
            Controls.Add(btnExit);
            Controls.Add(btnLogin);
            Controls.Add(txtPassword);
            Controls.Add(label2);
            Controls.Add(txtUsername);
            Controls.Add(label1);
            Controls.Add(lblSubTitle);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đặng nhập hệ thống";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblSubTitle;
        private Label label1;
        private TextBox txtUsername;
        private Label label2;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private Label lblMessage;
    }
}