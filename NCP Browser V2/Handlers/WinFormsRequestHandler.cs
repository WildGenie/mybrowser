// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using CefSharp.Example;
using CefSharp.WinForms.Internals;
using CefSharp;
using System.IO;
using System.Collections.Generic;

namespace NCP_Browser.Handlers
{
	public class WinFormsRequestHandler : RequestHandler, IRequestHandler
	{
        private Action<string, int?, string, object, NCP_Browser.NativeMessaging.Extension, NCP_Browser.Internals.AsyncBrowserScripting> openNewTab;
        private Salesforce salesforce;

        public WinFormsRequestHandler(Action<string, int?, string, object, NCP_Browser.NativeMessaging.Extension, NCP_Browser.Internals.AsyncBrowserScripting> openNewTab)
		{
			this.openNewTab = openNewTab;
		}

        public WinFormsRequestHandler(Action<string, int?, string, object, NativeMessaging.Extension, NCP_Browser.Internals.AsyncBrowserScripting> openNewTab, Salesforce salesforce)
        {
            // TODO: Complete member initialization
            this.openNewTab = openNewTab;
            this.salesforce = salesforce;
        }

		protected override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
		{
			if(openNewTab == null)
			{
				return false;
			}

			var control = (Control)browserControl;

			control.InvokeOnUiThreadIfRequired(delegate ()
			{
				openNewTab(targetUrl, null, null, null, null, null);
			});			

			return true;
		}

        bool CefSharp.IRequestHandler.GetAuthCredentials(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, bool isProxy, string host, int port, string realm, string scheme, CefSharp.IAuthCallback callback)
        {
            callback.Dispose();
            return false;
        }

        bool CefSharp.IRequestHandler.OnBeforeBrowse(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, bool isRedirect)
        {
            if (ValidPage(request.Url.Split('?')[0].Split(';')[0]))
            {
                return false;
            }
            else
            {
                return true;
            }            
        }

        private bool ValidPage(String BaseURL)
        {
            if
            (
                BaseURL.ToLower().Contains("force.com") ||
                BaseURL.ToLower().Contains("salesforce.com") ||
                BaseURL.ToLower().Contains("staticforce.com") ||
                BaseURL.ToLower().Contains("salesforceliveagent.com") ||
                BaseURL.ToLower().Contains("sfdcstatic.com") ||
                BaseURL.ToLower().Contains("localhost") ||
                BaseURL.ToLower().Contains("cefsharp-extension") ||
                BaseURL.ToLower().Contains("custom://") ||
                BaseURL.ToLower().Contains("chrome-devtools")
            )
                return true;
            else
                return false;
        }

        CefSharp.CefReturnValue CefSharp.IRequestHandler.OnBeforeResourceLoad(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IRequestCallback callback)
        {
            if(frame.Url.Contains("https"))
            {
                if (!String.IsNullOrEmpty(frame.Url))
                    CefSharp.Cef.AddCrossOriginWhitelistEntry(frame.Url, "https", "ppbllmlcmhfnfflbkbinnhacecaankdh", true);
                request.Url = request.Url.Replace("chrome-extension", "https");
            }
            else
            {
                if(!String.IsNullOrEmpty(frame.Url))
                    CefSharp.Cef.AddCrossOriginWhitelistEntry(frame.Url, "cefsharp-extension", "ppbllmlcmhfnfflbkbinnhacecaankdh", true);
                request.Url = request.Url.Replace("chrome-extension", "cefsharp-extension");

            }

            if (request.Url.Contains("ppbllmlcmhfnfflbkbinnhacecaankdh"))
            {
                lock(this.salesforce.browserTabs["Salesforce"].JavascriptQueueLock)
                {
                    this.salesforce.browserTabs["Salesforce"].ReInitializing();
                }
            }
            return CefSharp.CefReturnValue.Continue;
        }

        bool CefSharp.IRequestHandler.OnCertificateError(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.CefErrorCode errorCode, string requestUrl, CefSharp.ISslInfo sslInfo, CefSharp.IRequestCallback callback)
        {
            callback.Continue(true);
            return true;
        }

        bool CefSharp.IRequestHandler.OnOpenUrlFromTab(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, string targetUrl, CefSharp.WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        void CefSharp.IRequestHandler.OnPluginCrashed(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string pluginPath)
        {
            throw new NotImplementedException();
        }

        bool CefSharp.IRequestHandler.OnProtocolExecution(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string url)
        {
            return false;
        }

        bool CefSharp.IRequestHandler.OnQuotaRequest(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, string originUrl, long newSize, CefSharp.IRequestCallback callback)
        {
            return false;
        }

        void CefSharp.IRequestHandler.OnRenderProcessTerminated(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.CefTerminationStatus status)
        {
            return;
        }

        void CefSharp.IRequestHandler.OnRenderViewReady(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
        {
            return;
        }

        public interface ISchemeHandlerResponse
        {
            /// <summary>
            /// A Stream with the response data. If the request didn't return any response, leave this property as null.
            /// </summary>
            Stream ResponseStream { get; set; }

            string MimeType { get; set; }

            IDictionary<string, string> ResponseHeaders { get; set; }

            /// <summary>
            /// The status code of the response. Unless set, the default value used is 200
            /// (corresponding to HTTP status OK).
            /// </summary>
            int StatusCode { get; set; }

            /// <summary>
            /// The length of the response contents. Defaults to -1, which means unknown length
            /// and causes CefSharp to read the response stream in pieces. Thus, setting a length
            /// is optional but allows for more optimal response reading.
            /// </summary>
            int ContentLength { get; set; }

            /// <summary>
            /// URL to redirect to (leave empty to not redirect).
            /// </summary>
            string RedirectUrl { get; set; }

            /// <summary>
            /// Set to true to close the response stream once it has been read. The default value
            /// is false in order to preserve the old CefSharp behavior of not closing the stream.
            /// </summary>
            bool CloseStream { get; set; }
        }

        void CefSharp.IRequestHandler.OnResourceLoadComplete(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IResponse response, CefSharp.UrlRequestStatus status, long receivedContentLength)
        {
            return;
        }

        void CefSharp.IRequestHandler.OnResourceRedirect(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, ref string newUrl)
        {
            return;
        }

        bool CefSharp.IRequestHandler.OnResourceResponse(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IResponse response)
        {
            //if (request.Url.Contains("cefsharp-extension"))
            //{
            //    response.ResponseHeaders.Add("Access-Control-Allow-Origin: *", "Access-Control-Allow-Origin: *");
            //}

            return false;
        }


        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }
    }
}
