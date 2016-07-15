var backgroundCallbacks = new Array();
var backgroundCallbackData = new Array();
function FinessePresenceStatus(data)
{
    backgroundCallbackData["FinessePresenceStatus"] = data;
    var fpReq = new XMLHttpRequest();    
    fpReq.addEventListener("load", jabberWebApiMessageSentHandler);
    fpReq.addEventListener("readystatechange", jabberWebApiReadyStateChanged);
    fpReq.open("GET", "http://localhost:9783/api/presence?status=" + data.status + "&show=" + data.show + "&ts=" + new Date().getTime());    
    fpReq.send();
}

function jabberWebApiMessageSentHandler()
{
    if(this.responseText.indexOf("success") == -1)
    {
        window.postMessage({ event: "messageSent", data: this.responseText }, "*");
    }
    else
    {
        if(backgroundCallbacks["FinessePresenceStatus"] && backgroundCallbacks["FinessePresenceStatus"].length > 0)
        {
            dispatchMessageHandlerEvents("FinessePresenceStatus", backgroundCallbacks["FinessePresenceStatus"]);
        }
    }
}

function dispatchMessageHandlerEvents(callBackName, backgroundCallBack)
{
    if (!backgroundCallbackData["FinessePresenceStatus"]) {
        backgroundCallbackData["FinessePresenceStatus"] = "NA";
        //delete backgroundCallbackData["FinessePresenceStatus"];
    }
    browser.tabs.query({ active: true }, function (result) {
        for (var j = 0; j < result.length; j++) {
            for (var i = 0; i < backgroundCallBack.length; i++) {
                browser.tabs.sendMessage(result[j].id, { event: callBackName, data: { handler: backgroundCallBack[i], data: backgroundCallbackData["FinessePresenceStatus"] } });
            };
        }
        delete backgroundCallbackData["FinessePresenceStatus"]
    });
}

function jabberWebApiReadyStateChanged()
{
    if (this.readyState == 4 && this.status != 200) {
        window.postMessage({ event: "loadError", data: "Unable to load" }, "*");
    }
}

function addBackgroundCallback(event, handler)
{
    if(!backgroundCallbacks[event])
    {
        backgroundCallbacks[event] = new Array();
    }

    var found = false;
    for (var i = 0; i < backgroundCallbacks[event].length; i++)
    {
        if(backgroundCallbacks[event][i] == handler)
        {
            found = true;
            break;
        }
    }
    if (!found)
    {
        // Only register the event one time
        backgroundCallbacks[event].push(handler);
    }
    
}

function messageHandler(message)
{
    switch(message.event)
    {
        case "FinessePresenceStatus":
            FinessePresenceStatus(message.data);
            break;
        case "AddBackgroundListener":
            addBackgroundCallback(message.data.event, message.data.handler);
            break;
    }
}

plugin = messageHandler;