var signalRFlightSearchConnection = null;

function setupSignalRFlightSearchConnection()
{
    signalRFlightSearchConnection = new signalR.HubConnectionBuilder()
        .withUrl("/api/signalr/flight-search")
        .build();

    signalRconnection.on("ReceiveResult", (result) => {
        alert(result);
    }
    );

    signalRFlightSearchConnection.on("Finished", function () {
        connection.stop();
    }
    );

    signalRFlightSearchConnection.start()
        .catch(err => console.error(err.toString()));
};

setupSignalRFlightSearchConnection();

document.getElementById("submit").addEventListener("click", e => {
    e.preventDefault();

    const product = document.getElementById("product").value;
    const size = document.getElementById("size").value;

    fetch("/api/flight-search",
        {
            method: "POST",
            body: JSON.stringify({ product, size }),
            headers: {
                'content-type': 'application/json'
            }
        })
        .then(response => response.text())
        .then(id => signalRFlightSearchConnection.invoke("GetResults", id));
});