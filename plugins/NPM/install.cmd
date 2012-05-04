msiexec /I node-v0.6.15.msi /quiet
AppendPath.exe "%programfiles(x86)%\nodejs"
ExecuteMultipleCommand.exe "npm -g install" Two10.NPM.CommaSeparatedPackageList > InstallPackages.cmd