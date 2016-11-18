using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.chrome.runtime
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class NativeBrowserScripting
    {
        internal static string Name = "ncp_runtime";
        private string _id;

        public static List<ChromeFrame> ActiveOnMessageListeners = new List<ChromeFrame>();
        private CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser;
        private CefSharp.WinForms.ChromiumWebBrowser browser;
        private BrowserTabUserControl browserTabUserControl;
        private Salesforce salesforce;
        private object loadLock;
        private bool doneInit = false;


        public NativeBrowserScripting(CefSharp.WinForms.ChromiumWebBrowser browser, BrowserTabUserControl browserTabUserControl, String Id, Salesforce salesforce)
        {
            // TODO: Complete member initialization
            this.browser = browser;
            this.browserTabUserControl = browserTabUserControl;
            this._id = Id;
            this.salesforce = salesforce;
            this.loadLock = new object();

        }

        public string id
        {
            get
            {
                return this._id;
            }
        }

        public Port connectNative(string application, string hostname)
        {
            if (application == "com.cisco.jabber.jsdk")
            { 
                if(Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == "ppbllmlcmhfnfflbkbinnhacecaankdh").Count() == 1)
                {
                    Port p = new Port();
                    p.name = application;

                    var ex = Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == "ppbllmlcmhfnfflbkbinnhacecaankdh").First();
                    ex.connectNative(hostname, @"C:\Program Files (x86)\Cisco Systems\Web Communicator\CiscoWebCommunicatorAddOn.exe");

                    return p;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }

        public void postMessage(string application, string message)
        {
            string portId = String.Empty;
            if(application == "com.cisco.jabber.jsdk")
            {
                portId = "ppbllmlcmhfnfflbkbinnhacecaankdh";
            }
            
            if (Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == portId).Count() == 1)
            {
                Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == portId).First().WriteText(message);
                return;
            }
            else
            {
                return;
            }
        }

        public void receiveMessage(string channelid, string message)
        {            
            //salesforce.browserTabs.Values.ToList().ForEach(x =>
            //{
            //    x.QueueJavascriptToAllFrames(String.Format("chrome.runtime.onMessage.fire('{0}');", message.Replace("\"", "\\\"")));
            //});

            BrowserScripting.MessageCallbacks.Where(x => x.Channel == channelid).First().JavacriptCallback.ExecuteAsync(new object[] { message });
        }

        public void onMessageListeners(string portId, int ActiveListeners)
        {
            var listen = NativeBrowserScripting.ActiveOnMessageListeners.Where(x => x.portId == portId).FirstOrDefault();
            if(listen == null)
            {
                listen = new ChromeFrame();
                listen.portId = portId;
                listen.browser = this.chromiumWebBrowser;
                NativeBrowserScripting.ActiveOnMessageListeners.Add(listen);
            }
            listen.ActiveListeners = ActiveListeners;
        }

        public void management_get(string extensionId, string callbackId)
        {
            management_getReturn(extensionId, callbackId);
            return;
        }



        public async Task management_getReturn(string extensionid, string callbackId)
        {
            await Task.Run(() =>
            {
                ExtensionInfo extensionInfo = new ExtensionInfo();
                extensionInfo.id = extensionid;
                if(extensionid == "ppbllmlcmhfnfflbkbinnhacecaankdh")
                {
                    extensionInfo.description = "Enables Cisco Jabber phone calls and video calls";
                    extensionInfo.enabled = true;
                    extensionInfo.homepageUrl = "https://chrome.google.com/webstore/detail/ppbllmlcmhfnfflbkbinnhacecaankdh";
                    extensionInfo.hostPermissions = new string[0];
                    extensionInfo.icons = new IconInfo[0];
                    extensionInfo.installType = "admin";
                    extensionInfo.isApp = false;
                    extensionInfo.mayDisable = true;
                    extensionInfo.name = "Cisco Web Communicator";
                    extensionInfo.offlineEnabled = false;
                    extensionInfo.optionsUrl = "";
                    extensionInfo.permissions = new string[2];
                    extensionInfo.permissions[0] = "management";
                    extensionInfo.permissions[1] = "nativeMessaging";
                    extensionInfo.shortName = "Cisco Web Communicator";
                    extensionInfo.type = "extension";
                    extensionInfo.updateUrl = "https://clients2.google.com/service/update2/crx";
                    extensionInfo.version = "3.1.0.363";

                }

                //chromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(String.Format("chrome.runtime.management.getCallback('{0}','{1}');", Newtonsoft.Json.JsonConvert.SerializeObject(extensionInfo, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default }), callbackId));
            });
            return;
        }

        public string extension_getURL(string url)
        {
            return String.Format("cefsharp-extension://{0}/{1}", this._id, url);
        }

        public void DoneInitializing()
        {
            // I had to add a threadlock here and a boolean value to signify that has been called.
            // For some reason the chromium javascript runtime was doubling the invocations
            lock(loadLock)
            {
                if(!doneInit)
                {
                    this.browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(NCP_Browser.Properties.Resources.background.Replace("chrome-extension", "cefsharp-extension"));
                    
                    this.browserTabUserControl.DoneInitializing();

                    doneInit = true;
                }
            }
            
        }

        public class ExtensionInfo
        {
            public string description { get; set; }
            public bool enabled { get; set; }
            public string homepageUrl { get; set; }
            public string[] hostPermissions { get; set; }
            public IconInfo[] icons { get; set; }
            public string id { get; set; }
            public string installType { get; set; }
            public bool isApp { get; set; }
            public bool mayDisable { get; set; }
            public string name { get; set; }
            public bool offlineEnabled { get; set; }
            public string optionsUrl { get; set; }
            public string[] permissions { get; set; }
            public string shortName { get; set; }
            public string type { get; set; }
            public string updateUrl { get; set; }
            public string version { get; set; }
        }

        public class IconInfo
        {
            public int size { get; set; }
            public string url { get; set; }
        }
        
        public object Disconnect(object x, object[] x2)
        {
            return null;
        }

        public class ChromeFrame
        {
            public string portId { get; set; }
            public int ActiveListeners { get; set; }
            public CefSharp.WinForms.ChromiumWebBrowser browser { get; set; }
        }
    }
}
