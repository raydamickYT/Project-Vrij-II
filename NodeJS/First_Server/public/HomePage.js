document.addEventListener("DOMContentLoaded", function() {
    hideReconnectWidget(); // Verberg de widget expliciet bij het laden van de pagina

    initializeWebSocket(
        handleWebSocketMessage,
        () => hideReconnectWidget(), // Verberg de reconnect-widget bij het openen van de verbinding
        () => showReconnectWidget(), // Toon de reconnect-widget bij het sluiten van de verbinding
        (error) => console.error('WebSocket fout:', error)
    );

    document.getElementById('Join Lobby').addEventListener('click', () => {
        console.log("knop werkt")
        window.location.href = 'UnityPage.html'; // Navigeer naar de game pagina
        // if (socket.readyState === WebSocket.OPEN) {
        //     const message = { lobbyStatus: 'inLobby', message: 'Deze client is gemarkeerd als: zit in de lobby' };
        //     socket.send(JSON.stringify(message));
        // } else {
        //     console.log('WebSocket is niet open.');
        // }
    });

});

function showReconnectWidget() {
    var widget = document.getElementById('reconnect-widget');
    widget.classList.remove('hidden');
    widget.style.display = 'flex'; // Toon de widget
}

function hideReconnectWidget() {
    var widget = document.getElementById('reconnect-widget');
    widget.classList.add('hidden');
    widget.style.display = 'none'; // Verberg de widget
}
