using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IPC_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxReceivedMessageSize = 2147483647;
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
                        var pipe = "TestConnection";
                        //serviceHost = NCP_CallRecorder.IPC.WCFFactory.OpenPipe(typeof(Program), typeof(NCP_CallRecorder.IPC.WCFCallbackInterface), pipe);
                        channel.Connect(pipe);
                        break;
                    case "disconnect":
                        channel.Disconnect();
                        break;
                    case "break":
                        channel.Break();
                        break;
                    case "get":
                        var info = channel.GetInformation();
                        Console.WriteLine(info.CurrentStatus.ToString());
                        if (info.CallDataList != null)
                        {
                            info.CallDataList.ForEach(cd =>
                            {
                                ForwardCallData(cd);
                            });
                        }
                        break;
                    case "play":
                        Console.Write("Enter File Path: ");
                        var file = Console.ReadLine();
                        var ms = channel.PlayFile(file);
                        SoundPlayer sp = new SoundPlayer(ms);
                        sp.Play();
                        break;
                    case "confirm":
                        Console.Write("Enter Confirmation #:");
                        var cn = Console.ReadLine();
                        channel.Confirm(int.Parse(cn));
                        break;
                    case "upload":
                        Console.Write("Enter Upload #:");
                        var un = Console.ReadLine();
                        Console.Write("Enter Case ID:");
                        var ci = Console.ReadLine();
                        channel.SendFile(int.Parse(un), ci);
                        break;
                    case "quit":
                        done = true;
                        break;
                }
            }
        }

        public static ServiceHost serviceHost { get; set; }


        static void ForwardCallData(NCP_CallRecorder.IPC.CallData callData)
        {
            foreach(var file in callData.OpusFiles)
            {
                Console.WriteLine("{1}|Opus File: {0}", file, callData.Number);
            }
        }
    }
}
