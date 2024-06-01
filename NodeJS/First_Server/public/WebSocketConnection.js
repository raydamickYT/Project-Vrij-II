let socket;

document.addEventListener("DOMContentLoaded", function() {
    initializeWebSocket(
        null, // Geen specifieke actie nodig bij het openen van de verbinding
        () => console.log('Verbinding gesloten'), // Log de sluiting van de verbinding
        (error) => console.error('WebSocket fout:', error)
    );
});

function initializeWebSocket(onMessage, onOpen, onClose, onError) {
    const wsHost = window.location.hostname;
    const wsPort = window.location.port ? `:${window.location.port}` : ''; 
    const wsProtocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
    const wsPath = '/ws';

    const wsUrl = `${wsProtocol}//${wsHost}${wsPort}${wsPath}`;
    socket = new WebSocket(wsUrl);

    socket.onopen = function(event) {
        console.log('WebSocket verbinding geopend:', event);
        if (onOpen) onOpen(event);
    };

    socket.onmessage = function(event) {
        console.log('Bericht van server:', event.data);
        if (onMessage) onMessage(event.data);
    };

    socket.onerror = function(error) {
        console.error('WebSocket fout:', error);
        if (onError) onError(error);
    };

    socket.onclose = function(event) {
        console.log('WebSocket verbinding gesloten:', event);
        if (onClose) onClose(event);
    };
}

// Functie om berichten van Unity te ontvangen en door te sturen naar de server
function receiveMessageFromUnity(jsonMessage) {
    console.log('Bericht ontvangen van Unity:', jsonMessage);

    if (socket.readyState === WebSocket.OPEN) {
        socket.send(JSON.stringify({ type: 'PerformUnityAction', message: jsonMessage }));
    }
}

// Voorbeeld functie om een bericht terug te sturen naar Unity (optioneel)
function sendMessageToUnity(message) {
    if (typeof unityInstance !== 'undefined') {
        unityInstance.SendMessage('UnityToJavaScript', 'ReceiveMessageFromJavaScript', message);
        console.log('Bericht verzonden naar Unity:', message);
    }
}

function handleWebSocketMessage(message) {
    var test = JSON.parse(message)
    console.log('Bericht ontvangen2 :', test);
    if (test.type === "ShowButton") {
    console.log('tot hier gekomen');
        sendMessageToUnity(message);
    }
}


