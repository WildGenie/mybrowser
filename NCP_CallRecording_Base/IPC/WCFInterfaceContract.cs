using System.ServiceModel;

namespace NCP_CallRecorder.IPC
{
    [ServiceContract(Namespace = "NCP_CallRecording.IPC")]
    public interface WCFInterfaceContract
    {
        [OperationContract(IsOneWay = true)]
        void StartRecording();

        [OperationContract(IsOneWay = true)]
        void StopRecording();

        [OperationContract(IsOneWay = true)]
        void Connect(string Pipe);

        [OperationContract(IsOneWay = true)]
        void Disconnect();

        [OperationContract(IsOneWay = true)]
        void Break();
    }
}
