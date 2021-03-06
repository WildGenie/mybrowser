﻿// Copyright © 2016 Cory Craig

using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Example;
using System.Threading.Tasks;
using System.Collections.Generic;
using CefSharp.Example.Proxy;
using System.Diagnostics;
using System.Linq;

namespace NCP_Browser
{
    public partial class Salesforce : Form
    {
        #region Overrides
        // Attempt to reduce flicker
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        #endregion

        // Static References for Extensions and background browsers
        public static Dictionary<string, NativeMessaging.Extension> NativeMessagingExtensions { get; set; }
        public static Dictionary<string, IWebBrowser> BackgroundBrowsers { get; set; }
        public static Dictionary<long, NCP_Browser.chrome.runtime.FrameLoad> FrameLoads { get; set;}
        public static object FrameLoadLock { get; set; }
        public static List<NCP_Browser.Internals.CallData> CallRecordings { get; set; }
        public static List<NCP_Browser.Internals.CallRecordingSelectCaseContainer> CallRecordingSelectCaseContainers { get; set; }

        // Browser tab holders, this is technically not set up for tabbing but the dictionary of tabss
        //  is useful for storing all the background browsers as well as the main salesforce instance
        private const string DefaultUrlForAddedTabs = "https://test.salesforce.com";
        public Dictionary<string, BrowserTabUserControl> browserTabs {get; set;}
        
        private static readonly bool DebuggingSubProcess = Debugger.IsAttached;

        // Async browser scripting
        public NCP_Browser.Internals.AsyncBrowserScripting browserScripting { get; set; }

        // Default to a small increment:
        private const double ZoomIncrement = 0.10;
        private string SalesforceInstance;

        // Event Delegates
        public delegate void ReloadMe();
        public delegate void CloseMe(FormClosingEventArgs e);
        public delegate void FocusForm(Form f);

        private ReloadMe reloader;
        private CloseMe closing;
        private JabberWebApi.Program JabberWebApi;
        public static List<IJavascriptCallback> PresenseNotifications;
        private static DateTime Finesse_LastStatus;
        private static DateTime Finesse_InitTime;
        private static bool Finesse_ErrorShown;

        /*public Salesforce()
        {
            Initialize();
        }*/

        /*
        public Salesforce(ReloadMe ReloadMe, CloseMe CloseMe, bool InitializeCEF, string SalesforceInstance)
        {
            // TODO: Complete member initialization
            this.reloader = ReloadMe;
            this.closing = CloseMe;
            this.SalesforceInstance = SalesforceInstance;

            if (InitializeCEF)
                this.InitializeCEF();

            Initialize();
        }*/


        private void Initialize()
        {
            InitializeComponent();

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            Text = "NCP Call Center Console";
            //WindowState = FormWindowState.Maximized;

            // Set up Asysnc Browser Scripting
            browserScripting = new Internals.AsyncBrowserScripting();
            browserScripting.SalesforceRef = this;

            // Set up dictionary of chrome "tabs"
            browserTabs = new Dictionary<string, BrowserTabUserControl>();

            // Set up static variables
            Salesforce.NativeMessagingExtensions = new Dictionary<string, NativeMessaging.Extension>();
            Salesforce.BackgroundBrowsers = new Dictionary<string, IWebBrowser>();
            Salesforce.FrameLoadLock = new object();
            Salesforce.FrameLoads = new Dictionary<long, chrome.runtime.FrameLoad>();
            Salesforce.PresenseNotifications = new List<IJavascriptCallback>();
            Salesforce.CallRecordings = new List<Internals.CallData>();
            Finesse_Status = String.Empty;
            Finesse_Show = String.Empty;

            this.HandleCreated += Salesforce_HandleCreated;
            
            //AddTab(CefExample.DefaultUrl, Name : "Cisco Web Communicator");

            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);
        }

        void Salesforce_HandleCreated(object sender, EventArgs e)
        {
            // Add salesforce tab
            AddTab(this.SalesforceInstance);
        }

