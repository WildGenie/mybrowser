using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using SHDocVw;
using System.IO;
using System.Reflection;
using System.Media;
using CefSharp;
using System.Diagnostics;

namespace NCP_Browser
{
    [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
    public partial class Salesforce : BaseForm
    {
        private int screenHeight = 0;
        private int screenWidth = 0;
        internal CefSharp.WinForms.ChromiumWebBrowser Chromium;
        
        private ReloadMe reloader;
        private CloseMe closing;
        internal BrowserScripting browserScripting;
        private MessageFilter messageFilter;
        private bool Initialize1;
        private string SalesforceInstance;
        private jabber.client.JabberClient JabberClient;



        public const string DefaultUrl = "custom://cefsharp/home.html";
        public const string BindingTestUrl = "custom://cefsharp/BindingTest.html";
        public const string PluginsTestUrl = "custom://cefsharp/plugins.html";
        public const string PopupTestUrl = "custom://cefsharp/PopupTest.html";
        public const string BasicSchemeTestUrl = "custom://cefsharp/SchemeTest.html";
        public const string ResponseFilterTestUrl = "custom://cefsharp/ResponseFilterTest.html";
        public const string DraggableRegionTestUrl = "custom://cefsharp/DraggableRegionTest.html";
        public const string TestResourceUrl = "http://test/resource/load";
        public const string RenderProcessCrashedUrl = "http://processcrashed";
        public const string TestUnicodeResourceUrl = "http://test/resource/loadUnicode";
        public const string PopupParentUrl = "http://www.w3schools.com/jsref/tryit.asp?filename=tryjsref_win_close";

        public Salesforce(ReloadMe reloader, CloseMe closing)
        {
            /*
            this.reloader = reloader;
            this.closing = closing;
            this.SalesforceInstance = "http://localhost:59507/sample.html";
            InitializeComponent();
            Initialize();*/
            //this.ShowInTaskbar = false;
        }

        public Salesforce(ReloadMe reloader, CloseMe closing, bool InitializeCEF)
        {
            /*
            this.reloader = reloader;
            this.closing = closing;
            if (InitializeCEF)
                this.InitializeCEF();
            InitializeComponent();
            Initialize();*/
            //this.ShowInTaskbar = false;
        }

        public Salesforce(ReloadMe ReloadMe, CloseMe CloseMe, bool InitializeCEF, string SalesforceInstance)
        {
            // TODO: Complete member initialization
            /*this.reloader = ReloadMe;
            this.closing = CloseMe;
            this.SalesforceInstance = SalesforceInstance;

            if (InitializeCEF)
                this.InitializeCEF();

            InitializeComponent();
            Initialize();*/
            RunV2(SalesforceInstance);
        }

        Process salesforceProcess; 

        private void RunV2(string SalesforceInstance)
        {
            
        }

        void salesforceProcess_Disposed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void salesforceProcess_Exited(object sender, EventArgs e)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action<object, EventArgs>(salesforceProcess_Exited), new object[] { sender, e });
            }
            else
            {
                this.Close();
            }            
        }

        

        private void Initialize()
        {
            browserScripting = new BrowserScripting();
            browserScripting.SalesforceRef = this;            
            this.screenHeight = Screen.GetWorkingArea(this).Height;
            this.screenWidth = Screen.GetWorkingArea(this).Width;
            //this.ncpBrowser.ObjectForScripting = bs;
            //this.ncpBrowser.ScriptErrorsSuppressed = true;
            //this.ncpBrowser.AllowWebBrowserDrop = false;
            this.webRef = this.ncpBrowser;
            BaseInit();
            this.MenuDock = this.openWindows;

            /*jabber.JID j = new jabber.JID()

            this.JabberClient = new jabber.client.JabberClient();
            this.JabberClient.User = "ccraig";
            this.JabberClient.Password = "Zxpq9601";
            this.JabberClient.Server = "192.168.150.15";
            this.JabberClient.Port = 5222;
            
            this.JabberClient.Connect();
            this.JabberClient.Login();
            this.JabberClient.OnMessage += JabberClient_OnMessage;
            this.JabberClient.OnPresence += JabberClient_OnPresence;
            */

            OpenChrome();
        }

        void JabberClient_OnPresence(object sender, jabber.protocol.client.Presence pres)
        {
            
        }

        void JabberClient_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            
        }

        private void OpenChrome()
        {
            /*
            this.Chromium = new CefSharp.WinForms.ChromiumWebBrowser(this.SalesforceInstance);
            this.panel1.Controls.Add(this.Chromium);
            this.Chromium.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Chromium.Name = "Chromium";
            this.Chromium.TabIndex = 0;

            this.Chromium.BrowserSettings.UniversalAccessFromFileUrls = CefSharp.CefState.Enabled;
            this.Chromium.BrowserSettings.FileAccessFromFileUrls = CefSharp.CefState.Enabled;

            // Register customer Javascript functions

            this.Chromium.RegisterAsyncJsObject("bound", browserScripting, false);
            this.Chromium.RequestHandler = new ChromiumRestriction();
            this.Chromium.ResourceHandlerFactory = new ChromiumResourceHandlerFactory();
            this.Chromium.DownloadHandler = new DownloadHandler(this);

            ////this.Chromium.LoadHandler = new ChromiumLoadHandler();
            // This is required to make the browser dock within its bounds
            this.Chromium.LoadError += Chromium_LoadError;
            //this.Chromium.ConsoleMessage += Chromium_ConsoleMessage;
            //this.Chromium.FrameLoadEnd += Chromium_FrameLoadEnd;
            this.Chromium.BringToFront();
            */

            this.Chromium = new CefSharp.WinForms.ChromiumWebBrowser(this.SalesforceInstance);
            this.panel1.Controls.Add(this.Chromium);
            this.Chromium.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Chromium.Name = "Chromium";
            this.Chromium.TabIndex = 0;

            this.Chromium.MenuHandler = new NCP_Browser.ChromiumHandlers.MenuHandler();
            //this.Chromium.RequestHandler = new NCP_Browser.ChromiumHandlers.WinFormsRequestHandler(openNewTab);
            this.Chromium.JsDialogHandler = new NCP_Browser.ChromiumHandlers.JsDialogHandler();
            this.Chromium.GeolocationHandler = new NCP_Browser.ChromiumHandlers.GeolocationHandler();
            this.Chromium.DownloadHandler = new NCP_Browser.ChromiumHandlers.DownloadHandler();
            this.Chromium.KeyboardHandler = new NCP_Browser.ChromiumHandlers.KeyboardHandler();
            this.Chromium.LifeSpanHandler = new NCP_Browser.ChromiumHandlers.LifeSpanHandler();

            this.Chromium.BringToFront();
        }

        void Chromium_ConsoleMessage(object sender, CefSharp.ConsoleMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        void Chromium_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            // Cisco Plugin
            
        }

        public ToolStripItemCollection OpenWindowCollection()
        {
            return this.openWindows.DropDownItems;
        }

        private class MessageFilter: IMessageFilter
        {
            public Form form;
            private const int WM_MOUSEMOVE = 0x0200;
            private const int WM_MOUSELEAVE = 0x02A3;
            private bool formFocused = true;

            public MessageFilter(Form form)
            {
                this.form = form;
            }

            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                if(m.Msg == WM_MOUSELEAVE || m.Msg == WM_MOUSEMOVE)
                {
                    if(form.ClientRectangle.Contains(MousePosition) && !formFocused)
                    {
                        formFocused = true;
                        form.Activate();
                        return true;
                    }
                    else
                    {
                        formFocused = false;
                        return false;
                    }
                }
                else
                {
                    formFocused = false;
                    return false;
                }
            }
        }
        


        private void nCPTimeCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowserScripting.NewBrowser("Time Card", "https://ncpfinance.attendanceondemand.com/ess/DEFAULT", this, openWindows);
        }

        private void exampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Internet nint = new Internet(this.openWindows);
            nint.internetBrowser.Url = new Uri("http://www.google.com");
            nint.Home = "http://www.google.com";

            ToolStripEnhanced tsi = new ToolStripEnhanced(nint.Text, generic_Menu, nint, openWindows);
            nint.SetUpText(tsi.TextSetter, "Google");
            openWindows.DropDownItems.Add(tsi);
            nint.Show();
        }

        private void example2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Internet nint = new Internet(this.openWindows);
            nint.internetBrowser.Url = new Uri("http://www.yahoo.com");
            nint.Home = "http://www.yahoo.com";

            ToolStripEnhanced tsi = new ToolStripEnhanced(nint.Text, generic_Menu, nint, openWindows);
            nint.SetUpText(tsi.TextSetter, "Yahoo");
            openWindows.DropDownItems.Add(tsi);
            nint.Show();
        }

        private void example3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Internet nint = new Internet(this.openWindows);
            nint.internetBrowser.Url = new Uri("http://www.msn.com");
            nint.Home = "http://www.msn.com";
            
            ToolStripEnhanced tsi = new ToolStripEnhanced(nint.Text, generic_Menu, nint, openWindows);
            nint.SetUpText(tsi.TextSetter, "MSN");
            openWindows.DropDownItems.Add(tsi);
            nint.Show();
        }

        private void Salesforce_Load(object sender, EventArgs e)
        {
            //DockMe();
        }

        private void DockMe()
        {
            this.Docked = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            var currentScreen = Screen.FromControl(this);
           
            this.Left = currentScreen.Bounds.Left;
            this.Top = 0;
            this.Width = Screen.GetWorkingArea(this).Width;
            this.Height = Screen.GetWorkingArea(this).Height;
            browserScripting.SalesforceScreen = Screen.FromControl(this);
        }

        private void UnDockMe()
        {
            this.Docked = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            var currentScreen = Screen.FromControl(this);
            this.Left = (int)Math.Round(currentScreen.Bounds.Left + currentScreen.Bounds.Width * .125);
            this.Top = 0;
            this.Width = (int)Math.Round(Screen.GetWorkingArea(this).Width * .75, 0, MidpointRounding.AwayFromZero);
            this.Height = (int)Math.Round(Screen.GetWorkingArea(this).Height * .75, 0, MidpointRounding.AwayFromZero);
            browserScripting.SalesforceScreen = Screen.FromControl(this);
        }

        private void Salesforce_Activated(object sender, EventArgs e)
        {
            this.TopMost = false;
        }

        private void ncpBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (((System.Windows.Forms.WebBrowser)sender).Url != null && ((System.Windows.Forms.WebBrowser)sender).Url.LocalPath.EndsWith(".pdf"))
            {
                OpenPDF(sender);
                ((System.Windows.Forms.WebBrowser)sender).GoBack();
            }
        }

        private void suspenseLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowserScripting.NewBrowser("Suspense Log", "http://apps/", this, openWindows);
        }

        private void customerLookupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowserScripting.NewBrowser("LMS Downtime - Customer Lookup", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fCustomerLookup&rs:Command=Render", this, openWindows);
        }

        private void loanDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowserScripting.NewBrowser("LMS Downtime - Loan Detail", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fLoanDetail&rs:Command=Render", this, openWindows);
        }

        private void loanLookupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowserScripting.NewBrowser("LMS Downtime - Loan Lookup", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fLoanLookup&rs:Command=Render", this, openWindows);
        }

        private void loanNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowserScripting.NewBrowser("LMS Downtime - Loan Notes", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fLoanNotes&rs:Command=Render", this, openWindows);
        }

        public delegate void ReloadMe();
        public delegate void CloseMe(FormClosingEventArgs e);
        public delegate void FocusForm(Form f);

        private void reloadSalesforceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseOpenWindows();
            this.Chromium.GetBrowser().CloseBrowser(true);
            this.Chromium.LoadError += Chromium_LoadError;
            this.Chromium.Dispose();
            this.panel1.Controls.Clear();
            this.OpenChrome();
        }

        void Chromium_LoadError(object sender, CefSharp.LoadErrorEventArgs e)
        {
            //MessageBox.Show(e.ErrorText);
        }

        private void CloseOpenWindows()
        {
            var owin = openWindows.DropDownItems.GetEnumerator();
            try
            {
                while (owin.MoveNext())
                {
                    ((ToolStripEnhanced)owin.Current).form.FormClosed -= ((ToolStripEnhanced)owin.Current).form_FormClosed;
                    CloseOpenWindow((ToolStripEnhanced)owin.Current);
                }
            }
            catch(Exception e)
            {}
            openWindows.DropDownItems.Clear();
        }

        private void CloseOpenWindow(ToolStripEnhanced toolStripEnhanced)
        {
            if(toolStripEnhanced.form.InvokeRequired)
            {
                toolStripEnhanced.form.Invoke(new Action(CloseOpenWindows));
            }
            else
            {
                toolStripEnhanced.form.Close();
            }
        }

        private delegate void CloseOpenWindowDeleage(Form form, ToolStripEnhanced.FormClosingDelegate action);


        private void CheckForUIUpdates_Tick(object sender, EventArgs e)
        {
            if(this.screenWidth != Screen.GetWorkingArea(this).Width || this.screenHeight != Screen.GetWorkingArea(this).Height)
            {
                this.WindowState = FormWindowState.Normal;
                this.screenWidth = Screen.GetWorkingArea(this).Width;
                this.screenHeight = Screen.GetWorkingArea(this).Height;
                this.Location = new Point(0, 0);
                this.Width = Screen.GetWorkingArea(this).Width;
                this.Height = Screen.GetWorkingArea(this).Height;
            }
        }

        private void ncpBrowser_NewWindow(object sender, CancelEventArgs e)
        {

        }


        private void Salesforce_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(closing != null)
            {
                closing.Invoke(e);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void pDFTESTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PDF pdf = new PDF(@"C:\TAXOQ1.pdf");
            ToolStripEnhanced tsi = new ToolStripEnhanced(pdf.Text, generic_Menu, pdf, openWindows);
            openWindows.DropDownItems.Add(tsi);
            pdf.Show();
        }

        private void closeSalesforceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseOpenWindows();
            this.Close();
        }

        private void openWindows_MouseEnter(object sender, EventArgs e)
        {
        }

        private void Salesforce_MouseEnter(object sender, EventArgs e)
        {
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
        }

        private void refreshSalesforceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Chromium.GetBrowser().Reload(true);
        }

        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String Title = "NCP Wiki";
            String URL = "http://wiki/mediawiki/index.php/Main_Page";

            BrowserScripting.NewBrowser(Title, URL, this, openWindows, true);
        }

        public void InitializeCEF()
        {
            CefSharp.CefSettings cfSettings = new CefSharp.CefSettings();

            cfSettings.RemoteDebuggingPort = 8088;
            if (!Directory.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NCPBrowser")))
                Directory.CreateDirectory(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NCPBrowser"));
            cfSettings.CachePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NCPBrowser");
            cfSettings.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
            cfSettings.MultiThreadedMessageLoop = true;
            cfSettings.IgnoreCertificateErrors = true;
            cfSettings.FocusedNodeChangedEnabled = true;
            cfSettings.RegisterScheme(new CefSharp.CefCustomScheme()
            {
                SchemeName = ChromeExtensionSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new ChromeExtensionSchemeHandlerFactory()
            });
            cfSettings.RegisterExtension(new CefSharp.CefExtension("cwic_background", NCP_Browser.Properties.Resources.cwic_background));
            cfSettings.FocusedNodeChangedEnabled = true;            

            cfSettings.CefCommandLineArgs.Add("--mute-audio", "--mute-audio");
            cfSettings.CefCommandLineArgs.Add("--external-devtools", "--external-devtools");
            
            
            //cfSettings.RegisterExtension(new CefSharp.CefExtension("cwic_cwic_plugin", NCP_Browser.Properties.Resources.cwic_plugin));
            //cfSettings.RegisterExtension(new CefSharp.CefExtension("test", "alert('dingdong');"));            
            //cfSettings.RegisterExtension(new CefSharp.CefExtension("cwic_contentscript", NCP_Browser.Properties.Resources.cwic_contentscript));
            CefSharp.Cef.Initialize(cfSettings, true, false);

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Docker_Click(object sender, EventArgs e)
        {
            if(this.Docked)
            {
                UnDockMe();
                Docker.Text = "Dock";
            }
            else
            {
                DockMe();
                Docker.Text = "Un Dock";
            }
        }

        public bool Docked { get; set; }

        private void teestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowserScripting.NewBrowser("QFund LMS", "http://ncponsiteqa.qfund.net/ncp/", this, openWindows, true);
        }


        /*
        * Timer For Ticker
        * This is used to ensure that the tab manager is running
        * Log the Last time the tab manager checked in
        */

        private void timerTicker_Tick(object sender, EventArgs e)
        {

        }

        private void showDevToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Chromium.GetBrowser().GetHost().ShowDevTools();
        }

        private void Salesforce_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
    }

    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class BrowserScripting
    {
        public Screen SalesforceScreen { get; set; }
        public Salesforce SalesforceRef { get; set; }

        public string NCPBrowserScripting ()
        {
            return "NCP Scripting Engine injected";
        }

        public void PlayDing()
        {
            Stream dingStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NCP_Browser.Ding.wav");
            SoundPlayer dinger = new SoundPlayer(dingStream);
            dinger.Play();
        }

        public static void NewBrowser(String Title, String URL, Salesforce FormControl, ToolStripMenuItem openWindows, bool showWebNav = false)
        {
            BaseBaseForm BrowserWindow = new BaseBaseForm();
            BrowserWindow.SuspendLayout();
            ToolStripEnhanced browserStrip = new ToolStripEnhanced(Title, FormControl.browserScripting.generic_Menu, BrowserWindow, openWindows);
            CefSharp.WinForms.ChromiumWebBrowser Browser = new CefSharp.WinForms.ChromiumWebBrowser(URL);
            Browser.Dock = DockStyle.Fill;
            
            BrowserWindow.Title = Title;
            BrowserWindow.Browser = Browser;
            BrowserWindow.URL = URL;

            Browser.RequestHandler = new QfundChromiumRestriction();

            // Browser Navigation
            if(showWebNav)
            { 
                BrowserWindow.webNav = new ToolStrip();
                BrowserWindow.webNav.SuspendLayout();
                BrowserWindow.backButton = new ToolStripButton(global::NCP_Browser.NCP_Browser_Resources.back);
                BrowserWindow.forwardButton = new ToolStripButton(global::NCP_Browser.NCP_Browser_Resources.forward);
                BrowserWindow.haltButton = new ToolStripButton(global::NCP_Browser.NCP_Browser_Resources.halt);
                BrowserWindow.homeButton = new ToolStripButton(global::NCP_Browser.NCP_Browser_Resources.home);
                BrowserWindow.reloadButton = new ToolStripButton(global::NCP_Browser.NCP_Browser_Resources.reload);
                BrowserWindow.webNav.AutoSize = true;
                BrowserWindow.webNav.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
                BrowserWindow.webNav.Height = 45;
                BrowserWindow.webNav.ImageScalingSize = new System.Drawing.Size(45, 45);
                BrowserWindow.webNav.Dock = DockStyle.Top;
                BrowserWindow.webNav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                BrowserWindow.homeButton,
                BrowserWindow.backButton,
                BrowserWindow.forwardButton,
                BrowserWindow.haltButton,
                BrowserWindow.reloadButton});

                BrowserWindow.backButton.Click += browserStrip.Browser_BackButton_Click;
                BrowserWindow.forwardButton.Click += browserStrip.Browser_ForwardButton_Click;
                Browser.LoadingStateChanged += browserStrip.Browser_LoadingStateChanged;
                BrowserWindow.homeButton.Click += browserStrip.Browser_HomeButton_Click;
                BrowserWindow.reloadButton.Click += browserStrip.Browser_ReloadButton_Click;
                BrowserWindow.haltButton.Click += browserStrip.Browser_HaltButton_Click;
            }

            
            Panel p = new Panel();
            p.SuspendLayout();
            p.Dock = DockStyle.Fill;
            p.BringToFront();
            BrowserWindow.Controls.Add(p);
            if(showWebNav)
                BrowserWindow.Controls.Add(BrowserWindow.webNav);
            
            Browser.TitleChanged += browserStrip.ChromiumBrowser_TitleChanged;

            if (showWebNav)
                Browser.IsBrowserInitializedChanged += browserStrip.Browser_IsBrowserInitializedChanged;

            Screen browserScreen = Screen.FromControl(FormControl);
            Screen notBrowserScreen = browserScreen;

            foreach(Screen s in Screen.AllScreens.ToList())
            {
                if(s.DeviceName != browserScreen.DeviceName)
                {
                    notBrowserScreen = s;
                }
            }

            BrowserWindow.StartPosition = FormStartPosition.Manual;
            BrowserWindow.Location = notBrowserScreen.WorkingArea.Location;
            BrowserWindow.Width = (int)(notBrowserScreen.WorkingArea.Width * .8);
            BrowserWindow.Height = (int)(notBrowserScreen.WorkingArea.Height * .8);
            BrowserWindow.Top = (int)(notBrowserScreen.WorkingArea.Height * .1);
            BrowserWindow.Left = (int)(notBrowserScreen.Bounds.Left + (notBrowserScreen.WorkingArea.Width * .1));
            ToolStripEnhanced.AddShowClose(browserStrip);
            openWindows.DropDownItems.Add(browserStrip);
            if(showWebNav)
                BrowserWindow.webNav.ResumeLayout(false);
            p.ResumeLayout(false);
            BrowserWindow.ResumeLayout(false);
            BrowserWindow.Show();
            p.Controls.Add(Browser);
            Browser.BringToFront();
            //WebNavUpdate(BrowserWindow);
        }

        internal static void WebNavUpdate(BaseBaseForm sender)
        {
            if(sender.InvokeRequired)
            {
                sender.Invoke(new WebNaveUpdateDelegate(WebNavUpdate), new object[] {sender});
            }
            else
            {
                if (((CefSharp.WinForms.ChromiumWebBrowser)sender.Browser).GetBrowser().CanGoBack)
                {
                    sender.backButton.Visible = true;
                }
                else
                {
                    sender.backButton.Visible = false;
                }
                if (((CefSharp.WinForms.ChromiumWebBrowser)sender.Browser).GetBrowser().CanGoForward)
                {
                    sender.forwardButton.Visible = true;
                }
                else
                {
                    sender.forwardButton.Visible = false;
                }
                if (((CefSharp.WinForms.ChromiumWebBrowser)sender.Browser).GetBrowser().IsLoading)
                {
                    sender.haltButton.Visible = true;
                }
                else
                {
                    sender.haltButton.Visible = false;
                }
            }
        }

        public void QFundOpen(string url, string name, int MaxWindows)
        {
            QFund qf = new QFund();
            qf.WindowValue = 1;
            var en = this.SalesforceRef.openWindows.DropDownItems.GetEnumerator();
            int winvalue = 0;
            while(en.MoveNext())
            {
                winvalue += ((BaseBaseForm)((ToolStripEnhanced)en.Current).form).WindowValue;
            }
            if(winvalue + 1 > MaxWindows)
            {
                MessageBox.Show(String.Format("You have exceeded the maximum number of {0} QFund windows.\nPlease Close some.",MaxWindows));
                return;
            }
            qf.Text = String.Format("QFund - {0}",name);
            var screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                if(screens[i].DeviceName != SalesforceScreen.DeviceName)
                {
                    qf.StartPosition = FormStartPosition.Manual;
                    qf.Location = screens[i].WorkingArea.Location;                    
                    qf.WindowState = FormWindowState.Maximized;
                }
            }
            
            ToolStripEnhanced tsi = new ToolStripEnhanced(qf.Text, generic_Menu, qf, this.SalesforceRef.openWindows, this);
            tsi.URI = url;

            QfundThread(tsi);                       
        }

        private void QfundThread(ToolStripEnhanced tsi)
        {
            if(this.SalesforceRef.InvokeRequired)
            {
                this.SalesforceRef.Invoke(new Action<ToolStripEnhanced>(QfundThread), tsi);
            }
            else
            { 
                this.SalesforceRef.openWindows.DropDownItems.Add(tsi);
                ToolStripEnhanced.AddShowClose(tsi);
                tsi.OpenForm();
            }
        }

        internal void generic_Menu(object sender, EventArgs e)
        {
            if (((ToolStripEnhanced)sender).form.InvokeRequired)
            {
                ((ToolStripEnhanced)sender).form.Invoke(new ActivateFormDelegate(generic_Menu), new object[]{sender, e});
            }
            else
            { 
                ((ToolStripEnhanced)sender).form.Activate();
            }
        }

        internal static void ActivateForm(object sender, EventArgs e)
        {
            if (((ToolStripEnhanced)sender).form.InvokeRequired)
            {
                ((ToolStripEnhanced)sender).form.Invoke(new ActivateFormDelegate(ActivateForm), new object[] { sender, e });
            }
            else
            {
                ((ToolStripEnhanced)sender).form.Activate();
            }
        }

        public delegate void ActivateFormDelegate(object sender, EventArgs e);
        public delegate void WebNaveUpdateDelegate(BaseBaseForm sender);

        public void FocusMe()
        {
            SalesforceRef.TopMost = true;           
        }

        internal static void CloseForm(object sender, EventArgs e)
        {
            ((BaseBaseForm)((ToolStripEnhanced)sender).form).SafeClose();
        }
    }
}
