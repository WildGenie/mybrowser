using SHDocVw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    public class BaseForm : BaseBaseForm
    {
        internal ToolStripMenuItem MenuDock { get; set; }
        public SHDocVw.WebBrowser_V1 axBrowserV1 { get; set; }
        internal System.Windows.Forms.WebBrowser webRef { get; set; }

        public void BaseInit()
        {
            if(this.webRef != null)
            { 
                axBrowserV1 = (SHDocVw.WebBrowser_V1)webRef.ActiveXInstance;
                axBrowserV1.NewWindow += new DWebBrowserEvents_NewWindowEventHandler(axBrowserV1_NewWindow);
            }
        }

        internal void generic_Menu(object sender, EventArgs e)
        {
            ((ToolStripEnhanced)sender).form.Activate();
        }

        internal void OpenPDF(object sender)
        {
            Internet pdf = new Internet(MenuDock, true);
            pdf.Text = String.Format("PDF - {0}", ((System.Windows.Forms.WebBrowser)sender).Url.AbsoluteUri);
            pdf.internetBrowser.Url = new Uri(((System.Windows.Forms.WebBrowser)sender).Url.AbsoluteUri);
            pdf.webNav.Visible = false;
            ToolStripEnhanced tsi = new ToolStripEnhanced(pdf.Text, pdf.generic_Menu, pdf, MenuDock);
            MenuDock.DropDownItems.Add(tsi);
            pdf.Show();
        }

        void axBrowserV1_NewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)
        {
            
            Internet nint = new Internet(this.MenuDock);
            nint.internetBrowser.Url = new Uri(URL);
            nint.Home = URL;

            ToolStripEnhanced tsi = new ToolStripEnhanced(nint.Text, generic_Menu, nint, MenuDock);
            MenuDock.DropDownItems.Add(tsi);
            nint.SetUpText(tsi.TextSetter, "Internet");
            nint.Show();
            Processed = true;
            
        }
    }
}
