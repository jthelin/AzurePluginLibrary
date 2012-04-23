powershell -command "Set-ExecutionPolicy Unrestricted"
powershell .\chocolateyInstall.ps1 > install.log
ExecuteMultipleCommand.exe "d:\Chocolatey\chocolateyInstall\chocolatey.cmd install" Two10.Chocolatey.CommaSeparatedPackageList > ChocInstall.ps1

exit /b 0
