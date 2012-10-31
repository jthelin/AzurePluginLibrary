powershell -command "Set-ExecutionPolicy Unrestricted"
powershell .\InstallMSMQ.ps1

netsh advfirewall firewall add rule name="Open port to Service" dir=in localport=135 protocol=TCP action=allow

exit /b 0
