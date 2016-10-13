using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                lock (RecordingEngine.lockObject)
                {
                    NCP_CallRecorder.RecordingEngine.ipcCallDataList.Where(x => x.Number == Number).First().OpusFiles.ToList().ForEach(x =>
                    {
                        NCP_CallRecorder.RecordingEngine.DeleteTheseFiles.Add(x + ".pgp");
                    });
                    NCP_CallRecorder.RecordingEngine.ipcCallDataList.RemoveAll(x => x.Number == Number);
                }                
            }
        }

        MemoryStream WCFInterfaceContract.PlayFile(string FilePath)
        {
            FileInfo fi = new FileInfo(FilePath);
            var targetFile = Path.Combine(Path.Combine(Path.Combine(RecordingEngine.ROOT_FILE_FOLDER, Environment.MachineName),"temp"), fi.Name);
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
            File.Move(FilePath, targetFile);
            if(targetFile.EndsWith(".pgp"))
            {
                NCP_CallRecording.Crypto.Manager.Decrypt(targetFile);
                return NCP_CallRecording.Audio.Streamer.OpenStream(targetFile.Replace(".pgp", ""));
            }
            else
            {
                return NCP_CallRecording.Audio.Streamer.OpenStream(targetFile);
            }
        }



        // TODO: CONFIG
        private static string AlertSMTPHost = NCP_CallRecording.Configuration.Settings.SMTP_HOST;
        private static MailAddress AlertFromAddress = new MailAddress(NCP_CallRecording.Configuration.Settings.FROM_ADDRESS, NCP_CallRecording.Configuration.Settings.FROM_NAME);
        private static string AlertSendAccountUsername = NCP_CallRecording.Configuration.Settings.FROM_ACCOUNT;
        private static string AlertSendAccountPassword = NCP_CallRecording.Configuration.Settings.FROM_PASSWORD;
        bool WCFInterfaceContract.SendFile(int Number, string CaseId)
        {
            try
            {
                if (NCP_CallRecorder.RecordingEngine.ipcCallDataList.Where(x => x.Number == Number).Count() == 1)
                {
                    var item = NCP_CallRecorder.RecordingEngine.ipcCallDataList.Where(x => x.Number == Number).First();

                    // GET API CREDENTIALS
                    string endpoint = null;
                    string username = null;
                    string password = null;
                    string token = null;

                    GetAPICredentials(ref endpoint, ref username, ref password, ref token);
                    bool success = true;
                    item.OpusFiles.ForEach(of => {
                        FileInfo fi = new FileInfo(of+".pgp");
                        var thisSuccess = SalesforceUploader.Program.UploadFile(of + ".pgp", endpoint, username, password, token, "Call " + fi.Name, CaseId, Environment.MachineName);
                        if(success)
                        {
                            success = thisSuccess == null;
                        }
                        else
                        {
                            NCP_CallRecording.Logging.Writer.Write(String.Format("Error - WCFInterfaceContract.SendFile - {0} - {1}", thisSuccess.Message, thisSuccess.StackTrace));
                        }
                    });
                    

                    return true;
                }
            }
            catch(Exception e)
            {
                return false;
            }
            return false;
        }

        private static void GetAPICredentials(ref string endpoint, ref string username, ref string password, ref string token)
        {
            MemoryStream ms = new MemoryStream();
            NCP_CallRecording.Crypto.Manager.Decrypt(Path.Combine(NCP_CallRecording.Configuration.Settings.KEY_LOC, "APICredentials.txt.pgp"), ms);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            string line = null;

            int currentLineNumber = 0;

            while ((line = sr.ReadLine()) != null)
            {
                if (currentLineNumber == 0)
                {
                    endpoint = line;
                }
                else if (currentLineNumber == 1)
                {
                    username = line;
                }
                else if (currentLineNumber == 2)
                {
                    password = line;
                }
                else if (currentLineNumber == 3)
                {
                    token = line;
                }
                currentLineNumber++;
            }
        }


        //MemoryStream WCFInterfaceContract.PlayFile(string MachineName, string FileName)
        //{
            //Path.Combine(Path.Combine(NCP_CallRecording.Configuration.Settings.ROOT_FILE_FOLDER,MachineName))
        //}
    }
}
