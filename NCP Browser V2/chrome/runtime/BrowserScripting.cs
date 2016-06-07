using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.chrome.runtime
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class BrowserScripting
    {
        internal static string Name = "ncp_runtime";

        private CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser;
        private BrowserTabUserControl browserTabUserControl;
        private Salesforce salesforce;

        public BrowserScripting(CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser, BrowserTabUserControl browserTabUserControl, Salesforce salesforce)
        {
            // TODO: Complete member initialization
            this.chromiumWebBrowser = chromiumWebBrowser;
            this.browserTabUserControl = browserTabUserControl;
            this.salesforce = salesforce;
        }

        public Port connect(string extensionId, string connectInfo, string channelId)
        {
            if(extensionId == "ppbllmlcmhfnfflbkbinnhacecaankdh")
            { 
                Port p = new Port();
                p.name = extensionId;
                // TODO: Any interop between foreground and background
                if(Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == extensionId).Count() == 0)
                {
                    // Start Background
                    NCP_Browser.NativeMessaging.Extension extension = new NativeMessaging.Extension(extensionId, chromiumWebBrowser, salesforce);
                    Salesforce.NativeMessagingExtensions.Add(extensionId, extension);
                }

                var ex = Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == extensionId).First();
                ex.connect(connectInfo, channelId);

                return p;
            }
            else
            {
                return null;
            }
            
        }

        public void postMessage(string extensionId, string message, string channelId)
        {
            var ex = Salesforce.NativeMessagingExtensions.Values.Where(x => x.Name == extensionId).First();
            ex.postMessage(message, channelId);
            //ex.WriteText(message);
        }

        public void DoneInitializing()
        {
            lock(salesforce.browserTabs["Salesforce"].JavascriptQueueLock)
            {
                salesforce.browserTabs["Salesforce"].DoneInitializing();
            }
        }

        public void onMessageListeners(string port, string channel, CefSharp.IJavascriptCallback callback)
        {
            if(MessageCallbacks == null)
            {
                MessageCallbacks = new List<MessageCallback>();
            }
            MessageCallbacks.Add(new MessageCallback() { Channel = channel, Port = port, JavacriptCallback = callback });
        }

        public static List<MessageCallback> MessageCallbacks;
    }

    public class MessageCallback
    {
        public string Port { get; set; }
        public string Channel { get; set; }
        public CefSharp.IJavascriptCallback JavacriptCallback { get; set; }
    }
}
