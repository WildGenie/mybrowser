using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NCP_CallRecording.Logging
{
    class Writer
    {
        private static StreamWriter streamWriter;
        public static void SetUp()
        {
            /*try
            {
                if(!Directory.Exists(@"C:\NCP\logs"))
                {
                    Directory.CreateDirectory(@"C:\NCP\logs");
                }
                if (File.Exists(@"C:\NCP\logs\CRLog.txt"))
                {
                    var lfiles = Directory.GetFiles(@"C:\NCP\logs", "CRLog.txt.*", SearchOption.TopDirectoryOnly);
                    foreach (var file in lfiles.ToList())
                    {
                        FileInfo fi = new FileInfo(file);
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
                    File.Move(Path.Combine(@"C:\NCP\logs", "CRLog.txt"), Path.Combine(@"C:\NCP\logs", "CRLog.txt.0"));
                }
            }
            finally
            {
                streamWriter = new StreamWriter(Path.Combine(@"C:\NCP\logs", "CRLog.txt"));
            }*/
            //streamWriter = new StreamWriter(Path.Combine(@"C:\NCP\logs", "CRLog.txt"));
        }

        public static void Write(String Message)
        {
            //streamWriter.WriteLine(Message);
            //streamWriter.Flush();
        }
    }
}
