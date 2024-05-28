var socket;
var reconnectInterval = 5000; // 5 seconden interval voor opnieuw verbinding maken

function initializeWebSocket(onMessageCallback, onOpenCallback, onCloseCallback, onErrorCallback) {
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
        if (onOpenCallback) onOpenCallback(event);
    };

    // Handler voor berichten van de server
    socket.onmessage = function(event) {
        console.log('Bericht van server:', event.data);

        try {
            var debuggedMessage = JSON.parse(event.data);
            if (onMessageCallback) onMessageCallback(debuggedMessage);
        } catch (error) {
            console.error('Error parsing JSON:', error);
            console.log('Received data:', event.data);
        }
    };

    // Handler voor fouten
    socket.onerror = function(error) {
        console.error('WebSocket fout:', error);
        if (onErrorCallback) onErrorCallback(error);
    };

    // Handler voor sluiten van de verbinding
    socket.onclose = function(event) {
        console.log('WebSocket verbinding gesloten:', event);
        if (onCloseCallback) onCloseCallback(event);
        attemptReconnect(onMessageCallback, onOpenCallback, onCloseCallback, onErrorCallback);
    };
}

function attemptReconnect(onMessageCallback, onOpenCallback, onCloseCallback, onErrorCallback) {
    console.log(`Proberen opnieuw verbinding te maken over ${reconnectInterval / 1000} seconden...`);
    setTimeout(() => {
        console.log('Opnieuw verbinden...');
        initializeWebSocket(onMessageCallback, onOpenCallback, onCloseCallback, onErrorCallback);
    }, reconnectInterval);
}
