using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NCP_CallRecording.Audio
{
    class Streamer
    {
        public Streamer()
        {
            processOutput = new StringBuilder();
            objectLock = new object();
        }
        StringBuilder processOutput;
        object objectLock;
        internal static System.IO.MemoryStream OpenStream(string OpusFile)
        {
            Streamer s = new Streamer();
            var OpusDec = Path.Combine(Path.GetTempPath(), "opusdec.exe");
            Process decPro = new Process();
            decPro.StartInfo = new ProcessStartInfo(OpusDec, String.Format("\"{0}\" \"{1}\"", OpusFile, OpusFile.Replace(".opus",".wav")));
            decPro.StartInfo.UseShellExecute = false;
            decPro.StartInfo.RedirectStandardError = true;
            decPro.StartInfo.RedirectStandardOutput = true;
            decPro.ErrorDataReceived += s.decPro_ErrorDataReceived;
            decPro.OutputDataReceived += s.decPro_OutputDataReceived;
            decPro.Start();
            decPro.PriorityClass = ProcessPriorityClass.High;
            decPro.BeginErrorReadLine();
            decPro.BeginOutputReadLine();
            
            try
            { 
                decPro.WaitForExit(120000);
            }
            catch
            {

            }
            if(!decPro.HasExited)
            {
                decPro.Kill();
            }

            lock(s.objectLock)
            {
                Logging.Writer.Write(s.processOutput.ToString());
            }
            
            var bytes = File.ReadAllBytes(OpusFile.Replace(".opus",".wav"));
            MemoryStream ms = new MemoryStream(bytes);

            // TODO: Dispose of files
            try
            {
                File.Delete(OpusFile);
            }
            catch
            {

            }

            try
            {
                File.Delete(OpusFile.Replace(".opus", ".wav"));
            }
            catch
            {

            }

            if(File.Exists(OpusFile + ".pgp"))
            {
                try
                {
                    File.Delete(OpusFile + ".pgp");
                }
                catch
                {

                }
            }
            return ms;
        }

        void decPro_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data != null)
            {
                lock (this.objectLock)
                {
                    processOutput.Append(e.Data);
                }
            }
        }

        void decPro_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data != null)
            {
                lock(this.objectLock)
                {
                    processOutput.Append(e.Data);
                }
            }
        }
    }
}
