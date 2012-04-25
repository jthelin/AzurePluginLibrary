msiexec /I node-v0.6.15.msi /quiet
AppendPath.exe "%programfiles(x86)%\nodejs"
netsh advfirewall firewall add rule name="Node.js" dir=in action=allow program="%programfiles(x86)%\nodejs\node.exe" enable=yes