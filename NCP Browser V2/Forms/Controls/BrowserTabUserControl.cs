// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.
// #1 - Add change to load background pages on initialization/first load. For whatever reason the dynamic loading of background pages when needed stopped working randomly

using CefSharp.Example;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NCP_Browser.Handlers;
using CefSharp.WinForms.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.WinForms;
using CefSharp;

namespace NCP_Browser
{
    public partial class BrowserTabUserControl : UserControl
    {
        public IWinFormsWebBrowser Browser { get; private set; }
        private IntPtr browserHandle;
        private ChromeWidgetMessageInterceptor messageInterceptor;
        public string Name {get; set;}
        private Queue<String> JavascriptQueue;
        private Queue<String> JavascriptToAllFramesQueue;
        public object JavascriptQueueLock;
        private bool frameLoaded = false;
        private Dictionary<long, NCP_Browser.chrome.runtime.FrameLock> frameLocks { get; set; }
        //private NCP_Browser.NativeMessaging.Extension Extension;

        public BrowserTabUserControl(Action<string, int?, string, object, NCP_Browser.NativeMessaging.Extension, NCP_Browser.Internals.AsyncBrowserScripting> openNewTab, string url, String Name = null, object InitializationLock = null, NCP_Browser.NativeMessaging.Extension Extension = null, Salesforce salesforce = null)
        {
            if(InitializationLock != null)
            {
                Monitor.Enter(InitializationLock);
            }
            JavascriptQueue = new Queue<String>();
            JavascriptToAllFramesQueue = new Queue<String>();
            JavascriptQueueLock = new object();
            this.frameLocks = new Dictionary<long, chrome.runtime.FrameLock>();
            InitializeComponent();
            this.Name = Name;
            this.salesforce = salesforce;
            //this.Extension = Extension;
            var browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill
            };

            browserPanel.Controls.Add(browser);

            Browser = browser;

            browser.MenuHandler = new MenuHandler();
            browser.RequestHandler = new WinFormsRequestHandler(openNewTab, salesforce);
            browser.JsDialogHandler = new JsDialogHandler();
            browser.GeolocationHandler = new GeolocationHandler();
            browser.DownloadHandler = new NCP_Browser.Handlers.DownloadHandler();
            browser.KeyboardHandler = new KeyboardHandler();
            browser.LifeSpanHandler = new LifeSpanHandler();
            if (Name == null)
            {
                browser.LoadingStateChanged += OnBrowserLoadingStateChanged;
                browser.ConsoleMessage += OnBrowserConsoleMessage;
                browser.TitleChanged += OnBrowserTitleChanged;
                browser.AddressChanged += OnBrowserAddressChanged;
                browser.StatusMessage += OnBrowserStatusMessage;
            }
            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            browser.LoadError += OnLoadError;

            if(Name == null)
            {
                // Foreground Browser
                browser.RegisterJsObject(NCP_Browser.chrome.runtime.BrowserScripting.Name, new NCP_Browser.chrome.runtime.BrowserScripting(browser, this, salesforce), new BindingOptions(){ CamelCaseJavascriptNames = false  });
                browser.RegisterAsyncJsObject("bound", salesforce.browserScripting, new BindingOptions() { CamelCaseJavascriptNames = false });
                browser.FrameLoadEnd += salesforce_FrameLoadEnd;
                browser.FrameLoadStart += browser_FrameLoadStart;
                
            }
            else
            {
                // Background Browser
                //Extension.SetBackgroundBrowser(browser);
                //Salesforce.BackgroundBrowsers.Add(Name, browser);
                browser.RegisterJsObject(NCP_Browser.chrome.runtime.NativeBrowserScripting.Name, new NCP_Browser.chrome.runtime.NativeBrowserScripting(browser, this, Name, salesforce), new BindingOptions() { CamelCaseJavascriptNames = false });
                browser.FrameLoadEnd += background_FrameLoadEnd;
            }


            //browser.RegisterJsObject("bound", new BoundObject());
            //browser.RegisterAsyncJsObject("boundAsync", new AsyncBoundObject());
            browser.RenderProcessMessageHandler = new RenderProcessMessageHandler();
            browser.DisplayHandler = new DisplayHandler();
            //browser.MouseDown += OnBrowserMouseClick;
            browser.HandleCreated += OnBrowserHandleCreated;
            //browser.ResourceHandlerFactory = new FlashResourceHandlerFactory();

