using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NCP_Browser
{
    public class DownloadHandler : IDownloadHandler
    {
        private Salesforce salesforce;

        public DownloadHandler(Salesforce salesforce)
        {
            // TODO: Complete member initialization
            this.salesforce = salesforce;
        }

        void IDownloadHandler.OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            //throw new NotImplementedException();            
            callback.Continue(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),downloadItem.SuggestedFileName), false);
        }

        void IDownloadHandler.OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if(downloadItem.IsComplete)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    //new DialogBox(this.salesforce, "File Download", String.Format("File: {0} downloaded!", downloadItem.FullPath), this.salesforce
                    DialogBox.ShowMeAsDialog(new DialogBox(this.salesforce, "File Download", String.Format("File: {0} downloaded!", downloadItem.FullPath)), this.salesforce);
                });
            }
            //throw new NotImplementedException();
        }
    }

    class ChromiumRestriction : CefSharp.IRequestHandler
    {
        bool CefSharp.IRequestHandler.GetAuthCredentials(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, bool isProxy, string host, int port, string realm, string scheme, CefSharp.IAuthCallback callback)
        {
            callback.Dispose();
            return false;
        }

        bool CefSharp.IRequestHandler.OnBeforeBrowse(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, bool isRedirect)
        {
            return false;
        }

        private bool ValidPage(String BaseURL)
        {
            if 
            (
                BaseURL.Contains("force.com") ||
                BaseURL.Contains("salesforce.com") ||
                BaseURL.Contains("staticforce.com") ||
                BaseURL.Contains("salesforceliveagent.com") ||
                BaseURL.Contains("sfdcstatic.com") ||
                BaseURL.Contains("localhost")
            )
                return true;
            else
                return true;
        }

        CefSharp.CefReturnValue CefSharp.IRequestHandler.OnBeforeResourceLoad(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IRequestCallback callback)
        {
            if (ValidPage(request.Url.Split('?')[0].Split(';')[0]))
            {
                return CefSharp.CefReturnValue.Continue;
            }
            else
            {

                return CefSharp.CefReturnValue.Cancel;
            }   
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
            /*var blah = response.ResponseHeaders.AllKeys;
            foreach (string key in response.ResponseHeaders.AllKeys)
            {
                if (key == "Content-Disposition")
                {
                    string value = response.ResponseHeaders[key];
                    if (value.ToLower().StartsWith("attachment;") || value.ToLower().StartsWith("inline;"))
                    {
                        // FILE DOWNLOAD
                        System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                        sfd.FileName = value.Split(';')[1].Trim().Replace("filename=", "").Replace("\"", "").Trim();
                        var res = sfd.ShowDialog();
                        if (res == System.Windows.Forms.DialogResult.OK)
                        {
                            WebRequest fileDownload = WebRequest.Create(!String.IsNullOrEmpty(request.ReferrerUrl) ? request.Url :request.Url);
                            request.Headers.AllKeys.ToList().ForEach(x =>
                            {
                                if ("Accept".Equals(x))
                                    ((HttpWebRequest)fileDownload).Accept = request.Headers[x];
                                else if ("User-Agent".Equals(x))
                                    ((HttpWebRequest)fileDownload).UserAgent = request.Headers[x];
                                else if ("Referer".Equals(x))
                                    ((HttpWebRequest)fileDownload).Referer = request.Headers[x];
                                else if ("Content-Type".Equals(x))
                                    ((HttpWebRequest)fileDownload).ContentType = request.Headers[x];
                                else if ("Content-Length".Equals(x))
                                    ((HttpWebRequest)fileDownload).ContentLength = Convert.ToInt32(request.Headers[x]);
                                else
                                    fileDownload.Headers.Add(x, request.Headers[x]);
                                
                            });

                            fileDownload.Method = "POST";
                            fileDownload.ContentLength = request.PostData.Elements[0].Bytes.Length;
                            fileDownload.ContentType = "application/x-www-form-urlencoded";
                            fileDownload.GetRequestStream().Write(request.PostData.Elements[0].Bytes, 0, request.PostData.Elements[0].Bytes.Length);
                            ((HttpWebRequest)fileDownload).UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
                            var fileResponse = fileDownload.GetResponse();
                            FileStream sw = new FileStream(sfd.FileName, FileMode.Create);
                            var fileResponseStream = fileResponse.GetResponseStream();
                            int currByte = -1;
                            while((currByte = fileResponseStream.ReadByte()) != -1)
                            {
                                sw.WriteByte(Convert.ToByte(currByte));
                            }
                            sw.Close();

                            var x = frame.GetTextAsync();
                            x.ContinueWith(t =>
                                {
                                    System.Windows.Forms.MessageBox.Show(t.Result);
                                });
                            
                            //var rWrapper = (ISchemeHandlerResponse)response;
                            //Console.WriteLine(rWrapper.StatusCode.ToString());
                        }
                    }
                }
            }*/
            return;
        }

        void CefSharp.IRequestHandler.OnResourceRedirect(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, ref string newUrl)
        {
            return;
        }

        bool CefSharp.IRequestHandler.OnResourceResponse(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IResponse response)
        {
            return false;
        }


        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }
    }

    class ChromiumLoadHandler : CefSharp.ILoadHandler
    {
        void CefSharp.ILoadHandler.OnFrameLoadEnd(CefSharp.IWebBrowser browserControl, CefSharp.FrameLoadEndEventArgs frameLoadEndArgs)
        {
            // Cisco Plugin
            frameLoadEndArgs.Frame.EvaluateScriptAsync(NCP_Browser.Properties.Resources.cwic_plugin);
            frameLoadEndArgs.Frame.EvaluateScriptAsync(NCP_Browser.Properties.Resources.cwic_background);
            frameLoadEndArgs.Frame.EvaluateScriptAsync(NCP_Browser.Properties.Resources.cwic_contentscript);
        }

        void CefSharp.ILoadHandler.OnFrameLoadStart(CefSharp.IWebBrowser browserControl, CefSharp.FrameLoadStartEventArgs frameLoadStartArgs)
        {
            //throw new NotImplementedException();
        }

        void CefSharp.ILoadHandler.OnLoadError(CefSharp.IWebBrowser browserControl, CefSharp.LoadErrorEventArgs loadErrorArgs)
        {
            //throw new NotImplementedException();
        }

        void CefSharp.ILoadHandler.OnLoadingStateChange(CefSharp.IWebBrowser browserControl, CefSharp.LoadingStateChangedEventArgs loadingStateChangedArgs)
        {
            //throw new NotImplementedException();
        }
    }

    class JsDialogHander : CefSharp.IJsDialogHandler
    {

        void IJsDialogHandler.OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
            throw new NotImplementedException();
        }

        bool IJsDialogHandler.OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            throw new NotImplementedException();
        }

        bool IJsDialogHandler.OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, string acceptLang, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            throw new NotImplementedException();
        }

        void IJsDialogHandler.OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
            throw new NotImplementedException();
        }
    }

    public class ChromeExtensionSchemeHandlerFactory : ISchemeHandlerFactory
    {

        public static string SchemeName { get { return "chrome-extension"; } }

        IResourceHandler ISchemeHandlerFactory.Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new ChromeExtensionResourceHandler();
        }
    }

    public class ChromeExtensionResourceHandler : IResourceHandler
    {

        bool IResourceHandler.CanGetCookie(CefSharp.Cookie cookie)
        {
            return false;
        }

        bool IResourceHandler.CanSetCookie(CefSharp.Cookie cookie)
        {
            return false;
        }

        void IResourceHandler.Cancel()
        {
            return;
        }

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = 0;
            redirectUrl = "";
            return;
        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback)
        {
            return true;
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            bytesRead = 0;
            return true;
        }
    }
}
