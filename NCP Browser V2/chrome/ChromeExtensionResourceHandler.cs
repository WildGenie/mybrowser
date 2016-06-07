using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.chrome
{
    public class ChromeExtensionResourceHandler : IResourceHandler
    {
        private static Dictionary<String, String> ResourceDictionary;

        private string mimeType;
        private MemoryStream stream;
        private string SchemeName;
        private static string ReplaceVal = "cefsharp-extension";


        public ChromeExtensionResourceHandler(string SchemeName)
        {
            this.SchemeName = SchemeName;
        }

        static ChromeExtensionResourceHandler()
        {
            ResourceDictionary = new Dictionary<string, string>
            {
                // work on alternate solution to chrome.runtime.id hack, contentscript should be loaded into an iframe with the runtime loaded likely
                {"ppbllmlcmhfnfflbkbinnhacecaankdh/cwic_plugin.js", NCP_Browser.Properties.Resources.cwic_plugin.Replace("chrome-extension",ReplaceVal)},
                {"ppbllmlcmhfnfflbkbinnhacecaankdh/contentscript.js", NCP_Browser.Properties.Resources.contentscript.Replace("chrome-extension",ReplaceVal)},
                {"ppbllmlcmhfnfflbkbinnhacecaankdh/extProps.json", NCP_Browser.Properties.Resources.extProps.ToString().Replace("chrome-extension",ReplaceVal)},
                {"background.js/", NCP_Browser.Properties.Resources.background.ToString().Replace("chrome-extension",ReplaceVal)},
                {"background.js", NCP_Browser.Properties.Resources.background.ToString().Replace("chrome-extension",ReplaceVal)}
                //{"ppbllmlcmhfnfflbkbinnhacecaankdh/_generated_background_page.html", "<!DOCTYPE html><html><head></head><body></body></html>"},
                //{"ppbllmlcmhfnfflbkbinnhacecaankdh/_generated_background_page.html/", "<!DOCTYPE html><html><head></head><body></body></html>"}
            };
        }

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
            responseLength = stream == null ? 0 : stream.Length;
            redirectUrl = null;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.StatusText = "OK";
            response.MimeType = mimeType;
            response.ResponseHeaders.Add("Access-Control-Allow-Origin", "*");
        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback)
        {
            var uri = new Uri(request.Url);
            var fileName = uri.AbsoluteUri.Replace(this.SchemeName + "://", "").Replace("https" + "://", "");
            string resource = string.Empty;
            if (ResourceDictionary.TryGetValue(fileName, out resource) && !string.IsNullOrEmpty(resource))
            {
                /*bool done = false;
                while(!done)
                {
                    lock(Salesforce.FrameLoadLock)
                    {
                        done = true;
                        Salesforce.FrameLoads.Values.ToList().ForEach(x =>
                        {
                            x.LoadingResourcees.ForEach(y =>
                            {
                                if(y.Replace(this.SchemeName + "://", "").Replace("https" + "://", "") == fileName)
                                {
                                    done = false;
                                }
                            });
                        });
                    }
                }*/
                Task.Run(() =>
                {
                    using (callback)
                    {
                        var bytes = Encoding.UTF8.GetBytes(uri.AbsoluteUri.Contains("https") ? NCP_Browser.Properties.Resources.chrome_runtime + "\n" + resource.Replace("cefsharp-extension","https") : resource);
                        stream = new MemoryStream(bytes);

                        var fileExtension = Path.GetExtension(fileName);
                        mimeType = ResourceHandler.GetMimeType(fileExtension);

                        callback.Continue();
                    }
                });

                return true;
            }
            else
            {
                callback.Dispose();
            }

            return false;
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            System.Threading.Thread.Sleep(1000);
            //Dispose the callback as it's an unmanaged resource, we don't need it in this case
            callback.Dispose();

            if (stream == null)
            {
                bytesRead = 0;
                return false;
            }

            //Data out represents an underlying buffer (typically 32kb in size).
            var buffer = new byte[dataOut.Length];
            bytesRead = stream.Read(buffer, 0, buffer.Length);

            dataOut.Write(buffer, 0, buffer.Length);

            return bytesRead > 0;
        }

        void IDisposable.Dispose()
        {
            this.stream.Dispose();
        }
    }
}
