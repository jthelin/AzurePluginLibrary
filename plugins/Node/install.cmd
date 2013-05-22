7za.exe x -y node.zip
AppendPath.exe "%cd%" >> install.log
netsh advfirewall firewall add rule name="Node.js" dir=in action=allow program="%cd%\node.exe" enable=yes >> install.log