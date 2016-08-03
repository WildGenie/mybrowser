using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace JabberWebApi
{
    public class Program
    {
        public delegate void SendPresence(string status, string show);
        public delegate void SendCallStatus(string status);
        static void Main(string[] args)
        {
            Program p = new Program(PresDegTest, CStatDegTest);
            p.Start();
            Console.ReadLine();
            p.Stop();
            Console.ReadLine();
        }

        public void Stop()
        {
            WebApplication.Dispose();
        }

        public void Start()
        {
            string baseAddress = "http://localhost:9783/";
            WebApplication = WebApp.Start<Startup>(url: baseAddress);
        }

        private static void PresDegTest(string status, string show)
        {
            Console.WriteLine("Status:{0}, Show{1}", status, show);
        }

        private static void CStatDegTest(string status)
        {
            Console.WriteLine("Call Status: {0}", status);
        }

        public Program(SendPresence SendPresenceDelegate, SendCallStatus SendCallStatusDelegate)
        {
            Program.SendPresenceDelegate = SendPresenceDelegate;
            Program.SendCallStatusDelegate = SendCallStatusDelegate;
        }

        internal static SendPresence SendPresenceDelegate { get; set; }
        internal static SendCallStatus SendCallStatusDelegate { get; set; }
        private static IDisposable WebApplication;
    }
}
