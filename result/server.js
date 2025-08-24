const express = require('express');
const { Pool } = require('pg');
const path = require('path');
const http = require('http');
const socketIo = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = socketIo(server);

const pool = new Pool({
  host: 'postgres',
  user: process.env.POSTGRES_USER || 'postgres',
  password: process.env.POSTGRES_PASSWORD || 'postgres',
  database: process.env.POSTGRES_DB || 'postgres',
  port: 5432,
});

app.use(express.static(path.join(__dirname, 'views')));

app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

async function getVotes() {
  try {
    const result = await pool.query('SELECT vote, COUNT(id) as count FROM votes GROUP BY vote');
    return result.rows;
  } catch (err) {
    console.error('Error getting votes:', err);
    return [];
  }
}

app.get('/votes', async (req, res) => {
  const votes = await getVotes();
  res.json(votes);
});

io.on('connection', (socket) => {
  console.log('Client connected');
  const sendVotes = async () => {
    const votes = await getVotes();
    socket.emit('votes', votes);
  };
  sendVotes();
  const interval = setInterval(sendVotes, 1000);
  socket.on('disconnect', () => {
    console.log('Client disconnected');
    clearInterval(interval);
  });
});

const PORT = process.env.PORT || 8080;
server.listen(PORT, '0.0.0.0', () => {
  console.log(`Result service running on port ${PORT}`);
});
