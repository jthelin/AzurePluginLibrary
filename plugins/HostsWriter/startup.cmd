:START

HostfileWriter.exe 

copy hosts %WINDIR%\system32\drivers\etc\hosts /Y

PING 1.1.1.1 -n 1 -w 60000 >NUL

GOTO START