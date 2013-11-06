powershell -command "Set-ExecutionPolicy Unrestricted"
powershell ./install.ps1 > install.log
ExecuteMultipleCommand.exe "C:\Applications\appdata\local\scoop\shims\scoop.ps1 install" Scoop.CommaSeparatedPackageList ScoopInstall.ps1

exit /b 0