        public Salesforce(bool InitializeCEF, string SalesforceInstance)
        {
            // TODO: Complete member initialization
            this.SalesforceInstance = SalesforceInstance;

            if (InitializeCEF)
                this.InitializeCEF();

            Initialize();

            // Jabber Web Api Initialization
            InitializeJabberWebApi();
            // Call Recorder Initialization
            try
            {
                CallRecorderImplementation = new CallRecorder.Implementation();
                CallRecorderImplementation.Open();
                //CallRecorderImplementation.Connect();            
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            StartMessageLoop();
            
        }

        private void StartMessageLoop()
        {
            Task.Run(() =>
            {
                int MaxCallNumber = 0;
                while (true)
                {

                    lock (Salesforce.FrameLoadLock)
                    {
                        // THIS BLOCK DISABLED AS THIS FEATURE IS NOT NEEDED
                        // CORY CRAIG - 2016-11-21
                        // RE-ENABLED AS SALESFORCE HAS A CONFIG FLAG (CALL RECORDING DISPLAY) TO HANDLE
                        try
                        {
                            
                            if (Salesforce.CallRecordingSelectCaseContainers != null && Salesforce.CallRecordingSelectCaseContainers.Count > 0)
                            {
                                var first = Salesforce.CallRecordingSelectCaseContainers.First();                                
                                this.Invoke(new Action<Internals.CallRecordingSelectCaseContainer>(ShowCaseForm),  first );
                                Salesforce.CallRecordingSelectCaseContainers.Remove(first);
                            }
                        }
                        catch
                        {
                            
                        }
                        
                    }

                    if (Salesforce.Finesse_Lock != null)
                    {
                        lock (Salesforce.Finesse_Lock)
                        {
                            if (!Finesse_ErrorShown)
                            if((DateTime.Now - Salesforce.Finesse_InitTime).TotalSeconds > 10 && (DateTime.Now - Salesforce.Finesse_LastStatus).TotalSeconds > 30)
                            {
                                Salesforce.Finesse_ErrorShown = true;
                                /*Form f = new Form();
                                f.Size = new System.Drawing.Size(1, 1);
                                f.TopMost = true;
                                f.StartPosition = FormStartPosition.Manual;
                                System.Drawing.Rectangle rect = SystemInformation.VirtualScreen;
                                f.Location = new System.Drawing.Point(rect.Bottom+10, rect.Right+10);
                                f.Show();
                                f.Focus();
                                f.BringToFront();

                                MessageBox.Show(f,"Please press the NCP button in firefox with the fineese tab selected to connect Finesse");
                                f.Dispose();*/
                            }
                        }
                    }
                    
                    try
                    {
                        if(this.IsHandleCreated)
                        {
                            this.Invoke(new Action(UpdateUI));
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    System.Threading.Thread.Sleep(1000);  
                }
            });
        }

        private void ShowCaseForm(Internals.CallRecordingSelectCaseContainer first)
        {
            NCP_Browser.Forms.SelectCase sc = new Forms.SelectCase();
            sc.Number = first.SelectCaseCallData.IPC_CallData.Number;
            sc.Populate(first.RelatedPhoneNumbers);
            sc.Text = String.Format("Phone Call with # {0}", first.SelectCaseCallData.PhoneNumber);
            ((Forms.SelectCase)sc).Show();
        }



        private void InitializeJabberWebApi()
        {
            Salesforce.Finesse_Show = "NONE";
            Salesforce.Finesse_Status = "NONE";
            Salesforce.Finesse_CallStatus = "Not_On_A_Call";
            Salesforce.Finesse_Stale = 0;
            Salesforce.Finesse_Lock = new object();
            Salesforce.Finesse_LastStatus = DateTime.MinValue;
            Salesforce.Finesse_InitTime = DateTime.Now;

            JabberWebApi = new JabberWebApi.Program(new JabberWebApi.Program.SendPresence(new Action<string, string>((string status, string show) =>
            {
                lock (Salesforce.Finesse_Lock)
                {
                    if (Salesforce.Finesse_Status != status || Salesforce.Finesse_Show != show)
                    {
                        Salesforce.Finesse_Status = status;
                        Salesforce.Finesse_Show = show;
                        Salesforce.Finesse_Stale = 0;
                    }
                    else if (Salesforce.Finesse_Stale >= 2 && Salesforce.Finesse_Stale < 3)
                    {
                        Salesforce.Finesse_Stale++;
                        PresenseNotifications.ForEach(x =>
                        {
                            x.ExecuteAsync(new object[] { status == "dnd" && Salesforce.Finesse_CallStatus == "On_A_Call" ? "busy" : status, show });
                        });
                    }
                    else
                    {
                        Salesforce.Finesse_Stale++;
                    }
                    Salesforce.Finesse_LastStatus = DateTime.Now;
                    Salesforce.Finesse_ErrorShown = false;
                }
                //MessageBox.Show(String.Format("{0}|{1}", status, show));
            })), new JabberWebApi.Program.SendCallStatus(new Action<string>((string status) =>
            {
                lock (Salesforce.Finesse_Lock)
                {
                    if (status != Salesforce.Finesse_CallStatus)
                    {
                        Salesforce.Finesse_CallStatus = status;
                        // Reset Staleness if need be to immediately forward status unless a new status comes in
                        if (Salesforce.Finesse_Stale >= 3)
                        {
                            Finesse_Stale = 2;
                        }
                    }
                }
            })));
            JabberWebApi.Start();
        }

        public void InitializeCEF()
        {
            CefSharp.CefSettings cfSettings = new CefSharp.CefSettings();
            //cfSettings.CefCommandLineArgs.Add(new KeyValuePair<string, string>("enable-npapi", "enable-npapi"));
            cfSettings.CachePath = "cache";
            //cfSettings.UserDataPath = @"C:\Users\ccraig\AppData\Local\Google\Chrome\User Data\Default";
            cfSettings.RemoteDebuggingPort = 8088;
            cfSettings.MultiThreadedMessageLoop = true;

            //cfSettings.BrowserSubprocessPath = "Ncp.Browser.exe";

            var proxy = ProxyConfig.GetProxyInformation();
            switch (proxy.AccessType)
            {
                case InternetOpenType.Direct:
                    {
                        //Don't use a proxy server, always make direct connections.
                        cfSettings.CefCommandLineArgs.Add("no-proxy-server", "1");
                        break;
                    }
                case InternetOpenType.Proxy:
                    {
                        cfSettings.CefCommandLineArgs.Add("proxy-server", proxy.ProxyAddress);
                        break;
                    }
                case InternetOpenType.PreConfig:
                    {
                        cfSettings.CefCommandLineArgs.Add("proxy-auto-detect", "1");
                        break;
                    }
            }

            if (DebuggingSubProcess)
            {
                var architecture = Environment.Is64BitProcess ? "x64" : "x86";
                cfSettings.BrowserSubprocessPath = String.Format("..\\..\\..\\..\\packages\\CefSharp.Common.{0}\\CefSharp\\", Cef.CefSharpVersion.EndsWith(".0") && FourVersions(Cef.CefSharpVersion) ? Cef.CefSharpVersion.Substring(0, Cef.CefSharpVersion.Length-2) : Cef.CefSharpVersion) + architecture + "\\CefSharp.BrowserSubprocess.exe";
            }

            //cfSettings.CefCommandLineArgs.Add(new KeyValuePair<string, string>("--load-plugin", @"C:\Users\ccraig\Desktop\cisco chrome plugin\3.1.0.363_0.crx"));

            //if (!Directory.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NCPBrowser")))
            //    Directory.CreateDirectory(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NCPBrowser"));
            //cfSettings.CachePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NCPBrowser");
            cfSettings.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

            /*Cef.OnContextInitialized = delegate
            {
                var cookieManager = Cef.GetGlobalCookieManager();
                cookieManager.SetStoragePath("cookies", true);
                cookieManager.SetSupportedSchemes("custom");

                //Dispose of context when finished - preferable not to keep a reference if possible.
                using (var context = Cef.GetGlobalRequestContext())
                {
                    string errorMessage;
                    //You can set most preferences using a `.` notation rather than having to create a complex set of dictionaries.
                    //The default is true, you can change to false to disable
                    context.SetPreference("webkit.webprefs.plugins_enabled", true, out errorMessage);
                }
            };*/

            cfSettings.IgnoreCertificateErrors = true;
            cfSettings.FocusedNodeChangedEnabled = true;

            cfSettings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = CefSharpSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory()
                //SchemeHandlerFactory = new InMemorySchemeAndResourceHandlerFactory()
            });

            cfSettings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = CefSharpSchemeHandlerFactory.SchemeNameTest,
                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory()
            });


