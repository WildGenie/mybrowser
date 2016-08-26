using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.Internals
{
    public class CallData
    {
        public DateTime DateAdded { get; set; }
        public NCP_CallRecorder.IPC.CallData IPC_CallData { get; set; }
        public string PhoneNumber { get; set; }

        public bool Remove { get; set; }
    }
}
