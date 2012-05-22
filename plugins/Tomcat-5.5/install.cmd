DownloadFile.exe http://mirror.lividpenguin.com/pub/apache/tomcat/tomcat-5/v5.5.35/bin/apache-tomcat-5.5.35.zip apache-tomcat-5.5.35.zip
7z.exe x apache-tomcat-5.5.35.zip
set CATALINA_HOME=%cd%\apache-tomcat-5.5.35
cd /d %windir%"\..\Program Files\Java\jre6"
SET JRE_HOME=%CD%
cd /d %CATALINA_HOME%\bin
startup.bat
