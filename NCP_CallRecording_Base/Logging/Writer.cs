using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NCP_CallRecording.Logging
{
    public class Writer
    {
        private static StreamWriter streamWriter;
        private static String LogLoc = (Path.Combine(Path.Combine(Configuration.Settings.ROOT_FILE_FOLDER, Environment.MachineName), "log"));
        private static DateTime StartDate;
        private static object lockObj = new object();
        public static void SetUp()
        {
            try
            {
                StartDate = DateTime.Now;
                if (File.Exists(Path.Combine(LogLoc,"CRLog.txt")))
                {
                    var lfiles = Directory.GetFiles(LogLoc, "CRLog.txt.*", SearchOption.TopDirectoryOnly);
                    foreach (var file in lfiles.ToList())
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.Name == "CRLog.txt")
                            continue;
                        try
                        {
                            var num = int.Parse(fi.Name.Replace("CRLog.txt.", ""));
                            num++;
                            if (num == 10)
                            {
                                fi.Delete();
                            }
                            else
                            {
                                fi.MoveTo(Path.Combine(fi.Directory.FullName, String.Format("CRLog.txt.{0}", num)));
                            }
                        }
                        catch
                        {
                            fi.Delete();
                        }
                    }
                    File.Move(Path.Combine(LogLoc, "CRLog.txt"), Path.Combine(LogLoc, "CRLog.txt.0"));
                }
            }
            finally
            {
                var fs = File.Create(Path.Combine(LogLoc, "CRLog.txt"));
                fs.Close();
            }
            //streamWriter = new StreamWriter(Path.Combine(@"C:\NCP\logs", "CRLog.txt"));
        }

        public static void Write(String Message)
        {
            lock (lockObj)
            {
                if (DateTime.Now.Date > StartDate.Date)
                {
                    SetUp();
                }
                using (streamWriter = new StreamWriter(Path.Combine(LogLoc, "CRLog.txt"), true))
                {   
                    streamWriter.WriteLine(Message);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }                        
        }
    }
}
