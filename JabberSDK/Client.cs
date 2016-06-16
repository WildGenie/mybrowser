using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;

namespace JabberSDK
{
    public class Client
    {
        private agsXMPP.XmppClientConnection xmppClient;
        private IXmppClient _XmppClient = null;

        public Client(bool UseStartTLS, bool UseSSL, bool AutoPresence, bool AutoAgents, bool AutoRoster, String Server, String ConnectServer, int Port, String UserName, String Password)
        {
            xmppClient = new XmppClientConnection();

            xmppClient.UseStartTLS = UseStartTLS;
            xmppClient.UseSSL = UseSSL;
            xmppClient.AutoPresence = AutoPresence;
            xmppClient.AutoAgents = AutoAgents;
            xmppClient.AutoRoster = AutoRoster;


            xmppClient.Server = Server;
            xmppClient.Port = Port;
            if (!String.IsNullOrEmpty(ConnectServer))
            {
                xmppClient.ConnectServer = ConnectServer;
                xmppClient.AutoResolveConnectServer = false;
            }
            else
            {
                xmppClient.AutoResolveConnectServer = true;
            }

            xmppClient.Username = UserName;
            xmppClient.Password = Password;

            xmppClient.Resource = null;
            xmppClient.ClientSocket.OnValidateCertificate += ClientSocket_OnValidateCertificate;
            
        }

        public void Open()
        {
            xmppClient.Open();
        }

        public IXmppClient XmppClient
        {
            set
            {
                if(this._XmppClient != null)
                {
                    this.xmppClient.OnMessage -= this._XmppClient.OnMessage;
                    this.xmppClient.OnPresence -= this._XmppClient.OnPresence;
                    this.xmppClient.OnAuthError -= this._XmppClient.OnAuthError;
                    this.xmppClient.OnLogin -= this._XmppClient.OnLogin;
                    this.xmppClient.OnXmppConnectionStateChanged -= this._XmppClient.OnXmppConnectionStateChanged;
                }

                this._XmppClient = value;

                this.xmppClient.OnMessage += this._XmppClient.OnMessage;
                this.xmppClient.OnPresence += this._XmppClient.OnPresence;
                this.xmppClient.OnAuthError += this._XmppClient.OnAuthError;
                this.xmppClient.OnLogin += this._XmppClient.OnLogin;
                this.xmppClient.OnXmppConnectionStateChanged += this._XmppClient.OnXmppConnectionStateChanged;
            }

            get
            {
                return this._XmppClient;
            }
        }

        public void SetPresence(agsXMPP.protocol.client.ShowType ShowType, String Status)
        {
            xmppClient.Show = ShowType;
            xmppClient.Status = Status;
            xmppClient.SendMyPresence();
        }

        public void GetRoster()
        {
            xmppClient.RequestRoster();
        }


        bool ClientSocket_OnValidateCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
