cwic_ext = function () {
    chrome.runtime.id = "ppbllmlcmhfnfflbkbinnhacecaankdh";
    function d() {
        var a = typeof arguments[0] == "boolean" ? arguments[0] : !1
            , b = typeof arguments[0] == "string" ? arguments[0] : arguments[1]
            , d = typeof arguments[1] == "object" ? arguments[1] : arguments[2];
        if (!a || a && c) {
            var e = new Date
                , f = ("0" + e.getHours()).slice(-2) + ":" + ("0" + e.getMinutes()).slice(-2) + ":" + ("0" + e.getSeconds()).slice(-2) + "." + ("00" + e.getMilliseconds()).slice(-3) + " ";
            d ? console.log("[cwic.ext] " + f + b, d) : console.log("[cwic.ext] " + f + b)
        }
    }
    var a = null
        , b = null
        , c = !1;
    cwic_reply = function (a) {
        var c = "unknown";
        a.ciscoSDKServerMessage ? (c = a.ciscoSDKServerMessage.name,
        d(!0, "send ciscoSDKServerMessage '" + c + "' to client")) : a.ciscoChannelServerMessage ? (c = a.ciscoChannelServerMessage.name,
        d(!0, "send ciscoChannelServerMessage '" + c + "' to client")) : d("Unknown message from background page.", a),
        b ? b.contentWindow.postMessage(a, location.origin) : typeof cwic_plugin != "undefined" ? cwic_plugin.directReply(a) : d("No responseHandler or iframe window to send reply on.")
    }
    ,
    sendChannelDisconnectMessage = function (b) {
        a = null;
        var c = {
            ciscoChannelServerMessage: {
                name: "ChannelDisconnect",
                cause: b
            }
        };
        cwic_reply(c)
    }
    ,
    connectPort = function (b) {
        if (a !== null)
            d("Already connected to Cisco extension: " + b);
        else {
            try {
                a = chrome.runtime.connect(b, {
                    name: "ContentScript"
                })
            } catch (c) {
                sendChannelDisconnectMessage(c);
                return
            }
            a.onMessage.addListener(cwic_reply),
            a.onDisconnect.addListener(function (a) {
                sendChannelDisconnectMessage("Port to background script disconnected")
            }),
            d(!0, "Content script connected to Cisco extension: " + b)
        }
    }
    ;
    return {
        relay: function (b) {
            var e = b.ciscoSDKClientMessage.name;
            if (a === null && e === "init") {
                d("initializing extension channel", b.ciscoChannelProperties);
                var f = b.ciscoChannelProperties.cwicExtId;
                c = b.ciscoChannelProperties.verbose,
                connectPort(f)
            }
            if (a) {
                d(!0, "forward ciscoSDKClientMessage '" + e + "' to background script");
                try {
                    a.postMessage(b)
                } catch (g) {
                    sendChannelDisconnectMessage(g)
                }
            } else
                sendChannelDisconnectMessage("Content Script Not Initialized")
        },
        init: function (a) {
            c = a.ciscoChannelProperties.verbose,
            b = document.getElementById(a.ciscoChannelProperties.objectId),
            b.contentWindow.addEventListener("message", function (a) {
                a.source == window && !!a.data.ciscoSDKClientMessage && cwic_ext.relay(a.data)
            }, !1),
            this.relay(event.data)
        }
    }
}();
if (typeof cwic_plugin != "undefined" && typeof cwic_plugin.directReply != "undefined") {
    console.log("[cwic.ext] loaded in extension: " + chrome.runtime.id);
    window.addEventListener("message", function (a) {
        a.source == window && !!a.data.ciscoSDKClientMessage && (a.data.ciscoSDKClientMessage.name == "init" && a.data.ciscoChannelProperties.cwicExtId == chrome.runtime.id ? cwic_ext.init(a.data) : console.log("[cwic.ext] ciscoSDKClientMessage received on main window but it is not 'init' or not for our ext id.", a.data))
    });
} else {
    (console.log("[cwic.ext] waiting for 'init' as extension: " + chrome.runtime.id),
    window.addEventListener("message", function (a) {
        if(a.source == window && !!a.data.ciscoSDKClientMessage && (a.data.ciscoSDKClientMessage.name == "init" && a.data.ciscoChannelProperties.cwicExtId == chrome.runtime.id)) {
            cwic_ext.init(a.data)
        } else {
            console.log("[cwic.ext] ciscoSDKClientMessage received on main window but it is not 'init' or not for our ext id.", a.data);
        }
    }, !1));
}