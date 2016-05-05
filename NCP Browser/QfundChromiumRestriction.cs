using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    class AllowedResource
    {
        public String Method { get; set; }
        public String URLStart { get; set; }
    }

    class BlahBLah: CefSharp.ILoadHandler
    {
        void CefSharp.ILoadHandler.OnFrameLoadEnd(CefSharp.IWebBrowser browserControl, CefSharp.FrameLoadEndEventArgs frameLoadEndArgs)
        {
            if (frameLoadEndArgs.Url.ToLower().Contains("collectioncourtesycallsaction.do"))
            {
                frameLoadEndArgs.Frame.EvaluateScriptAsync("var notesBox = document.getElementsByName('requestBean.notesData'); notesBox[0]['onblur'] = null;");
                frameLoadEndArgs.Frame.EvaluateScriptAsync("var notesBox2 = document.getElementsByName('loanOriginationRequestBean.reason'); notesBox2[0]['onblur'] = null;");
            }

            
            //if (frameLoadEndArgs.Url.ToLower().Contains("suspenseaccsearch.do"))
            //{
            //    frameLoadEndArgs.Frame.EvaluateScriptAsync("	function changeSuspenseStatus(ssn,loanNbr,paymentAmt,csoId,refNbr,csoStCode){ 		 		var tranType = document.forms[0].elements[\"requestBean.transactionType\"].value; 		if(isEmpty(tranType)){ 			alert(\"please select transaction type\"); 			return false; 		} 		if('REF' == tranType){ 			if(document.getElementById(\"disbType\").style.display == 'block'){ 				document.forms[0].elements[\"requestBean.disbType\"].value = document.forms[0].disbType[0].value; 			}else{ 				document.forms[0].elements[\"requestBean.disbType\"].value = document.forms[0].disbType[1].value; 			} 			var payTo = document.forms[0].elements[\"requestBean.payTo\"].value; 			if(isEmpty(payTo)){ 				alert(\"please select the pay type\"); 				return false; 			}			 			if(isEmpty(document.forms[0].elements[\"requestBean.disbType\"].value )){ 				alert(\"please select the tender type\"); 				return false; 			} 			 				if(isEmpty(document.forms[0].elements[\"requestBean.payToCSO\"].value )){ 					alert(\"please select CSO\"); 					return false; 				} 			 			if(document.forms[0].elements[\"requestBean.disbType\"].value == 'ACH'){ 				var bankDetailsExistsFlg = 'null'; 				if(bankDetailsExistsFlg == 'false'){ 					alert(\"Bank Details does not exists for the CSO\"); 					return false; 				}else{ 					var currTime = '11:36'; 					var HHMM = '19:30'; 					var cmpTime = compareTime(HHMM,\"HH:MM\",currTime,\"<\",\"ACH processing will be done tomorrow as the time exceeded\"); 					if (cmpTime == false) 					{ 						document.forms[0].elements[\"requestBean.timeOutFlag\"].value = 'timeOut'; 					} 				} 			} 			  		} 		 		 		document.forms[0].elements[\"requestBean.ssn\"].value=ssn; 		document.forms[0].elements[\"requestBean.loanNo\"].value=loanNbr; 		document.forms[0].elements[\"requestBean.paymentAmount\"].value=paymentAmt; 		document.forms[0].elements[\"requestBean.selCsoId\"].value=csoId; 		document.forms[0].elements[\"requestBean.referenceNumber\"].value=refNbr; 		document.forms[0].elements[\"requestBean.csoStCode\"].value=csoStCode; 		document.forms[0].elements[\"requestBean.moduleStatus\"].value=\"updateSuspenseTransaction\"; 		document.forms[0].elements[\"requestBean.payTo\"].readOnly= true; 		/*document.forms[0].elements[\"payToCSO1\"].disabled= true;*/ 		document.forms[0].elements[\"disbType\"].readOnly= true; 		  		/*document.forms[0].elements[\"seldisbType1\"].disabled= true;*/ 		document.forms[0].elements[\"requestBean.transactionType\"].readOnly = true;  		document.forms[0].elements[\"button\"].disabled= true;         document.forms[0].action=\"/ncp/suspenseAccSearch.do\"; 		document.forms[0].submit(); 	  }");
            //}
            
            //throw new NotImplementedException();
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

    class Blah : CefSharp.ILifeSpanHandler
    {

        bool CefSharp.ILifeSpanHandler.DoClose(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
        {
            return false;
        }

        void CefSharp.ILifeSpanHandler.OnAfterCreated(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
        {
            return;
        }

        void CefSharp.ILifeSpanHandler.OnBeforeClose(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser)
        {
            return;
        }

        bool CefSharp.ILifeSpanHandler.OnBeforePopup(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, string targetUrl, string targetFrameName, CefSharp.WindowOpenDisposition targetDisposition, bool userGesture, CefSharp.IWindowInfo windowInfo, ref bool noJavascriptAccess, out CefSharp.IWebBrowser newBrowser)
        {
            // Position the popup in the center
            // Get Screen
            Screen screen = null;
            GetControlScreenAction<Control, Screen> x = new GetControlScreenAction<Control,Screen>(GetControlScreen);
            var y = ((CefSharp.WinForms.ChromiumWebBrowser)browserControl).Parent;
            x.Invoke(y, out screen);            
             
            // Position
            windowInfo.X = screen.Bounds.Left + ((screen.WorkingArea.Width - windowInfo.Width)/2);
            windowInfo.Y = (screen.WorkingArea.Height - windowInfo.Height) / 2;

            // this has to be done
            newBrowser = null;

            // Return false to create the popup
            return false;
        }

        public delegate void GetControlScreenAction<T1,T2>(Control control, out Screen screen);
        static void GetControlScreen(Control control, out Screen screen)
        {
            if(control.InvokeRequired)
            {
                screen = (Screen)control.Invoke(new Func<Screen>(() => Screen.FromControl(control)));
            }
            else
            {
                screen = Screen.FromControl(control);
            }            
        }
    }

    class QfundChromiumRestriction : CefSharp.IRequestHandler
    {
        public List<AllowedResource> AllowedResources { get; set; }
        public QfundChromiumRestriction()
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
            //throw new NotImplementedException();
            return false;
        }

        CefSharp.CefReturnValue CefSharp.IRequestHandler.OnBeforeResourceLoad(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IRequest request, CefSharp.IRequestCallback callback)
        {
            return CefSharp.CefReturnValue.Continue;
            /*
            if(AllowedResources.Where(x => x.Method == request.Method && request.Url.Split('?')[0].Split(';')[0].StartsWith(x.URLStart)).Count() > 0)
            {
                if (request.Url.Split('?')[0].Split(';')[0] == "http://ncponsiteqa.qfund.net/ncp/customerChecksHistory.do")
                {
                    //((CefSharp.WinForms.ChromiumWebBrowser)browserControl).Parent.Location = new System.Drawing.Point(3000, 400);
                    MessageBox.Show("blah");
                }
                return CefSharp.CefReturnValue.Continue;
            }
            else
            {
                MessageBox.Show("Unable to click this button");
                return CefSharp.CefReturnValue.Cancel;
            }
            */
        }

        bool CefSharp.IRequestHandler.OnCertificateError(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.CefErrorCode errorCode, string requestUrl, CefSharp.ISslInfo sslInfo, CefSharp.IRequestCallback callback)
        {
            throw new NotImplementedException();
        }

        bool CefSharp.IRequestHandler.OnOpenUrlFromTab(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, string targetUrl, CefSharp.WindowOpenDisposition targetDisposition, bool userGesture)
        {
            throw new NotImplementedException();
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
    }
}
