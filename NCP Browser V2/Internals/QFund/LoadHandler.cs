using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP_Browser.Internals.QFund
{
    class LoadHandler : CefSharp.ILoadHandler
    {
        void CefSharp.ILoadHandler.OnFrameLoadEnd(CefSharp.IWebBrowser browserControl, CefSharp.FrameLoadEndEventArgs frameLoadEndArgs)
        {
            if (frameLoadEndArgs.Url.ToLower().Contains("collectioncourtesycallsaction.do"))
            {
                frameLoadEndArgs.Frame.EvaluateScriptAsync("var notesBox = document.getElementsByName('requestBean.notesData'); notesBox[0]['onblur'] = null;");
                frameLoadEndArgs.Frame.EvaluateScriptAsync("var notesBox2 = document.getElementsByName('loanOriginationRequestBean.reason'); notesBox2[0]['onblur'] = null;");
            }
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
}
