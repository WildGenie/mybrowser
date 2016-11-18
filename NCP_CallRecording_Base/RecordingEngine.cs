using System;
using System.Collections;
using System.Collections.Generic;
using SharpPcap;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Collections.Specialized;
using System.ServiceModel;

namespace NCP_CallRecorder
{
    public sealed class RecordingEngine
    {
        internal static List<UInt16> watchPorts;
        internal static List<NCP_CallRecorder.CallData> callDataList;
        internal static List<IPC.CallData> ipcCallDataList;
        internal static int callDataNumber = 0;
        internal static object lockObject;
        internal static List<String> DeleteTheseFiles { get; set; }

        private static bool startMarker = false;
        private static bool stopMarker = false;
        private static bool recording = false;

        public static string IPC_ADDRESS = NCP_CallRecording.Configuration.Settings.IPC_ADDRESS;

        // TODO: CONFIG FILE
        internal static string ROOT_FILE_FOLDER = NCP_CallRecording.Configuration.Settings.ROOT_FILE_FOLDER;
        
        public static void Main(string[] args)
        {
            System.Threading.Thread mainThread = new System.Threading.Thread(Run);
            mainThread.Start();
        }

        private static ICaptureDevice OpenCaptureDevice()
        {
            try
            {
                lock (lockObject)
                {
                    BreakIt = false;
                }
                if (ipcCallDataList == null)
                {
                    ipcCallDataList = new List<IPC.CallData>();
                }
                // Initialize Fields/Properties
                if (RecordingEngine.device != null && ((SharpPcap.WinPcap.WinPcapDevice)RecordingEngine.device).Opened)
                {
                    // Device is open we don't need to restart
                    return RecordingEngine.device;
                }
                else
                {
                    LastReportedCallStatus = IPC.CallStatus.Init;
                    // We don't want to drop any current call recordings on the floor if a single malformed packet causes an exception
                    if (watchPorts != null && watchPorts.Count > 0 && callDataList != null && callDataList.Count > 0)
                    {
                        // Because we are restarting the listener these calls may be effectivly "dead" so add a 10 second expiration date
                        //  that will be cleared if new packets are written to to the audio streams
                        lock (lockObject)
                        {
                            callDataList.ForEach(x =>
                            {
                                x.ExpirationDate = DateTime.Now.AddSeconds(10);
                            });
                        }
                    }
                    else
                    {
                        watchPorts = new List<UInt16>();
                        callDataList = new List<CallData>();
                    }

                    // Retrieve the device list
                    CaptureDeviceList devices = null;
                    System.Threading.Thread getDeviceThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((object x) =>
                    {
                        devices = CaptureDeviceList.Instance;
                    }));
                    getDeviceThread.Start(null);
                    bool done = false;
                    int ii = 0;
                    while(!done)
                    {

                        done = getDeviceThread.Join(1000);
                        ii++;
                        System.Threading.Thread.Sleep(1000);
                        if(ii > 10)
                        {
                            done = true;
                        }
                    }

                    ICaptureDevice device = null;

                    

                    int i = 0;
                    if(devices != null && devices.Count > 0)
                    {
                        foreach (var d in devices)
                        {
                            if (d.Description.Contains("MS NDIS 6.0 LoopBack Driver") || d.Description.ToLower().Contains("loopback"))
                            {
                                continue;
                            }
                            else
                            {
                                device = d;
                                NCP_CallRecording.Logging.Writer.Write("DEV: " + d.Description);
                            }

                        }
                    }
                    

                    // If no devices were found print an error
                    if (device == null)
                    {
                        Console.WriteLine("No devices were found on this machine");
                        return null;
                    }

                    //Register our handler function to the 'packet arrival' event
                    device.OnPacketArrival +=
                        new PacketArrivalEventHandler(device_OnPacketArrival);

                    // Write the OPUS encoder to disk for use
                    if (File.Exists(Path.Combine(Path.GetTempPath(), "opusenc.exe")))
                    {
                        File.Delete(Path.Combine(Path.GetTempPath(), "opusenc.exe"));
                    }
                    File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "opusenc.exe"), NCP_CallRecording.Properties.Resources.opusenc);

