using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace NCP_CallRecorder.IPC
{
    [ServiceContract(Namespace = "NCP_CallRecordingFrontend.IPC")]
    public interface WCFCallbackInterface
    {
        [OperationContract(IsOneWay = true)]
        void SendCallStatus(CallStatus CallStatus);

        [OperationContract(IsOneWay = true)]
        void ForwardCallData(CallData callData);
    }

    public enum CallStatus
    {
        Init = -1,
        Ready = 0,
        OnACall = 2,
        Recording = 4
    }

    [DataContract]
    public class CallData
    {
        [DataMember]
        public List<String> OpusFiles { get; set; }

        [DataMember]
        public int Number { get; set; }
    }
}
