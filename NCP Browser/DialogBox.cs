using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    public partial class DialogBox : Form
    {
        private Control control;
        private string p1;
        private string p2;

        public DialogBox()
        {
            InitializeComponent();
        }

        public DialogBox(Control Parent, string Heading, string Value)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            //this.Parent = Parent;
            this.Text = Heading;
            this.txt_Val.Text = Value;

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DialogBox_Load(object sender, EventArgs e)
        {
            
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static void ShowMeAsDialog(DialogBox bx, Salesforce sf)
        {
            if(sf.InvokeRequired)
            {
                sf.Invoke(new Action(() => { DialogBox.ShowMeAsDialog(bx, sf); }));
            }
            else
            {
                bx.ShowDialog(sf);
            }
        }
    }
}
