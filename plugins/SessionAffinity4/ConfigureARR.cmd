@rem check if running in emulator , if yes then ignore session affinity settings silently
if "%EMULATED%"=="true" goto Exit

@rem creating path variable for session affinity folder to refer in multiple places
set  WEBPI_PATH="c:\apps\webpi"
rmdir /S /Q %WEBPI_PATH%
mkdir c:\apps\webpi

@rem Downloading webpicmdline
cscript "download.vbs" "http://download.microsoft.com/download/7/0/4/704CEB4C-9F42-4962-A2B0-5C84B0682C7A/WebPlatformInstaller_amd64_en-US.msi" "c:\apps\webpi.msi"

@rem run the webPi
c:\apps\webpi.msi /quiet /L c:\apps\webpi_install.log

@REM : Change the location of the Local AppData 
reg add "HKEY_USERS\.DEFAULT\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v "Local AppData" /t REG_EXPAND_SZ /d "C:\apps" /f 

@rem Installing ARR
"%ProgramFiles%\Microsoft\Web Platform Installer\WebpiCmd.exe" /Install /Products:ARRv2_5 /AcceptEula /Log:C:\apps\WebPi_Install.log

@REM : Change the location of the Local AppData back to default 
reg add "HKEY_USERS\.DEFAULT\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v "Local AppData" /t REG_EXPAND_SZ /d %%USERPROFILE%%AppDataLocal /f 

@rem calling the 
start SessionAffinityAgent4.exe %1 %2
SessionAffinityAgent4.exe -blockstartup

:Exit
exit 0