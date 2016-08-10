using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace NCP_Browser.CallRecorder
{
    class Implementation : NCP_CallRecorder.IPC.WCFCallbackInterface
    {
        private static string AlertSMTPHost = "192.168.100.12";
        private static MailAddress AlertFromAddress = new MailAddress("Alerts@schear.net", "NCP ALERTS");
        private static string AlertSendAccountUsername = "Alerts";
        private static string AlertSendAccountPassword = @"n0+!fiCat1on$";

        NCP_CallRecorder.IPC.CallStatus CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
        List<CefSharp.IJavascriptCallback> CallStatusCallbacks { get; set; }

        public Implementation()
        {
            CallStatusCallbacks = new List<CefSharp.IJavascriptCallback>();
            CallRecorderLock = new object();
        }

        public void AddSendCallStatusListener(CefSharp.IJavascriptCallback callback)
        {
            CallStatusCallbacks.Add(callback);
        }

        void NCP_CallRecorder.IPC.WCFCallbackInterface.SendCallStatus(NCP_CallRecorder.IPC.CallStatus CallStatus)
        {
            lock(CallRecorderLock)
            {
                if (CallStatus != this.CallStatus)
                {
                    this.CallStatus = CallStatus;
                    foreach (var callback in CallStatusCallbacks)
                    {
                        callback.ExecuteAsync(this.CallStatus.ToString());
                    }
                }
            }            
        }

        void NCP_CallRecorder.IPC.WCFCallbackInterface.ForwardCallData(NCP_CallRecorder.IPC.CallData callData)
        {
            SmtpClient mailClient = new SmtpClient();
            mailClient.Credentials = new NetworkCredential(AlertSendAccountUsername, AlertSendAccountPassword);
            mailClient.EnableSsl = false;
            mailClient.Host = AlertSMTPHost;
            mailClient.Port = 25;
            MailMessage mailMessage = new MailMessage(AlertFromAddress, new MailAddress("cory.craig@schear.net"));
            mailMessage.IsBodyHtml = false;
            StringBuilder sb = new StringBuilder();
            foreach(var file in callData.OpusFiles)
            {
                var fi = new FileInfo(file);
                sb.Append(String.Format("Call-File:{0}\n", fi.Name));
                mailMessage.Attachments.Add(new Attachment(fi.FullName));
            }
            mailClient.Send(mailMessage);
        }

        internal void Open()
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            EndpointAddress ep = new EndpointAddress(NCP_CallRecorder.RecordingEngine.IPC_ADDRESS);
            channel = ChannelFactory<NCP_CallRecorder.IPC.WCFInterfaceContract>.CreateChannel(binding, ep);
        }

        public void StartRecording()
        {
            lock (CallRecorderLock)
            {
                channel.StartRecording();
            }
        }

        public void StopRecording()
        {
            lock (CallRecorderLock)
            {
                
                channel.StopRecording();
            }
        }

        public void Connect()
        {
            lock(CallRecorderLock)
            {
                var pipe = "net.pipe://localhost/NCP_CallRecorderFrontend/IPC";
                serviceHost = NCP_CallRecorder.IPC.WCFFactory.OpenPipe(typeof(Implementation), typeof(NCP_CallRecorder.IPC.WCFCallbackInterface), pipe);
                channel.Connect(pipe);
            }
        }

        public void Disconnect()
        {
            lock(CallRecorderLock)
            {
                channel.Disconnect();
            }
        }
        
        public ServiceHost serviceHost { get; set; }
        public NCP_CallRecorder.IPC.WCFInterfaceContract channel { get; set; }
        public object CallRecorderLock { get; set; }

        internal string GetStatus()
        {
            lock(CallRecorderLock)
            {
                return this.CallStatus.ToString();
            }
        }
    }
}

