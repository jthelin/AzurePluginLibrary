powershell -command "Set-ExecutionPolicy Unrestricted"
powershell -command "iex (new-object net.webclient).downloadstring('https://get.scoop.sh')" > install.log
ExecuteMultipleCommand.exe "scoop install" Scoop.CommaSeparatedPackageList > ScoopInstall.ps1

exit /b 0
