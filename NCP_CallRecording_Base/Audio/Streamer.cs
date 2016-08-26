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
        internal static System.IO.MemoryStream OpenStream(string OpusFile)
        {
            var OpusDec = Path.Combine(Path.GetTempPath(), "opusdec.exe");
            Process decPro = new Process();
            decPro.StartInfo = new ProcessStartInfo(OpusDec, String.Format("{0} {1}", OpusFile, OpusFile.Replace(".opus",".wav")));
            decPro.Start();
            decPro.WaitForExit();
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
    }
}
