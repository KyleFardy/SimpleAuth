using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using SimpleAuth.Properties;

namespace SimpleAuth
{
    public partial class Form1 : XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textEdit1.Text = Settings.Default.username;
            textEdit2.Text = Settings.Default.password;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit1.Text) || string.IsNullOrEmpty(textEdit2.Text))
            {
                if(string.IsNullOrEmpty(textEdit1.Text))
                    XtraMessageBox.Show("Please Enter Your Username!", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if(string.IsNullOrEmpty(textEdit2.Text))
                    XtraMessageBox.Show("Please Enter Your Password!", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Auth.SimpleAuthLogin LoginResult = Auth.Login(textEdit1.Text, textEdit2.Text);
                switch (LoginResult)
                {
                    case Auth.SimpleAuthLogin.HWIDERROR:
                        XtraMessageBox.Show(string.Format("{0}\nYour HWID Does Not Match The One In Our Database", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Auth.SimpleAuthLogin.HWIDUPDATED:
                        XtraMessageBox.Show(string.Format("{0}, \nYour HWID Has Been Updated\nYou May Now Login", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case Auth.SimpleAuthLogin.INTERNETERROR:
                        XtraMessageBox.Show("Internet Error\nPlease Check Your Internet Connection", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                        break;
                    case Auth.SimpleAuthLogin.UNKNOWNERROR:
                        XtraMessageBox.Show("Unknown Error\nPlease Try Again Later", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                        break;
                    case Auth.SimpleAuthLogin.USERNAMENOTFOUND:
                        XtraMessageBox.Show(string.Format("The Username {0} Does Not Exist", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Auth.SimpleAuthLogin.PASSWORDNOTFOUND:
                        XtraMessageBox.Show(string.Format("The Password For {0} Is Incorrect", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Auth.SimpleAuthLogin.BANNED:
                        XtraMessageBox.Show(string.Format("{0}\nYou Have Been Banned", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                        break;
                    case Auth.SimpleAuthLogin.EMPTYPARAMS:
                        XtraMessageBox.Show("Please Fill Out All The Information", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Auth.SimpleAuthLogin.SUCCESS:
                        XtraMessageBox.Show(string.Format("Welcome {0},\nYou Have Successfully Signed", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        new Form3().Show();
                        this.Hide();
                        break;
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Auth.FREE_MODE MAIN = Auth.CHECK_FOR_FREEMODE();
            switch (MAIN)
            {
                case Auth.FREE_MODE.ENABLED:
                    XtraMessageBox.Show("Free Mode Enabled\nEnjoy The Free Time", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new Form3().Show();
                    this.Hide();
                    break;
                case Auth.FREE_MODE.DISABLED:
                    XtraMessageBox.Show("Free Mode Not Enabled\nYou May Purchase A Token To Use The Tool", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Auth.FREE_MODE.UNKNOWNERROR:
                    XtraMessageBox.Show("Unknown Error\nPlease Try Again Later", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Auth.FREE_MODE.INTERNETERROR:
                    XtraMessageBox.Show("Internet Error\nPlease Check Your Internet Connection", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Form2().ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
