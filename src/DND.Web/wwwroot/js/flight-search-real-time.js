const flightSearchRealtimeConnection = new signalR.HubConnectionBuilder()
    .withUrl("/flight-search-real-time")
    .build();

flightSearchRealtimeConnection.on("ReceiveSearchResult", (searchResult) => {
    alert(searchResult);
});

flightSearchRealtimeConnection.start().catch(err => console.error(err.toString()));

document.getElementById("sendButton").addEventListener("click", event => {
    const request = 'request';
    flightSearchRealtimeConnection.invoke("Search", request).catch(err => console.error(err.toString()));
    event.preventDefault();
});