var signalRNotificationsConnection = null;

function setupSignalRNotificationsConnection()
{
    signalRNotificationsConnection = new signalR.HubConnectionBuilder()
        .withUrl("/api/signalR/notifications")
        .build();

    signalRNotificationsConnection.on("ReceiveMessage", function (message) {
        alert(message);
    }
    );

    signalRNotificationsConnection.on("Finished", function () {
        signalRNotificationsConnection.stop();
    }
    );

    signalRNotificationsConnection.start()
        .catch(function (err) {
            console.error(err.toString());
        });
};

setupSignalRNotificationsConnection();