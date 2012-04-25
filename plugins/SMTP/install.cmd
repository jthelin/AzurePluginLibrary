powershell -command "Set-ExecutionPolicy Unrestricted"
powershell .\InstallSmtpServer.ps1

cscript setupsmtpserver.vbs "%Two10.SMTP.Alias%"

exit /b 0
