msiexec.exe /i "httpd-2.2.22-win32-x86-no_ssl.msi" /qn INSTALLDIR="%SystemDrive%\ProgramFiles\ApacheGroup" SERVERNAME="%computername%" SERVERADMIN="admin@localhost" ALLUSERS=1 RebootYesNo=No


exit /b 0