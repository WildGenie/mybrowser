using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            NCP_CallRecording.Crypto.Manager.Encrypt(@"\\schear-fs\SecureShare$\CallRecording\key\APICredentials.txt");
            
            
            /*MemoryStream ms = new MemoryStream();
            NCP_CallRecording.Crypto.Manager.Decrypt(@"\\schear-fs\SecureShare$\CallRecording\key\APICredentials.txt.pgp", ms);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            string line = null;
            string endpoint = null;
            string username = null;
            string password = null;
            string token = null;
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
            }*/

            

            Console.ReadKey();
        }
    }
}
