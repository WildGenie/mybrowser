using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCP_Browser.Jabber
{
    class XmppClient : JabberSDK.IXmppClient
    {
        private System.Windows.Forms.ToolStripMenuItem MenuItem;
        private static List<CefSharp.IJavascriptCallback> JavascriptCallBacks;

        public XmppClient(System.Windows.Forms.ToolStripMenuItem MenuItem)
        {
            this.MenuItem = MenuItem;
            JavascriptCallBacks = new List<CefSharp.IJavascriptCallback>();
        }

        void JabberSDK.IXmppClient.OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            //throw new NotImplementedException();
        }

        void JabberSDK.IXmppClient.OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if(pres.From.User == Environment.UserName)
            {
                // TODO Set up Forwarding
                foreach(var cb in JavascriptCallBacks)
                {
                    cb.ExecuteAsync(new object[] { String.IsNullOrEmpty(pres.Status) ? "NONE" : pres.Status, pres.Show.ToString() });
                }
            }
            //throw new NotImplementedException();
        }

        void JabberSDK.IXmppClient.OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            MenuItem.Visible = true;
        }

        void JabberSDK.IXmppClient.OnLogin(object sender)
        {
            //throw new NotImplementedException();
        }

        void JabberSDK.IXmppClient.OnXmppConnectionStateChanged(object sender, agsXMPP.XmppConnectionState state)
        {
            if (state != agsXMPP.XmppConnectionState.SessionStarted)
            {
                this.MenuItem.Visible = true;
            }
            else
            {
                this.MenuItem.Visible = false;
            }
        }

        internal static void AddCallBack(CefSharp.IJavascriptCallback callback)
        {
            JavascriptCallBacks.Add(callback);
        }
    }
}
