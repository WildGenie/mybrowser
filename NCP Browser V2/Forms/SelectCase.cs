using NCP_Browser.Forms.Controls;
using NCP_Browser.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCP_Browser.Forms
{
    public partial class SelectCase : Form
    {
        public SelectCase()
        {
            InitializeComponent();
        }

        public List<CaseRelatedPhoneNumber> RelatedPhoneNumbers { get; set; }
        public int Number { get; set; }

        private void SelectCase_Load(object sender, EventArgs e)
        {

            this.panel1.AutoScroll = false;
            this.panel1.HorizontalScroll.Enabled = false;
            this.panel1.HorizontalScroll.Visible = false;
            this.panel1.AutoScroll = true;

            /*
            CaseControl cc = new CaseControl();
            cc.Left = 0;
            cc.Top = 0;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            int i = 1;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height*i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);

            i++;
            cc = new CaseControl();
            cc.Left = 0;
            cc.Top = cc.Height * i;
            cc.txt_CaseNumber.Text = "xxx";
            cc.txt_CreationDate.Text = "yyy";
            cc.txt_CustomerName.Text = "zzz";
            cc.txt_LoanNumber.Text = "fff";
            this.panel1.Controls.Add(cc);
            */
        }

        public VScrollBar vScroll { get; set; }
        public List<String> CaseIds { get; set; }

        internal void Populate(List<CaseRelatedPhoneNumber> list)
        {
            int i = 0;
            CaseIds = new List<string>();
            foreach(var item in list.OrderByDescending(x => x.CreatedDate))
            {
                var cc = new CaseControl();               

                if(CaseIds.Where(x => x == item.CaseId).Count() == 0)
                {
                    
                    cc.Left = 0;
                    cc.Top = cc.Height * i;
                    cc.CaseId = item.CaseId;
                    cc.txt_CaseNumber.Text = item.CaseNumber;
                    cc.txt_CustomerName.Text = item.CustomerName;
                    cc.txt_LoanNumber.Text = item.LoanNumber;
                    cc.txt_CreationDate.Text = item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    cc.SelectCaseDelegate = new SelectCaseDelegate(btn_Select_Click);
                    CaseIds.Add(item.CaseId);
                    this.panel1.Controls.Add(cc);
                    i++;
                }
            }
            var cct = new CaseControl();
            var barwidth = ((i * cct.Height) > this.panel1.Height) ? System.Windows.Forms.SystemInformation.VerticalScrollBarWidth * 2 : 0;
            this.MaximumSize = new Size(cct.Width + barwidth + 17, this.Height);
            this.MinimumSize = new Size(cct.Width + barwidth + 17, this.Height);
        }

        public delegate void SelectCaseDelegate(String CaseId);

        
        async void btn_Select_Click(String CaseId)
        {
            System.Threading.Thread barThread = new System.Threading.Thread(BarProgress);
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            progressBar1.Width = this.Width;
            progressBar1.Dock = DockStyle.Bottom;
            panel1.Visible = false;
            barThread.Start();
            System.Threading.Thread uploadThread = new System.Threading.Thread(async_btn_Select_Click);
            uploadThread.Start(new object[] { CaseId });
            
        }

        async void async_btn_Select_Click(object ThreadStartParam)
        {
            if (Salesforce.CallRecorderImplementation.SendFile(Number, (string)((object[])ThreadStartParam)[0]))
            {
                lock (Salesforce.FrameLoadLock)
                {
                    Salesforce.CallRecorderImplementation.Confirm(Number);
                    Salesforce.CallRecordingUpdated = true;
                    Salesforce.CallRecordings.Where(x => x.IPC_CallData.Number == Number).First().Remove = true;
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Error attaching recording. Please contact IT");
            }
        }

        void BarProgress()
        {
            for(;;)
            {
                UpdateBarProgress();
                System.Threading.Thread.Sleep(1000);
            }            
        }

        private void UpdateBarProgress()
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke(new Action(UpdateBarProgress));
            }
            else
            {
                if (this.progressBar1.Value + 10 > 100)
                {
                    this.progressBar1.Value = 0;
                }
                else
                {
                    this.progressBar1.Value += 10;
                }
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Discard_Click(object sender, EventArgs e)
        {
            lock(Salesforce.FrameLoadLock)
            {
                try
                {
                    Salesforce.CallRecordingUpdated = true;
                    Salesforce.CallRecordings.Where(x => x.IPC_CallData.Number == Number).First().Remove = true;
                    Salesforce.CallRecorderImplementation.Confirm(Number);
                }
                catch
                {
                    MessageBox.Show("Error discarding call. Please contact IT");
                }
                
            }            
        }

        private void btn_Close_MouseEnter(object sender, EventArgs e)
        {
            btn_Close.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#FFFFFF");
        }

        private void btn_Close_MouseLeave(object sender, EventArgs e)
        {
            this.btn_Close.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
        }

        private void btn_Discard_MouseEnter(object sender, EventArgs e)
        {
            btn_Discard.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#FFFFFF");
        }

        private void btn_Discard_MouseLeave(object sender, EventArgs e)
        {
            this.btn_Discard.FlatAppearance.BorderColor = System.Drawing.Color.Red;
        }

    }
}
