using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabberSDK
{
    public interface IXmppClient
    {
        void OnMessage(object sender, agsXMPP.protocol.client.Message msg);
        
        void OnPresence(object sender, agsXMPP.protocol.client.Presence pres);

        void OnAuthError(object sender, agsXMPP.Xml.Dom.Element e);

        void OnLogin(object sender);

        void OnXmppConnectionStateChanged(object sender, agsXMPP.XmppConnectionState state);
    }
}
