using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace JabberWebApi
{
    public class CallStatusController : ApiController
    {
        public string Get(string status, string ts = "")
        {
            JabberWebApi.Program.SendCallStatusDelegate(status);
            return "success";
        }
    }
}
