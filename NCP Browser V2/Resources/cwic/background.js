function handleClientPortRequest(a) {
    console.log("Incoming new port:", a);
    var b = null
      , c = null
      , d = function (a) {
          c = a
      }
      , e = function (a) {
          return c
      }
    ;
    if (!a.sender || !a.sender.id)
        throw {
            name: "port missing id",
            port: a
        };
    var f = a.sender.id
      , g = "";
    f === location.host ? (a.sender.tab.url ? b = a.sender.tab.url : a.sender.url ? b = a.sender.url : console.log("FAILED determining origin for port"),
    a.sender.tab.title ? d(a.sender.tab.title) : console.log("FAILED determining origin for port")) : (g = "Chrome",
    b = "chrome-extension://" + f + "/",
    chrome.management.get(f, function (a) {
        console.log("Info for port.sender.id:"),
        console.log(a),
        a.name ? d(a.name) : console.log("Strange: object has no name")
    }));
    var h = getnextlocalportid();
    console.log("local port id = " + h);
    var i = getHostnameFromUrl(b);
    console.log("portSenderUrl->hostname = " + i);
    var j = {
        id: h,
        url: b,
        hostname: i
    };
    g && (j.type = g),
    c && (j.name = c),
    a.client = j,
    a.getportname = e,
    ports[h] = a,
    console.log(ports),
    a.onMessage.addListener(handleClientMessage),
    a.onDisconnect.addListener(handleClientDisconnect)
}
function handleClientMessage(a, b) {
    var c = b.getportname();
    c && c.length > 0 && (b.client.name = c),
    a.client = b.client,
    console.log("handleClientMessage from client:", a.client),
    a.ciscoChannelProperties && console.log("initializing channel with props: ", a.ciscoChannelProperties);
    if (!a.ciscoSDKClientMessage && !a.clientConnected)
        console.log("handleClientMessage ignoring unknown message");
    else {
        var d = a.ciscoSDKClientMessage.name
          , e = d === "encryptCucmPassword" ? "*****" : a.ciscoSDKClientMessage.content;
        console.log("ciscoSDKClientMessage '" + d + "' with content:", e);
        if (d === "releaseInstance" || a.clientConnected === !1) {
            var f = ports[a.client.id];
            f.disconnect(),
            handleClientDisconnect(f);
            return
        }
        nativeport || connectNative();
        var g = {
            ciscoChannelMessage: a
        };
        postToNative(g)
    }
}
function connectNative() {
    try {
        nativeport = chrome.runtime.connectNative("com.cisco.jabber.jsdk")
    } catch (a) {
        console.log("Failed connecting to native port"),
        console.log(a),
        nativeport = null,
        sendChannelDisconnectMessage(a);
        return
    }
    console.log("native port to host: " + hostName, nativeport),
    nativeport.onMessage.addListener(handleNativeMessage),
    nativeport.onDisconnect.addListener(function () {
        console.log("Received nativeport.onDisconnected"),
        nativeport = null,
        sendChannelDisconnectMessage(chrome.extension.lastError)
    })
}
function handleNativeMessage(a) {
    var b = a;
    if (typeof a == "string")
        try {
            b = JSON.parse(a)
        } catch (c) {
            console.log("Message from native side failed JSON.parse, discarding"),
            console.log(c);
            return
        }
    console.log("Received from native:", b);
    if (b.ciscoChannelMessage) {
        var d = null;
        b.ciscoChannelMessage.ciscoSDKServerMessage && (d = {
            ciscoSDKServerMessage: b.ciscoChannelMessage.ciscoSDKServerMessage
        });
        if (!d) {
            console.log("Message missing ciscoSDKServerMessage. Nothing to route to client");
            return
        }
        console.log("extracted ciscoSDKServerMessage '" + d.ciscoSDKServerMessage.name + "'");
        if (b.ciscoChannelMessage.client)
            if (b.ciscoChannelMessage.client.id) {
                var e = b.ciscoChannelMessage.client.id
                  , f = ports[e];
                f ? f.postMessage(d) : console.log("port [" + e + "] not found. Unable to route message.")
            } else
                console.log("Don't know how to route the message, missing ciscoChannelMessage.client.id");
        else
            broadcastToClients(d)
    } else
        console.log("Don't know how to parse the message that's not ciscoChannelMessage")
}
function handleClientDisconnect(a) {
    delete ports[a.client.id];
    var b = Object.keys(ports).length;
    console.log("Client port destroyed. " + b + " client ports remaining.");
    if (nativeport) {
        var c = {
            ciscoChannelMessage: {
                client: a.client,
                clientConnected: !1
            }
        };
        postToNative(c),
        b === 0 && (console.log("Last client port removed.  Disconnecting from native client."),
        nativeport.disconnect(),
        nativeport = null)
    }
}
function postToNative(a) {
    console.log("posting message to native port"),
    hostName === "com.cisco.jabber.jsdk" && delete a.ciscoChannelMessage.ciscoChannelProperties;
    try {
        console.log(a),
        nativeport.postMessage(a)
    } catch (b) {
        console.log("exception posting to native port, assume it's dead:"),
        console.log(b),
        nativeport = null,
        sendChannelDisconnectMessage(b)
    }
}
function sendChannelDisconnectMessage(a) {
    var b = {
        ciscoChannelServerMessage: {
            name: "HostDisconnect",
            cause: a
        }
    };
    broadcastToClients(b)
}
function broadcastToClients(a) {
    console.log("broadcast message:", a);
    if (Object.keys(ports).length > 0) {
        console.log("ports:"),
        console.log(ports);
        for (var b in ports)
            if (ports[b])
                try {
                    ports[b].postMessage(a)
                } catch (c) {
                    console.log(c),
                    console.log("Failed posting server message to client port, discarding."),
                    console.log(ports[b])
                }
    } else
        console.log("No connected client ports to broadcast on.")
}
function getnextlocalportid() {
    lastlocalportid = lastlocalportid + 1;
    return CLIENT_ID_BASE + "." + lastlocalportid.toString()
}
function getHostnameFromUrl(a) {
    var b = document.createElement("a");
    b.href = a;
    var c = b.hostname;
    return c
}
var ports = {}
  , CLIENT_ID_BASE = Math.floor(Math.random() * 1e6 + 1).toString()
  , lastlocalportid = 100
  , nativeport = null
  , hostName = "com.cisco.jabber.jsdk";

// TODO, fix this event
//document.addEventListener("DOMContentLoaded", function () {
    console.log("Background: DomContentLoaded"),
    console.log("Extension URL: " + chrome.extension.getURL("")),
    console.log("document.domain: " + document.domain)
//}),
chrome.runtime.onConnect.addListener(handleClientPortRequest),
chrome.runtime.onConnectExternal.addListener(handleClientPortRequest);