            cfSettings.RegisterScheme(new CefSharp.CefCustomScheme()
            {
                SchemeName = NCP_Browser.chrome.extensions.ChromeExtensionSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new NCP_Browser.chrome.extensions.ChromeExtensionSchemeHandlerFactory(),
                IsLocal = false,
                IsStandard = true,
            });

            cfSettings.RegisterScheme(new CefSharp.CefCustomScheme()
            {
                SchemeName = "https",
                SchemeHandlerFactory = new NCP_Browser.chrome.extensions.ChromeExtensionSchemeHandlerFactory(),
                IsLocal = false,
                IsStandard = true,
                DomainName = "ppbllmlcmhfnfflbkbinnhacecaankdh"
            });

            

            cfSettings.FocusedNodeChangedEnabled = true;

            cfSettings.CefCommandLineArgs.Add("--mute-audio", "--mute-audio");

            if (!Cef.Initialize(cfSettings))
            {
                throw new Exception("Unable to Initialize Cef");
            }
            else
            {
                var cookieManager = Cef.GetGlobalCookieManager();
                cookieManager.SetStoragePath("cookies", true);
                cookieManager.SetSupportedSchemes("custom");

                //Dispose of context when finished - preferable not to keep a reference if possible.
                using (var context = Cef.GetGlobalRequestContext())
                {
                    string errorMessage;
                    //You can set most preferences using a `.` notation rather than having to create a complex set of dictionaries.
                    //The default is true, you can change to false to disable
                    context.SetPreference("webkit.webprefs.plugins_enabled", true, out errorMessage);
                }
            }
            
