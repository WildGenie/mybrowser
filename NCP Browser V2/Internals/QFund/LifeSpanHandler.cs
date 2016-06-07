using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCP_Browser.Internals.QFund
{
    class LifeSpanHandler : CefSharp.ILifeSpanHandler
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

        public delegate void GetControlScreenAction<T1, T2>(Control control, out Screen screen);
        static void GetControlScreen(Control control, out Screen screen)
        {
            if (control.InvokeRequired)
            {
                screen = (Screen)control.Invoke(new Func<Screen>(() => Screen.FromControl(control)));
            }
            else
            {
                screen = Screen.FromControl(control);
            }
        }


        public bool OnBeforePopup(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, string targetUrl, string targetFrameName, CefSharp.WindowOpenDisposition targetDisposition, bool userGesture, CefSharp.IPopupFeatures popupFeatures, CefSharp.IWindowInfo windowInfo, CefSharp.IBrowserSettings browserSettings, ref bool noJavascriptAccess, out CefSharp.IWebBrowser newBrowser)
        {
            // Position the popup in the center
            // Get Screen
            Screen screen = null;
            GetControlScreenAction<Control, Screen> x = new GetControlScreenAction<Control, Screen>(GetControlScreen);
            var y = ((CefSharp.WinForms.ChromiumWebBrowser)browserControl).Parent;
            x.Invoke(y, out screen);

            // Position
            windowInfo.X = screen.Bounds.Left + ((screen.WorkingArea.Width - windowInfo.Width) / 2);
            windowInfo.Y = (screen.WorkingArea.Height - windowInfo.Height) / 2;

            // this has to be done
            newBrowser = null;

            // Return false to create the popup
            return false;
        }
    }
}
