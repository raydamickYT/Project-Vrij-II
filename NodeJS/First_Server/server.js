const WebSocket = require('ws');
const http = require('http');
const express = require('express');
const path = require('path');

const app = express();
const port = 3000;
const server = http.createServer(app);
const wss = new WebSocket.Server({ server });

const clientsInLobby = new Set();
let unityClient = null;

app.use(express.static('public'));

// Serve the WebGL build files
app.use('/UnityBuild', express.static(path.join(__dirname, 'public/UnityBuild')));

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

wss.on('connection', function connection(ws, req) {
    console.log('Client verbonden');

    if (req.url === '/unity') {
        ws.clientType = 'Unity';  // Voeg een vlag toe om te identificeren dat dit de Unity client is
        unityClient = ws;
        console.log('Unity client connected');
    } else {
        ws.clientType = 'WebClient';  // Markeer als een reguliere webclient
        console.log('Web client connected');
    }

    ws.on('message', function incoming(data) {
        let decodedMessage;
        console.log(ws.clientType);
        try {
            decodedMessage = JSON.parse(data);
            if (!decodedMessage) {
                console.log('Leeg bericht ontvangen.');
                return;
            }

            if (ws.clientType === 'Unity') {
                console.log("Unity stuurt de groeten");
                broadcastToLobby(decodedMessage); // Je moet in de lobby zitten om hier wat mee te kunnen.
                return;
            } else {
                console.log("Niet Unity");
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
        if (ws === unityClient) {
            unityClient = null;
        }
        broadcastConnectionCount();
    });

    ws.on('error', error => {
        console.error('Fout:', error);
    });

    broadcastConnectionCount();
});

function broadcastConnectionCount() {
    const count = wss.clients.size;
    console.log("Clients connected: " + count);
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify({ type: 'count', count: count }));
        }
    });
}

function handleLobbyJoin(ws, messageData) {
    clientsInLobby.add(ws);
    console.log("Mensen die in een lobby zitten: " + clientsInLobby.size);
    const convertedMessage = { success: true, message: "Joined lobby successfully" }; // Zet wat in de message
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
