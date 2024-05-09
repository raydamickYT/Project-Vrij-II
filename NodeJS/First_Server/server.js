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

const express = require('express');
const http = require('http');
const socketIo = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = socketIo(server);

const port = 3000;

app.use(express.static('public'));

io.on('connection', (socket) => {
  console.log('Een gebruiker is verbonden');

  socket.on('disconnect', () => {
    console.log('Een gebruiker is losgekoppeld');
  });

  socket.on('message', (msg) => {
    console.log('Bericht ontvangen:', msg);
    io.emit('message', msg); // Echo het bericht naar alle clients
  });
});

server.listen(port, () => {
  console.log(`Server luistert op http://localhost:${port}`);
});

