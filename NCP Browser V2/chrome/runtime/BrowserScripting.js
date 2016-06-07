var CurrentFrameIdentifier = "{!FrameID}";


if (!chrome.runtime) {
    chrome.runtime = {}
    chrome.runtime.channelId = Math.floor(Math.random() * 1e6 + 1).toString();
}

if(!chrome.runtime.connect)
{
    chrome.runtime.connect = function (x, y) {
        if (!chrome.runtime.onMessage) {
            chrome.runtime.onMessage = {};
            chrome.runtime.onMessage.listeners = new Array();
            chrome.runtime.onMessage.fire = function (data) {
                //if (portId == chrome.runtime.id) {
                data = JSON.parse(data);
                for (var i = 0; i < chrome.runtime.onMessage.listeners.length; i++) {
                    chrome.runtime.onMessage.listeners[i](data);
                }
                //}
            }
        }

        if (!chrome.runtime.onDisconnect) {
            chrome.runtime.onDisconnect = {};
            chrome.runtime.onDisconnect.listeners = new Array();
            chrome.runtime.onDisconnect.fire = function (data) {
                //if (portId == chrome.runtime.id) {
                    for (var i = 0; i < chrome.runtime.onDisconnect.listeners.length; i++) {
                        chrome.runtime.onDisconnect.listeners[i](data);
                    }
                //}
            }
        }

        sender = {
            frameId: CurrentFrameIdentifier,
            id: x,
            tab: {},
            url: window.location.href
        };

        connectPort = {
            name: y.name,
            sender: sender
        };

        port = ncp_runtime.connect(x,JSON.stringify(connectPort), chrome.runtime.channelId);
        returnPort = {
            name: port.name,
            sender: sender,
            disconnect: function () {
                // TODO
            },
            onDisconnect: {
                addListener: function (method) {
                    chrome.runtime.onDisconnect.listeners.push(method);

                    //TODO?
                    //ncp_runtime.onMessageListeners(port.name, chrome.runtime.onMessage.listeners.length);
                }
            },
            onMessage: {
                addListener: function (method) {
                    chrome.runtime.onMessage.listeners.push(method);
                    ncp_runtime.onMessageListeners(port.name, chrome.runtime.channelId, function (message) { method(JSON.parse(message)); });
                    //ncp_runtime.onMessageListeners(port.name, chrome.runtime.onMessage.listeners.length);
                }
            },
            postMessage: function (message) {
                message = JSON.stringify(message);
                ncp_runtime.postMessage(this.name, message, chrome.runtime.channelId);
            }            
        };

        return returnPort;
    };
}

ncp_runtime.DoneInitializing();
/*
if(!chrome.runtime.onConnectExternal.addListener)
{
    chrome.runtime.onConnectExternal.addListener = function (method) {
        chrome.runtime.onConnect.listeners.push(method);
    }
}

if(!chrome.runtime.connectExternal)
{
    chrome.runtime.connectExternal = function (x, y) {
        for (var i = 0; i < chrome.runtime.onConnect.listeners.length; i++) {
            ncp_runtime.connectExternal(x);
            chrome.runtime.onConnect.listeners[i](x);
        }
    };
}

if (!chrome.runtime.management.get)
{
    // This method is async
    chrome.runtime.management.get = function (extensionId, callback) {
        // build a callback id to use as a key for the callback list
        var d = new Date();
        var callbackId = extensionId + d.getTime();

        // add callback to list
        chrome.runtime.management.asynchronousCallbacks[callbackId] = callback;

        // call async browserscripting method
        ncp_runtime.management_get(extensionId, callbackId);
    }
}

if (!chrome.runtime.management.getCallback)
{
    // This method is an async callback
    chrome.runtime.management.getCallback = function (extensionInfo, callbackId) {
        if(chrome.runtime.management.asynchronousCallbacks(callbackId))
        {
            // get a reference to the method
            var cb = callbackId.runtime.management.asynchronousCallbacks[callbackId];

            // delete method from callback list
            delete callbackId.runtime.management.asynchronousCallbacks[callbackId];

            // call method
            cb(JSON.parse(extensionInfo));
        }
    }
}*/