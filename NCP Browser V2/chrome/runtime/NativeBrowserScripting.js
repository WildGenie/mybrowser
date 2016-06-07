// Base Runtime
if (!chrome.runtime) {
    chrome.runtime = {};
    chrome.runtime.id = ncp_runtime.id;
}

// On Connect and handlers/events
if (!chrome.runtime.onConnect) {
    chrome.runtime.onConnect = {};
    chrome.runtime.onConnect.listeners = new Array();

    chrome.runtime.onConnect.addListener = function (method) {
        chrome.runtime.onConnect.listeners.push(method);
    }

    chrome.runtime.onConnect.fire = function (port) {
        for (var i = 0; i < chrome.runtime.onConnect.listeners.length; i++) {
            chrome.runtime.onConnect.listeners[i](JSON.parse(port));
        }
    }
}

// On Connect External handlers/events
if (!chrome.runtime.onConnectExternal) {
    chrome.runtime.onConnectExternal = {};
    chrome.runtime.onConnectExternal.listeners = new Array();

    chrome.runtime.onConnectExternal.addListener = function (method) {
        chrome.runtime.onConnectExternal.listeners.push(method);
    }

    chrome.runtime.onConnectExternal.fire = function (port, channelId) {
        for (var i = 0; i < chrome.runtime.onConnectExternal.listeners.length; i++) {
            port = JSON.parse(port);
            port.channelId = channelId;
            port.onMessage = {};
            port.onMessage.listeners = new Array();
            port.onMessage.addListener = function (method) {
                this.listeners.push(method);
            }
            port.onMessage.fire = function (message, port) {
                for (var i = 0; i < this.listeners.length; i++) {                    
                    this.listeners[i](message, port);
                }
            }

            port.onDisconnect = {};
            port.onDisconnect.listeners = new Array();
            port.onDisconnect.addListener = function (method) {
                this.listeners.push(method);
            }
            port.onDisconnect.fire = function (message) {
                for (var i = 0; i < this.listeners.length; i++) {
                    this.listeners[i](message);
                }
            }

            port.postMessage = function (message) {
                ncp_runtime.receiveMessage(this.channelId, JSON.stringify(message));
            }

            chrome.runtime.port.openPorts.push(port)

            chrome.runtime.onConnectExternal.listeners[i](port);
        }
    }
}

if (!chrome.runtime.port) {
    chrome.runtime.port = {};
    chrome.runtime.port.openPorts = new Array();
    chrome.runtime.onPortMessage = {};
    chrome.runtime.onPortMessage.fire = function (message, channelId) {
        message = JSON.parse(message);
        for (var i = 0; i < chrome.runtime.port.openPorts.length; i++) {
            if (chrome.runtime.port.openPorts[i].channelId == channelId) {
                chrome.runtime.port.openPorts[i].onMessage.fire(message, chrome.runtime.port.openPorts[i]);
            }
        }
    }
}

if (!chrome.runtime.nativePort) {
    chrome.runtime.nativePort = {};
    chrome.runtime.nativePort.openPorts = new Array();
}

if(!chrome.runtime.onNativeMessage) {
    chrome.runtime.onNativeMessage = {};
    chrome.runtime.onNativeMessage.fire = function(message) {
        // Send message to all open ports
        for (var i = 0; i < chrome.runtime.onMessage.listeners.length; i++) {
            chrome.runtime.onMessage.fire(message);
        }
    }
}



// Connect Native method + handlers/events
if (!chrome.runtime.connectNative)
{
    chrome.runtime.connectNative = function (x) {

        if (!chrome.runtime.onMessage) {
            chrome.runtime.onMessage = {};
            chrome.runtime.onMessage.listeners = new Array();
            chrome.runtime.onMessage.fire = function (data) {
                //if (portId == chrome.runtime.id) {
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

        port = ncp_runtime.connectNative(x,window.location.hostname);
        returnPort = {
            name: port.name,
            disconnect: function () {
                // TODO
                chrome.runtime.onDisconnect.fire(this);
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
                    //ncp_runtime.onMessageListeners(port.name, chrome.runtime.onMessage.listeners.length);
                }
            },
            postMessage: function (message) {
                //delete message.ciscoChannelProperties;
                //message.client = this.client;
                //message = {
                //    ciscoChannelMessage: message
                //};
                message = JSON.stringify(message);
                ncp_runtime.postMessage(this.name, message);
            }            
        };
        return returnPort;
    };
}

// Base Management
if (!chrome.management) {
    chrome.management = {};
    chrome.management.asynchronousCallbacks = new Array();
}

// Management Get
if (!chrome.management.get)
{
    // This method is async
    chrome.management.get = function (extensionId, callback) {
        // build a callback id to use as a key for the callback list
        var d = new Date();
        var callbackId = extensionId + d.getTime();

        // add callback to list
        chrome.management.asynchronousCallbacks[callbackId] = callback;

        // call async browserscripting method
        ncp_runtime.management_get(extensionId, callbackId);
    }
}

// Management Get CallBack
if (!chrome.management.getCallback)
{
    // This method is an async callback
    chrome.management.getCallback = function (extensionInfo, callbackId) {
        if(chrome.management.asynchronousCallbacks(callbackId))
        {
            // get a reference to the method
            var cb = chrome.management.asynchronousCallbacks[callbackId];

            // delete method from callback list
            delete chrome.management.asynchronousCallbacks[callbackId];

            // call method
            cb(JSON.parse(extensionInfo));
        }
    }
}

// Extension
if (!chrome.extension) {
    chrome.extension = {}
}

if (!chrome.extension.getURL) {
    chrome.extension.getURL = function (url) {
        return ncp_runtime.extension_getURL(url);
    }
}