using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCP_CallRecording.IPC
{
    public class Information
    {
        public List<NCP_CallRecorder.IPC.CallData> CallDataList { get; set; }

        public NCP_CallRecorder.IPC.CallStatus CurrentStatus { get; set; }
    }
}
