const WebSocket = require('ws');
const http = require('http');
const express = require('express');

const app = express();
const port = 3000;
const server = http.createServer(app);
const wss = new WebSocket.Server({ server });

const clientsInLobby = new Set();
let unityClient = null;

app.use(express.static('public'));

wss.on('connection', function connection(ws, req) {
    console.log('Client verbonden');

    if (req.url === '/unity') {
        unityClient = ws;
        console.log('Unity client connected');
    }

    ws.on('message', function incoming(data) {
        let decodedMessage;
        try {
            decodedMessage = JSON.parse(data);
            console.log(decodedMessage.message);
            if (!decodedMessage) {
                console.log('Leeg bericht ontvangen.');
                return;
            }
            console.log(decodedMessage.lobbyStatus);

            if (decodedMessage.lobbyStatus === 'inLobby') {
                handleLobbyJoin(ws, decodedMessage);
            } else {
                if (unityClient != null) {
                    forwardMessageToUnity(decodedMessage);
                }
                else {
                    console.log("unity client is unassigned")
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
    console.log("clients connected: " + count);
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify({ type: 'count', count: count }));
        }
    });
}

function handleLobbyJoin(ws, message) {
    clientsInLobby.add(ws);
    console.log("Mensen die in een lobby zitten: " + clientsInLobby.size);
    ws.send("Joined lobby successfully");
}

function forwardMessageToUnity(message) {
    if (unityClient) {
        unityClient.send(JSON.stringify(message));
        console.log(JSON.parse.stringify(message.message + unityClient));
    }
}

server.listen(port, () => {
    console.log('Server luistert op http://localhost:' + port);
});
