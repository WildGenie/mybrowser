(function () {
    // Don't clutter up Page frontend in the event ther are collisions
    var currentPresence = "";
    var watchSelector = "username";
    //var watchSelector = "state-text";
    var backgroundCallbacks = new Array();
    var backgroundCallbackData = new Array();

    function FinessePresenceStatus(show) {
        var status = "NONE";

        if (show.indexOf("Not Ready") != -1) {
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

    currentPresence = getValueFromSelector(watchSelector)
    window.setInterval(function () {
        //console.log("tic");
        if (currentPresence != getValueFromSelector(watchSelector)) {
            currentPresence = getValueFromSelector(watchSelector);
            //alert(currentPresence);
            FinessePresenceStatus(currentPresence);
        }
    }, 1000);


    function getValueFromSelector(selector)
    {
        var selected = document.getElementById(selector);
        var retVal = selected.value;
        if(retVal == undefined)
        {
            retVal = selected.text;
        }
        return retVal;
    }

    self.port.emit("tacoEvent", "COOOKIES");
})();