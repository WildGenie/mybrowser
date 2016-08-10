using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace NCP_CallRecorder_Monitor
{
    class Program
    {
        private static string SERVICE_NAME = @"NCP_CallRecording_Service";
        private static int Depth = 0;
        static void Main(string[] args)
        {
            try
            {
                ServiceController sc = new ServiceController(SERVICE_NAME);
                CheckAndStartService(sc, false, null);
            }
            catch(Exception e)
            {
                LogErrorMessage(e.Message);
            }
        }

        private static void LogErrorMessage(string p)
        {
            throw new NotImplementedException();
        }

        private static void CheckAndStartService(ServiceController sc, bool restart, string previousStatus)
        {
            Depth++;
            if(Depth > 5)
            {
                LogErrorMessage("Max depth exceeded");
            }
            else
            {
                sc.Refresh();
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        // Everything is ok
                        break;
                    case ServiceControllerStatus.Stopped:
                        AttemptStart(sc, restart, previousStatus);
                        break;
                    case ServiceControllerStatus.Paused:
                        AttemptStart(sc, restart, previousStatus);
                        break;
                    case ServiceControllerStatus.StopPending:
                        WaitResetCheck(sc);
                        break;
                    case ServiceControllerStatus.StartPending:
                        WaitResetCheck(sc);
                        break;
                    default:
                        WaitResetCheck(sc);
                        break;
                }
            }
            
        }

        private static void WaitResetCheck(ServiceController sc)
        {
            System.Threading.Thread.Sleep(45000);
            CheckAndStartService(sc, false, null);
        }

        private static void AttemptStart(ServiceController sc, bool restart, string previousStatus)
        {
            if (restart)
            {
                sc.Start();
            }
            else if (previousStatus != GetStatus(sc.Status))
            {
                System.Threading.Thread.Sleep(45000);
                CheckAndStartService(sc, true, GetStatus(sc.Status));
            }
        }

        private static string GetStatus(ServiceControllerStatus scs)
        {
            switch (scs)
            {
                case ServiceControllerStatus.Running:
                    return "Running";
                case ServiceControllerStatus.Stopped:
                    return "Stopped";
                case ServiceControllerStatus.Paused:
                    return "Paused";
                case ServiceControllerStatus.StopPending:
                    return "Stopping";
                case ServiceControllerStatus.StartPending:
                    return "Starting";
                default:
                    return "Status Changing";
            }
        }
    }
}
