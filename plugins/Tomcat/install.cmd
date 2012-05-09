DownloadFile.exe http://mirror.lividpenguin.com/pub/apache/tomcat/tomcat-7/v7.0.27/bin/apache-tomcat-7.0.27-windows-x64.zip apache-tomcat-7.0.27-windows-x64.zip
7z.exe x apache-tomcat-7.0.27-windows-x64.zip
set CATALINA_HOME=%cd%\apache-tomcat-7.0.27
cd /d %windir%"\..\Program Files\Java\jre6"
SET JRE_HOME=%CD%
cd /d %CATALINA_HOME%\bin
startup.bat