                    if (File.Exists(Path.Combine(Path.GetTempPath(), "opusdec.exe")))
                    {
                        File.Delete(Path.Combine(Path.GetTempPath(), "opusdec.exe"));
                    }
                    File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "opusdec.exe"), NCP_CallRecording.Properties.Resources.opusdec);



                    //Open the device for capturing
                    int readTimeoutMilliseconds = 1000;
                    device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

                    Console.WriteLine
                        ("-- Listening on {0}, hit 'Ctrl-C' to exit...",
                        device.Description);

                    return device;
                }
            }
            catch (Exception e)
            {
                NCP_CallRecording.Logging.Writer.Write("Open Device Error: " + e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        internal static void Run()
        {            
            lockObject = new object();
            DeleteTheseFiles = new List<string>();
            string ver = SharpPcap.Version.VersionString;
            NCP_CallRecording.Logging.Writer.Write(String.Format("SharpPcap {0}, NCP_CallRecorder.RecordingEngine\n", ver));

            // Make Folder structure is needed
            if(!Directory.Exists(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName)))
            {
                Directory.CreateDirectory(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName));
            }

            if(!Directory.Exists(Path.Combine(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName), "recording")))
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName), "recording"));
            }

            if(!Directory.Exists(Path.Combine(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName), "temp")))
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName), "temp"));
            }

            if (!Directory.Exists(Path.Combine(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName), "log")))
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName), "log"));
            }

            NCP_CallRecording.Logging.Writer.SetUp();

            // Load any files sitting in our machine's folder
            try
            {
                NCP_CallRecording.Logging.Writer.Write("Loading Existing Files");
                LoadExistingFiles();
            }
            catch
            {
                // Nothing to do
            }
            
            // Start IPC
            //wcfServer = new NCP_CallRecording.IPC.WCFServer();
            NCP_CallRecording.Logging.Writer.Write("Service Host Made");
            serviceHost = IPC.WCFFactory.OpenPipe(typeof(IPC.WCFServer), typeof(IPC.WCFInterfaceContract), IPC_ADDRESS);

            // Start capture packets
            device = OpenCaptureDevice();
            if(device != null)
            {
                System.Threading.Thread t = new System.Threading.Thread(device.Capture);
                t.Start();
                NCP_CallRecording.Logging.Writer.Write("Device Capture Started");

                System.Threading.Thread messageThread = new System.Threading.Thread(MessageLoop);
                messageThread.Start(t);
                NCP_CallRecording.Logging.Writer.Write("Message Thread Started");
            }
            else
            {
                NCP_CallRecording.Logging.Writer.Write("Cannot open device");
                Environment.Exit(1);
            }
            
        }

        private static void LoadExistingFiles()
        {
            try
            {
                var pgpFiles = Directory.GetFiles(Path.Combine(Path.Combine(ROOT_FILE_FOLDER, Environment.MachineName), "recording"), "*.pgp");
                Dictionary<DateTime, List<String>> DateAndFiles = new Dictionary<DateTime, List<String>>();
                foreach (string pgpFile in pgpFiles)
                {
                    var pgpFileInfo = new FileInfo(pgpFile);
                    string DateString = "";
                    if (pgpFileInfo.Name.StartsWith("CallRecording"))
                    {
                        DateString = pgpFileInfo.Name.Split('_')[8];
                    }
                    else if (pgpFileInfo.Name.StartsWith("From"))
                    {
                        DateString = pgpFileInfo.Name.Split('_')[6];
                    }

                    if (DateString != "")
                    {
                        var datetimevalue = DateTime.ParseExact(DateString, "yyyyMMddHHmmss", null);
                        if (!DateAndFiles.ContainsKey(datetimevalue))
                        {
                            DateAndFiles.Add(datetimevalue, new List<string>());
                        }

                        DateAndFiles.First(x => x.Key == datetimevalue).Value.Add(pgpFile.Replace(".pgp", ""));
                    }
                }

                if (DateAndFiles.Count > 0)
                {
                    ipcCallDataList = new List<IPC.CallData>();
                    DateAndFiles.OrderBy(x => x.Key).ToList().ForEach(x =>
                    {
                        callDataNumber++;
                        var nicp = new IPC.CallData() { Number = callDataNumber, OpusFiles = new List<string>() };
                        x.Value.ForEach(y =>
                        {
                            nicp.OpusFiles.Add(y);
                        });
                        ipcCallDataList.Add(nicp);
                    });
                }
            }
            catch(Exception ex)
            {
                NCP_CallRecording.Logging.Writer.Write("Loading Existing Files - Exception - " + ex.Message + "\n" + ex.StackTrace);
            }
            
        }

        private static void MessageLoop(object threadRef)
        {
            var t = (System.Threading.Thread)threadRef;
            bool done = false;
            while (!done)
            {
                // Keep alive code for the packet sniffer
                lock (lockObject)
                {
                    if (RequiresRestart)
                    {
                        Restart(ref t);
                    }
                    else
                    {
                        List<CallData> callsToKill = callDataList.Where(x => x.ExpirationDate.HasValue && DateTime.Now > x.ExpirationDate.Value).ToList();
                        callsToKill.ForEach(x =>
                        {
                            try
                            {
                                watchPorts.Remove(x.port);
                            }
                            catch(Exception ex)
                            {
                                NCP_CallRecording.Logging.Writer.Write(String.Format("Watch Port Remove: {0}", ex.Message + "\n" + ex.StackTrace));
                            }
                            finally
                            {
                                x.Write();
                                callDataList.Remove(x);
                            }
                        });
                        List<String> DeletedFiles = new List<string>();
                        foreach(var file in DeleteTheseFiles)
                        {
                            try
                            {
                                FileInfo fi = new FileInfo(file);
                                if(!Directory.Exists(Path.Combine(fi.Directory.FullName, "deleted")))
                                {
                                    Directory.CreateDirectory(Path.Combine(fi.Directory.FullName, "deleted"));
                                }
                                File.Move(fi.FullName, Path.Combine(fi.Directory.FullName, "deleted", fi.Name));
                                //File.Delete(file);
                                NCP_CallRecording.Logging.Writer.Write(String.Format("File Deleted: {0}", file));
                                DeletedFiles.Add(file);
                            }
                            catch (Exception e)
                            {
                                NCP_CallRecording.Logging.Writer.Write(String.Format("File Not Deleted: {0} | {1}", file, e.Message + "\n" + e.StackTrace));
                            }
                        }
                        foreach(var dfile in DeletedFiles)
                        {
                            DeleteTheseFiles.Remove(dfile);
                        }
                    }
                }
                //NCP_CallRecording.Logging.Writer.Write("CheckClientConnected");
                // Report current status to client
                //if (ClientConnected)
                //{
                    HandleClientConnected();

                //}
                System.Threading.Thread.Sleep(1000);
            }

            // Close the pcap device
            // (Note: this line will never be called since
            //  we're capturing infinite number of packets
            device.Close();
        }

        private static void HandleClientConnected()
        {
            //NCP_CallRecording.Logging.Writer.Write("HandleClientConnected");
            IPC.CallStatus CallStatusToReport = IPC.CallStatus.Ready;
            lock (lockObject)
            {
                if (recording)
                {
                    CallStatusToReport = IPC.CallStatus.Recording;
                }
                else if (watchPorts.Count > 0)
                {
                    CallStatusToReport = IPC.CallStatus.OnACall;
                }
            }
            try
            {
                //if (LastReportedCallStatus != CallStatusToReport)
                //{
                    //callbackChannel.SendCallStatus(CallStatusToReport);
                    LastReportedCallStatus = CallStatusToReport;
                //}
                    //NCP_CallRecording.Logging.Writer.Write("HandleClientConnected:" + CallStatusToReport.ToString());
            }
            catch(Exception e)
            {
                ClientConnected = false;
                NCP_CallRecording.Logging.Writer.Write("HandleClientConnected:false");
                NCP_CallRecording.Logging.Writer.Write("Error: " + e.Message + "\n" + e.StackTrace);
                if(e.InnerException != null)
                    NCP_CallRecording.Logging.Writer.Write("Error: " + e.InnerException.Message + "\n" + e.InnerException.StackTrace);
            }
        }

        private static void Restart(ref System.Threading.Thread t)
        {
            NCP_CallRecording.Logging.Writer.Write("Restart()");
            lock(lockObject)
            {
                try
                {
                    RequiresRestart = false;
                    device.Close();
                }
                catch(Exception ex)
                {
                    NCP_CallRecording.Logging.Writer.Write("Restart() Error: " + ex.Message + "\n" + ex.StackTrace);
                    RequiresRestart = true;
                }
                try
                {
                    device = OpenCaptureDevice();
                }
                catch (Exception ex)
                {
                    NCP_CallRecording.Logging.Writer.Write("Restart() Error2: " + ex.Message + "\n" + ex.StackTrace);
                }
                finally
                {
                    if (device != null)
                    {
                        RequiresRestart = false;
                        t = new System.Threading.Thread(device.Capture);
                        t.Start();
                    }
                    else
                    {
                        Restart(ref t);
                    }
                }
            }            
        }

        internal static void CloseDevice()
        {
            NCP_CallRecording.Logging.Writer.Write("CloseDevice()");
            lock (lockObject)
            {
                // There isn't anything associated with a call at this point so no clean up required on that front.
                // Just stop the devide capture so it restarts
                try
                {
                    device.StopCapture();
                }
                catch (Exception ex)
                {
                    NCP_CallRecording.Logging.Writer.Write(String.Format("CloseDevide() Failure: {0}", ex.Message + "\n" + ex.StackTrace));
                }
                finally
                {
                    RequiresRestart = true;
                }                
            }
        }


        internal static void StopRecording()
        {
            NCP_CallRecording.Logging.Writer.Write("StopRecording()");
            lock (lockObject)
            {
                if (!stopMarker && recording)
                {
                    startMarker = false;
                    stopMarker = true;
                }
            }
        }

        internal static void StartRecording()
        {
            NCP_CallRecording.Logging.Writer.Write("StartRecording()");
            lock (lockObject)
            {
                if (!startMarker && !recording)
                {
                    startMarker = true;
                    stopMarker = false;
                }
            }
        }

        /// <summary>
        /// Prints the time and length of each received packet
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            try
            {
                lock(lockObject)
                {
                    if(BreakIt)
                    {
                        BreakIt = false;
                        throw new Exception("BOOM");
                    }
                }
                int i = 0;
                // Handle IPV4 Packets            
                PacketData pd = new PacketData();
                pd.Timevel = e.Packet.Timeval;
                // Confirm this is an Ethernet Type: IPV4 packet
                if(HexStringTable[e.Packet.Data[12]] == "08" && HexStringTable[e.Packet.Data[13]] == "00")
                {
                    // The first 6 bytes are the MAC address of the Destination
                    pd.ToMac = String.Format("{0}:{1}:{2}:{3}:{4}:{5}", HexStringTable[e.Packet.Data[0]], HexStringTable[e.Packet.Data[1]], HexStringTable[e.Packet.Data[2]], HexStringTable[e.Packet.Data[3]], HexStringTable[e.Packet.Data[4]], HexStringTable[e.Packet.Data[5]]);

                    // The second 6 bytes are the MAC address of the Source
                    pd.ToMac = String.Format("{0}:{1}:{2}:{3}:{4}:{5}", HexStringTable[e.Packet.Data[6]], HexStringTable[e.Packet.Data[7]], HexStringTable[e.Packet.Data[8]], HexStringTable[e.Packet.Data[9]], HexStringTable[e.Packet.Data[10]], HexStringTable[e.Packet.Data[11]]);
                    i += 12;
                    // Advance two as we already checked 12,13 at the beginning
                    i += 2;
                    // Confirm again IPV4 and send packet to IPV4 processor along with header length
                    if(HexStringTable[e.Packet.Data[i]][0] == '4')
                    {
                        ProcessIPV4(pd, e.Packet.Data, int.Parse(HexStringTable[e.Packet.Data[i]][1].ToString()) * 4, i);
                    }
                }

                var time = e.Packet.Timeval.Date;
                var len = e.Packet.Data.Length;
            }
            catch
            {
                CloseDevice();                
            }
        }

        private static void ProcessIPV4(PacketData pd, byte[] data, int HeaderLength, int offset)
        {
            int i = 0;
            pd.HeaderLength = HeaderLength;
            // Determine TCP, UDP, etc
            if(HexStringTable[data[23]] == "06")
            {
                ProcessTCP(pd, data, offset + 20);
            }
            else if(HexStringTable[data[23]] == "11")
            {
                ProcessUDP(pd, data, offset + 20);
            }
            //Console.WriteLine("Header Length: " + HeaderLength.ToString());
        }

        private static void ProcessUDP(PacketData pd, byte[] data, int offset)
        {
            int i = 0;
            // The first two bytes are a 16 bit signed short that is the source port
            var sourcePort = BitConverter.ToUInt16(new byte[] { data[offset + i + 1], data[offset + i] }, 0);
            // The next two bytes are a 16 bit signed shrot that is the destination port
            var destinationPort = BitConverter.ToUInt16(new byte[] { data[offset + i + 3], data[offset + i + 2] }, 0);

            var synchId = BitConverter.ToUInt32(new byte[]{data[53], data[52], data[51], data[50]}, 0);
            var timeStamp = BitConverter.ToUInt32(new byte[] { data[49], data[48], data[47], data[46] }, 0);
            i += 4;
            //Console.WriteLine("UDP | Source Port: {0}, Destination Port: {1}", sourcePort, destinationPort);

            // Check if the port is being watched for RTP data
            lock(lockObject)
            {
                if (startMarker)
                {
                    recording = true;
                    startMarker = false;
                    callDataList.ToList().ForEach(x =>
                    {
                        x.StartRecording();
                    });
                }
                else if (stopMarker)
                {
                    recording = false;
                    stopMarker = false;
                    callDataList.ToList().ForEach(x =>
                    {
                        x.StopRecording();
                    });
                }
                ProcessUDPPort(data, offset, sourcePort, synchId, pd.Timevel);
                ProcessUDPPort(data, offset, destinationPort, synchId, pd.Timevel);
                
            }            
        }

        private static void ProcessUDPPort(byte[] data, int offset, ushort sourcePort, UInt32 syncId, PosixTimeval timeStamp)
        {
            if (watchPorts.Contains(sourcePort))
            {
                if (callDataList.Where(x => x.port == sourcePort).Count() != 0)
                {
                    // RTP has no proper definition so we use heuristics to determine that it is RTP
                    // The definiton is UDP with a source port defined by the SIP/SDP: SIP/2.0 183 Session Progress packet
                    // For now we can skip the rest of the UDP header as well as the RTP header information as we only
                    // care about the payload which is 54-213
                    callDataList.Where(x => x.port == sourcePort).First().WriteData(syncId, timeStamp, data, offset + 20, data.Length - offset - 20);
                }
            }
        }

        private static void ProcessTCP(PacketData pd, byte[] data, int offset)
        {
            int i = 0;
            // The first two bytes are a 16 bit signed short that is the source port
            var sourcePort = BitConverter.ToUInt16(new byte[] { data[offset + i + 1], data[offset + i] }, 0);
            // The next two bytes are a 16 bit signed shrot that is the destination port
            var destinationPort = BitConverter.ToUInt16(new byte[] { data[offset + i + 3], data[offset + i + 2] }, 0);

            i += 4;

            if (sourcePort == 5060 || destinationPort == 5060)
            {
                // The next four bytes are the sequence number
                var sequenceNumber = BitConverter.ToUInt32(new byte[] { data[offset + i + 3], data[offset + i + 2], data[offset + i + 1], data[offset + i + 0] }, 0);
                i += 4;

                // The next four bytes are the ack number            
                var ackNumber = BitConverter.ToUInt32(new byte[] { data[offset + i + 3], data[offset + i + 2], data[offset + i + 1], data[offset + i + 0] }, 0);
                i += 4;

                // The next byte is the TCP Header length * 4
                var headerLength = TableConvert.Convert(HexStringTable[data[offset + i]]) / 4;
                //Console.WriteLine("TCP | Source Port: {0}, Destination Port: {1}, Header Length: {2}", sourcePort, destinationPort, headerLength);

                // We don't care about the rest
                ProcessTCPPayload(data, offset + headerLength);
            }
        }

        private static void ProcessTCPPayload(byte[] data, int offset)
        {
            // Read First Line, see if it signals call start
            int i = 0;
            int numLines = 0;
            int lineStart = i;
            //Example5.CallData.PacketType packatType = Example5.CallData.PacketType.Unknown;
            //Example5.CallData callData = null;
            var packetData = new NCP_CallRecorder.VOIP.ParsedPacketData();
            packetData.Fill(data, offset, HexStringTable);

            if(packetData.SipMethod == "INVITE")
            {
                // Call Starting
                if(callDataList.Where(x => x.CallId == packetData.CallID).Count() == 0)
                {
                    var newCall = new NCP_CallRecorder.CallData();
                    newCall.CallId = packetData.CallID;
                    newCall.audioData = new MemoryStream();
                    NCP_CallRecording.Logging.Writer.Write(String.Format("New Call : {0}", newCall.CallId));
                    callDataList.Add(newCall);
                }
            }
            else if (packetData.SipMethod == "BYE" || packetData.SipMethod == "CANCEL")
            {
                if(callDataList.Where(x => x.CallId == packetData.CallID).Count() > 0)
                {
                    var callData = callDataList.Where(x => x.CallId == packetData.CallID).First();
                    callData.Write();
                    if(watchPorts.Where(x => x == callData.port).Count() == 1)
                    {
                        NCP_CallRecording.Logging.Writer.Write(String.Format("Remove Port({1}) : {0}", callData.port, callData.CallId));
                        watchPorts.Remove(callData.port);
                    }
                    NCP_CallRecording.Logging.Writer.Write(String.Format("End Call : {0}", callData.CallId));
                    callDataList.Remove(callData);
                }
            }

            var foundCallData = callDataList.Where(x => x.CallId == packetData.CallID).FirstOrDefault();

            if (foundCallData != null && !String.IsNullOrEmpty(packetData.From_Number))
            {
                foundCallData.From = packetData.From_Number;
            }

            if (foundCallData != null && !String.IsNullOrEmpty(packetData.To_Number))
            {
                foundCallData.To = packetData.To_Number;
            }

            if (foundCallData != null && !String.IsNullOrEmpty(packetData.RemotePhoneNumber))
            {
                foundCallData.RemotePhone = packetData.RemotePhoneNumber;
            }



            if (!String.IsNullOrEmpty(packetData.MediaDescription) && foundCallData != null)
            {
                if (foundCallData.port == 0)
                {
                    foundCallData.port = ushort.Parse(packetData.MediaPort);
                }

                if (watchPorts.Where(x => x == foundCallData.port).Count() == 0)
                {
                    NCP_CallRecording.Logging.Writer.Write(String.Format("Add Port({1}) : {0}", foundCallData.port, foundCallData.CallId));
                    watchPorts.Add(foundCallData.port);
                }
            }
            else
            {
                

            }
            //Console.WriteLine("{0}|{1}|{2}", packetData.SIPVersion, packetData.SipMethod, packetData.SipData);
            //Console.WriteLine(Encoding.UTF8.GetString(data, offset, data.Length - offset));
        }

        private static string TryParseData(string line, string searchStart, string searchStop)
        {

            int startIndex = line.IndexOf(searchStart);
            if(startIndex == -1)
            {
                return String.Empty;
            }
            startIndex += searchStart.Length;
            int take = 0;
            for (int i = startIndex; i < line.Length; i++)
            {
                take++;
                if (i+searchStop.Length < line.Length && PullStringFromArray(line, i+1, searchStop.Length) == searchStop)
                {
                    break;
                }                
            }
            if(take == 0)
            {
                return String.Empty;
            }
            else
            {
                return PullStringFromArray(line, startIndex, take);
            }            
        }

        private static string PullStringFromArray(string line, int startindex, int length)
        {
            char[] charArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                charArray[i] = line[startindex+i];
            }
            return new String(charArray);
        }

        private static readonly string[] HexStringTable = new string[]
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
            "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
            "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
            "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
        };

        internal static ServiceHost serviceHost { get; set; }

        internal static IPC.WCFServer wcfServer { get; set; }

        internal static bool ClientConnected { get; set; }

        internal static void ConnectClient(string Pipe)
        {
            ClientConnected = true;
            //try
            //{
            //    NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            //    EndpointAddress ep = new EndpointAddress(Pipe);
            //    callbackChannel = ChannelFactory<IPC.WCFCallbackInterface>.CreateChannel(binding, ep);
                
            //}
            //catch(Exception e)
            //{
            //    NCP_CallRecording.Logging.Writer.Write(e.Message);
            //}            
        }

        internal static IPC.WCFCallbackInterface callbackChannel { get; set; }

        internal static IPC.CallStatus LastReportedCallStatus { get; set; }

        internal static bool RequiresRestart { get; set; }

        internal static ICaptureDevice device { get; set; }

        internal static void Break()
        {
            lock(lockObject)
            {
                BreakIt = true;
            }
        }

        internal static bool BreakIt { get; set; }

        internal static void AddCallData(IPC.CallData callData)
        {
            ipcCallDataList.Add(callData);
        }
    }

    internal class TableConvert
    {
        static sbyte[] unhex_table =
        { 
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
            ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
            ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
            , 0, 1, 2, 3, 4, 5, 6, 7, 8, 9,-1,-1,-1,-1,-1,-1
            ,-1,10,11,12,13,14,15,-1,-1,-1,-1,-1,-1,-1,-1,-1
            ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
            ,-1,10,11,12,13,14,15,-1,-1,-1,-1,-1,-1,-1,-1,-1
            ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
        };

        public static int Convert(string hexNumber)
        {
            int decValue = unhex_table[(byte)hexNumber[0]];
            for (int i = 1; i < hexNumber.Length; i++)
            {
                decValue *= 16;
                decValue += unhex_table[(byte)hexNumber[i]];
            }
            return decValue;
        }
    }

    internal class PacketData
    {
        public string ToMac { get; set; }
        public string FromMax { get; set; }


        public int HeaderLength { get; set; }

        public PosixTimeval Timevel { get; set; }
    }
}
