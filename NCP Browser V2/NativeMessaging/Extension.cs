using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CefSharp.Example;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NCP_Browser.Handlers;
using CefSharp.WinForms.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;

namespace NCP_Browser.NativeMessaging
{
    public class Extension
    {
        public string Name {get; set;}
        private Process ExtensionProcess {get; set;}
        private CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser;
        private Salesforce salesforce;
        public NativeMessagingOutputQueue output { get; set; }
        public NativeMessagingOutputQueue errorOutput { get; set; }
        private object initializationLock;

        public Extension(string ExtensionId, CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser, Salesforce salesforce)
        {
            this.Name = ExtensionId;
            this.chromiumWebBrowser = chromiumWebBrowser;
            this.salesforce = salesforce;
            Initialize(ExtensionId, chromiumWebBrowser);
        }

        public void Initialize(string ExtensionId, CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser)
        {
            initializationLock = new object();
            salesforce.InvokeOnUiThreadIfRequired(new Action(() => {
                salesforce.AddTab(String.Format(CefExample.DefaultUrl, ExtensionId), Name: ExtensionId, InitializationLock: initializationLock, Extension : this);
            }));
        }

        private void ForwardMessageToBrowser(string Message)
        {
            ForwardMessageToBackground(String.Format("chrome.runtime.onNativeMessage.fire(\"{0}\");", Message.Replace("\n","").Replace("'", "\'").Replace("\"", "\\\"")));       
        }

        public void WriteText(String Text)
        {
            var bina = ExtensionProcess.StandardInput.BaseStream;
            var bytes = Encoding.UTF8.GetBytes(Text);
            
            var lbytes = BitConverter.GetBytes((UInt32)Text.Length);
            //for (int i = 0; i < (lbytes.Length / 2); i++)
            //{
            //    byte b = lbytes[i];
            //    lbytes[i] = lbytes[lbytes.Length - (i + 1)];
            //    lbytes[lbytes.Length - (i + 1)] = b;
            //}
            bina.Write(lbytes, 0, lbytes.Length);
            //bina.Write(Encoding.UTF8.GetBytes("\n"), 0, 1);
            bina.Write(bytes, 0, bytes.Length);
            //var blah = Encoding.UTF8.GetBytes("\n").Length;
            //bina.Write(Encoding.UTF8.GetBytes("\n"), 0, Encoding.UTF8.GetBytes("\n").Length);
            
            bina.Flush();
            
        }
        

        internal void connect(string connectInfo, string channelId)
        {
            ForwardMessageToBackground(String.Format("chrome.runtime.onConnectExternal.fire('{0}','{1}');", connectInfo, channelId));
            //ForwardMessageToBackground("alert(done);");
        }

        private void ForwardMessageToBackground(string JavascriptCode)
        {
            bool done = false;
            while (!done)
            {
                try
                {
                    if (salesforce.browserTabs.Where(x => x.Key == this.Name).Count() > 0)
                    {
                        salesforce.browserTabs.Where(x => x.Key == this.Name).First().Value.QueueJavascript(JavascriptCode);
                        done = true;
                    }
                }
                catch
                {
                    // Duct Tape Solution
                }
            }
        }

        internal void connectNative(string Hostname, string Executable)
        {

            FileInfo fi = new FileInfo(Executable);
            try
            {
                ExtensionProcess = new Process();
                ExtensionProcess.StartInfo = new ProcessStartInfo(Executable);
                ExtensionProcess.StartInfo.WorkingDirectory = fi.Directory.FullName;
                ExtensionProcess.StartInfo.CreateNoWindow = true;
                ExtensionProcess.StartInfo.UseShellExecute = false;
                ExtensionProcess.StartInfo.RedirectStandardInput = true;
                ExtensionProcess.StartInfo.RedirectStandardOutput = true;
                ExtensionProcess.StartInfo.RedirectStandardError = true;
                ExtensionProcess.StartInfo.Arguments = String.Format("--parent-window=0 chrome-extension://{0}/", Hostname);
                ExtensionProcess.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Starting CWIC");
            }
            

            output = new NativeMessagingOutputQueue(ExtensionProcess.StandardOutput.BaseStream, this.ForwardMessageToBrowser);
            output.Start();

            errorOutput = new NativeMessagingOutputQueue(ExtensionProcess.StandardError.BaseStream, this.ForwardMessageToBrowser);
            errorOutput.Start();
        }

        internal void postMessage(string message, string channelId)
        {
            ForwardMessageToBackground(String.Format("chrome.runtime.onPortMessage.fire('{0}','{1}')", message, channelId));
        }
    }
}
