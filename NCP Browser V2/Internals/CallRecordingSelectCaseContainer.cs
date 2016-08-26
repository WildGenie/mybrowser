using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.Internals
{
    public class CallRecordingSelectCaseContainer
    {
        public CallData SelectCaseCallData { get; set; }
        public List<CaseRelatedPhoneNumber> RelatedPhoneNumbers { get; set; }
    }
}
