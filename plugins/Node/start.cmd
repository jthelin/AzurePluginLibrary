cd \AppRoot

"\plugins\node\npm.cmd" install >> start.log

"\plugins\node\node.exe" server.js >> start.log

"\plugins\node\node.exe" app.js >> start.log

cd bin

"\plugins\node\npm.cmd" install >> start.log

"\plugins\node\node.exe" server.js >> start.log

"\plugins\node\node.exe" app.js >> start.log

