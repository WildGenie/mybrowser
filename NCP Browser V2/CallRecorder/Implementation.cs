using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;
using NCP_Browser.Internals;

namespace NCP_Browser.CallRecorder
{
    class Implementation
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

        void SendCallStatus(NCP_CallRecorder.IPC.CallStatus CallStatus)
        {
            foreach (var callback in CallStatusCallbacks)
            {
                callback.ExecuteAsync(this.CallStatus.ToString());
            }         
        }
        /*
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
        }*/

        internal void Open()
        {
            OpenChannel();
            System.Threading.Thread messageLoop = new System.Threading.Thread(MessageLoop);
            messageLoop.Start();
        }

        private void OpenChannel()
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxReceivedMessageSize = 2147483647;
            //binding.SendTimeout = new TimeSpan(0, 0, 5, 30, 0);
            //binding.ReceiveTimeout = new TimeSpan(0, 0, 5, 30, 0);
            EndpointAddress ep = new EndpointAddress(NCP_CallRecorder.RecordingEngine.IPC_ADDRESS);
            channelFactory = new ChannelFactory<NCP_CallRecorder.IPC.WCFInterfaceContract>(binding);
            channelFactory.Faulted += channelFactory_Faulted;
            channel = channelFactory.CreateChannel(ep);
            ((IContextChannel)channel).OperationTimeout = new TimeSpan(0, 0, 5, 30, 0);
        }

        void channelFactory_Faulted(object sender, EventArgs e)
        {
            lock(CallRecorderLock)
            {
                // Re-open channel if it is still faulted
                if(channelFactory.State == CommunicationState.Faulted)
                {
                    OpenChannel();
                }                
            }
        }

        private void MessageLoop(object obj)
        {
            int MaxCallNumber = 0;
            int LastShownNumber = 0;
            for(;;)
            {
                
                NCP_CallRecording.IPC.Information info = null;
                lock(CallRecorderLock)
                {
                    if (ChannelActive())
                    {
                        info = MessageLoop_HandleOpenedState();
                    }
                    else
                    {
                        MessageLoop_HandleOtherState(((ICommunicationObject)channel).State);
                    }
                }
                // Populate Drop Down with calls
                if(info != null)
                {
                    lock (Salesforce.FrameLoadLock)
                    {
                        if (Salesforce.CallRecordings == null)
                        {
                            Salesforce.CallRecordings = new List<CallData>();
                        }
                        int CurCallNumber = MaxCallNumber;
                        int AddedNumbers = 0;

                        // Add calls from the call recorder into memory
                        info.CallDataList.ForEach(x =>
                        {
                            if(AddCallToList(x, false))
                            {
                                AddedNumbers++;
                            }
                            if (x.Number > MaxCallNumber)
                            {
                                MaxCallNumber = x.Number;
                            }
                        });

                        // If we add more than 1 call then this is an 'initial' load
                        // We don't want to display a dialog
                        if(AddedNumbers > 1)
                        {
                            var call = Salesforce.CallRecordings.OrderByDescending(x => x.IPC_CallData.Number).FirstOrDefault();
                            if (call != null)
                            {
                                LastShownNumber = call.IPC_CallData.Number;
                            }
                        }
                        
                        // Remove calls no longer being tracked by the recorder                        
                        Salesforce.CallRecordings.ForEach(x =>
                        {
                            if (info.CallDataList.Where(y => y.Number == x.IPC_CallData.Number).Count() == 0)
                            {
                                x.Remove = true;
                            }
                        });

                        // Show Call Dialog if it has been triggered
                        if(Salesforce.CallEndTrigger)
                        {
                            var call = Salesforce.CallRecordings.OrderByDescending(x => x.IPC_CallData.Number).FirstOrDefault();
                            if(call != null && call.IPC_CallData.Number > LastShownNumber)
                            {
                                LastShownNumber = call.IPC_CallData.Number;
                                Salesforce.CallEndTrigger = false;
                                AsyncBrowserScripting.CheckCallRecordingAgainstCasePhoneNumbers(call, true);
                            }
                        }
                    }
                }                
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void MessageLoop_HandleOtherState(CommunicationState communicationState)
        {
            // Re-open if closed/faulted but not if opening/closing
            if(communicationState != CommunicationState.Opening && communicationState != CommunicationState.Closing)
            {
                OpenChannel();
            }
        }

        private NCP_CallRecording.IPC.Information MessageLoop_HandleOpenedState()
        {
            try
            {
                var info = channel.GetInformation();
                if (CallStatus != info.CurrentStatus)
                {
                    CallStatus = info.CurrentStatus;
                    SendCallStatus(CallStatus);
                }

                return info;
            }
            catch
            {
                // Reset status
                CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
                return null;
            }
        }

        public void StartRecording()
        {
            lock (CallRecorderLock)
            {
                try
                {
                    if (ChannelActive())
                    {
                        channel.StartRecording();
                    }
                }
                catch
                {
                    // Reset status
                    CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
                }
                
            }
        }

        private bool ChannelActive()
        {
            return ((ICommunicationObject)channel).State == CommunicationState.Opened || ((ICommunicationObject)channel).State == CommunicationState.Created;
        }

        public void StopRecording()
        {
            lock (CallRecorderLock)
            {
                try
                {
                    if (ChannelActive())
                    {
                        channel.StopRecording();
                    }
                }
                catch
                {
                    // Reset status
                    CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
                }                
            }
        }

        /*public void Connect()
        {
            lock(CallRecorderLock)
            {
                var pipe = "net.pipe://localhost/NCP_CallRecorderFrontend/IPC";
                serviceHost = NCP_CallRecorder.IPC.WCFFactory.OpenPipe(typeof(Implementation), typeof(NCP_CallRecorder.IPC.WCFCallbackInterface), pipe);
                channel.Connect(pipe);
            }
        }*/

        public void Disconnect()
        {
            lock(CallRecorderLock)
            {
                try
                {
                    if (ChannelActive())
                    {
                        channel.Disconnect();
                    }
                }
                catch
                {
                    // Reset status
                    CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
                }   
            }
        }
        
        public ServiceHost serviceHost { get; set; }
        public NCP_CallRecorder.IPC.WCFInterfaceContract channel { get; set; }
        public ChannelFactory<NCP_CallRecorder.IPC.WCFInterfaceContract> channelFactory { get; set; }
        public object CallRecorderLock { get; set; }

        internal string GetStatus()
        {
            lock(CallRecorderLock)
            {
                return this.CallStatus.ToString();
            }
        }

        public NCP_CallRecording.IPC.Information GetInformation()
        {
            NCP_CallRecording.IPC.Information retVal = new NCP_CallRecording.IPC.Information();            
            lock(CallRecorderLock)
            {
                try
                {
                    retVal.CallDataList = new List<NCP_CallRecorder.IPC.CallData>();
                    retVal.CurrentStatus = CallStatus;
                    if (ChannelActive())
                    {
                        retVal = channel.GetInformation();
                    }
                }
                catch
                {
                    // Reset status
                    CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
                }
                return retVal;
            }
        }

        internal bool SendFile(int Number, string CaseId)
        {
            lock(CallRecorderLock)
            {
                try
                {
                    if (ChannelActive())
                    {
                        if (channel.SendFile(Number, CaseId))
                        {
                            channel.Confirm(Number);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    // Reset status
                    CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
                    return false;
                }                
            }
        }

        internal void Confirm(int Number)
        {
            lock(CallRecorderLock)
            {
                try
                {
                    if (ChannelActive())
                    {
                        channel.Confirm(Number);
                    }
                }
                catch
                {
                    // Reset status
                    CallStatus = NCP_CallRecorder.IPC.CallStatus.Init;
                }
            }
        }

        public static bool AddCallToList(NCP_CallRecorder.IPC.CallData lastCall, bool DialogCheck)
        {
            if (Salesforce.CallRecordings == null)
            {
                Salesforce.CallRecordings = new List<CallData>();
            }
            // Only treat this as triggered if a new call is there.
            if (lastCall != null && Salesforce.CallRecordings.Where(x => x.IPC_CallData.Number == lastCall.Number).Count() == 0)
            {
                NCP_Browser.Internals.CallData cd = new NCP_Browser.Internals.CallData();
                cd.IPC_CallData = lastCall;
                var file = lastCall.OpusFiles.First();
                var fParts = file.Split('_');
                if (fParts[1].Length >= 10)
                {
                    cd.PhoneNumber = fParts[1].Substring(fParts[1].Length - 10, 10);
                }
                else if (fParts[3].Length >= 10)
                {
                    cd.PhoneNumber = fParts[3].Substring(fParts[3].Length - 10, 10);
                }
                else if (fParts[5].Length >= 10)
                {
                    cd.PhoneNumber = fParts[5].Substring(fParts[5].Length - 10, 10);
                }
                else
                {
                    cd.PhoneNumber = "Unknown";
                }

                cd.DateAdded = DateTime.ParseExact(fParts[6], "yyyyMMddHHmmss", null);

                lock (Salesforce.FrameLoadLock)
                {
                    Salesforce.CallRecordingUpdated = true;
                    Salesforce.CallRecordings.Add(cd);
                }
                if(DialogCheck)
                {
                    NCP_Browser.Internals.AsyncBrowserScripting.CheckCallRecordingAgainstCasePhoneNumbers(cd);
                }
                return true;
            }
            else if (lastCall != null && DialogCheck && Salesforce.CallRecordings.Where(x => x.IPC_CallData.Number == lastCall.Number).Count() == 1)
            {
                NCP_Browser.Internals.AsyncBrowserScripting.CheckCallRecordingAgainstCasePhoneNumbers(Salesforce.CallRecordings.Where(x => x.IPC_CallData.Number == lastCall.Number).First());
            }
            return false;
        }

        public void SetPaginationDate(DateTime Date)
        {
            lock (CallRecorderLock)
            {
                try
                {
                    if (ChannelActive())
                    {
                        channel.SetDate(Date);
                    }
                }
                catch
                {
                }
            }
        }

        internal void UpdateCalls()
        {
            lock(CallRecorderLock)
            {
                Salesforce.CallRecordings = null;
                try
                {
                    if(ChannelActive())
                    {
                        var info = channel.GetInformation();
                        if(info != null && info.CallDataList != null)
                        {
                            info.CallDataList.ForEach(x =>
                            {
                                AddCallToList(x, false);
                            });
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}

