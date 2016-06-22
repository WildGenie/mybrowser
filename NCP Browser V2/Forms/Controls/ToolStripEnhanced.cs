using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    public class ToolStripEnhanced : ToolStripMenuItem
    {
        private string SpecialType;

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

        public ToolStripEnhanced(String Text, EventHandler OnClick, Form form, ToolStripMenuItem DropDownParent, NCP_Browser.Internals.AsyncBrowserScripting bs)
        {
            base.Text = Text;
            base.Click += OnClick;
            this.form = form;
            this.form.FormClosed += form_FormClosed;
            this.DropDownParent = DropDownParent;
            this.BrowserScripting = bs;
        }

        public ToolStripEnhanced(string SpecialType, string URI, ToolStripMenuItem DropDownParent)
        {
            // TODO: Complete member initialization
            this.SpecialType = SpecialType;
            this.URI = URI;
            base.Text = SpecialType;
            this.DropDownParent = DropDownParent;

            switch(this.SpecialType)
            {
                case "Finess":
                    LoadFireFox();
                    break;
                default:
                    throw new Exception("Unknown special type");
            }
        }

        #region FireFox
        private void LoadFireFox()
        {
            object currentVersion = null;

            currentVersion = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Mozilla\Mozilla Firefox", "CurrentVersion", null);
            if(currentVersion != null)
            {
                LoadFireFoxVersion((string)currentVersion);
            }
            else
            {
                MessageBox.Show("It appears firefox is not installed. Contact support.");
            }
        }

        private void LoadFireFoxVersion(string currentVersion)
        {
            object pathToExe = null;

            pathToExe = Registry.GetValue(String.Format(@"HKEY_CURRENT_USER\Software\Mozilla\Mozilla Firefox\{0}\Main", currentVersion), "PathToExe", null);

            if(pathToExe != null)
            {
                StartFireFoxProcess((string)pathToExe);
            }
            else
            {
                MessageBox.Show("It appears firefox is not installed. Contact support.");
            }
        }

        Process ffP;
        IntPtr? ffpIntPtr = null;

        private void StartFireFoxProcess(string PathToExe)
        {
            ffP = new Process();
            
            ffP.StartInfo = new ProcessStartInfo(PathToExe, "-new-window " + this.URI);
            ffP.Start();
            //ffP.WaitForInputIdle();
            this.Hooks = new NCP_Browser.Win32.Hooks();
            this.Hooks.Handle = ffP.MainWindowHandle;
            this.Hooks.OnWindowDestroy = FireFoxExited;
            var zeroVal = new IntPtr(0);
            for(int i = 0; i < 300 && !ffpIntPtr.HasValue; i++)
            {
                ffP.Refresh();
                if(ffP.MainWindowHandle != zeroVal)
                {
                    ffpIntPtr = ffP.MainWindowHandle;
                }
                System.Threading.Thread.Sleep(10);
            }
            /*ToolStripEnhanced Show = new ToolStripEnhanced();
            Show.Text = "Show";
            Show.Name = "Show";
            //Show.form = this.form;
            ToolStripEnhanced Close = new ToolStripEnhanced();
            Close.Text = "Close";
            Close.Name = "Close";
            //Close.form = tse.form;
            Show.AutoSize = true;
            Close.AutoSize = true;
            Show.Click += Show_Click;
            Close.Click += Close_Click;
            this.DropDownItems.AddRange(new ToolStripEnhanced[] { Show, Close });*/
        }

        void Close_Click(object sender, EventArgs e)
        {
            var ctl = Control.FromHandle(ffP.MainWindowHandle);
            var type = ctl.GetType();
        }

        void Show_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FireFoxExited(IntPtr hWnd)
        {
 	        if(hWnd == ffpIntPtr)
            {
                if (this.DropDownParent.Owner.InvokeRequired)
                {
                    this.DropDownParent.Owner.Invoke(new Action<IntPtr>((IntPtr hwnd) => 
                    { 
                        this.DropDownParent.DropDownItems.Remove(this); 
                    }), new object[] { hWnd });
                }
                else
                {
                    this.DropDownParent.DropDownItems.Remove(this);
                }
            }
        }
        #endregion

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
                NCP_Browser.Internals.AsyncBrowserScripting.WebNavUpdate(((BaseBaseForm)this.form));
            }
        }

        internal void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.WebNavUpdate((BaseBaseForm)this.form);
        }

        internal void Browser_ForwardButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)((BaseBaseForm)this.form).Browser).GetBrowser().GoForward();
            NCP_Browser.Internals.AsyncBrowserScripting.WebNavUpdate((BaseBaseForm)this.form);
        }

        internal void Browser_BackButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)((BaseBaseForm)this.form).Browser).GetBrowser().GoBack();
            NCP_Browser.Internals.AsyncBrowserScripting.WebNavUpdate((BaseBaseForm)this.form);
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
                //browser.RequestHandler = new NCP_Browser.Internals.QFund.RequestHandler();
                browser.LifeSpanHandler = new NCP_Browser.Internals.QFund.LifeSpanHandler();
                browser.LoadHandler = new NCP_Browser.Internals.QFund.LoadHandler();
            }
            Application.Run();
        }

        void RefreshButton_Click(object sender, EventArgs e)
        {
            ((CefSharp.WinForms.ChromiumWebBrowser)this.browser).GetBrowser().Reload(true);
        }

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
            Show.Click += NCP_Browser.Internals.AsyncBrowserScripting.ActivateForm;
            Close.Click += NCP_Browser.Internals.AsyncBrowserScripting.CloseForm;
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

        public NCP_Browser.Internals.AsyncBrowserScripting BrowserScripting { get; set; }

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
    
public  Win32.Hooks Hooks { get; set; }}
}
