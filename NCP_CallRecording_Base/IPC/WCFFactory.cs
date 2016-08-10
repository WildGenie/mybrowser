using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace NCP_CallRecorder.IPC
{
    public static class WCFFactory
    {
        public static ServiceHost OpenPipe(Type serviceType, Type endpointType, String EndpointAddress)
        {
            var svcHost = new ServiceHost(serviceType);
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            svcHost.AddServiceEndpoint(endpointType, binding, EndpointAddress);
            svcHost.Open();
            return svcHost;
        }
    }
}
