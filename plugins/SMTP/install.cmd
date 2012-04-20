powershell -command "Set-ExecutionPolicy Unrestricted"
powershell .\InstallSmtpServer.ps1
ReadSetting.exe Two10.SMTP.Alias
cscript setupsmtpserver.vbs "%Two10.SMTP.Alias%"

exit /b 0
