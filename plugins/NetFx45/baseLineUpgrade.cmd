@echo off
@setlocal
REM http://www.magnusmartensson.com/post/2012/04/02/howto_put_net45_beta_and_aspnetmvc4_beta_on_windowsazure.aspx 
REM http://blog.smarx.com/posts/windows-azure-startup-tasks-tips-tricks-and-gotchas
REM http://www.davidaiken.com/2011/01/19/running-azure-startup-tasks-as-a-real-user/ (alternatively)

:start
echo :start****************************************  >> %0.log 2>>&1
echo %0 >> %0.log 2>>&1
time /t >> %0.log 2>>&1
echo Running as %USERNAME$ >> %0.log 2>>&1
echo **********************************************  >> %0.log 2>>&1
echo REM Install .Net 4.5 : WARNING - this does take several minutes of startup time and then reboots >> %0.log 2>>&1


:jobs


:dotNetFx_check
echo :dotNetFxCheck >> %0.log 2>>&1
REM delims is a TAB followed by a space
FOR /F "tokens=2* delims=	 " %%A in ('reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v4.0.30319\SKUs\.NETFramework,Version=v4.5" /ve') do set FX45=%%A
if REG_SZx==%FX45%x goto dotNetFx45_installed
echo Need to install NetFX45 >> %0.log 2>>&1

echo dotNetFxRunning=%dotNetFxRUNNING% >> %0.log 2>>&1
if  dotNetFx_RUNNINGx==truex goto skip_dotNetFx45
set dotNetFx_RUNNING=true
echo set dotNetFx_RUNNING=true >> %0.log 2>>&1

echo REM Change the location of App Data for running 32 bit install tasks on Win64 (downloading installers like WebPI have trouble with this otherwise) >> %0.log 2>>&1
@echo on
md "%~dp0appdata"
reg add "hku\.default\software\microsoft\windows\currentversion\explorer\user shell folders" /v "Local AppData" /t REG_EXPAND_SZ /d "%~dp0appdata" /f  >> %0.log 2>>&1
@echo off

echo REM Run FULL setup if we have it, otherwise WEB setup - see flags /passive vs /q >> %0.log 2>>&1
REM Could also use WebPI Command Line tool http://msdn.microsoft.com/en-us/library/windowsazure/gg433059.aspx
if     exist .\mu_.net_framework_4.5_x86_x64_921019.exe echo Running FULL installer... >> %0.log 2>>&1
if not exist .\mu_.net_framework_4.5_x86_x64_921019.exe echo Running WEB  installer... >> %0.log 2>>&1
if     exist .\mu_.net_framework_4.5_x86_x64_921019.exe start /wait .\mu_.net_framework_4.5_x86_x64_921019.exe /q /serialdownload /log "%~dp0appdatadotNetFx45_setup.log"
if not exist .\mu_.net_framework_4.5_x86_x64_921019.exe start /wait .\mu_.net_framework_4.5_x86_x64_web_installer_921019.exe /q /serialdownload /log "%~dp0appdatadotNetFx45_setup.log"

echo REM Restore Local AppData >> %0.log 2>>&1
reg add "hku\.default\software\microsoft\windows\currentversion\explorer\user shell folders" /v "Local AppData" /t REG_EXPAND_SZ /d %%USERPROFILE%%\AppData\Local /f  >> %0.log 2>>&1

REM no need to set dotNetFx_RUNNING=false
echo doesntAppearToNeedReboot >> %0.log 2>>&1
goto doesntAppearToNeedReboot
:reboot
echo REBOOT ***************************************************  >> %0.log 2>>&1
REM shutdown /r /t 0 >> %0.log 2>>&1
:doesntAppearToNeedReboot


:dotNetFx45_installed
echo :dotNetFx45_installed >> %0.log 2>>&1
:skip_dotNetFx45
echo :skip_dotNetFx45 >> %0.log 2>>&1


:next_job

:exit
echo :exit >> %0.log 2>>&1
@endlocal

echo ERRORLEVEL=%ERRORLEVEL%
echo ERRORLEVEL=%ERRORLEVEL% >> %0.log 2>>&1
