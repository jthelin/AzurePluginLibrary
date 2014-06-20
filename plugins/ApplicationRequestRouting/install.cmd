@echo off
if "%IsEmulated%"=="true" goto :eof

echo Installing WebDeploy
"%~dp0Webpicmd.exe" /install /products:
ARRv3_0 /AcceptEula /Log:"%~dp0WebPI.log"
IF NOT ERRORLEVEL 0 GOTO ERROR

echo Configure IIS
%windir%\system32\inetsrv\appcmd.exe set config -section:system.webServer/proxy /enabled:"True" /commit:apphost >> "%~dp0setProxyLog.txt"
IF NOT ERRORLEVEL 0 GOTO ERROR

%windir%\system32\inetsrv\appcmd.exe set config -section:applicationPools2 -applicationPoolDefaults.processModel.idleTimeout:00:00:00 >> "%~dp0setAppPool.txt"
IF NOT ERRORLEVEL 0 GOTO ERROR

GOTO SUCCESS

:ERROR
exit /b 1

:SUCCESS
exit /b 0
