using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace JabberWebApi
{
    public class PresenceController : ApiController
    {
        public string Get(string status, string show, string ts = "")
        {
            JabberWebApi.Program.SendPresenceDelegate(status, show);
            return "success";
        }
    }
}
