using SharpPcap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NCP_CallRecorder
{
    internal class CallPort
    {
        public MemoryStream audioData;
        public UInt32 syncId;
        public PosixTimeval timeStamp;

        public int offset { get; set; }
        public long maxi
        {
            get
            {
                return audioData.Length + offset - 1;
            }
        }
    }

    internal class CallRecording
    {
        public int start { get; set; }
        public int stop { get; set; }
    }

    internal class CallData
    {
        public MemoryStream audioData;
        public UInt16 port;
        public UInt16 numStreams = 0;
        public List<String> LineList;

        private List<CallPort> callPorts;
        public string CallId;

        public string PhoneNumber { get; set; }
        public string CallerId { get; set; }

        public List<CallRecording> CallRecordings { get; set; }
        public CallRecording CurrentCallRecording { get; set; }

        private const decimal MICRO_WEIGHT = (decimal)SECONDS / (decimal)MICROSECONDS;
        private const decimal SAMPLE_WEIGHT = (decimal)SECONDS / (decimal)BYTE_RATE;
        private const int BITS_PER_SAMPLE = 16;
        private const int CHANNELS = 1;
        private const int SAMPLE_RATE = 8000;
        private const int BYTE_LENGTH = 8;
        private const int BYTE_RATE = (int)((decimal)SAMPLE_RATE * (decimal)CHANNELS * (decimal)BITS_PER_SAMPLE / (decimal)BYTE_LENGTH);
        private const int MICROSECONDS = 1000000;
        private const int SECONDS = 60;
        private const decimal CONVERSION = (decimal)MICRO_WEIGHT / (decimal)SAMPLE_WEIGHT;
        private int totalBytes;

        public CallData()
        {
            this.callPorts = new List<CallPort>();
            this.LineList = new List<string>();
            this.CallRecordings = new List<CallRecording>();
        }

        private void AddPort(UInt32 SynchronizationId, PosixTimeval TimeStamp)
        {
            CallPort p = new CallPort();
            p.syncId = SynchronizationId;
            p.timeStamp = TimeStamp;
            numStreams++;
            p.audioData = new MemoryStream();
            this.callPorts.Add(p);
        }

        internal void Write()
        {
            if (audioData == null || callPorts.Select(x => x.timeStamp.MicroSeconds + x.timeStamp.Seconds * MICROSECONDS).Count() == 0)
            {
                return;
            }
            /* We need to determine the stream offset
            *  We need to determine the total length of the audio data int he event the streams are like this:
            *  ----------------------------------------------
            *      ***********************************************
            */

            // First Determine the first timestamp
            var minMicro = GetMinimumMicroseconds();
            long maxIndex = 0;
            long maxOffset = 0;
            int numStreams = callPorts.Count;
            byte[][] byteStreams = new byte[numStreams][];
            // Set the offsets
            for (int i = 0; i < callPorts.Count; i++)
            {
                if(callPorts[i].timeStamp.Seconds*MICROSECONDS + callPorts[i].timeStamp.MicroSeconds == (ulong)minMicro)
                {
                    callPorts[i].offset = 0;
                }
                else if (callPorts[i].timeStamp.Seconds * MICROSECONDS + callPorts[i].timeStamp.MicroSeconds > (ulong)minMicro)
                {
                    callPorts[i].offset = (int)GetOffset(minMicro, callPorts[i]);
                }
                // Determine Max offset
                if(callPorts[i].offset > maxOffset)
                {
                    maxOffset = callPorts[i].offset;
                }

                if(maxIndex + (long)callPorts[i].offset + callPorts[i].audioData.Length - 1 > maxIndex)
                {
                    maxIndex = (long)callPorts[i].offset + callPorts[i].audioData.Length - 1;
                }
                byteStreams[i] = callPorts[i].audioData.GetBuffer();
            }

            byte[] bs = new byte[(maxIndex + 1) * 2];
            
            // i is the index of the combined byte array
            for (int i = 0; i <= maxIndex; i++)
            {
                int x = 0;
                int divisor = 0;

                Int16 sample1 = 0;
                // IF index of main array is between
                if (i >= callPorts[0].offset && i <= callPorts[0].maxi)
                {
                    byte[] sample = new byte[2];
                    int iofs = i-callPorts[0].offset;
                    sample[0] = (byte)(Decoder.muLawToPcmMap[byteStreams[0][iofs]] & 0xff);
                    sample[1] = (byte)(Decoder.muLawToPcmMap[byteStreams[0][iofs]] >> 8);
                    sample1 = BitConverter.ToInt16(sample, 0);
                    if(sample1 != 0)
                        divisor++;
                }

                Int16 sample2 = 0;
                if (i >= callPorts[1].offset && i <= callPorts[1].maxi)
                {
                    byte[] sample = new byte[2];
                    int iofs = i - callPorts[1].offset;
                    sample[0] = (byte)(Decoder.muLawToPcmMap[byteStreams[1][iofs]] & 0xff);
                    sample[1] = (byte)(Decoder.muLawToPcmMap[byteStreams[1][iofs]] >> 8);
                    sample2 = BitConverter.ToInt16(sample, 0);
                    if(sample2 != 0)
                        divisor++;
                }

                Int32 output = sample1+sample2;
                if(output > Int16.MaxValue)
                {
                    output = Int16.MaxValue;
                }
                else if (output < Int16.MinValue)
                {
                    output = Int16.MinValue;
                }

                //byte[] sample = new byte[2];
                //for(int j = 0; j < numStreams; j++)
                //{
                //    if(callPorts[j].offset >= i && callPorts[j].audioData.Length-1 >= i)
                //    {
                //        x += Decoder.muLawToPcmMap[byteStreams[j][i]];
                //        divisor++;
                //    }
                    
                //}
                if(divisor != 0)
                {
                    bs[2 * i] = (byte)((short)(output) & 0xff);
                    bs[2 * i + 1] = (byte)((short)(output) >> 8);
                }
            }

            List<String> WavFilesToConvert = new List<string>();
                
            // TODO, need to linke the BYE to the call id, party id for the target of call
            var fullCall = @"C:\CallRecording\From_" + From + "_To_" + To + "_RemotePhone_" + RemotePhone + String.Format("_{0:yyyyMMddHHmmss}_.wav", DateTime.Now);
            FileStream fs = new FileStream(fullCall, FileMode.Create);
            WavFilesToConvert.Add(fullCall);
            //byte[] bs = new byte[audioData.Length * 2];
            //var audioBuffer = audioData.GetBuffer();
            /*for (int i = 0; i < audioData.Length; i++)
            {
                short x = Decoder.muLawToPcmMap[audioBuffer[i]];
                bs[2 * i] = (byte)(Decoder.muLawToPcmMap[audioBuffer[i]] & 0xff);
                bs[2 * i + 1] = (byte)(Decoder.muLawToPcmMap[audioBuffer[i]] >> 8);
            }*/
            // We need to write the WAV header... it starts with a RIFF
            byte[] header = new byte[44];
            MakeWaveHeader(bs, header);

            fs.Write(header, 0, header.Length);
            fs.Write(bs, 0, bs.Length);
            fs.Flush();
            fs.Close();

            int CallRecordingNumber = 0;
            foreach(var callRecording in CallRecordings)
            {
                CallRecordingNumber++;
                if(callRecording.stop == 0)
                {
                    callRecording.stop = bs.Length / 2 - 1;
                }
                byte[] crbs = new byte[(callRecording.stop - callRecording.start)*2];
                for (int copyPCMIndex = 0; copyPCMIndex < crbs.Length; copyPCMIndex++ )
                {
                    crbs[copyPCMIndex] = bs[copyPCMIndex + (callRecording.start*2)];
                }

                // TODO, need to linke the BYE to the call id, party id for the target of call
                var callSlice = @"C:\CallRecording\CallRecording_"+CallRecordingNumber.ToString()+"_From_" + From + "_To_" + To + "_RemotePhone_" + RemotePhone + String.Format("_{0:yyyyMMddHHmmss}_.wav",DateTime.Now);
                fs = new FileStream(callSlice, FileMode.Create);
                WavFilesToConvert.Add(callSlice);
                //byte[] bs = new byte[audioData.Length * 2];
                //var audioBuffer = audioData.GetBuffer();
                /*for (int i = 0; i < audioData.Length; i++)
                {
                    short x = Decoder.muLawToPcmMap[audioBuffer[i]];
                    bs[2 * i] = (byte)(Decoder.muLawToPcmMap[audioBuffer[i]] & 0xff);
                    bs[2 * i + 1] = (byte)(Decoder.muLawToPcmMap[audioBuffer[i]] >> 8);
                }*/
                // We need to write the WAV header... it starts with a RIFF
                header = new byte[44];
                MakeWaveHeader(crbs, header);

                fs.Write(header, 0, header.Length);
                fs.Write(crbs, 0, crbs.Length);
                fs.Flush();
                fs.Close();
                
            }
            IPC.CallData callData = new IPC.CallData();
            callData.OpusFiles = new List<string>();
            foreach (var wavFile in WavFilesToConvert)
            {
                Process opusEnc = new Process();
                opusEnc.StartInfo = new ProcessStartInfo(Path.Combine(Path.GetTempPath(), "opusenc.exe"), String.Format("--vbr \"{0}\" \"{1}\"", wavFile, wavFile.Replace("wav", "opus")));
                opusEnc.StartInfo.UseShellExecute = false;
                opusEnc.StartInfo.RedirectStandardOutput = true;
                opusEnc.Start();
                opusEnc.WaitForExit();
                callData.OpusFiles.Add(wavFile.Replace("wav", "opus"));
                Console.WriteLine(opusEnc.StandardOutput.ReadToEnd());
            }
            try
            {
                NCP_CallRecorder.RecordingEngine.callDataNumber++;
                callData.Number = NCP_CallRecorder.RecordingEngine.callDataNumber;
                NCP_CallRecorder.RecordingEngine.AddCallData(callData);

                //if (NCP_CallRecorder.RecordingEngine.ClientConnected)
                //{
                //    NCP_CallRecorder.RecordingEngine.callbackChannel.ForwardCallData(callData);
                //}
            }
            catch
            {

            }
        }

        private ulong? GetMinimumMicroseconds()
        {
            var minMicro = callPorts.Select(x => x.timeStamp.MicroSeconds + x.timeStamp.Seconds * MICROSECONDS).Min();
            return minMicro;
        }

        private ulong GetOffset(ulong? minMicro, CallPort callPort)
        {
            return (ulong)decimal.Round((CONVERSION * (callPort.timeStamp.Seconds * MICROSECONDS + callPort.timeStamp.MicroSeconds - (ulong)minMicro)), MidpointRounding.AwayFromZero)-1;
        }

        private static void MakeWaveHeader(byte[] bs, byte[] header)
        {
            byte[] ChunkId = Encoding.ASCII.GetBytes("RIFF");
            header[0] = ChunkId[0];
            header[1] = ChunkId[1];
            header[2] = ChunkId[2];
            header[3] = ChunkId[3];

            // Little Endian Total File Size - the Four Bytes for the ChunkId and ChunkSize
            byte[] ChunkSize = BitConverter.GetBytes((Int32)bs.Length + 36);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(ChunkSize);
            }
            header[4] = ChunkSize[0];
            header[5] = ChunkSize[1];
            header[6] = ChunkSize[2];
            header[7] = ChunkSize[3];

            // Format: WAVE
            byte[] Format = Encoding.ASCII.GetBytes("WAVE");
            header[8] = Format[0];
            header[9] = Format[1];
            header[10] = Format[2];
            header[11] = Format[3];

            // Subchunk1ID
            byte[] Subchunk1ID = Encoding.ASCII.GetBytes("fmt ");
            header[12] = Subchunk1ID[0];
            header[13] = Subchunk1ID[1];
            header[14] = Subchunk1ID[2];
            header[15] = Subchunk1ID[3];

            // 4byte Little Endian SubChunkSize - 16 for PCM
            byte[] SubChunkSize = BitConverter.GetBytes((Int32)16);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(SubChunkSize);
            }
            header[16] = SubChunkSize[0];
            header[17] = SubChunkSize[1];
            header[18] = SubChunkSize[2];
            header[19] = SubChunkSize[3];

            // 2byte Little Endian AudioFormat - 1 for PCM
            byte[] AudioFormat = BitConverter.GetBytes((Int16)1);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(AudioFormat);
            }
            header[20] = AudioFormat[0];
            header[21] = AudioFormat[1];

            // 2byte Little Endian NumChannels - 1 for G.711 u-law on our cisco system
            byte[] NumChannels = BitConverter.GetBytes((Int16)1);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(NumChannels);
            }
            header[22] = NumChannels[0];
            header[23] = NumChannels[1];

            // 4byte Little Endian SampleRate - 8000 for G.711 u-law on our cisco system
            byte[] SampleRate = BitConverter.GetBytes((Int32)8000);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(SampleRate);
            }
            header[24] = SampleRate[0];
            header[25] = SampleRate[1];
            header[26] = SampleRate[2];
            header[27] = SampleRate[3];

            // 4byte Little Endian ByteRate - 8000 * 1 * 16 / 8 for G.711 u-law on our cisco system
            byte[] ByteRate = BitConverter.GetBytes((Int32)(8000 * 1 * 16 / 8));
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(SampleRate);
            }
            header[28] = ByteRate[0];
            header[29] = ByteRate[1];
            header[30] = ByteRate[2];
            header[31] = ByteRate[3];

            // 2byte Little Endian BlockAlign - 1 * 16 / 8 for G.711 u-law on our cisco system
            byte[] BlockAlign = BitConverter.GetBytes((Int16)(1 * 16 / 8));
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(SampleRate);
            }
            header[32] = BlockAlign[0];
            header[33] = BlockAlign[1];

            // 2byte Little Endian BitsPerSample - 1 * 16 / 8 for G.711 u-law on our cisco system
            byte[] BitsPerSample = BitConverter.GetBytes((Int16)(16));
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(SampleRate);
            }
            header[34] = BitsPerSample[0];
            header[35] = BitsPerSample[1];

            // Subchunk2ID
            byte[] Subchunk2ID = Encoding.ASCII.GetBytes("data");
            header[36] = Subchunk2ID[0];
            header[37] = Subchunk2ID[1];
            header[38] = Subchunk2ID[2];
            header[39] = Subchunk2ID[3];

            // 4byte Little Endian SubChunk2Size - 16 for PCM
            byte[] SubChunk2Size = BitConverter.GetBytes((Int32)bs.Length * 2);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(SubChunk2Size);
            }
            header[40] = SubChunk2Size[0];
            header[41] = SubChunk2Size[1];
            header[42] = SubChunk2Size[2];
            header[43] = SubChunk2Size[3];
        }

        public enum PacketType
        {
            Unknown = 0,
            SessionEnd = 4,
            SessionCancel = 8
        }



        internal void WriteData(uint syncId, PosixTimeval timeStamp, byte[] data, int p1, int p2)
        {
            if (callPorts.Where(x => x.syncId == syncId).Count() == 0)
            {
                this.AddPort(syncId, timeStamp);
            }
            callPorts.Where(x => x.syncId == syncId).First().audioData.Write(data, p1, p2);
            totalBytes += p2;
            if(ExpirationDate.HasValue)
            {
                ExpirationDate = null;
            }
        }

        public string StatusLine { get; set; }

        internal void AddLine(string line)
        {
            LineList.Add(line);
        }

        public string From { get; set; }

        public string RemotePhone { get; set; }

        public string To { get; set; }

        internal void StartRecording()
        {
            CurrentCallRecording = new CallRecording();
            this.CallRecordings.Add(CurrentCallRecording);

            CurrentCallRecording.start = (int)GetCurrentTotalBytes();
        }

        private long GetCurrentTotalBytes()
        {
            var minMicro = GetMinimumMicroseconds();
            long maxIndex = 0;
            foreach (var aStream in this.callPorts)
            {

                var cOffset = (int)GetOffset(minMicro, aStream);
                if (cOffset + aStream.audioData.Length - 1 > maxIndex)
                {
                    maxIndex = cOffset + aStream.audioData.Length - 1;
                }
            }
            return maxIndex+1;
        }

        internal void StopRecording()
        {
            this.CurrentCallRecording.stop = (int)GetCurrentTotalBytes();
        }

        public DateTime? ExpirationDate { get; set; }
    }
}
