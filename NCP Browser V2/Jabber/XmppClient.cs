using Microsoft.Win32;
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
        private static List<CefSharp.IJavascriptCallback> LockCallBacks;
        private static List<CefSharp.IJavascriptCallback> UnLockCallBacks;
        private static agsXMPP.XmppClientConnection clientConnection;
        private static string status = null;
        private static string show = null;

        public XmppClient(System.Windows.Forms.ToolStripMenuItem MenuItem)
        {
            this.MenuItem = MenuItem;
            JavascriptCallBacks = new List<CefSharp.IJavascriptCallback>();
            LockCallBacks = new List<CefSharp.IJavascriptCallback>();
            UnLockCallBacks = new List<CefSharp.IJavascriptCallback>();
            Microsoft.Win32.SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                // ...
                case SessionSwitchReason.SessionLock:
                    // Do whatever you need to do for a lock
                    // ...
                    if (LockCallBacks == null)
                        LockCallBacks = new List<CefSharp.IJavascriptCallback>();
                    foreach(var cb in LockCallBacks)
                    {
                        cb.ExecuteAsync();
                    }
                    break;
                case SessionSwitchReason.SessionUnlock:
                    // Do whatever you need to do for an unlock
                    // ...
                    if (UnLockCallBacks == null)
                        UnLockCallBacks = new List<CefSharp.IJavascriptCallback>();
                    foreach (var cb in UnLockCallBacks)
                    {
                        cb.ExecuteAsync();
                    }
                    break;
                // ...
            }
        }

        void JabberSDK.IXmppClient.OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            //throw new NotImplementedException();
        }

        void JabberSDK.IXmppClient.OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if (JavascriptCallBacks == null)
                JavascriptCallBacks = new List<CefSharp.IJavascriptCallback>();
            if(pres.From.User == Environment.UserName)
            {
                status = String.IsNullOrEmpty(pres.Status) ? "NONE" : pres.Status;
                show = pres.Show.ToString();
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
                clientConnection = null;                
            }
            else
            {
                this.MenuItem.Visible = false;
                clientConnection = (agsXMPP.XmppClientConnection)sender;
            }
        }

        internal static void AddCallBack(CefSharp.IJavascriptCallback callback)
        {
            Salesforce.PresenseNotifications.Add(callback);
            /*if(JavascriptCallBacks == null)
                JavascriptCallBacks = new List<CefSharp.IJavascriptCallback>();
            JavascriptCallBacks.Add(callback);*/
        }

        internal static void AddLockCallBack(CefSharp.IJavascriptCallback callback)
        {
            if (LockCallBacks == null)
                LockCallBacks = new List<CefSharp.IJavascriptCallback>();
            LockCallBacks.Add(callback);
        }

        internal static void AddUnlockCallBack(CefSharp.IJavascriptCallback callback)
        {
            if (UnLockCallBacks == null)
                UnLockCallBacks = new List<CefSharp.IJavascriptCallback>();
            UnLockCallBacks.Add(callback);
        }

        internal static object[] GetPresenceStatus()
        {
            return new object[] { status, show };
        }

        internal static void SetAvailable()
        {
            if(clientConnection != null && show != "NONE")
            {
                clientConnection.Show = agsXMPP.protocol.client.ShowType.NONE;
                clientConnection.Status = null;
                clientConnection.SendMyPresence();
            }
        }

        internal static void SetDND(string status)
        {
            if (clientConnection != null && show != "dnd")
            {
                clientConnection.Show = agsXMPP.protocol.client.ShowType.dnd;
                clientConnection.Status = status;
                clientConnection.SendMyPresence();
            }
        }
    }
}
