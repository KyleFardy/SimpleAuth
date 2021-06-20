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

namespace SimpleAuth
{
    public partial class Form3 : XtraForm
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            barStaticItem1.Caption = string.Format("Welcome : {0}", Properties.Settings.Default.username);
            label1.Text = string.Format("Username : {0}", Properties.Settings.Default.username);
            label2.Text = string.Format("Email Address : {0}", Properties.Settings.Default.email);
            label3.Text = string.Format("User Level : {0}", Properties.Settings.Default.user_level);
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
    
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}