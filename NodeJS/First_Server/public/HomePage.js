const socket = new WebSocket('ws://localhost:3000');

socket.onopen = function (event) {
    console.log('Verbinding geopend:', event);
    toggleButton();
};

socket.onmessage = function (event) {
    console.log('Bericht van server:', event.data);
   
    try {
        var debuggedMessage = JSON.parse(event.data);
        console.log(debuggedMessage.message + "2nd try");
        if(debuggedMessage.type === "ShowButton"){
            console.log(debuggedMessage.type + " Type");
            toggleButton();
        }
    } catch (error) {
        console.error('Error parsing JSON:', error);
        console.log('Received data:', event.data);
    }
};

socket.onerror = function (error) {
    console.error('WebSocket fout:', error);
};

socket.onclose = function (event) {
    console.log('WebSocket verbinding gesloten:', event);
};

document.getElementById('Join Lobby').addEventListener('click', () => {
    if (socket.readyState === WebSocket.OPEN) {
        const message = { lobbyStatus: 'inLobby', message: 'Deze client is gemarkeerd als: zit in de lobby' };
        socket.send(JSON.stringify(message));
    } else {
        console.log('WebSocket is niet open.');
    }
});

document.getElementById('UnityActions').addEventListener('click', () => {
    if (socket.readyState === WebSocket.OPEN) {
        const message = {success: true, message: "operatie voltooid", type: "PerformUnityAction"}; //zet wat in de message
        socket.send(JSON.stringify(message));
    } else {
        console.log('WebSocket is niet open.');
    }
});


function toggleButton() {
    var btn = document.getElementById('UnityActions');
    if (btn.style.display === 'none') {
        btn.style.display = 'block';  // Toon de knop als deze verborgen is
    } else {
        btn.style.display = 'none';  // Verberg de knop als deze zichtbaar is
    }
};
