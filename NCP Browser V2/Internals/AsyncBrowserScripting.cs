using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCP_Browser.Internals
{
    public class AsyncBrowserScripting
    {
        public Screen SalesforceScreen { get; set; }
        public Salesforce SalesforceRef { get; set; }

        public string NCPBrowserScripting()
        {
            if (CaseRelatedPhoneNumbers == null)
            {
                CaseRelatedPhoneNumbers = new List<CaseRelatedPhoneNumber>();
            }

            if (UnRelatedCasePhoneNumbers == null)
            {
                UnRelatedCasePhoneNumbers = new List<CaseRelatedPhoneNumber>();
            }

            if (Salesforce.CallRecordings == null)
            {
                Salesforce.CallRecordings = new List<CallData>();
            }
            return "NCP Scripting Engine injected";            
        }

        public void PlayDing()
        {
            Stream dingStream = NCP_Browser.Properties.Resources.Ding;
            //Assembly.GetExecutingAssembly().GetManifestResourceStream("NCP_Browser.Ding.wav");
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
            BrowserWindow.Icon = NCP_Browser.Properties.Resources.chromium_256;
            BrowserWindow.Title = Title;
            BrowserWindow.Browser = Browser;
            BrowserWindow.URL = URL;

            Browser.RequestHandler = new NCP_Browser.Internals.QFund.RequestHandler();

            // Browser Navigation
            if (showWebNav)
            {
                BrowserWindow.webNav = new ToolStrip();
                BrowserWindow.webNav.SuspendLayout();
                BrowserWindow.backButton = new ToolStripButton(global::NCP_Browser.Properties.Resources.back);
                BrowserWindow.forwardButton = new ToolStripButton(global::NCP_Browser.Properties.Resources.forward);
                BrowserWindow.haltButton = new ToolStripButton(global::NCP_Browser.Properties.Resources.halt);
                BrowserWindow.homeButton = new ToolStripButton(global::NCP_Browser.Properties.Resources.home);
                BrowserWindow.reloadButton = new ToolStripButton(global::NCP_Browser.Properties.Resources.reload);
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
                    BrowserWindow.reloadButton
                });

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
            if (showWebNav)
                BrowserWindow.Controls.Add(BrowserWindow.webNav);

            Browser.TitleChanged += browserStrip.ChromiumBrowser_TitleChanged;

            if (showWebNav)
                Browser.IsBrowserInitializedChanged += browserStrip.Browser_IsBrowserInitializedChanged;

            PositionChildBrowserOnOppositeMonitor(FormControl, BrowserWindow);
            ToolStripEnhanced.AddShowClose(browserStrip);
            openWindows.DropDownItems.Add(browserStrip);
            if (showWebNav)
                BrowserWindow.webNav.ResumeLayout(false);
            p.ResumeLayout(false);
            BrowserWindow.ResumeLayout(false);
            BrowserWindow.Show();
            p.Controls.Add(Browser);
            Browser.BringToFront();
            //WebNavUpdate(BrowserWindow);
        }

        public static void PositionChildBrowserOnOppositeMonitor(Salesforce FormControl, BaseBaseForm BrowserWindow)
        {
            if (FormControl.InvokeRequired)
            {
                FormControl.Invoke(new Action<Salesforce, BaseBaseForm>(PositionChildBrowserOnOppositeMonitor), new object[] { FormControl, BrowserWindow });
            }
            else
            {
                Screen browserScreen = Screen.FromControl(FormControl);
                Screen notBrowserScreen = browserScreen;

                foreach (Screen s in Screen.AllScreens.ToList())
                {
                    if (s.DeviceName != browserScreen.DeviceName)
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
            }
        }

        internal static void WebNavUpdate(BaseBaseForm sender)
        {
            if (sender.InvokeRequired)
            {
                sender.Invoke(new WebNaveUpdateDelegate(WebNavUpdate), new object[] { sender });
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

            NCP_Browser.QFund qf = new NCP_Browser.QFund();
            qf.WindowValue = 1;
            var en = this.SalesforceRef.openWindows.DropDownItems.GetEnumerator();
            int winvalue = 0;
            if (MaxWindows > 0)
            {
                while (en.MoveNext())
                {
                    winvalue += ((BaseBaseForm)((ToolStripEnhanced)en.Current).form).WindowValue;
                }
                if (winvalue + 1 > MaxWindows)
                {
                    MessageBox.Show(String.Format("You have exceeded the maximum number of {0} QFund windows.\nPlease Close some.", MaxWindows));
                    return;
                }
            }
            qf.Text = String.Format("QFund - {0}", name);

            NCP_Browser.Internals.AsyncBrowserScripting.PositionChildBrowserOnOppositeMonitor(this.SalesforceRef, qf);
            if (MaxWindows > 0)
            {
                qf.WindowState = FormWindowState.Maximized;
            }
            /*
            var screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].DeviceName != SalesforceScreen.DeviceName)
                {
                    qf.StartPosition = FormStartPosition.Manual;
                    qf.Location = screens[i].WorkingArea.Location;
                    qf.WindowState = FormWindowState.Maximized;
                }
            }*/

            ToolStripEnhanced tsi = new ToolStripEnhanced(qf.Text, generic_Menu, qf, this.SalesforceRef.openWindows, this);
            tsi.URI = url;

            QfundThread(tsi);
        }

        private void QfundThread(ToolStripEnhanced tsi)
        {
            if (this.SalesforceRef.InvokeRequired)
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
                ((ToolStripEnhanced)sender).form.Invoke(new ActivateFormDelegate(generic_Menu), new object[] { sender, e });
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

        public void RegisterJabberCallBack(CefSharp.IJavascriptCallback callback)
        {
            // TODO set up linker to forwarder
            NCP_Browser.Jabber.XmppClient.AddCallBack(callback);
        }

        public void RegisterLockCallback(CefSharp.IJavascriptCallback callback)
        {
            NCP_Browser.Jabber.XmppClient.AddLockCallBack(callback);
        }

        public void RegisterUnlockCallback(CefSharp.IJavascriptCallback callback)
        {
            NCP_Browser.Jabber.XmppClient.AddUnlockCallBack(callback);
        }

        public void GetPresenceStatus(CefSharp.IJavascriptCallback callback)
        {
            // No longer using XMPP
            //callback.ExecuteAsync(NCP_Browser.Jabber.XmppClient.GetPresenceStatus());

            callback.ExecuteAsync(NCP_Browser.Salesforce.GetPresence());
        }

        public void JabberAvailable()
        {
            NCP_Browser.Jabber.XmppClient.SetAvailable();
        }

        public void JabberDND(string status)
        {
            NCP_Browser.Jabber.XmppClient.SetDND(status);
        }

        public void RegisterSendCallStatusCallback(CefSharp.IJavascriptCallback callback)
        {
            Salesforce.CallRecorderImplementation.AddSendCallStatusListener(callback);
        }

        public void StartRecording()
        {
            Salesforce.CallRecorderImplementation.StartRecording();
        }

        public void StopRecording()
        {
            Salesforce.CallRecorderImplementation.StopRecording();
        }

        public void GetRecorderStatus(CefSharp.IJavascriptCallback callback)
        {
            callback.ExecuteAsync(Salesforce.CallRecorderImplementation.GetStatus());
        }

        public void AddRelatedPhoneNumber(String CaseId, String PhoneNumber, String LoanNumber, String CustomerName, String CaseNumber, String CreatedDate)
        {
            if (CaseRelatedPhoneNumbers == null)
            {
                CaseRelatedPhoneNumbers = new List<CaseRelatedPhoneNumber>();
            }

            var CleanedPhoneNumber = PhoneNumber.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");

            if (CaseRelatedPhoneNumbers.Where(x => x.CaseId == CaseId && x.PhoneNumber == CleanedPhoneNumber).Count() == 0)
            {
                CaseRelatedPhoneNumbers.Add(new CaseRelatedPhoneNumber() { CaseId = CaseId, PhoneNumber = CleanedPhoneNumber, CaseNumber = CaseNumber, CustomerName = CustomerName, LoanNumber = LoanNumber, CreatedDate = DateTime.ParseExact(CreatedDate,"yyyy-MM-dd HH:mm:ss", null) });
            }

        }

        public void AddUnRelatedPhoneNumber(String CaseId, String PhoneNumber, String LoanNumber, String CustomerName, String CaseNumber, String CreatedDate)
        {
            if (UnRelatedCasePhoneNumbers == null)
            {
                UnRelatedCasePhoneNumbers = new List<CaseRelatedPhoneNumber>();
            }

            var CleanedPhoneNumber = PhoneNumber.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");

            if (UnRelatedCasePhoneNumbers.Where(x => x.CaseId == CaseId && x.PhoneNumber == CleanedPhoneNumber).Count() == 0)
            {
                UnRelatedCasePhoneNumbers.Add(new CaseRelatedPhoneNumber() { CaseId = CaseId, PhoneNumber = CleanedPhoneNumber, CaseNumber = CaseNumber, CustomerName = CustomerName, LoanNumber = LoanNumber, CreatedDate = DateTime.ParseExact(CreatedDate, "yyyy-MM-dd HH:mm:ss", null) });
            }
        }

        public void TriggerCallEnd()
        {
            lock(Salesforce.FrameLoadLock)
            {
                Salesforce.CallEndTrigger = true;
            }
            /*
            if(Salesforce.CallRecordings == null)
            {
                Salesforce.CallRecordings = new List<CallData>();
            }

            var information = Salesforce.CallRecorderImplementation.GetInformation();
            var lastCall = information.CallDataList.OrderByDescending(x => x.Number).FirstOrDefault();

            NCP_Browser.CallRecorder.Implementation.AddCallToList(lastCall,true);
            */
        }

        

        private static List<CaseRelatedPhoneNumber> CaseRelatedPhoneNumbers { get; set; }
        private static List<CaseRelatedPhoneNumber> UnRelatedCasePhoneNumbers { get; set; }

        public static void CheckCallRecordingAgainstCasePhoneNumbers(CallData x, bool showConfirmation = false)
        {
            if (UnRelatedCasePhoneNumbers == null)
            {
                UnRelatedCasePhoneNumbers = new List<CaseRelatedPhoneNumber>();
            }

            if(CaseRelatedPhoneNumbers == null)
            {
                CaseRelatedPhoneNumbers = new List<CaseRelatedPhoneNumber>();
            }

            if (UnRelatedCasePhoneNumbers.Where(y => y.PhoneNumber == x.PhoneNumber).Select(y => new CaseRelatedPhoneNumber { CaseId = y.CaseId }).Distinct().Count() == 1)
            {
                x.Remove = true;
            }
            if (CaseRelatedPhoneNumbers.Where(y => y.PhoneNumber == x.PhoneNumber).Select(y => new CaseRelatedPhoneNumber { CaseId = y.CaseId }).Distinct().Count() == 1)
            {
                // Just Attach it
                SendMailToSalesforce(x, CaseRelatedPhoneNumbers.Where(y => y.PhoneNumber == x.PhoneNumber).Select(y => new CaseRelatedPhoneNumber { CaseId = y.CaseId }).First().CaseId);
                if(showConfirmation)
                {
                    MessageBox.Show("Call recording automatically attached");
                }
            }
            else if(CaseRelatedPhoneNumbers.Where(y => y.PhoneNumber == x.PhoneNumber).Select(y => new CaseRelatedPhoneNumber { CaseId = y.CaseId }).Distinct().Count() > 1)
            {
                // Show List of Cases to attach to (filtered)
                ShowRecordingSelectionDialog(x, CaseRelatedPhoneNumbers.Where(y => y.PhoneNumber == x.PhoneNumber).Select(y => new CaseRelatedPhoneNumber { CaseId = y.CaseId, CaseNumber = y.CaseNumber, CustomerName = y.CustomerName, LoanNumber = y.LoanNumber, PhoneNumber = y.PhoneNumber, CreatedDate = y.CreatedDate }).Distinct().ToList());
            }
            else
            {
                // Show list of all Cases to attach to (unfiltered)
                ShowRecordingSelectionDialog(x, CaseRelatedPhoneNumbers.Select(y => new CaseRelatedPhoneNumber { CaseId = y.CaseId, CaseNumber = y.CaseNumber, CustomerName = y.CustomerName, LoanNumber = y.LoanNumber, PhoneNumber = y.PhoneNumber, CreatedDate = y.CreatedDate }).Distinct().ToList());
            }
        }

        private static void ShowRecordingSelectionDialog(CallData x, List<CaseRelatedPhoneNumber> list)
        {
            lock(Salesforce.FrameLoadLock)
            {
                CallRecordingSelectCaseContainer crcc = new CallRecordingSelectCaseContainer();
                crcc.RelatedPhoneNumbers = list;
                crcc.SelectCaseCallData = x;
                if (Salesforce.CallRecordingSelectCaseContainers == null)
                {
                    Salesforce.CallRecordingSelectCaseContainers = new List<CallRecordingSelectCaseContainer>();
                }
                Salesforce.CallRecordingSelectCaseContainers.Add(crcc);
            }
        }

        private static void ShowForm(object param)
        {
            List<CaseRelatedPhoneNumber> list = (List<CaseRelatedPhoneNumber>)param;
            NCP_Browser.Forms.SelectCase sc = new Forms.SelectCase();
            sc.Show();
            sc.Populate(list);
        }

        private static void SendMailToSalesforce(CallData x, string CaseId)
        {
            if(Salesforce.CallRecorderImplementation.SendFile(x.IPC_CallData.Number, CaseId))
            {
                x.Remove = true;
            }
            else
            {
                // TODO: relay the error
            }
            //SalesforceRef.UpdateUI();
        }
    }
}