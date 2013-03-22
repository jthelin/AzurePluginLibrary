msiexec /I node-v0.10.1-x64.msi /quiet
AppendPath.exe "%programfiles%\nodejs"
netsh advfirewall firewall add rule name="Node.js" dir=in action=allow program="%programfiles%\nodejs\node.exe" enable=yes