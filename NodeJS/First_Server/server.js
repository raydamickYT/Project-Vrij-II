const WebSocket = require('ws');
const http = require('http');
const express = require('express');
const { connect } = require('http2');

const app = express();
const port = 3000; // stel je port in

// Maak een HTTP server met Express
const server = http.createServer(app);
// Maak de WebSocket server aan
const wss = new WebSocket.Server({ server });
const clientsInLobby = new Set();

function broadcastConnectionCount() {
    var count = wss.clients.size;  // Haal het aantal verbonden clients op
    console.log("clients connected: " + count)
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify({ type: 'count', count: count })); //schrijf de count weg in een json
        }
    });
}

function broadcastToLobby(message) {
    for (const client of clientsInLobby) {
        if (client.readyState === WebSocket.OPEN) {
            client.send(message);
        }
    }
}

// Serveer bestanden uit de 'public' directory
app.use(express.static('public'));

//#region Websocket Setup
//FUNCTION CALLS 
//websockets
wss.on('connection', function connection(ws) {
    console.log('Client verbonden');
    broadcastConnectionCount(); //update alle clients wanneer een nieuwe verbinding wordt gemaakt.

    ws.on('message', function incoming(Data) {
        const StringMessage = JSON.parse(Data);
        console.log(StringMessage.lobbyStatus)
        if (!StringMessage) {
            console.log('Leeg bericht ontvangen.');
            return; // Voorkom verdere verwerking van een leeg bericht
        }
        if (StringMessage.lobbyStatus === 'inLobby') {
            clientsInLobby.add(ws);
            console.log("mensen die in een lobby zitten: " + clientsInLobby.size);
            ws.send("joined lobby succesfully")
        }
        else {
            console.log("client niet in lobby, lobbystatus: " + StringMessage.lobbyStatus);
            ws.send("echo: failed to join lobby.")
        }
    });

    ws.on('close', () => {
        console.log('Verbinding gesloten');
        clientsInLobby.delete(ws); //ws is de socket (kort voor websocket)
        console.log("mensen die in een lobby zitten: " + clientsInLobby.size);
        broadcastConnectionCount();
    });

    ws.on('error', error => {
        console.error('Fout:', error);
    });
});
//#endregion

// Stel de server in om te luisteren op poort 3000
server.listen(port, () => {
    console.log('Server luistert op http://localhost:' + port);
});




