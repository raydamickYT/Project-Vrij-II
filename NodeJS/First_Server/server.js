const WebSocket = require('ws');
const http = require('http');
const express = require('express');

const app = express();
const port = 3000; // stel je port in

// Maak een HTTP server met Express
const server = http.createServer(app);
// Maak de WebSocket server aan
const wss = new WebSocket.Server({ server });

function broadcastConnectionCount() {
    const count = wss.clients.size;  // Haal het aantal verbonden clients op
    console.log(count)
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify({ type: 'count', count: count })); //schrijf de count weg in een json
        }
    });
}

// Serveer bestanden uit de 'public' directory
app.use(express.static('public'));

// Specifieke route handler voor de root
app.get('/', (req, res) => {
    res.send('Dit is de hoofdpagina');
    req.on('close', () => {
        console.log('Verbinding verbroken');
        broadcastConnectionCount();
    });
});

//FUNCTION CALLS 
//browsers
// Voeg een 'connection' event listener toe aan de server
server.on('connection', (socket) => {
    console.log('Een nieuwe verbinding is gemaakt.');
    broadcastConnectionCount();
});

//unity
wss.on('connection', function connection(ws) {
    console.log('Client verbonden');
    broadcastConnectionCount(); //update alle clients wanneer een nieuwe verbinding wordt gemaakt.

    ws.on('message', function incoming(message) {
        console.log('Ontvangen bericht:', message);
        ws.send('Echo: ' + message); // Stuur een echo terug naar de client
    });

    ws.on('close', () => {
        console.log('Verbinding gesloten');
        broadcastConnectionCount();
    });

    ws.on('error', error => {
        console.error('Fout:', error);
    });
});


// Stel de server in om te luisteren op poort 3000
server.listen(port, () => {
    console.log('Server luistert op http://localhost:' + port);
});




