using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InstallerFileTagGenerator
{
    class Program
    {
        static StreamWriter sw;
        static void Main(string[] args)
        {
            if(File.Exists("WixFiles.txt"))
            {
                File.Delete("WixFiles.txt");
            }
            sw = new StreamWriter("WixFiles.txt");
            EnumerateDiwectory(@"\\schear-fs\it\_CODE\NCP Browser\NCP Browser V2\bin\x86\Release");

            WriteFile();
        }

        private static void EnumerateDiwectory(string p)
        {
            string curD = "";
            Directory.EnumerateFiles(p).ToList().OrderBy(x => new FileInfo(x).DirectoryName).ToList().ForEach(x =>
            {
                FileInfo f = new FileInfo(x);
                if(curD != f.DirectoryName)
                {
                    sw.WriteLine(f.DirectoryName);
                    curD = f.DirectoryName;
                }
                sw.WriteLine(String.Format("<File Id=\"{0}\" Name=\"{3}\" Source=\"{1}\" KeyPath=\"{2}\" Vital=\"yes\" />", f.Name.Replace('-','_').Replace(' ','_'), f.FullName, f.Name == "NCP_Browser_V2.exe" ? "yes" : "no", f.Name));
            });
            Directory.EnumerateDirectories(p).ToList().ForEach(x =>
            {
                EnumerateDiwectory(x);
            });
        }

        private static void WriteFile()
        {
            sw.Flush();
            sw.Close();
        }
    }
}
