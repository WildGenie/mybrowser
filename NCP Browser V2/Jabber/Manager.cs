using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.Jabber
{
    class Manager
    {
        private const string  EncFile = "JabberCred.enc";
        private const string pf = "j*3fj2188fj$$*@(0JFJVM";
        private const string sl = "289284780592456";
        internal static Info LoadCredentials()
        {
            Info retVal = new Info();
            retVal.UserName = Environment.UserName;
            StreamReader sr = null;
            try
            {
                if(!File.Exists(EncFile))
                {
                    File.Create(EncFile);
                }
                sr = new StreamReader(EncFile);
                var bytes = Convert.FromBase64String(sr.ReadToEnd());

                var key = NCP_Browser.Encryption.Rijindael.GetKey(pf, sl);
                var iv = NCP_Browser.Encryption.Rijindael.GetIV(retVal.UserName, sl);

                retVal.Password = NCP_Browser.Encryption.Rijindael.DecryptStringFromBytes(bytes, key, iv);
            }
            catch(Exception ex)
            {
                if(sr != null)
                {
                    sr.Dispose();
                }
                // N T D
            }
            return retVal;
        }

        internal static void WriteCredentials(String Password)
        {
            if(File.Exists(EncFile))
            {
                File.Delete(EncFile);
            }
            File.WriteAllText(EncFile, Convert.ToBase64String(NCP_Browser.Encryption.Rijindael.EncryptStringToBytes(Password, NCP_Browser.Encryption.Rijindael.GetKey(pf, sl), NCP_Browser.Encryption.Rijindael.GetIV(Environment.UserName, sl))));
        }

        private static JabberSDK.Client Client;

        internal static void LogIn(System.Windows.Forms.ToolStripMenuItem toolStripMenuItem)
        {
            var cred = LoadCredentials();
            if(String.IsNullOrEmpty(cred.Password))
            {
                toolStripMenuItem.Visible = true;
            }
            else
            {
                Client = new JabberSDK.Client(true, false, true, false, true, "schearfin.local", "192.168.150.15", 5222, cred.UserName, cred.Password);
                Client.XmppClient = new XmppClient(toolStripMenuItem);
                Client.Open();                
            }            
        }
    }
}
