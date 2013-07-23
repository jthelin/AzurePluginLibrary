This plugin is extracted from Windows Azure Plugin for Eclipse (http://bit.ly/XqOryS) 
and refactored to work as a plugin for any Worker Role created with Visual Studio

** WARNING 
This plugin requires Windows Server 2012 (osFamily = 3). It will not work on Earlier versions!!
Set osFamily=3 in ServiceConfiguration.Cloud.cscfg.

How to work with this plugin
=============================

* Create a Worker Role (it must be a worker role!)
* Add the plugin like any other plugin to your project
* Configure the Server Timout via Two10.WindowsAzure.Plugins.SessionAffinity4.ArrTimeOutSeconds configuration setting - this configures the server timeout in seconds. If you have "heavy" pages with long-running processes, set this value to something high - 3-5 minutes (180 - 300 seconds)
* Create your own startup tasks to install custom HTTP server 
(such as Apache, NGNIX, JBOSS or any other, but not IIS!)
* Configure your own server to use port 8080 for incoming requests!
* You now have sticky sessions on your custom HTTP server

Note: public port 80 will be used by the plugin, so your public URL will still be on port 80! 
The plugin will internaly make routing, based on sticky sessions to port 8080, where your 
custom HTTP server will run.

Note 2: Do not use IIS and do not use WebRole with this plugin! The plugin only works with Worker Roles!