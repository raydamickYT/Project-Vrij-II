const express = require('express');
const path = require('path');
const app = express();
const port = 3001; // Je kunt elke poort kiezen

app.get('/', (req, res) => {
  res.send('Hallo Wereld!');
});

app.listen(port, () => {
  console.log(`Server luistert op http://localhost:${port}`);
});

// Statische bestanden serveren
app.use(express.static('public'));

// Route voor de hoofdpagina
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, '/public/index.html'));
});

// Server starten
app.listen(port, () => {
  console.log(`Server luistert op http://localhost:${port}`);
});
