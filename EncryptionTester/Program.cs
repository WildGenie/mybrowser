using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //NCP_CallRecording.Crypto.Manager.Encrypt(@"C:\CallRecording\CallRecording_1_From_121_To_9_RemotePhone_4192301197_20160811162141_.opus");
            NCP_CallRecording.Crypto.Manager.Decrypt(@"C:\CallRecording\CallRecording_1_From_121_To_9_RemotePhone_4192301197_20160811162141_.opus.pgp");
        }
    }
}
