msiexec /I node-v0.8.22-x64.msi /quiet /l nodeinstall.log >> install.log
AppendPath.exe "%programfiles%\nodejs" >> install.log
netsh advfirewall firewall add rule name="Node.js" dir=in action=allow program="%programfiles%\nodejs\node.exe" enable=yes >> install.log