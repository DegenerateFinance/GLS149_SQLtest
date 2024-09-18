﻿namespace GLS149_SQLtest
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BtnConnect = new Button();
            LblServer = new Label();
            TbServer = new TextBox();
            LblDatabase = new Label();
            LblUser = new Label();
            LblPassword = new Label();
            TbDatabase = new TextBox();
            TbUser = new TextBox();
            TbPassword = new TextBox();
            LblDriver = new Label();
            TbDriver = new TextBox();
            SuspendLayout();
            // 
            // BtnConnect
            // 
            BtnConnect.Location = new Point(148, 242);
            BtnConnect.Name = "BtnConnect";
            BtnConnect.Size = new Size(193, 64);
            BtnConnect.TabIndex = 0;
            BtnConnect.Text = "Connect";
            BtnConnect.UseVisualStyleBackColor = true;
            BtnConnect.Click += BtnConnect_Click;
            // 
            // LblServer
            // 
            LblServer.AutoSize = true;
            LblServer.Location = new Point(99, 92);
            LblServer.Name = "LblServer";
            LblServer.Size = new Size(39, 15);
            LblServer.TabIndex = 1;
            LblServer.Text = "Server";
            // 
            // TbServer
            // 
            TbServer.Location = new Point(178, 84);
            TbServer.Name = "TbServer";
            TbServer.Size = new Size(210, 23);
            TbServer.TabIndex = 2;
            TbServer.Text = "127.0.0.1";
            // 
            // LblDatabase
            // 
            LblDatabase.AutoSize = true;
            LblDatabase.Location = new Point(99, 126);
            LblDatabase.Name = "LblDatabase";
            LblDatabase.Size = new Size(55, 15);
            LblDatabase.TabIndex = 3;
            LblDatabase.Text = "Database";
            // 
            // LblUser
            // 
            LblUser.AutoSize = true;
            LblUser.Location = new Point(99, 164);
            LblUser.Name = "LblUser";
            LblUser.Size = new Size(30, 15);
            LblUser.TabIndex = 4;
            LblUser.Text = "User";
            // 
            // LblPassword
            // 
            LblPassword.AutoSize = true;
            LblPassword.Location = new Point(99, 196);
            LblPassword.Name = "LblPassword";
            LblPassword.Size = new Size(57, 15);
            LblPassword.TabIndex = 5;
            LblPassword.Text = "Password";
            // 
            // TbDatabase
            // 
            TbDatabase.Location = new Point(178, 118);
            TbDatabase.Name = "TbDatabase";
            TbDatabase.Size = new Size(210, 23);
            TbDatabase.TabIndex = 6;
            TbDatabase.Text = "ODBC_GLS149_test";
            // 
            // TbUser
            // 
            TbUser.Location = new Point(178, 156);
            TbUser.Name = "TbUser";
            TbUser.Size = new Size(210, 23);
            TbUser.TabIndex = 7;
            TbUser.Text = "root";
            // 
            // TbPassword
            // 
            TbPassword.Location = new Point(178, 188);
            TbPassword.Name = "TbPassword";
            TbPassword.Size = new Size(210, 23);
            TbPassword.TabIndex = 8;
            TbPassword.Text = "root";
            // 
            // LblDriver
            // 
            LblDriver.AutoSize = true;
            LblDriver.Location = new Point(99, 54);
            LblDriver.Name = "LblDriver";
            LblDriver.Size = new Size(38, 15);
            LblDriver.TabIndex = 9;
            LblDriver.Text = "Driver";
            // 
            // TbDriver
            // 
            TbDriver.Location = new Point(178, 46);
            TbDriver.Name = "TbDriver";
            TbDriver.Size = new Size(210, 23);
            TbDriver.TabIndex = 10;
            TbDriver.Text = "MySQL ODBC 8.4 ANSI Driver";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(694, 523);
            Controls.Add(TbDriver);
            Controls.Add(LblDriver);
            Controls.Add(TbPassword);
            Controls.Add(TbUser);
            Controls.Add(TbDatabase);
            Controls.Add(LblPassword);
            Controls.Add(LblUser);
            Controls.Add(LblDatabase);
            Controls.Add(TbServer);
            Controls.Add(LblServer);
            Controls.Add(BtnConnect);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnConnect;
        private Label LblServer;
        private TextBox TbServer;
        private Label LblDatabase;
        private Label LblUser;
        private Label LblPassword;
        private TextBox TbDatabase;
        private TextBox TbUser;
        private TextBox TbPassword;
        private Label LblDriver;
        private TextBox TbDriver;
    }
}