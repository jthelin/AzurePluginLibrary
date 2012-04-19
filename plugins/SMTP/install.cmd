powershell -command "Set-ExecutionPolicy Unrestricted"
powershell .\InstallSmtpServer.ps1
cscript setupsmtpserver.vbs

exit /b 0
