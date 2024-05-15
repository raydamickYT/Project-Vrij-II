const socket = new WebSocket('ws://localhost:3000');

socket.onopen = function (event) {
    console.log('Verbinding geopend:', event);
};

socket.onmessage = function (event) {
    console.log('Bericht van server:', event.data);
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
        const message = {success: true, message: "operatie voltooid"}; //zet wat in de message
        socket.send(JSON.stringify(message));
    } else {
        console.log('WebSocket is niet open.');
    }
})
