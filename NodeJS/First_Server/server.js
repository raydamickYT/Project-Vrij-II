const WebSocket = require('ws');
const http = require('http');
const express = require('express');
const path = require('path');
const compression = require('compression');  // Voeg de compression middleware toe

const app = express();
const port = 3000;
const server = http.createServer(app);
const wssWebClients = new WebSocket.Server({ noServer: true });
const wssUnityClients = new WebSocket.Server({ noServer: true });

const clientsInLobby = new Set();
let unityClient = null;

// Gebruik de compression middleware
app.use(compression());

// Serve static files from the 'public' directory
app.use(express.static(path.join(__dirname, 'public')));

// Serve the WebGL build files
app.use('/UnityBuild', express.static(path.join(__dirname, 'public', 'UnityBuild')));

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

app.get('/unity', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', 'UnityPage.html'));
});

server.on('upgrade', (request, socket, head) => {
    const pathname = new URL(request.url, `http://${request.headers.host}`).pathname;

    if (pathname === '/ws') {
        wssWebClients.handleUpgrade(request, socket, head, (ws) => {
            wssWebClients.emit('connection', ws, request);
        });
    } else if (pathname === '/unity') {
        wssUnityClients.handleUpgrade(request, socket, head, (ws) => {
            wssUnityClients.emit('connection', ws, request);
        });
    } else {
        socket.destroy();
    }
});

// WebClient WebSocket server
wssWebClients.on('connection', function connection(ws, req) {
    console.log('Web client verbonden');

    ws.on('message', function incoming(data) {
        let decodedMessage;
        try {
            decodedMessage = JSON.parse(data);
            if (!decodedMessage) {
                console.log('Leeg bericht ontvangen.');
                return;
            }

            if (decodedMessage.lobbyStatus === 'inLobby') {
                handleLobbyJoin(ws, decodedMessage);
            } else {
                if (unityClient != null) {
                    forwardMessageToUnity(decodedMessage);
                } else {
                    console.log("Unity client is unassigned");
                }
            }
        } catch (error) {
            console.log('Fout bij het parsen van het bericht:', error);
        }
    });

    ws.on('close', () => {
        console.log('Verbinding gesloten');
        clientsInLobby.delete(ws);
        broadcastConnectionCount();
    });

    ws.on('error', error => {
        console.error('Fout:', error);
    });

    broadcastConnectionCount();
});

// Unity WebSocket server
wssUnityClients.on('connection', function connection(ws, req) {
    console.log('Unity client verbonden');
    unityClient = ws;

    broadcastUnityConnectionStatus(true); // Unity connected

    ws.on('message', function incoming(data) {
        let decodedMessage;
        try {
            decodedMessage = JSON.parse(data);
            if (!decodedMessage) {
                console.log('Leeg bericht ontvangen.');
                return;
            }

            broadcastToLobby(decodedMessage);
        } catch (error) {
            console.log('Fout bij het parsen van het bericht:', error);
        }
    });

    ws.on('close', () => {
        console.log('Verbinding gesloten');
        unityClient = null;
        broadcastUnityConnectionStatus(false); // Unity disconnected
        broadcastConnectionCount();
    });

    ws.on('error', error => {
        console.error('Fout:', error);
    });

    broadcastConnectionCount();
});

function broadcastUnityConnectionStatus(isConnected) {
    const message = {
        type: 'unityConnectionStatus',
        isConnected: isConnected
    };

    wssWebClients.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify(message));
        }
    });

    if (unityClient && unityClient.readyState === WebSocket.OPEN) {
        unityClient.send(JSON.stringify(message));
    }
}

function broadcastConnectionCount() {
    const count = wssWebClients.clients.size;
    console.log("Clients connected: " + count);
    
    // Verzend het `count` bericht naar alle web clients
    wssWebClients.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify({ type: 'count', count: count }));
        }
    });

    // Verzend het `count` bericht naar de Unity client
    if (unityClient && unityClient.readyState === WebSocket.OPEN) {
        unityClient.send(JSON.stringify({ type: 'count', count: count }));
    }
}

function handleLobbyJoin(ws, messageData) {
    clientsInLobby.add(ws);
    console.log("Mensen die in een lobby zitten: " + clientsInLobby.size);
    const convertedMessage = { success: true, message: "Joined lobby successfully", type: 'info' }; // Zet wat in de message
    ws.send(JSON.stringify(convertedMessage));
}

function forwardMessageToUnity(message) {
    if (unityClient) {
        unityClient.send(JSON.stringify(message));
    }
}

function broadcastToLobby(message) {
    clientsInLobby.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify(message));
            console.log('Bericht verstuurd naar client in de lobby');
        } else {
            console.log('Client verbinding niet open');
        }
    });
}

server.listen(port, () => {
    console.log('Server luistert op http://localhost:' + port);
});
