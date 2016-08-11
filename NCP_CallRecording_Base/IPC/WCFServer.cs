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


        NCP_CallRecording.IPC.Information WCFInterfaceContract.GetInformation()
        {
            NCP_CallRecording.IPC.Information info = new NCP_CallRecording.IPC.Information();
            info.CallDataList = NCP_CallRecorder.RecordingEngine.ipcCallDataList;
            info.CurrentStatus = NCP_CallRecorder.RecordingEngine.LastReportedCallStatus;
            return info;
        }


        void WCFInterfaceContract.Confirm(int Number)
        {
            if(NCP_CallRecorder.RecordingEngine.ipcCallDataList.Where(x => x.Number == Number).Count() == 1)
            {
                NCP_CallRecorder.RecordingEngine.ipcCallDataList.RemoveAll(x => x.Number == Number);
            }
        }
    }
}
