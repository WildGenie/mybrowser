﻿// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

namespace CefSharp.Example
{
    public class DownloadHandler : IDownloadHandler
    {
        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            

            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    if(downloadItem.SuggestedFileName.EndsWith(".opus.pgp") || downloadItem.SuggestedFileName.EndsWith(".opus"))
                    {
                        var filePath = System.IO.Path.Combine(@"C:\CallRecording", downloadItem.SuggestedFileName);
                        callback.Continue(filePath, showDialog: false);
                    }
                    else
                    {
                        callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
                    }
                    
                }
            }
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if(downloadItem.IsComplete)
            {
                if (browser.IsPopup)
                {
                    browser.CloseBrowser(false);
                }

                if(downloadItem.SuggestedFileName.EndsWith(".opus.pgp") || downloadItem.SuggestedFileName.EndsWith(".opus"))
                {
                    // TODO: Send Command to recording service to play the file
                    
                }
            }
        }
    }
}
