var jabberWebApi = function () {
    var currentPresence = "";
    function jabberWebApiSendPresence(status, show) {
        if (status == null || status.length == 0) {
            status = "NONE";
        }
        if (show == null || show.length == 0) {
            show = "NONE";
        }

        if (show.indexOf("Not Ready") != -1) {
            status = "dnd";
        }
        
        window.postMessage({ event: "FinessePresenceStatus", data: { status: status, show: show} }, "*");
    }

    function handleFinessePresenceStatusSuccess(data)
    {
        console.log("Presence Status Sucessfully Sent",data);
    }

    return {
        init: function () {
            console.log("Jabber Web Api Loaded");

            // Add Message listener for Background => Foreground communication
            //  The Root level event is 'MessageFromBackground', which contains the real event
            window.addEventListener("message", function (message) {
                if (message.source == window && message.data.event == "MessageFromBackground") {
                    switch (message.data.data.event) {
                        case "FinessePresenceStatus":
                            jabberWebApi.handleFinessePresenceStatus(message.data.data.data);
                            break;
                    }
                }
            });

            // Add Background => Foreground listener for 'FinessePresenceStatusSuccess'
            window.postMessage({ event: "AddBackgroundListener", data: { event: "FinessePresenceStatus", handler: "FinessePresenceStatusSuccess" } }, "*");

            // Set up UI watchers
            /*$("#sdmode").change(function () {
                jabberWebApiSendPresence(null, $(this).val());
            });

            $("#username").change(function () {
                jabberWebApiSendPresence(null, $(this).val());
            });*/

            currentPresence = $("a[id=state-text]").text();
            window.setInterval(function () {
                if (currentPresence != $("a[id=state-text]").text()) {
                    currentPresence = $("a[id=state-text]").text();
                    jabberWebApiSendPresence(null, $("a[id=state-text]").text());
                }
            }, 1000);
        },
        handleFinessePresenceStatus: function(data)
        {
            switch(data.handler)
            {
                case "FinessePresenceStatusSuccess":
                    handleFinessePresenceStatusSuccess(data);
                    break;
            }
        }
    }
}();

jabberWebApi.init();

