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
            string username = "";
            Console.Write("Enter your username:");
            username = Console.ReadLine();
            string pass = "";
            Console.Write("Enter your password: ");
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            Client c = new Client(true, false, true, false, true, "schearfin.local", "192.168.150.16", 5222, username, pass);
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
            Console.WriteLine(e.InnerXml);
        }


        void IXmppClient.OnLogin(object sender)
        {
            
        }


        void IXmppClient.OnXmppConnectionStateChanged(object sender, agsXMPP.XmppConnectionState state)
        {
            
        }
    }
}
