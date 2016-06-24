using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCP_Browser.Jabber
{
    public partial class Credentials : Form
    {
        public Credentials()
        {
            InitializeComponent();
        }

        private void btn_LogIn_Click(object sender, EventArgs e)
        {
            Manager.WriteCredentials(this.txtPassword.Text);
            this.Close();
        }

        private void Credentials_Load(object sender, EventArgs e)
        {
            var info = Manager.LoadCredentials();
            this.txtUserName.Text = info.UserName;
            this.txtPassword.Text = info.Password;
        }
    }
}
