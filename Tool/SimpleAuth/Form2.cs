using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SimpleAuth.Properties;

namespace SimpleAuth
{
    public partial class Form2 : XtraForm
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Auth.SimpleAuthRegister RegisterResult = Auth.Register(textEdit1.Text, textEdit2.Text, textEdit3.Text, textEdit5.Text);
            switch (RegisterResult)
            {
                case Auth.SimpleAuthRegister.INTERNETERROR:
                    XtraMessageBox.Show("Internet Error\nPlease Check Your Internet Connection", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                    break;
                case Auth.SimpleAuthRegister.UNKNOWNERROR:
                    XtraMessageBox.Show("Unknown Error\nPlease Try Again Later", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                    break;
                case Auth.SimpleAuthRegister.usernameExists:
                    XtraMessageBox.Show(string.Format("The Username {0} Has Already Been Taken", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Auth.SimpleAuthRegister.passwordNoMatch:
                    XtraMessageBox.Show("The Passwords You Entered Did Not Match", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Auth.SimpleAuthRegister.EMPTYPARAMS:
                    XtraMessageBox.Show("Please Fill Out All The Information", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Auth.SimpleAuthRegister.InvalidToken:
                    XtraMessageBox.Show("The Token You Entered Was Invlaid", "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Auth.SimpleAuthRegister.SUCCESS:
                    XtraMessageBox.Show(string.Format("Welcome {0}, \nYou Have Successfully Registered", textEdit1.Text), "SimpleAuth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Settings.Default.username = textEdit1.Text;
                    Settings.Default.password = textEdit2.Text;
                    Settings.Default.Save();
                    new Form1().ShowDialog();
                    this.Hide();
                    break;
            }
        }

        private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
        {
            new Form1().ShowDialog();
            this.Hide();
        }
    }
}