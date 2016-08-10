using NCP_CallRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCP_CallRecorder.VOIP
{

    internal class ParsedPacketData
    {
        public string StatusLine { get; set; }
        public string SIPVersion
        {
            get
            {
                if (!String.IsNullOrEmpty(this.StatusLine))
                {
                    return this.StatusLine.Substring(this.StatusLine.IndexOf("SIP/", 0) + 4, 3);
                }
                else
                {
                    return String.Empty;
                }                
            }            
        }

        public string SipMethod
        {
            get
            {
                if (!String.IsNullOrEmpty(this.StatusLine))
                {
                    return _SipNoVersion.Substring(0, _SipNoVersion.IndexOf(" ")).Trim();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        private string _SipNoVersion
        {
            get
            {
                if (!String.IsNullOrEmpty(this.StatusLine))
                {
                    return this.StatusLine.Replace("SIP/" + this.SIPVersion, "").Trim();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string SipData
        {
            get
            {
                if (!String.IsNullOrEmpty(this.StatusLine))
                {
                    return _SipNoVersion.Replace(SipMethod, "").Trim();
                }
                else
                {
                    return String.Empty;
                }                
            }
        }



        public string SipVersion { get; set; }
        public string Via_Full = null;
        public string Via_Address { get; set; }
        public string Via_Branch { get; set; }
        public string Vial_Max_Forwards { get; set; }
        public string CallID { get; set; }
        public CallData.PacketType packetType { get; set; }
        public CallData callData { get; set; }

        private int numLines;
        
        private string From;
        public string  From_Number
        {
            get
            {
                if(!String.IsNullOrEmpty(From) && From.Contains("@"))
                {
                    return From.Substring(From.IndexOf("<sip:") + 5, From.IndexOf("@")-(From.IndexOf("<sip:") + 5));
                }
                else
                {
                    return String.Empty;
                }                
            }
        }

        private string To;
        public string To_Number
        {
            get
            {
                if (!String.IsNullOrEmpty(To) && To.Contains("@"))
                {
                    return To.Substring(To.IndexOf("<sip:") + 5, To.IndexOf("@") - (To.IndexOf("<sip:") + 5));
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        private string MaxForwards;
        private string Date;
        private string CSEQ;
        private string UserAgent;
        private string ContentType;
        private string CallInfo;
        public string MediaDescription;
        public string MediaPort
        {
            get
            {
                if(!String.IsNullOrEmpty(MediaDescription))
                {
                    return MediaDescription.Substring(MediaDescription.IndexOf("audio") + 6, MediaDescription.IndexOf("RTP") - 7);
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        private string RemotePartyID;
        private string _RemotePartyID;
        public string RemotePhoneNumber
        {
            get
            {
                return ExtractRemotePhoneNumber(RemotePartyID);                
            }
        }

        private string ExtractRemotePhoneNumber(string remotePartyID)
        {
            if (!String.IsNullOrEmpty(remotePartyID) && remotePartyID.Contains("@"))
            {
                var fullPhone = remotePartyID.Substring(remotePartyID.IndexOf("<sip:") + 5, remotePartyID.IndexOf("@") - (remotePartyID.IndexOf("<sip:") + 5));
                if (fullPhone.Length > 10)
                {
                    return fullPhone.Substring(fullPhone.Length - 10, 10);
                }
                else
                {
                    return fullPhone;
                }
            }
            else
            {
                return String.Empty;
            }
        }

        // The only thing this should do is pull the fields from the SIP/SDP packets. This should not have the logic to start/stop recording
        internal void Fill(byte[] data, int offset, string[] HexStringTable)
        {
            numLines = 0;
            int i = 0;
            int lineStart = 0;
            //string sessionReceived = String.Empty;
            while(i < data.Length-offset)
            {
                if(HexStringTable[data[offset+i]] == "0A" && HexStringTable[data[offset+i-1]] == "0D")
                {
                    numLines++;
                    string line = Encoding.UTF8.GetString(data, lineStart + offset, i - lineStart-1);
                    //callData.AddLine(line);
                    if(numLines == 1)
                    {
                        this.StatusLine = line;
                        // TODO: Special Handling
                        ParseStatusLine(line);
                    }
                    else
                    {
                        PopulateHeaderInformation(line);

                        if(!String.IsNullOrEmpty(this.ContentType) && this.ContentType == "application/sdp")
                        {
                            PopulateSDPData(line);
                        }
                    }

                    /*
                    // Get Call Starting Call Id, add the call to pick up the RTP packets
                    // Call Start Media Port
                    else if (packetType == CallData.PacketType.SessionStart)
                    {
                        HandleCallStart(packetType, callData, sessionReceived, line);
                        if (line.Contains("Remote-Party-ID:") && line.Contains("party=called"))
                        {
                            callData.PhoneNumber = TryParseData(line, "<sip:", "@");
                            callData.CallerId = TryParseData(line, "Remote-Party-ID: \"", "\" <");
                        }
                    }
                    else if(packetType == CallData.PacketType.SessionsReceived)
                    {
                        HandleCallStart(packetType, callData, sessionReceived, line);
                        if(line.Contains("From:"))
                        {
                            callData.PhoneNumber = TryParseData(line, "<sip:", "@");
                            callData.CallerId = TryParseData(line, "From: \"", "\" <");
                        }
                    }
                    else if (packetType == CallData.PacketType.SessionsReceived)
                    {
                        HandleCallStart(packetType, callData, sessionReceived, line);
                        if (line.Contains("To:"))
                        {
                            callData.PhoneNumber = TryParseData(line, "<sip:", "@");
                            callData.CallerId = TryParseData(line, "To: \"", "\" <");
                        }
                    }
                    // TODO: On external call, the phone number retrieved is the forwarding extension
                    // Call Cancel Call Id, No Write remove from list
                    else if (packetType == CallData.PacketType.SessionCancel)
                    {
                        if (line.Contains("Call-ID:"))
                        {
                            string callId = line.Replace("Call-ID:", "").Trim();
                            callData = callDataList.Where(x => x.CallId == callId).FirstOrDefault();
                            if (callData != null)
                            {
                                callDataList.Remove(callData);
                                if (watchPorts.Contains(callData.port))
                                {
                                    watchPorts.Remove(callData.port);
                                }
                                Console.WriteLine("Call Cancel:{0}", callId);

                            }                            
                        }
                    }
                    // Call Ending Call Id, Write the call to disk and remove from list
                    else if (packetType == CallData.PacketType.SessionEnd)
                    {
                        if(line.Contains("Call-ID:"))
                        {
                            string callId = line.Replace("Call-ID:", "").Trim();
                            callData = callDataList.Where(x => x.CallId == callId).FirstOrDefault();
                            if(callData != null)
                            {
                                callData.Write();
                                callDataList.Remove(callData);
                                if (watchPorts.Contains(callData.port))
                                {
                                    watchPorts.Remove(callData.port);
                                }
                                Console.WriteLine("Call End:{0}", callId);
                            }                            
                        }
                    }*/
                    /*
                    else if(line.Contains("m=audio"))
                    {
                        UInt16 port = (UInt16)int.Parse(line.Substring(7, line.IndexOf("RTP") - 7));
                        if (start && !watchPorts.Contains(port))
                        {
                            if(callDataList.Where(x => x.port == port).Count() != 0)
                            {
                                callDataList.Where(x => x.port == port).First().Write();
                                callDataList.Remove(callDataList.Where(x => x.port == port).First());
                            }

                            CallData cd = new CallData();
                            cd.port = port;
                            cd.audioData = new MemoryStream();
                            callDataList.Add(cd);

                            watchPorts.Add(port);
                            Console.WriteLine("Media Port Added: " + port);
                        }
                        else if (watchPorts.Contains(port))
                        {
                            if (callDataList.Where(x => x.port == port).Count() != 0)
                            {
                                callDataList.Where(x => x.port == port).First().Write();
                                callDataList.Remove(callDataList.Where(x => x.port == port).First());
                            }
                            watchPorts.Remove(port);
                            Console.WriteLine("Media Port Removed: " + port);
                        }
                        
                    }*/
                    lineStart = i + 1;
                }
                i++;
            }
        }

        private void ParseStatusLine(string line)
        {
            // The catch all method to to consider a session open until we see a cancel/bye associated with the call-id
            // Call starting, We initiated
            //if (line.Contains("SIP/2.0 183 Session Progress"))
            //{
            //    packetType = CallData.PacketType.SessionOpen;
            //    callData = new CallData();
            //    Console.WriteLine(line);
            //}
            //// Call Starting, We recieved
            //else if (line.Contains("ACK sip:"))
            //{
            //    packetType = CallData.PacketType.SessionsReceived;
            //    //sessionReceived = line;
            //    callData = new CallData();
            //}
            //// Call Starting, We sent internal
            //else if (line.Contains("SIP/2.0 200 OK"))
            //{
            //    packetType = CallData.PacketType.SessionStart;
            //    //sessionReceived = line;
            //    callData = new CallData();
            //}
            // Call Cencel
            if (line.Contains("CANCEL sip:"))
            {
                packetType = CallData.PacketType.SessionCancel;
            }
            // Call Ending
            else if (line.Contains("BYE sip:"))
            {
                packetType = CallData.PacketType.SessionEnd;
            }
            else
            {
                packetType = CallData.PacketType.Unknown;
            }

        }

        private void PopulateHeaderInformation(string line)
        {
            // VIA
            this.Via_Full = TryExtractDataFromLine(line, "Via:", this.Via_Full);

            // FROM
            this.From = TryExtractDataFromLine(line, "From:", this.From);

            // TO
            this.To = TryExtractDataFromLine(line, "To:", this.To);

            // CALL-ID
            this.CallID = TryExtractDataFromLine(line, "Call-ID:", this.CallID);

            // MAX-FORWARDS
            this.MaxForwards = TryExtractDataFromLine(line, "Max-Forwards:", this.MaxForwards);

            // DATE
            this.Date = TryExtractDataFromLine(line, "Date:", this.Date);

            // CSEQ
            this.CSEQ = TryExtractDataFromLine(line, "CSeq:", this.CSEQ);

            // USER-AGENT
            this.UserAgent = TryExtractDataFromLine(line, "User-Agent:", this.UserAgent);

            // Content Type, if this is set to application/sdp then we have a SIP/SDP packet
            this.ContentType = TryExtractDataFromLine(line, "Content-Type:", this.ContentType);

            this.CallInfo = TryExtractDataFromLine(line, "Call-Info:", this.CallInfo);

            this._RemotePartyID = TryExtractDataFromLine(line, "Remote-Party-ID:", this._RemotePartyID);
            CheckSetRemotePartyID();
        }

        private void CheckSetRemotePartyID()
        {
            if(ExtractRemotePhoneNumber(this._RemotePartyID) != this.From_Number && ExtractRemotePhoneNumber(this._RemotePartyID) != this.To_Number)
            {
                this.RemotePartyID = this._RemotePartyID;
            }
        }

        private void PopulateSDPData(string line)
        {
            this.MediaDescription = TryExtractDataFromLine(line, "m=", this.MediaDescription);
        }

        private String TryExtractDataFromLine(string line, string Prefix, string CurrentValue)
        {
            if(line.Contains(Prefix))
            {
                return line.Replace(Prefix, "").Trim();
            }
            else
            {
                return CurrentValue;
            }
        }
    }
}
