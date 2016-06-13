// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

namespace CefSharp.Example
{
    public class JsDialogHandler : IJsDialogHandler
    {
        void IJsDialogHandler.OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
            return;
        }

        bool IJsDialogHandler.OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            return false;
        }

        bool IJsDialogHandler.OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, string acceptLang, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            return false;
        }

        void IJsDialogHandler.OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
            return;
        }
    }
}
