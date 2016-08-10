using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IPC_Tester
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
    class Program : NCP_CallRecorder.IPC.WCFCallbackInterface
    {
        static void Main(string[] args)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            EndpointAddress ep = new EndpointAddress(NCP_CallRecorder.RecordingEngine.IPC_ADDRESS);
            NCP_CallRecorder.IPC.WCFInterfaceContract channel = ChannelFactory<NCP_CallRecorder.IPC.WCFInterfaceContract>.CreateChannel(binding, ep);
            
            bool done = false;
            while (!done)
            {
                var x = Console.ReadLine();
                switch (x)
                {
                    case "start":
                        channel.StartRecording();
                        break;
                    case "stop":
                        channel.StopRecording();
                        break;
                    case "connect":
                        var pipe = "net.pipe://localhost/NCP_CallRecorderFrontend/IPC";
                        serviceHost = NCP_CallRecorder.IPC.WCFFactory.OpenPipe(typeof(Program), typeof(NCP_CallRecorder.IPC.WCFCallbackInterface), pipe);
                        channel.Connect(pipe);
                        break;
                    case "disconnect":
                        channel.Disconnect();
                        break;
                    case "break":
                        channel.Break();
                        break;
                    case "quit":
                        done = true;
                        break;
                }
            }
        }

        void NCP_CallRecorder.IPC.WCFCallbackInterface.SendCallStatus(NCP_CallRecorder.IPC.CallStatus CallStatus)
        {
            Console.WriteLine(CallStatus.ToString());
        }

        public static ServiceHost serviceHost { get; set; }


        void NCP_CallRecorder.IPC.WCFCallbackInterface.ForwardCallData(NCP_CallRecorder.IPC.CallData callData)
        {
            foreach(var file in callData.OpusFiles)
            {
                Console.WriteLine("Opus File: {0}", file);
            }
        }
    }
}
