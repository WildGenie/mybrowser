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
    public partial class Internet : BaseForm
    {
        internal string Home;

        public delegate void SetText(String Text);
        private SetText TextSetter { get; set; }
        private String Prefix;
        private ToolStripMenuItem Dock { get; set; }
        private bool IsPdf { get; set; }

        /// <summary>
        /// You Must Call SetUpText to propery initialize title text/menu strip text updates
        /// </summary>
        public Internet(ToolStripMenuItem Dock)
        {
            this.Prefix = "Internet";
            this.MenuDock = Dock;
            InitializeComponent();
        }

        public Internet(ToolStripMenuItem Dock, bool IsPdf)
        {
            this.Prefix = "Internet";
            this.Dock = Dock;
            this.IsPdf = IsPdf;
            InitializeComponent();
        }

        public void SetUpText(SetText TextSetter, String Prefix)
        {
            this.TextSetter = TextSetter;
            this.Prefix = Prefix;
        }

        private void internetBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //this.reloadButton.Visible = false;
            //this.haltButton.Visible = true;
        }

        private void OpenPDF(object sender)
        {
            Internet pdf = new Internet(MenuDock, true);
            pdf.Text = String.Format("PDF - {0}", ((System.Windows.Forms.WebBrowser)sender).Url.AbsoluteUri);
            pdf.internetBrowser.Url = new Uri(((System.Windows.Forms.WebBrowser)sender).Url.AbsoluteUri);
            pdf.webNav.Visible = false;
            ToolStripEnhanced tsi = new ToolStripEnhanced(pdf.Text, pdf.generic_Menu, pdf, MenuDock);
            MenuDock.DropDownItems.Add(tsi);
            pdf.Show();
        }

        private void internetBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //this.haltButton.Visible = false;
            //this.reloadButton.Visible = true;
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            this.internetBrowser.Url = new Uri(Home);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.internetBrowser.GoBack();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            this.internetBrowser.GoForward();
        }

        private void haltButton_Click(object sender, EventArgs e)
        {
            this.internetBrowser.Stop();
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            this.internetBrowser.Refresh();
        }

        private void internetBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (((System.Windows.Forms.WebBrowser)sender).Url != null && ((System.Windows.Forms.WebBrowser)sender).Url.LocalPath.EndsWith(".pdf") && !this.IsPdf)
            {                
                OpenPDF(sender);
                ((System.Windows.Forms.WebBrowser)sender).GoBack();
            }
            else if(this.TextSetter != null)
            { 
                TextSetter.Invoke(String.Format("{0} - {1}", this.Prefix, this.internetBrowser.DocumentTitle));
            }
        }

        private void Internet_Load(object sender, EventArgs e)
        {

        }

    }
}
