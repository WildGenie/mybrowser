using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.Internals.QFund
{
    class RequestHandler : CefSharp.IRequestHandler
    {
        public List<AllowedResource> AllowedResources { get; set; }
        public RequestHandler()
        {
            AllowedResources = new List<AllowedResource>();
            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/loanTranSearch.do"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "POST",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/loanTranSearch.do"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "ncponsiteqa.qfund.net/ncp/customerEditPopup.do"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "POST",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/custLoanTrans.do"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "POST",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/intTransInstallmentHistory.do"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/customerChecksHistory.do"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/suspenseAccountInitialSearch.do"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/images/"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/css/"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "GET",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/js/"
            });

            AllowedResources.Add(new AllowedResource()
            {
                Method = "POST",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/login.do"
            });
            
            AllowedResources.Add(new AllowedResource()
            {
                Method = "POST",
                URLStart = "http://ncponsiteqa.qfund.net/ncp/loanTranSearchResults.do"
            });
        }

        bool CefSharp.IRequestHandler.GetAuthCredentials(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, bool isProxy, string host, int port, string realm, string scheme, CefSharp.IAuthCallback callback)
        {
            callback.Dispose();
            return false;
        }

        bool CefSharp.IRequestHandler.OnBeforeBrowse(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, bool isRedirect)
        {
            return false;
        }

        CefSharp.CefReturnValue CefSharp.IRequestHandler.OnBeforeResourceLoad(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IRequestCallback callback)
        {
            return CefSharp.CefReturnValue.Continue;
        }

        bool CefSharp.IRequestHandler.OnCertificateError(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.CefErrorCode errorCode, string requestUrl, CefSharp.ISslInfo sslInfo, CefSharp.IRequestCallback callback)
        {
            return false;
        }

        bool CefSharp.IRequestHandler.OnOpenUrlFromTab(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, string targetUrl, CefSharp.WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        void CefSharp.IRequestHandler.OnPluginCrashed(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string pluginPath)
        {
            //throw new NotImplementedException();
        }

        bool CefSharp.IRequestHandler.OnProtocolExecution(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string url)
        {
            return false;
        }

        bool CefSharp.IRequestHandler.OnQuotaRequest(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string originUrl, long newSize, CefSharp.IRequestCallback callback)
        {
            callback.Dispose();
            return false;
        }

        void CefSharp.IRequestHandler.OnRenderProcessTerminated(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.CefTerminationStatus status)
        {
            //throw new NotImplementedException();
        }

        void CefSharp.IRequestHandler.OnRenderViewReady(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
        {
            //throw new NotImplementedException();
        }

        void CefSharp.IRequestHandler.OnResourceLoadComplete(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IResponse response, CefSharp.UrlRequestStatus status, long receivedContentLength)
        {

            //throw new NotImplementedException();
        }

        void CefSharp.IRequestHandler.OnResourceRedirect(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, ref string newUrl)
        {
            //throw new NotImplementedException();
        }

        bool CefSharp.IRequestHandler.OnResourceResponse(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IResponse response)
        {
            return false;
        }


        public CefSharp.IResponseFilter GetResourceResponseFilter(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IResponse response)
        {
            return null;
        }
    }
}
