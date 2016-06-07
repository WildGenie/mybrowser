var cwic_plugin = function () {
    function c() {
        var a = typeof arguments[0] == "boolean" ? arguments[0] : !1
          , c = typeof arguments[0] == "string" ? arguments[0] : arguments[1]
          , d = typeof arguments[1] == "object" ? arguments[1] : arguments[2];
        if (!a || a && b.verbose) {
            var e = new Date
              , f = ("0" + e.getHours()).slice(-2) + ":" + ("0" + e.getMinutes()).slice(-2) + ":" + ("0" + e.getSeconds()).slice(-2) + "." + ("00" + e.getMilliseconds()).slice(-3) + " ";
            d ? console.log("[cwic.ext] " + f + c, d) : console.log("[cwic.ext] " + f + c)
        }
    }
    var a = $.noop
      , b = {};
    loadExtProps = function () {
        var a = new XMLHttpRequest;
        a.onreadystatechange = function () {
            a.readyState == 4 && $.extend(b, JSON.parse(a.responseText))
        }
        ,
        a.open("GET", "chrome-extension://" + b.cwicExtId + "/extProps.json", !1);
        try {
            a.send()
        } catch (d) {
            c("Couldn not load extProps.json", d)
        }
    }
    ,
    append_iframe = function () {
        var a = document.createElement("iframe");
        a.id = b.objectId,
        a.style.display = "none",
        a.onload = function () {
            c(!0, "append_iframe onload"),
            $("#" + b.objectId)[0].contentWindow.addEventListener("message", receiveMessage, !1);
            try {
                // You need to be specific with this chrome javascript engine
                //$("#" + b.objectId)[0].contentWindow.postMessage(initMsg, location.origin)
                //cwic_ext.relay(initMsg)
                // We need to make an iframe for the contentscript
                console.log(chrome.runtime);
                // One small change, cef does not support isolated space, which is some sort of hidden iframe with bindings to the home document
                // So just load the script
                append_contentscript();
            } catch (a) {
                c("Failed posting message to " + b.objectId, a)
            }
        }
        ,
        a.onerror = function () {
            c("Could not load iframe.")
        }
        ,
        document.body.appendChild(a)
    }
    ,
    append_contentscript = function () {
        var a = document.createElement("script");
        a.id = b.objectId,
        a.src = "chrome-extension://" + b.cwicExtId + "/contentscript.js",
        a.onload = function () {
            c(!0, "append_contentscript onload");
            try {
                cwic_ext.relay(initMsg)
            } catch (a) {
                c(a)
            }
        }
        ,
        a.onerror = function () {
            c("Could not load contentscript.")
        }
        ,
        document.head.appendChild(a)
    }
    ,
    receiveMessage = function (d) {
        
        if (d.source != window) {
            c("Received message from unexpected source on " + b.objectId, d)
        } else {
            if (d.data.ciscoChannelServerMessage || d.data.ciscoSDKServerMessage) {
                
                a(d.data);
            } else if (d.data.ciscoChannelProperties || d.data.ciscoSDKClientMessage) {
                //alert(window.location);
                //console.log(cwic_ext);
                //cwic_ext.relay(d.data);
            }
            else {
                c("Received unexpected message data on " + b.objectId, d.data);
            }
        }
    }
    ;
    return {
        about: function () {
            return b
        },
        init: function (d, e) {
            if (!$.isFunction(d))
                c("cwic ext cannot load with invalid response handler");
            else {
                c("loaded with origin: " + location.origin);
                try {
                    $("#" + b.objectId).remove()
                } catch (f) { }
                a = d,
                b = e,
                loadExtProps(),
                initMsg = {
                    ciscoSDKClientMessage: {
                        name: "init",
                        messageId: "0"
                    },
                    ciscoChannelProperties: b
                },
                append_iframe()
                //location.protocol === "chrome-extension:" ? append_contentscript() : append_iframe()
            }
        },
        sendRequest: function (a) {
            !a.ciscoSDKClientMessage || (typeof cwic_ext != "undefined" && $.isFunction(cwic_ext.relay) ? cwic_ext.relay(a) : $("#" + b.objectId)[0].contentWindow.postMessage(a, location.origin))
        },
        directReply: function (b) {
            a(b)
        }
    }
}();
