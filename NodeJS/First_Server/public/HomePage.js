var socket;
var reconnectInterval = 5000; // 5 seconden interval voor opnieuw verbinding maken

document.addEventListener("DOMContentLoaded", function() {
    hideReconnectWidget(); // Verberg de widget expliciet bij het laden van de pagina
    initializeWebSocket();

    document.getElementById('Join Lobby').addEventListener('click', () => {
        if (socket.readyState === WebSocket.OPEN) {
            const message = { lobbyStatus: 'inLobby', message: 'Deze client is gemarkeerd als: zit in de lobby' };
            socket.send(JSON.stringify(message));
            var btn = document.getElementById('Join Lobby');
            btn.style.display = 'none'; // Verberg de knop om te voorkomen dat er 2 keer op gedrukt wordt
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

function initializeWebSocket() {
    // Bouw de WebSocket URL dynamisch op
    var wsHost = window.location.hostname;
    var wsPort = window.location.port ? `:${window.location.port}` : ''; 
    var wsProtocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
    var wsPath = '/ws'; // Pas deze aan als je WebSocket pad anders is

    var wsUrl = `${wsProtocol}//${wsHost}${wsPort}${wsPath}`;
    socket = new WebSocket(wsUrl);

    console.log(wsUrl);

    // Handler voor als de WebSocket-verbinding wordt geopend
    socket.onopen = function(event) {
        console.log('WebSocket verbinding geopend:', event);
        hideReconnectWidget(); // Verberg de reconnect-widget
    };

    socket.onmessage = function(event) {
        console.log('Bericht van server:', event.data);

        try {
            var debuggedMessage = JSON.parse(event.data);
            console.log(debuggedMessage.message + "2nd try");
            if (debuggedMessage.type === "ShowButton") {
                console.log(debuggedMessage.type + " Type");
                toggleButton(true); // Toon de knop
            }
        } catch (error) {
            console.error('Error parsing JSON:', error);
            console.log('Received data:', event.data);
        }
    };

    socket.onerror = function(error) {
        console.error('WebSocket fout:', error);
    };

    socket.onclose = function(event) {
        console.log('WebSocket verbinding gesloten:', event);
        showReconnectWidget(); // Toon de reconnect-widget
        attemptReconnect();
    };
}

function attemptReconnect() {
    console.log(`Proberen opnieuw verbinding te maken over ${reconnectInterval / 1000} seconden...`);
    setTimeout(() => {
        console.log('Opnieuw verbinden...');
        initializeWebSocket();
    }, reconnectInterval);
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
