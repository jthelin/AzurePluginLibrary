Installs the Chocolatey package manager for Windows.

  http://chocolatey.org/

Supply a comma separated list of package names, which Chocolatey will install on machine startup.

  Two10.Chocolatey.CommaSeparatedPackageList

i.e. "Ruby, 7Zip, nodejs"

A complete list of packages is available here:

  http://chocolatey.org/packages

** WARNING **
This plugin requires Windows Server 2008 R2.
Set osFamily = 2 in ServiceConfiguration.Cloud.cscfg.
