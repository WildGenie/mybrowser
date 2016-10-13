(function () {
    // Don't clutter up Page frontend in the event ther are collisions
    var currentPresence = "";
    var currentAction = "";
    var watchSelector = "state-text";
    //var watchSelector = "state-text";
    var backgroundCallbacks = new Array();
    var backgroundCallbackData = new Array();

    function FinessePresenceStatus(show, adj) {
        var status = "NONE";

        if (adj.indexOf("Busy") != -1)
        {
            status = "busy"
        }
        else if (adj.indexOf("Not Ready") != -1) {
            status = "dnd";
        }
        else
        {
            show = "NONE";
        }
        backgroundCallbackData["FinessePresenceStatus"] = show;
        var fpReq = new XMLHttpRequest();
        //fpReq.addEventListener("load", jabberWebApiMessageSentHandler);
        //fpReq.addEventListener("readystatechange", jabberWebApiReadyStateChanged);
        fpReq.open("GET", "http://localhost:9783/api/presence?status=" + status + "&show=" + show + "&ts=" + new Date().getTime());
        fpReq.send();
    }

    function CallPresenceStatus(callStatus) {
        backgroundCallbackData["FinesseCallStatus"] = callStatus;
        var fpReq = new XMLHttpRequest();
        //fpReq.addEventListener("load", jabberWebApiMessageSentHandler);
        //fpReq.addEventListener("readystatechange", jabberWebApiReadyStateChanged);
        fpReq.open("GET", "http://localhost:9783/api/callstatus?status=" + callStatus + "&ts=" + new Date().getTime());
        fpReq.send();
    }

    currentPresence = getValueFromSelector(watchSelector)
    window.setInterval(function () {
        currentPresence = getValueFromSelector(watchSelector);
        var adjustedcurrentPresence = mapWithTable(currentPresence);
        if (currentPresence != "")
        {
            FinessePresenceStatus(currentPresence, adjustedcurrentPresence);
        }

        var callPresence = handleCall();
        if(callPresence != "")
        {
            CallPresenceStatus(callPresence)
        }

        SetPlacingOutboundCall();
        /*if (currentPresence != getValueFromSelector(watchSelector)) {
            currentPresence = getValueFromSelector(watchSelector);
            //alert(currentPresence);
            FinessePresenceStatus(currentPresence);
        }*/
    }, 1000);

    function SetPlacingOutboundCall()
    {
        // Get Current CWIC status

        // IF Finesses "Not Ready" and "Off The Hook" then set 'Not Ready - Placing outbound Call'

    }

    function getValueFromSelector(selector)
    {
        try
        {
            var selected = document.getElementById(selector);
            var retVal = selected.value;
            if(retVal == undefined)
            {
                retVal = selected.text;
            }
            return retVal;
        }
        catch (exception)
        {
            return "";
        }

    }

    function handleCall()
    {
        try {
            var callListArea = document.getElementById("call-list-area")
            var callListElements = callListArea.getElementsByTagName("ul");
            var totalLength = 0;
            for (var i = 0; i < callListElements.length; i++) {
                switch (callListElements[i].id) {
                    case "call-incoming-list":
                    case "call-wrapup-list":
                    case "call-active-list":
                    case "call-outgoing-list":
                        totalLength += callListElements[i].innerText.trim().length;
                        break;
                }
            }
            if (totalLength > 0) {
                return "On_A_Call";
            }
            else
            {
                return "Not_On_A_Call";
            }
        }
        catch (exception) {
            return "";
        }
    }


    function mapWithTable(currentPresence) {
        try {
            var searchStrings = ["approved busy", "escalated task", "meeting", "placing outbound call", "training"];
            var retVal = currentPresence;

            for (var i = 0; i < searchStrings.length; i++) {
                if (currentPresence.toLowerCase().indexOf(searchStrings[i]) != -1) {
                    retVal = "Busy";
                }
            }
            return retVal;
        }
        catch (exception) {
            return currentPresence;
        }
    }
})();