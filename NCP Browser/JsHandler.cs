using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCP_Browser
{
    class JsHandler : IJsDialogHandler
    {
    
        public void OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
 	        throw new NotImplementedException();
        }

        public bool OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
 	        throw new NotImplementedException();
        }

        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, string acceptLang, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            throw new NotImplementedException();
        }

        public void OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
 	        throw new NotImplementedException();
        }
    }

    class DialogHandler : IDialogHandler
    {
        bool IDialogHandler.OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            throw new NotImplementedException();
        }
    }

    class LoadHandler : ILoadHandler
    {

        void ILoadHandler.OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs)
        {
            throw new NotImplementedException();
        }

        void ILoadHandler.OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs)
        {
            throw new NotImplementedException();
        }

        void ILoadHandler.OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs)
        {
            throw new NotImplementedException();
        }

        void ILoadHandler.OnLoadingStateChange(IWebBrowser browserControl, LoadingStateChangedEventArgs loadingStateChangedArgs)
        {
            throw new NotImplementedException();
        }
    }

}
