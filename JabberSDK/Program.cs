using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabberSDK
{
    class Program : IXmppClient
    {
        [STAThread]
        static void Main()
        {
            Program p = new Program();
            Client c = new Client(true, false, true, false, true, "schearfin.local", "192.168.150.15", 5222, "ccraig", "nothere");
            c.Open();
            c.XmppClient = p;
            bool done = false;
            while(!done)
            {
                String Command = Console.ReadLine();
                if(Command == "exit")
                {
                    done = true;
                }
                else if (Command.Contains("status:"))
                {
                    try
                    {
                        var vals = Command.Split(':')[1].Split(',');
                        agsXMPP.protocol.client.ShowType st = agsXMPP.protocol.client.ShowType.NONE;

                        switch(vals[0])
                        {
                            case "away":
                                st = agsXMPP.protocol.client.ShowType.away;
                                break;
                            case "chat":
                                st = agsXMPP.protocol.client.ShowType.chat;
                                break;
                            case "dnd":
                                st = agsXMPP.protocol.client.ShowType.dnd;
                                break;
                            case "xa":
                                st = agsXMPP.protocol.client.ShowType.xa;
                                break;
                            default:
                                st = agsXMPP.protocol.client.ShowType.NONE;
                                break;
                        }

                        c.SetPresence(st, vals[1]);
                    }
                    catch
                    {
                        Console.WriteLine("Format|status:away,Away");
                    }
                    
                }
                else if (Command == "roster")
                {
                    c.GetRoster();
                }
            }
        }

        void IXmppClient.OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            Console.WriteLine(msg.Body);
        }

        void IXmppClient.OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            Console.WriteLine(String.Format("Presence:{2}|{3}|{0},{1}", pres.Show, pres.Status, pres.From, pres.To));
            Console.WriteLine(GetUserFullName("schearfin", pres.From.User.Split('@')[0]));
        }

        public static string GetUserFullName(string domain, string userName)
        {
            System.DirectoryServices.DirectoryEntry userEntry = new System.DirectoryServices.DirectoryEntry("WinNT://" + domain + "/" + userName + ",User");
            return (string)userEntry.Properties["fullname"].Value;
        }


        void IXmppClient.OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            throw new NotImplementedException();
        }


        void IXmppClient.OnLogin(object sender)
        {
            
        }


        void IXmppClient.xmppClient_OnXmppConnectionStateChanged(object sender, agsXMPP.XmppConnectionState state)
        {
            
        }
    }
}
