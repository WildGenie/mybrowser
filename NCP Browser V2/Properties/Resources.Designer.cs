﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NCP_Browser.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NCP_Browser.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap back {
            get {
                object obj = ResourceManager.GetObject("back", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function handleClientPortRequest(a) {
        ///    console.log(&quot;Incoming new port:&quot;, a);
        ///    var b = null
        ///      , c = null
        ///      , d = function (a) {
        ///          c = a
        ///      }
        ///      , e = function (a) {
        ///          return c
        ///      }
        ///    ;
        ///    if (!a.sender || !a.sender.id)
        ///        throw {
        ///            name: &quot;port missing id&quot;,
        ///            port: a
        ///        };
        ///    var f = a.sender.id
        ///      , g = &quot;&quot;;
        ///    f === location.host ? (a.sender.tab.url ? b = a.sender.tab.url : a.sender.url ? b = a.sender.url : console [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string background {
            get {
                return ResourceManager.GetString("background", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Base Runtime
        ///if (!chrome.runtime) {
        ///    chrome.runtime = {};
        ///    chrome.runtime.id = ncp_runtime.id;
        ///}
        ///
        ///// On Connect and handlers/events
        ///if (!chrome.runtime.onConnect) {
        ///    chrome.runtime.onConnect = {};
        ///    chrome.runtime.onConnect.listeners = new Array();
        ///
        ///    chrome.runtime.onConnect.addListener = function (method) {
        ///        chrome.runtime.onConnect.listeners.push(method);
        ///    }
        ///
        ///    chrome.runtime.onConnect.fire = function (port) {
        ///        for (var i = 0; i &lt; chrome.runtime.onConnect.l [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string chrome_native_runtime {
            get {
                return ResourceManager.GetString("chrome_native_runtime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var CurrentFrameIdentifier = &quot;{!FrameID}&quot;;
        ///
        ///
        ///if (!chrome.runtime) {
        ///    chrome.runtime = {}
        ///    chrome.runtime.channelId = Math.floor(Math.random() * 1e6 + 1).toString();
        ///}
        ///
        ///if(!chrome.runtime.connect)
        ///{
        ///    chrome.runtime.connect = function (x, y) {
        ///        if (!chrome.runtime.onMessage) {
        ///            chrome.runtime.onMessage = {};
        ///            chrome.runtime.onMessage.listeners = new Array();
        ///            chrome.runtime.onMessage.fire = function (data) {
        ///                //if (portId == chrome [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string chrome_runtime {
            get {
                return ResourceManager.GetString("chrome_runtime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap chromium256 {
            get {
                object obj = ResourceManager.GetObject("chromium256", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to cwic_ext = function () {
        ///    chrome.runtime.id = &quot;ppbllmlcmhfnfflbkbinnhacecaankdh&quot;;
        ///    function d() {
        ///        var a = typeof arguments[0] == &quot;boolean&quot; ? arguments[0] : !1
        ///            , b = typeof arguments[0] == &quot;string&quot; ? arguments[0] : arguments[1]
        ///            , d = typeof arguments[1] == &quot;object&quot; ? arguments[1] : arguments[2];
        ///        if (!a || a &amp;&amp; c) {
        ///            var e = new Date
        ///                , f = (&quot;0&quot; + e.getHours()).slice(-2) + &quot;:&quot; + (&quot;0&quot; + e.getMinutes()).slice(-2) + &quot;:&quot; + (&quot;0&quot; + e.ge [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string contentscript {
            get {
                return ResourceManager.GetString("contentscript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var cwic_plugin = function () {
        ///    function c() {
        ///        var a = typeof arguments[0] == &quot;boolean&quot; ? arguments[0] : !1
        ///          , c = typeof arguments[0] == &quot;string&quot; ? arguments[0] : arguments[1]
        ///          , d = typeof arguments[1] == &quot;object&quot; ? arguments[1] : arguments[2];
        ///        if (!a || a &amp;&amp; b.verbose) {
        ///            var e = new Date
        ///              , f = (&quot;0&quot; + e.getHours()).slice(-2) + &quot;:&quot; + (&quot;0&quot; + e.getMinutes()).slice(-2) + &quot;:&quot; + (&quot;0&quot; + e.getSeconds()).slice(-2) + &quot;.&quot; + (&quot;00&quot; + e.getMilliseco [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string cwic_plugin {
            get {
                return ResourceManager.GetString("cwic_plugin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.IO.UnmanagedMemoryStream similar to System.IO.MemoryStream.
        /// </summary>
        internal static System.IO.UnmanagedMemoryStream Ding {
            get {
                return ResourceManager.GetStream("Ding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] extProps {
            get {
                object obj = ResourceManager.GetObject("extProps", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap forward {
            get {
                object obj = ResourceManager.GetObject("forward", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap go {
            get {
                object obj = ResourceManager.GetObject("go", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap halt {
            get {
                object obj = ResourceManager.GetObject("halt", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap home {
            get {
                object obj = ResourceManager.GetObject("home", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Jabber_128x128 {
            get {
                object obj = ResourceManager.GetObject("Jabber_128x128", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Jabber_16x16 {
            get {
                object obj = ResourceManager.GetObject("Jabber_16x16", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Jabber_48x48 {
            get {
                object obj = ResourceManager.GetObject("Jabber_48x48", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap nav_left_green {
            get {
                object obj = ResourceManager.GetObject("nav_left_green", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap nav_plain_green {
            get {
                object obj = ResourceManager.GetObject("nav_plain_green", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap nav_plain_red {
            get {
                object obj = ResourceManager.GetObject("nav_plain_red", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap nav_right_green {
            get {
                object obj = ResourceManager.GetObject("nav_right_green", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap reload {
            get {
                object obj = ResourceManager.GetObject("reload", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}