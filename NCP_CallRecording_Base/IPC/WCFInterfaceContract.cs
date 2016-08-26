﻿using NCP_CallRecording.IPC;
using System.IO;
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

        [OperationContract(IsOneWay = false)]
        Information GetInformation();

        [OperationContract(IsOneWay = true)]
        void Confirm(int Number);

        [OperationContract(IsOneWay = false)]
        MemoryStream PlayFile(string FilePath);

        [OperationContract(IsOneWay = false)]
        bool SendFile(int Number, string CaseId);
    }
}