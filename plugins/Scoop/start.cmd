setlocal
set PATH=C:\Applications\appdata\local\scoop\shims;%PATH%
powershell -command "Set-ExecutionPolicy Unrestricted"
powershell .\ScoopInstall.ps1 > start.log