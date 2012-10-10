:START

HostfileWriter.exe > %WINDIR%\system32\drivers\etc\hosts

PING 1.1.1.1 -n 1 -w 60000 >NUL

GOTO START