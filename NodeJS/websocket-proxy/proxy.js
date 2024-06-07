const WebSocket = require('ws');

// Luister op poort 3000 voor inkomende WebSocket-verbindingen
const server = new WebSocket.Server({ port: 3000 });

// Doel WebSocket-server om naar door te sturen
const targetUrl = 'wss://project-vrij-ii.onrender.com/unity';

server.on('connection', (clientSocket) => {
    console.log('Client connected');

    // Maak een verbinding naar de doel WebSocket-server
    const targetSocket = new WebSocket(targetUrl);

    // Stuur berichten van de client naar de doelserver
    clientSocket.on('message', (message) => {
        console.log('Received message from client:', message);
        if (targetSocket.readyState === WebSocket.OPEN) {
            targetSocket.send(message);
        }
    });

    // Stuur berichten van de doelserver naar de client
    targetSocket.on('message', (message) => {
        console.log('Received message from target server:', message.toString());
        if (clientSocket.readyState === WebSocket.OPEN) {
            var temp = JSON.parse(message)
            clientSocket.send(JSON.stringify(temp));
        }
    });
    

    // Behandel sluiting van de verbindingen
    clientSocket.on('close', () => {
        console.log('Client disconnected');
        targetSocket.close();
    });

    targetSocket.on('close', () => {
        console.log('Target server disconnected');
        clientSocket.close();
    });

    // Behandel fouten
    clientSocket.on('error', (error) => {
        console.error('Client error:', error);
        targetSocket.close();
    });

    targetSocket.on('error', (error) => {
        console.error('Target server error:', error);
        clientSocket.close();
    });
});

console.log('WebSocket proxy server listening on port 3000');
