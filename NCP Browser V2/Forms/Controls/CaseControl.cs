using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NCP_Browser.Forms.Controls
{
    public partial class CaseControl : UserControl
    {
        public string CaseId { get; set; }
        public CaseControl()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public SelectCase.SelectCaseDelegate SelectCaseDelegate { get; set; }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            if(SelectCaseDelegate != null)
            {
                SelectCaseDelegate.Invoke(CaseId);
            }
        }

        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern long HideCaret(IntPtr hwnd);

        private void CaseControl_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml("#F4F6F9");
            this.label1.ForeColor = ColorTranslator.FromHtml("#0070D2");
            this.label2.ForeColor = ColorTranslator.FromHtml("#0070D2");
            this.label3.ForeColor = ColorTranslator.FromHtml("#0070D2");
            this.label4.ForeColor = ColorTranslator.FromHtml("#0070D2");
            this.txt_CaseNumber.ForeColor = ColorTranslator.FromHtml("#0070D2");
            this.txt_CustomerName.ForeColor = ColorTranslator.FromHtml("#0070D2");
            this.txt_LoanNumber.ForeColor = ColorTranslator.FromHtml("#0070D2");
            this.txt_CreationDate.ForeColor = ColorTranslator.FromHtml("#0070D2");

            this.txt_CaseNumber.MouseClick += txt_CaseNumber_Enter;
            this.txt_CustomerName.MouseClick += txt_CaseNumber_Enter;
            this.txt_LoanNumber.MouseClick += txt_CaseNumber_Enter;
            this.txt_CreationDate.MouseClick += txt_CaseNumber_Enter;
            //this.btn_Select.ForeColor = ColorTranslator.FromHtml("#0070D2");

            //HideCaret(this.txt_CaseNumber.Handle);
            //HideCaret(this.txt_CustomerName.Handle);
            //HideCaret(this.txt_LoanNumber.Handle);
            //HideCaret(this.txt_CreationDate.Handle);
        }

        private void txt_CaseNumber_Enter(object sender, EventArgs e)
        {
            HideCarrots();
            this.Focus();            
        }

        private void txt_CaseNumber_MouseEnter(object sender, EventArgs e)
        {
            HideCarrots();
        }

        private void HideCarrots()
        {
            HideCaret(this.txt_CaseNumber.Handle);
            HideCaret(this.txt_CustomerName.Handle);
            HideCaret(this.txt_LoanNumber.Handle);
            HideCaret(this.txt_CreationDate.Handle);
        }

        private void txt_CaseNumber_MouseDown(object sender, MouseEventArgs e)
        {
            HideCarrots();
            this.Focus(); 
        }

        private void txt_CaseNumber_MouseMove(object sender, MouseEventArgs e)
        {
            HideCarrots();
        }

        private void btn_Select_MouseEnter(object sender, EventArgs e)
        {
            btn_Select.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#FFFFFF");
        }

        private void btn_Select_MouseLeave(object sender, EventArgs e)
        {
            this.btn_Select.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
        }
    }
}