            if (!CefSharp.Cef.AddCrossOriginWhitelistEntry("http://localhost:59507", "cefsharp-extension", "ppbllmlcmhfnfflbkbinnhacecaankdh", true))
            {

            }
        }

        private bool FourVersions(string p)
        {
            int dotCount = 0;
            foreach (char c in p)
            {
                if (c == '.')
                    dotCount++;
            }
            return dotCount == 3;
        }


        internal void AddTab(string url, int? insertIndex = null, string Name = null, object InitializationLock = null, NCP_Browser.NativeMessaging.Extension Extension = null, NCP_Browser.Internals.AsyncBrowserScripting asyncScripting = null)
        {
            var browser = new BrowserTabUserControl(AddTab, url, Name: Name, InitializationLock: InitializationLock, Extension: Extension, salesforce: this)
            {
                Dock = DockStyle.Fill,
            };

            //var tabPage = new TabPage(url)
            //{
            //    Dock = DockStyle.Fill
            //};

            //This call isn't required for the sample to work. 
            //It's sole purpose is to demonstrate that #553 has been resolved.
            browser.CreateControl();

            //tabPage.Controls.Add(browser);

            //if (insertIndex == null)
            //{
            //    browserTabControl.TabPages.Add(tabPage);
            //}
            //else
            //{
            //    browserTabControl.TabPages.Insert(insertIndex.Value, tabPage);
            //}

            //Make newly created tab active
            // browserTabControl.SelectedTab = tabPage;
            // browserTabControl

            //browserTabControl.ResumeLayout(true);
            this.browserTabs.Add(Name == null ? "Salesforce" : Name, browser);

            if(Name == null)
            {
                browserPanel.Controls.Add(browser);
            }
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void ExitApplication()
        {
            JabberWebApi.Stop();
            Close();
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void FindMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.ShowFind();
            }
        }

        private void CopySourceToClipBoardAsyncClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.CopySourceToClipBoardAsync();
            }
        }

        private BrowserTabUserControl GetCurrentTabControl()
        {
            /*if (browserTabControl.SelectedIndex == -1)
            {
                return null;
            }

            var tabPage = browserTabControl.Controls[browserTabControl.SelectedIndex];
            var control = (BrowserTabUserControl)tabPage.Controls[0];

            return control;*/
            return null;
        }

        private void NewTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddTab(DefaultUrlForAddedTabs);
        }

        private void CloseTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            /*if(browserTabControl.Controls.Count == 0)
            {
                return;
            }

            var currentIndex = browserTabControl.SelectedIndex;

            var tabPage = browserTabControl.Controls[currentIndex];

            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Dispose();
            }

            browserTabControl.Controls.Remove(tabPage);

            tabPage.Dispose();

            browserTabControl.SelectedIndex = currentIndex - 1;

            if (browserTabControl.Controls.Count == 0)
            {
                ExitApplication();
            }*/
        }

        private void UndoMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Undo();
            }
        }

        private void RedoMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Redo();
            }
        }

        private void CutMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Cut();
            }
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Copy();
            }
        }

        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Paste();
            }
        }

        private void DeleteMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Delete();
            }
        }

        private void SelectAllMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.SelectAll();
            }
        }

        private void PrintToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Print();
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.ShowDevTools();

                //Example below shows how to use a control to host DevTools
                //(in this case it's added as a new TabPage)
                //var tabPage = new TabPage("DevTools")
                //{
                //    Dock = DockStyle.Fill
                //};

                //var panel = new Panel
                //{
                //    Dock = DockStyle.Fill
                //};

                ////We need to call CreateControl as we need the Handle later
                //panel.CreateControl();

                //tabPage.Controls.Add(panel);

                //browserTabControl.TabPages.Add(tabPage);

                ////Make newly created tab active
                //browserTabControl.SelectedTab = tabPage;

                ////Grab the client rect
                //var rect = panel.ClientRectangle;
                //var webBrowser = control.Browser;
                //var browser = webBrowser.GetBrowser().GetHost();
                //var windowInfo = new WindowInfo();
                ////DevTools becomes a child of the panel, we use it's dimesions
                //windowInfo.SetAsChild(panel.Handle, rect.Left, rect.Top, rect.Right, rect.Bottom);
                ////Show DevTools in our panel 
                //browser.ShowDevTools(windowInfo);
            }
        }

        private void CloseDevToolsMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.CloseDevTools();
            }
        }

        private void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();

                task.ContinueWith(previous =>
                {
                    if (previous.IsCompleted)
                    {
                        var currentLevel = previous.Result;
                        control.Browser.SetZoomLevel(currentLevel + ZoomIncrement);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected failure of calling CEF->GetZoomLevelAsync", previous.Exception);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();
                task.ContinueWith(previous =>
                {
                    if (previous.IsCompleted)
                    {
                        var currentLevel = previous.Result;
                        control.Browser.SetZoomLevel(currentLevel - ZoomIncrement);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected failure of calling CEF->GetZoomLevelAsync", previous.Exception);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void CurrentZoomLevelToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();
                task.ContinueWith(previous =>
                {
                    if (previous.IsCompleted)
                    {
                        var currentLevel = previous.Result;
                        MessageBox.Show("Current ZoomLevel: " + currentLevel.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Unexpected failure of calling CEF->GetZoomLevelAsync: " + previous.Exception.ToString());
                    }
                }, TaskContinuationOptions.HideScheduler);
            }
        }

        private void DoesActiveElementAcceptTextInputToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var frame = control.Browser.GetFocusedFrame();
                
                //Execute extension method
                frame.ActiveElementAcceptsTextInput().ContinueWith(task =>
                {
                    string message;
                    var icon = MessageBoxIcon.Information;
                    if (task.Exception == null)
                    {
                        var isText = task.Result;
                        message = string.Format("The active element is{0}a text entry element.", isText ? " " : " not ");
                    }
                    else
                    {
                        message = string.Format("Script evaluation failed. {0}", task.Exception.Message);
                        icon = MessageBoxIcon.Error;
                    }

                    MessageBox.Show(message, "Does active element accept text input", MessageBoxButtons.OK, icon);
                });
            }
        }

        private void DoesElementWithIdExistToolStripMenuItemClick(object sender, EventArgs e)
        {
            // This is the main thread, it's safe to create and manipulate form
            // UI controls.
            var dialog = new InputBox
            {
                Instructions = "Enter an element ID to find.",
                Title = "Find an element with an ID"
            };

            dialog.OnEvaluate += (senderDlg, eDlg) =>
            {
                // This is also the main thread.
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    var frame = control.Browser.GetFocusedFrame();

                    //Execute extension method
                    frame.ElementWithIdExists(dialog.Value).ContinueWith(task =>
                    {
                        // Now we're not on the main thread, perhaps the
                        // Cef UI thread. It's not safe to work with
                        // form UI controls or to block this thread.
                        // Queue up a delegate to be executed on the
                        // main thread.
                        BeginInvoke(new Action(() =>
                        {
                            string message;
                            if (task.Exception == null)
                            {
                                message = task.Result.ToString();
                            }
                            else
                            {
                                message = string.Format("Script evaluation failed. {0}", task.Exception.Message);
                            }

                            dialog.Result = message;
                        }));
                    });
                }
            };

            dialog.Show(this);
        }

        private void GoToDemoPageToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Load("custom://cefsharp/ScriptedMethodsTest.html");
            }
        }

        private void InjectJavascriptCodeToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var frame = control.Browser.GetFocusedFrame();

                //Execute extension method
                frame.ListenForEvent("test-button", "click");
            }
        }

        private async void printToPdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".pdf",
                    Filter = "Pdf documents (.pdf)|*.pdf"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var success = await control.Browser.PrintToPdfAsync(dialog.FileName, new PdfPrintSettings
                    {
                        MarginType = CefPdfPrintMarginType.Custom,
                        MarginBottom = 10,
                        MarginTop = 0,
                        MarginLeft = 20,
                        MarginRight = 10
                    });

                    if (success)
                    {
                        MessageBox.Show("Pdf was saved to " + dialog.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Unable to save Pdf, check you have write permissions to " + dialog.FileName);
                    }

                }

            }
        }

        private void hardReloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.browserTabs["Salesforce"].Browser.GetBrowser().Reload(true);
        }

        private void closeSalesforceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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
            catch (Exception e)
            { }
            
        }

        private void CloseOpenWindow(ToolStripEnhanced toolStripEnhanced)
        {
            if (toolStripEnhanced.form.InvokeRequired)
            {
                toolStripEnhanced.form.Invoke(new Action(CloseOpenWindows));
            }
            else
            {
                toolStripEnhanced.form.Close();
            }
        }

        private void refreshSalesforceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.browserTabs["Salesforce"].Browser.GetBrowser().Reload(true);
        }

        #region New Browsers
        private void nCPTimeCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("Time Card", "https://ncpfinance.attendanceondemand.com/ess/DEFAULT", this, this.openWindows);
        }

        private void suspenseLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("Suspense Log", "http://apps/", this, openWindows);
        }

        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("NCP Wiki", "http://wiki/mediawiki/index.php/Main_Page", this, openWindows, true);
        }

        private void qFundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.browserScripting.QFundOpen("http://ncponsiteqa.qfund.net/ncp/", "QFund LMS", -1);
            //NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("QFund LMS", "http://ncponsiteqa.qfund.net/ncp/", this, openWindows, true);
        }

        private void customerLookupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("LMS Downtime - Customer Lookup", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fCustomerLookup&rs:Command=Render", this, openWindows);
        }

        private void loanDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("LMS Downtime - Loan Detail", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fLoanDetail&rs:Command=Render", this, openWindows);
        }

        private void loanLookupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("LMS Downtime - Loan Lookup", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fLoanLookup&rs:Command=Render", this, openWindows);
        }

        private void loanNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Internals.AsyncBrowserScripting.NewBrowser("LMS Downtime - Loan Notes", "http://schear-data/ReportServer_SCHEARDATA2012/?%2fLMS+Downtime%2fLoanNotes&rs:Command=Render", this, openWindows);
        }
        #endregion

        private void Salesforce_FormClosing(object sender, FormClosingEventArgs e)
        {
            CallRecorderImplementation.Disconnect();
            CloseOpenWindows();
            if(closing != null)
            {
                closing.Invoke(e);
            }
            else
            {
                
            }
            this.Visible = false;
            CefSharpSettings.WcfTimeout = TimeSpan.Zero;
            Cef.Shutdown();
            Environment.Exit(0);
            e.Cancel = true;
        }

        private void toolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).BackColor = System.Drawing.Color.DeepSkyBlue;
            ((ToolStripMenuItem)sender).ForeColor = System.Drawing.Color.Black;
        }

        private void toolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).BackColor = System.Drawing.Color.SteelBlue;
            ((ToolStripMenuItem)sender).ForeColor = System.Drawing.SystemColors.Info;
        }

        private void Salesforce_Load(object sender, EventArgs e)
        {
            for(int i = 0; i <  this.menuStrip.Items.Count; i++)
            {
                this.menuStrip.Items[i].MouseEnter += toolStripMenuItem_MouseEnter;
                this.menuStrip.Items[i].MouseLeave += toolStripMenuItem_MouseLeave;
            }

            // Disabled 2016-07-15 - no need to do XMPP
            //NCP_Browser.Jabber.Manager.LogIn(this.cicoJabberToolStripMenuItem);

            // Start up finesse watcher
        }

        private void Salesforce_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Visible = false;
            Cef.Shutdown();
            Environment.Exit(0);
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void mnu_Finess(object sender, EventArgs e)
        {
            ToolStripEnhanced tsi = new ToolStripEnhanced("Finess", "http://apps/", this.openWindows);
            openWindows.DropDownItems.Add(tsi);

        }

        private void cicoJabberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NCP_Browser.Jabber.Credentials c = new Jabber.Credentials();
            c.ShowDialog();
            NCP_Browser.Jabber.Manager.LogIn(cicoJabberToolStripMenuItem);
        }

        internal static object[] GetPresence()
        {
            return new object[] { Salesforce.Finesse_Status, Salesforce.Finesse_Show };
        }

        public static string Finesse_Status { get; set; }

        public static string Finesse_Show { get; set; }

        public static int Finesse_Stale { get; set; }

        public static string Finesse_CallStatus { get; set; }

        public static object Finesse_Lock { get; set; }

        internal static CallRecorder.Implementation CallRecorderImplementation { get; set; }

        /// <summary>
        /// Handler for Date Pagination
        /// DATE TIME HANDLER
        /// </summary>
        /// <param name="da"></param>
        private void dth(DateTime da)
        {
            paginator.ShowFrozenPixels();
            if(this.InvokeRequired)
            {
                this.Invoke(new Action<DateTime>(dth), new object[]{da});
            }
            else
            {
                CallRecorderImplementation.SetPaginationDate(da);
                // We need to clear the dropdown and put the paginator back 
                // so that calls from day x are not shown if day x-- or x++ has no calls
                lock (Salesforce.FrameLoadLock)
                {
                    if (CallRecordings == null)
                    {
                        CallRecordings = new List<Internals.CallData>();
                    }
                    CallRecordings.Clear();
                    callRecordingsToolStripMenuItem.DropDownItems.Clear();
                    callRecordingsToolStripMenuItem.DropDownItems.Add(paginator);
                }
                CallRecorderImplementation.UpdateCalls();
                
                this.callRecordingsToolStripMenuItem.ShowDropDown();
                paginator.HideFrozenPixels();
                
            }            
        }

        NCP_Browser.Forms.Controls.SendPaginationDate hand;
        NCP_Browser.Forms.Controls.ToolStripItemPaginator paginator;
        internal void UpdateUI()
        {
            if (hand == null)
            {
                hand = new Forms.Controls.SendPaginationDate(dth);
                CallRecorderImplementation.SetPaginationDate(DateTime.Now.Date);
                CallRecorderImplementation.UpdateCalls();
            }
            if (paginator == null)
            {
                paginator = new Forms.Controls.ToolStripItemPaginator(dth);
                callRecordingsToolStripMenuItem.DropDownItems.Add(paginator);
            }
            List<Internals.CallData> RemoveRec = new List<Internals.CallData>();
            if (CallRecordings != null && CallRecordings.Count > 0)
            {
                lock(Salesforce.FrameLoadLock)
                {
                    if(Salesforce.CallRecordingUpdated)
                    {
                        callRecordingsToolStripMenuItem.DropDownItems.Clear();
                        
                        callRecordingsToolStripMenuItem.DropDownItems.Add(paginator);
                        foreach (var c in CallRecordings)
                        {
                            if (c.Remove)
                            {
                                RemoveRec.Add(c);
                            }
                            else
                            {
                                ToolStripItem tsi = new ToolStripMenuItem();
                                tsi.Text = String.Format("Call To/From {0} ending at {1: MM/dd hh:mm tt} ", c.PhoneNumber, c.DateAdded);
                                tsi.Click += new EventHandler(delegate(Object sender, EventArgs e)
                                {
                                    NCP_Browser.Internals.AsyncBrowserScripting.CheckCallRecordingAgainstCasePhoneNumbers(c,true);
                                });
                                callRecordingsToolStripMenuItem.DropDownItems.Add(tsi);
                            }
                        }
                        foreach (var c in RemoveRec)
                        {
                            CallRecordings.Remove(c);
                        }
                        Salesforce.CallRecordingUpdated = false;
                    }                    
                }                
            }
            
        }

        public static bool CallRecordingUpdated { get; set; }

        public static bool CallEndTrigger { get; set; }


        public static List<Action> BackgroundActions { get; set; }
        private void backroundDevToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(BackgroundActions != null && BackgroundActions.Count() > 0)
            {
                BackgroundActions.ForEach(x => x.Invoke());
            }
        }

        private void callRecordingsToolStripMenuItem_VisibleChanged(object sender, EventArgs e)
        {
            
        }
    }
}
