using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.chrome.runtime
{
    public class FrameLoad
    {
        public List<String> LoadingResourcees { get; set; }

        public FrameLoad()
        {
            this.LoadingResourcees = new List<string>();
        }
    }
}
