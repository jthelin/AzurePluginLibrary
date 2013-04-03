cd \AppRoot
"%programfiles(x86)%\nodejs\npm.cmd" install >> start.log
"%programfiles(x86)%\nodejs\node.exe" server.js >> start.log
"%programfiles(x86)%\nodejs\node.exe" app.js >> start.log
cd bin
"%programfiles(x86)%\nodejs\npm.cmd" install >> start.log
"%programfiles(x86)%\nodejs\node.exe" server.js >> start.log
"%programfiles(x86)%\nodejs\node.exe" app.js >> start.log

