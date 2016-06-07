using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.chrome.runtime
{
    public class FrameLock
    {
        public object Lock { get; set; }
        public bool IsLocked { get; set; }
    }
}
