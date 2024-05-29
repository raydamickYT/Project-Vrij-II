document.addEventListener("DOMContentLoaded", function() {
    hideReconnectWidget(); // Verberg de widget expliciet bij het laden van de pagina

    initializeWebSocket(
        handleWebSocketMessage,
        () => hideReconnectWidget(), // Verberg de reconnect-widget bij het openen van de verbinding
        () => showReconnectWidget(), // Toon de reconnect-widget bij het sluiten van de verbinding
        (error) => console.error('WebSocket fout:', error)
    );

    document.getElementById('Join Lobby').addEventListener('click', () => {
        if (socket.readyState === WebSocket.OPEN) {
            const message = { lobbyStatus: 'inLobby', message: 'Deze client is gemarkeerd als: zit in de lobby' };
            socket.send(JSON.stringify(message));
            window.location.href = 'UnityPage.html'; // Navigeer naar de game pagina
        } else {
            console.log('WebSocket is niet open.');
        }
    });

    document.getElementById('UnityActions').addEventListener('click', () => {
        if (socket.readyState === WebSocket.OPEN) {
            const message = { success: true, message: "operatie voltooid", type: "PerformUnityAction" }; // Zet wat in de message
            socket.send(JSON.stringify(message));
            var btn = document.getElementById('UnityActions');
            btn.style.display = 'none'; // Verberg de knop om te voorkomen dat er 2 keer op gedrukt wordt. In dit geval willen we 1 input
        } else {
            console.log('WebSocket is niet open.');
        }
    });
});

function handleWebSocketMessage(message) {
    console.log('Bericht ontvangen:', message);
    if (message.type === "ShowButton") {
        toggleButton(true); // Toon de knop
    } else if (message.type === "HideButton") {
        toggleButton(false); // Verberg de knop
    }
}

function toggleButton(show) {
    var btn = document.getElementById('UnityActions');
    if (show) {
        btn.style.display = 'block'; // Toon de knop
    } else {
        btn.style.display = 'none'; // Verberg de knop
    }
}

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