            var eventObject = new ScriptedMethodsBoundObject();
            eventObject.EventArrived += OnJavascriptEventArrived;
            // Use the default of camelCaseJavascriptNames
            // .Net methods starting with a capitol will be translated to starting with a lower case letter when called from js
            browser.RegisterJsObject("boundEvent", eventObject, new BindingOptions() { CamelCaseJavascriptNames = true });
            browser.RegisterJsObject("chrome.runtime", new BoundObject(), new BindingOptions() { CamelCaseJavascriptNames = true });

            // Cef.AddWebPluginDirectory(@"C:\Users\ccraig\AppData\Local\Google\Chrome\User Data\Default\Extensions\ppbllmlcmhfnfflbkbinnhacecaankdh\C:\Users\ccraig\AppData\Local\Google\Chrome\User Data\Default\Extensions\ppbllmlcmhfnfflbkbinnhacecaankdh\3.1.0.363_0");
            //Cef.RefreshWebPlugins();
            CefExample.RegisterTestResources(browser);

            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);
            DisplayOutput(version);
            if(InitializationLock != null)
            {
                Monitor.Pulse(InitializationLock);
                Monitor.Exit(InitializationLock);                
            }
        }

        void browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            lock(this.JavascriptQueueLock)
            {
                if(!this.frameLocks.ContainsKey(e.Frame.Identifier))
                {
                    this.frameLocks.Add(e.Frame.Identifier, new chrome.runtime.FrameLock() { Lock = new object(), IsLocked = false });
                }

                lock(this.frameLocks[e.Frame.Identifier].Lock)
                {
                    this.frameLocks[e.Frame.Identifier].IsLocked = true;
                }
            }
        }

        void background_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            lock(this.JavascriptQueueLock)
            {
                e.Frame.ExecuteJavaScriptAsync("var CurrentFrameIdentifier = " + e.Frame.Identifier.ToString());
                e.Browser.MainFrame.EvaluateScriptAsync(NCP_Browser.Properties.Resources.chrome_native_runtime);
                //e.Browser.MainFrame.ExecuteJavaScriptAsync(NCP_Browser.Properties.Resources.background.Replace("chrome-extension", "cefsharp-extension"));
                e.Browser.MainFrame.ExecuteJavaScriptAsync("ncp_runtime.DoneInitializing();");
            }            
        }

        void salesforce_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //lock(Salesforce.FrameLoadLock)
            //{
            //    if(Salesforce.FrameLoads.Where(x => x.Key == e.Frame.Identifier).Count() == 0)
            //    {
            //        Salesforce.FrameLoads.Add(e.Frame.Identifier, new chrome.runtime.FrameLoad());
            //    }
                //e.Frame.ExecuteJavaScriptAsync("var CurrentFrameIdentifier = " + e.Frame.Identifier.ToString());
                //e.Frame.ExecuteJavaScriptAsync(NCP_Browser.Properties.Resources.chrome_runtime.Replace("{!FrameID}", e.Frame.Identifier.ToString()));
                //e.Frame.ExecuteJavaScriptAsync(String.Format("ncp_runtime.DoneInitializingFrame('{0}');", e.Frame.Identifier));
            //}
            if (this.frameLocks.Keys.Contains(e.Frame.Identifier))
            {
                lock (this.frameLocks[e.Frame.Identifier].Lock)
                {
                    this.frameLocks[e.Frame.Identifier].IsLocked = false;
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }

                if (messageInterceptor != null)
                {
                    messageInterceptor.ReleaseHandle();
                    messageInterceptor = null;
                }
            }
            base.Dispose(disposing);
        }

        private void OnBrowserHandleCreated(object sender, EventArgs e)
        {
            browserHandle = ((ChromiumWebBrowser)Browser).Handle;
        }

        private void OnBrowserMouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Mouse Clicked" + e.X + ";" + e.Y + ";" + e.Button);
        }

        private void OnLoadError(object sender, LoadErrorEventArgs args)
        {
            DisplayOutput("Load Error:" + args.ErrorCode + ";" + args.ErrorText);
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Parent.Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private static void OnJavascriptEventArrived(string eventName, object eventData)
        {
            switch (eventName)
            {
                case "click":
                {
                    var message = eventData.ToString();
                    var dataDictionary = eventData as Dictionary<string, object>;
                    if (dataDictionary != null)
                    {
                        var result = string.Join(", ", dataDictionary.Select(pair => pair.Key + "=" + pair.Value));
                        message = "event data: " + result;
                    }
                    MessageBox.Show(message, "Javascript event arrived", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
            }
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";
            goButton.Image = isLoading ?
                Properties.Resources.nav_plain_red :
                Properties.Resources.nav_plain_green;

            HandleToolStripLayout();
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            if (args.IsBrowserInitialized)
            {
                //Get the underlying browser host wrapper
                var browserHost = Browser.GetBrowser().GetHost();
                var requestContext = browserHost.RequestContext;
                string errorMessage;
                // Browser must be initialized before getting/setting preferences
                var success = requestContext.SetPreference("enable_do_not_track", true, out errorMessage);
                if(!success)
                {
                    this.InvokeOnUiThreadIfRequired(() => MessageBox.Show("Unable to set preference enable_do_not_track errorMessage: " + errorMessage));
                }

                //Example of disable spellchecking
                //success = requestContext.SetPreference("browser.enable_spellchecking", false, out errorMessage);

                var preferences = requestContext.GetAllPreferences(true);
                var doNotTrack = (bool)preferences["enable_do_not_track"];


                Task.Run(() =>
                {
                    try
                    {
                        while (true)
                        {
                            IntPtr chromeWidgetHostHandle;
                            if (ChromeWidgetHandleFinder.TryFindHandle(browserHandle, out chromeWidgetHostHandle))
                            {
                                messageInterceptor = new ChromeWidgetMessageInterceptor((Control)Browser, chromeWidgetHostHandle, message =>
                                {
                                    const int WM_MOUSEACTIVATE = 0x0021;
                                    const int WM_NCLBUTTONDOWN = 0x00A1;
                                    const int WM_LBUTTONDOWN = 0x0201;

                                    if (message.Msg == WM_MOUSEACTIVATE)
                                    {
                                        // The default processing of WM_MOUSEACTIVATE results in MA_NOACTIVATE,
                                        // and the subsequent mouse click is eaten by Chrome.
                                        // This means any .NET ToolStrip or ContextMenuStrip does not get closed.
                                        // By posting a WM_NCLBUTTONDOWN message to a harmless co-ordinate of the
                                        // top-level window, we rely on the ToolStripManager's message handling
                                        // to close any open dropdowns:
                                        // http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ToolStripManager.cs,1249
                                        var topLevelWindowHandle = message.WParam;
                                        PostMessage(topLevelWindowHandle, WM_NCLBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                                    }
                                    //Forward mouse button down message to browser control
                                    //else if(message.Msg == WM_LBUTTONDOWN)
                                    //{
                                    //    PostMessage(browserHandle, WM_LBUTTONDOWN, message.WParam, message.LParam);
                                    //}

                                    // The ChromiumWebBrowserControl does not fire MouseEnter/Move/Leave events, because Chromium handles these.
                                    // However we can hook into Chromium's messaging window to receive the events.
                                    //
                                    //const int WM_MOUSEMOVE = 0x0200;
                                    //const int WM_MOUSELEAVE = 0x02A3;
                                    //
                                    //switch (message.Msg) {
                                    //    case WM_MOUSEMOVE:
                                    //        Console.WriteLine("WM_MOUSEMOVE");
                                    //        break;
                                    //    case WM_MOUSELEAVE:
                                    //        Console.WriteLine("WM_MOUSELEAVE");
                                    //        break;
                                    //}
                                });

                                break;
                            }
                            else
                            {
                                // Chrome hasn't yet set up its message-loop window.
                                Thread.Sleep(10);
                            }
                        }
                    }
                    catch
                    {
                        // Errors are likely to occur if browser is disposed, and no good way to check from another thread
                    }
                });

                Task.Run(() => {
                    while(true)
                    {
                        lock(this.JavascriptQueueLock)
                        {
                            if(this.frameLoaded)
                            {
                                if (this.JavascriptQueue.Count() > 0)
                                {
                                    string val = this.JavascriptQueue.Dequeue();
                                    this.InvokeOnUiThreadIfRequired(() =>
                                    {
                                        this.Browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(val);
                                    });
                                }
                            }
                            
                            if(this.JavascriptToAllFramesQueue.Count() > 0)
                            {
                                string val = this.JavascriptToAllFramesQueue.Dequeue();
                                //this.InvokeOnUiThreadIfRequired(() =>
                                //{
                                for(int i = 0; i < this.Browser.GetBrowser().GetFrameCount(); i++)
                                {
                                    try
                                    {
                                        /*bool done = false;
                                        while(!done)
                                        {
                                            lock (this.frameLocks[i].Lock)
                                            {
                                                if(!this.frameLocks[i].IsLocked)
                                                {
                                                    done = true;
                                                }
                                                else
                                                {
                                                    System.Threading.Thread.Sleep(10);
                                                }
                                            }
                                        }*/                                        
                                        this.Browser.GetBrowser().GetFrame(i).ExecuteJavaScriptAsync(val);
                                    }
                                    catch(Exception e)
                                    {

                                    }
                                        
                                }
                                //});
                            }
                        }

                        Thread.Sleep(10);
                    }
                });

                if(Name != null)
                {
                    if(Salesforce.BackgroundActions == null)
                    {
                        Salesforce.BackgroundActions = new List<Action>();
                    }

                    Salesforce.BackgroundActions.Add(new Action(() =>
                    {
                        this.Browser.GetBrowser().ShowDevTools();
                    }));
                }
                // #1
                else
                {
                    LoadBackgroundPages();
                }
            }
        }

        private void LoadBackgroundPages()
        {
            // Load Background pages
            // CWIC
            if (Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == "ppbllmlcmhfnfflbkbinnhacecaankdh").Count() == 0)
            {
                // Start Background
                NCP_Browser.NativeMessaging.Extension extension = new NativeMessaging.Extension("ppbllmlcmhfnfflbkbinnhacecaankdh", (ChromiumWebBrowser)Browser, this.salesforce);
                Salesforce.NativeMessagingExtensions.Add("ppbllmlcmhfnfflbkbinnhacecaankdh", extension);
            }
        }

        private void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            Browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            Browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                Browser.Load(url);
            }
        }

        public async void CopySourceToClipBoardAsync()
        {
            var htmlSource = await Browser.GetSourceAsync();

            Clipboard.SetText(htmlSource);
            DisplayOutput("HTML Source copied to clipboard");
        }

        private void ToggleBottomToolStrip()
        {
            if (toolStrip2.Visible)
            {
                Browser.StopFinding(true);
                toolStrip2.Visible = false;
            }
            else
            {
                toolStrip2.Visible = true;
                findTextBox.Focus();
            }
        }

        private void FindNextButtonClick(object sender, EventArgs e)
        {
            Find(true);
        }

        private void FindPreviousButtonClick(object sender, EventArgs e)
        {
            Find(false);
        }

        private void Find(bool next)
        {
            if (!string.IsNullOrEmpty(findTextBox.Text))
            {
                Browser.Find(0, findTextBox.Text, next, false, false);
            }
        }

        private void FindTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            Find(true);
        }

        public void ShowFind()
        {
            ToggleBottomToolStrip();
        }

        private void FindCloseButtonClick(object sender, EventArgs e)
        {
            ToggleBottomToolStrip();
        }

        

        internal void QueueJavascript(string p)
        {
            lock(this.JavascriptQueueLock)
            {
                JavascriptQueue.Enqueue(p);
            }            
        }

        internal void DoneInitializing()
        {
            this.frameLoaded = true;
        }

        internal void ReInitializing()
        {
            this.frameLoaded = false;
        }

        internal void QueueJavascriptToAllFrames(string Javascript)
        {
            lock (this.JavascriptQueueLock)
            {
                JavascriptToAllFramesQueue.Enqueue(Javascript);
            } 
        }

        public Salesforce salesforce { get; set; }
    }
}
