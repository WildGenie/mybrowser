using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    public class ToolStripEnhanced : ToolStripMenuItem
    {
        public Form form { get; set; }
        internal ToolStripMenuItem DropDownParent { get; set; }
        public string URI { get; set; }
        public System.Threading.Thread browserThread { get; set; }
        public object browser { get; set; }
        public object parentBrowser { get; set; }

        public ToolStripEnhanced()
        {
            
        }

        public ToolStripEnhanced(String Text, EventHandler OnClick, Form form, ToolStripMenuItem DropDownParent)
        {
            base.Text = Text;
            base.Click += OnClick;
            this.form = form;
            this.form.FormClosed += form_FormClosed;
            this.DropDownParent = DropDownParent;
        }

        public ToolStripEnhanced(String Text, EventHandler OnClick, Form form, ToolStripMenuItem DropDownParent, BrowserScripting bs)
        {
            base.Text = Text;
            base.Click += OnClick;
            this.form = form;
            this.form.FormClosed += form_FormClosed;
            this.DropDownParent = DropDownParent;
            this.BrowserScripting = bs;
        }

        internal void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(this.DropDownParent.Owner.InvokeRequired)
            {
                this.DropDownParent.Owner.Invoke(new FormClosingDelegate(this.form_FormClosed), new object[] { sender, e });
            }
            else
            {
                this.DropDownParent.DropDownItems.Remove(this);
            }
        }

        internal void Browser_IsBrowserInitializedChanged(object sender, CefSharp.IsBrowserInitializedChangedEventArgs e)
        {
            if (e.IsBrowserInitialized)
            {
                BrowserScripting.WebNavUpdate(((BaseBaseForm)this.form));
            }
        }

        internal void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            BrowserScripting.WebNavUpdate((BaseBaseForm)this.form);
        }

        internal void Browser_ForwardButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)((BaseBaseForm)this.form).Browser).GetBrowser().GoForward();
            BrowserScripting.WebNavUpdate((BaseBaseForm)this.form);
        }

        internal void Browser_BackButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)((BaseBaseForm)this.form).Browser).GetBrowser().GoBack();
            BrowserScripting.WebNavUpdate((BaseBaseForm)this.form);
        }

        internal void TextSetter(String Text)
        {
            this.form.Text = Text;
            this.Text = Text;
        }

        internal void OpenForm()
        {
            browserThread = new System.Threading.Thread(Run);
            browserThread.SetApartmentState(System.Threading.ApartmentState.STA);
            browserThread.Start();
            
            /*if(this.form.InvokeRequired)
            {
                this.form.Invoke(new Action(OpenForm));
            }
            else
            { 
                this.form.Show();
                ((QFund)this.form).qfundBrowser.Url = new Uri(this.URI);
            }*/
        }
        internal void Run()
        {
            this.form.Show();
            
            this.browser = new CefSharp.WinForms.ChromiumWebBrowser(this.URI);
            var browser = (CefSharp.WinForms.ChromiumWebBrowser)this.browser;
            browser.Dock = DockStyle.Fill;            
            
            this.form.Controls.Add(browser);
            if (this.form.GetType().FullName == Type.GetType("NCP_Browser.QFund").FullName)
            {
                ((QFund)this.form).RefreshButton.Click += RefreshButton_Click;
                ((QFund)this.form).RefreshButton.Visible = true;
                browser.BringToFront();
                browser.RequestHandler = new QfundChromiumRestriction();
                browser.LifeSpanHandler = new Blah();
                browser.LoadHandler = new BlahBLah();
            }
            //((SHDocVw.WebBrowser)browser.ActiveXInstance).NewWindow2 += this.browser_NewWindow2;            

            //this.form.FormClosing += form_FormClosing;
            Application.Run();
        }

        void RefreshButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)this.browser).GetBrowser().Reload(true);
        }

        /*
        void ToolStripEnhanced_NewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)
        {
            Processed = true;
            QFund popupBrowser = new QFund();
            popupBrowser.BrowserName = String.Format("{0} - Popup", this.form.Text);
            ToolStripEnhanced tse = new ToolStripEnhanced("QFund Popup", this.BrowserScripting.generic_Menu, popupBrowser, this.DropDownParent, this.BrowserScripting);
            tse.parentBrowser = this.browser;
            tse.browser = new WebBrowser();
            tse.browser.DocumentCompleted += tse.browser_DocumentCompleted;
            tse.browser.DocumentCompleted += tse.popupWebBrowser_Navigated;
            tse.browser.Navigate(URL);            
            tse.browser.ObjectForScripting = this.BrowserScripting;
            popupBrowser.Controls.Add(tse.browser);
            tse.browser.Dock = DockStyle.Fill;
            //tse.browser.ScriptErrorsSuppressed = true;            
            popupBrowser.StartPosition = FormStartPosition.Manual;
            Screen[] screens = Screen.AllScreens;
            Screen qfScreen = Screen.FromControl(this.form);
            int MonitorIndex = 0;
            for (int i = 0; i < screens.Length; i++)
            {
                //MessageBox.Show(s.DeviceName);
                if (screens[i].DeviceName == qfScreen.DeviceName)
                {
                    MonitorIndex = i;
                }
            }
            popupBrowser.StartPosition = FormStartPosition.Manual;
            popupBrowser.Location = screens[MonitorIndex].WorkingArea.Location;
            popupBrowser.Width = (int)(screens[MonitorIndex].WorkingArea.Width * .8);
            popupBrowser.Height = (int)(screens[MonitorIndex].WorkingArea.Height * .8);
            popupBrowser.Top = (int)(screens[MonitorIndex].WorkingArea.Height * .1);
            popupBrowser.Left = (int)(screens[MonitorIndex].WorkingArea.Width * 1.1);
            popupBrowser.Show();
        }

        void browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //MessageBox.Show(((WebBrowser)sender).DocumentText.IndexOf("window.opener.document.forms[0].transactionCode.value=\"History\";").ToString());
            ((WebBrowser)sender).Document.InvokeScript("alert('bing bong');");
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlWindow htmlPopup = ((WebBrowser)sender).Document.Window;
            HtmlWindow htmlOpener = this.parentBrowser.Document.Window;

            FieldInfo fi = htmlPopup.GetType().GetField("htmlWindow2", BindingFlags.Instance | BindingFlags.NonPublic);

            mshtml.IHTMLWindow2 htmlPopup2 = (mshtml.IHTMLWindow2)fi.GetValue(htmlPopup);
            mshtml.IHTMLWindow2 htmlOpener2 = (mshtml.IHTMLWindow2)fi.GetValue(htmlOpener);

            htmlPopup2.window.opener = htmlOpener2.window.self;
        }

        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((SHDocVw.WebBrowser)this.browser.ActiveXInstance).NewWindow2 -= this.browser_NewWindow2;
        }

        internal void browser_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            var popupBrowser = new QFund();
            
            popupBrowser.Show(this.form);
            var popupWebBrowser = new WebBrowser();
            popupWebBrowser.ScriptErrorsSuppressed = true;
            popupBrowser.Controls.Add(popupWebBrowser);
            popupWebBrowser.Dock = DockStyle.Fill;
            ((SHDocVw.WebBrowser)popupWebBrowser.ActiveXInstance).NewWindow2 += this.browser_NewWindow2;
            ToolStripEnhanced tse = new ToolStripEnhanced("QFund Popup", this.BrowserScripting.generic_Menu, popupBrowser, this.DropDownParent);
            this.AddToDropDown(tse);
            tse.parentBrowser = this.browser;
            tse.browser = popupWebBrowser;
            popupWebBrowser.DocumentCompleted += tse.browser_DocumentCompleted;
            ppDisp = popupWebBrowser.ActiveXInstance;
        }

        void popupWebBrowser_Navigated(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //((WebBrowser)sender).Document.Body.AppendChild(((WebBrowser)sender).Document.CreateElement("script"));

            var webBrowser1 = (WebBrowser)sender;
            var frm = (QFund)this.form;
            this.form.Text = String.Format("{0} - {1}",frm.BrowserName,webBrowser1.DocumentTitle);

            HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = @"function check() { 
                if(refreshFlag==false){
		            var prodType=window.opener.sessionStorage.getItem('prodType');
                    alert(prodType);
		            window.opener.document.getElementsByName('transactionCode').value = 'History';
		            if(prodType=='IL' && prodType!=null){ 
		                window.opener.document.forms[0].action='/ncp/intTransInstallmentHistory.do'; //?requestBean.dealNbr='+dealNbr;
		                window.opener.document.forms[0].submit();
		            }else if(prodType=='PDL' && prodType!=null){
		                window.opener.document.forms[0].action = '/ncp/transHistory.do?transactionCode=History';
		                window.opener.document.forms[0].submit();
	                }else if(prodType=='TLP' && prodType!=null){ 
		                window.opener.document.forms[0].action='/ncp/titleTransHistory.do' // ?requestBean.dealNbr='+dealNbr;
		                window.opener.document.forms[0].submit();
		            }else if(prodType=='PTL' && prodType!=null){
		                window.opener.document.forms[0].action='/ncp/titleTransHistory.do' // ?requestBean.dealNbr='+dealNbr;
		                window.opener.document.forms[0].submit();
		            }
		        }
            }";
            head.AppendChild(scriptEl);
            //webBrowser1.Document.InvokeScript("sayHello");


            //((WebBrowser)sender).Document.InvokeScript("alert('bing bong');");
        }        
        */
        internal void AddToDropDown(ToolStripEnhanced tse)
        {
            if (this.DropDownParent.GetCurrentParent().InvokeRequired)
            {
                this.DropDownParent.GetCurrentParent().Invoke(new AddToDropDownDelegate(this.AddToDropDown), tse);
            }
            else
            {
                AddShowClose(tse);
                this.DropDownParent.DropDownItems.Add(tse);
            }
        }

        internal static void AddShowClose(ToolStripEnhanced tse)
        {
            ToolStripEnhanced Show = new ToolStripEnhanced();
            Show.Text = "Show";
            Show.Name = "Show";
            Show.form = tse.form;
            ToolStripEnhanced Close = new ToolStripEnhanced();
            Close.Text = "Close";
            Close.Name = "Close";
            Close.form = tse.form;
            Show.AutoSize = true;
            Close.AutoSize = true;
            Show.Click += BrowserScripting.ActivateForm;
            Close.Click += BrowserScripting.CloseForm;
            tse.DropDownItems.AddRange(new ToolStripEnhanced[] { Show, Close });
        }

        internal void ChromiumBrowser_TitleChanged(object sender, CefSharp.TitleChangedEventArgs e)
        {
            if(this.form.InvokeRequired)
            {
                this.form.Invoke(new ChromiumBrowserTitleChanged(ChromiumBrowser_TitleChanged), new object[] { sender, e });
            }
            else
            { 
                this.form.Text = String.Format("{0} - {1}",((BaseBaseForm)this.form).Title, e.Title);
            }
        }

        public delegate void AddToDropDownDelegate(ToolStripEnhanced tse);
        public delegate void FormClosingDelegate(object sender, FormClosedEventArgs e);
        public delegate void ChromiumBrowserTitleChanged(object sender, CefSharp.TitleChangedEventArgs e);

        public BrowserScripting BrowserScripting { get; set; }

        internal void Browser_HomeButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)((BaseBaseForm)this.form).Browser).Load(((BaseBaseForm)this.form).URL);
        }

        internal void Browser_ReloadButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)((BaseBaseForm)this.form).Browser).GetBrowser().Reload(true);
        }

        internal void Browser_HaltButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)((BaseBaseForm)this.form).Browser).GetBrowser().StopLoad();
        }
    }
}
