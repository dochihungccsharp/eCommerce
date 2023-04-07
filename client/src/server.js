const express = require('express');
const app = express();
const path = require('path');
const opener = require('opener');

// Set the port number
const port = process.env.PORT || 3000;

// Serve static files from the public directory
app.use(express.static(path.join(__dirname)));

// Set up the default route to serve the index.html file
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});

// Start the server and open the default browser
app.listen(port, () => {
  console.log(`Server running on port ${port}`);
  opener(`http://localhost:${port}`);
});