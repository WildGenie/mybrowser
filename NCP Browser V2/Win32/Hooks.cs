using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NCP_Browser.Win32
{
    

    public delegate void OnForegroundWindowChangedDelegate(IntPtr hWnd);
    public delegate void OnWindowMinimizeStartDelegate(IntPtr hWnd);
    public delegate void OnWindowMinimizeEndDelegate(IntPtr hWnd);
    public delegate void OnWindowDestroyDelegate(IntPtr hWnd);

    public sealed class Hooks
    {
        #region Windows API

        private enum SystemEvents : uint
        {
            EVENT_SYSTEM_DESTROY = 0x8001,
            EVENT_SYSTEM_MINIMIZESTART = 0x0016,
            EVENT_SYSTEM_MINIMIZEEND = 0x0017,
            EVENT_SYSTEM_FOREGROUND = 0x0003
        }

        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        private delegate void WinEventDelegate(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hWnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(
            IntPtr hWinEventHook
            );

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        List<IntPtr> GetRootWindowsOfProcess(int pid)
        {
            List<IntPtr> rootWindows = GetChildWindows(IntPtr.Zero);
            List<IntPtr> dsProcRootWindows = new List<IntPtr>();
            foreach (IntPtr hWnd in rootWindows)
            {
                uint lpdwProcessId;
                Hooks.GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                if (lpdwProcessId == pid)
                    dsProcRootWindows.Add(hWnd);
            }
            return dsProcRootWindows;
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Hooks.Win32Callback childProc = new Hooks.Win32Callback(EnumWindow);
                Hooks.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }


        #endregion

        private WinEventDelegate dEvent;
        private IntPtr pHook;
        public OnForegroundWindowChangedDelegate OnForegroundWindowChanged;
        public OnWindowMinimizeStartDelegate OnWindowMinimizeStart;
        public OnWindowMinimizeEndDelegate OnWindowMinimizeEnd;
        public OnWindowDestroyDelegate OnWindowDestroy;

        public Hooks()
        {
            dEvent = this.WinEvent;
            pHook = SetWinEventHook(
                (uint)SystemEvents.EVENT_SYSTEM_DESTROY,
                (uint)SystemEvents.EVENT_SYSTEM_DESTROY,
                IntPtr.Zero,
                dEvent,
                0,
                0,
                WINEVENT_OUTOFCONTEXT
                );

            if (IntPtr.Zero.Equals(pHook)) throw new Win32Exception();

            

            GC.KeepAlive(dEvent);
        }

        private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch (eventType)
            {
                case (uint)SystemEvents.EVENT_SYSTEM_DESTROY:
                    if (OnWindowDestroy != null) OnWindowDestroy(hWnd);
                    break;

                //extend here when required
            }
        }

        ~Hooks()
        {
            if (!IntPtr.Zero.Equals(pHook)) UnhookWinEvent(pHook);
            pHook = IntPtr.Zero;
            dEvent = null;

            OnWindowDestroy = null;
        }

        public IntPtr Handle { get; set; }
    }
}
