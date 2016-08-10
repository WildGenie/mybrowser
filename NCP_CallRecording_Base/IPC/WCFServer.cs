using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace NCP_CallRecorder.IPC
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WCFServer : WCFInterfaceContract
    {
        public bool clientConnected = false;
        void WCFInterfaceContract.StartRecording()
        {
            Console.WriteLine("Start Recording");
            NCP_CallRecorder.RecordingEngine.StartRecording();
        }

        void WCFInterfaceContract.StopRecording()
        {
            Console.WriteLine("Stop Recording");
            NCP_CallRecorder.RecordingEngine.StopRecording();
        }

        void WCFInterfaceContract.Connect(string Pipe)
        {
            Console.WriteLine("Connect");
            NCP_CallRecorder.RecordingEngine.ConnectClient(Pipe);
        }

        void WCFInterfaceContract.Disconnect()
        {
            Console.WriteLine("Disconnect");
            NCP_CallRecorder.RecordingEngine.ClientConnected = false;
        }


        void WCFInterfaceContract.Break()
        {
            NCP_CallRecorder.RecordingEngine.Break();
        }
    }
}
