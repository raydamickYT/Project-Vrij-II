// const express = require('express');
// const path = require('path');
// const app = express();
// const port = 3000;

// // Statische bestanden serveren
// app.use(express.static('public'));

// // Route voor de hoofdpagina
// app.get('/', (req, res) => {
//   res.sendFile(path.join(__dirname, '/public/index.html'));
// });
// app.get('/about', (req, res) => {
//   res.sendFile(path.join(__dirname, '/public/about.html'));
// });

// // Server starten
// app.listen(port, () => {
//   console.log(`Server luistert op http://localhost:${port}`);
// });
const WebSocket = require('ws');
const server = new WebSocket.Server({ port: 3000 });

server.on('connection', socket => {
    console.log('Een nieuwe client is verbonden!');

    socket.on('message', message => {
        console.log('Ontvangen bericht: ' + message);
        socket.send('Bericht ontvangen: ' + message);
    });

    socket.on('close', () => {
        console.log('Client heeft de verbinding verbroken.');
    });

    socket.on('error', (error) => {
        console.error('WebSocket-fout: ' + error);
    });
});

console.log('WebSocket-server draait op ws://localhost:3000');



