using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.chrome.extensions
{
    public class ChromeExtensionSchemeHandlerFactory : ISchemeHandlerFactory
    {

        public static string SchemeName { get { return "cefsharp-extension"; } }

        IResourceHandler ISchemeHandlerFactory.Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            if (schemeName == SchemeName && request.Url.EndsWith("CefSharp.Core.xml", System.StringComparison.OrdinalIgnoreCase))
            {
                //Display the debug.log file in the browser
                return ResourceHandler.FromFileName("CefSharp.Core.xml", ".xml");
            }
            return new ChromeExtensionResourceHandler(ChromeExtensionSchemeHandlerFactory.SchemeName);
        }
    }
}
