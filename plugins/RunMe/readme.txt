Works in a similar way to AzureRunMe:
	https://github.com/RobBlackwell/AzureRunMe

This plugin will connect to Azure Blob Storage, and download zip files stored as blobs.

Each archive will extracted to the c:\Applications folder.

If the archive contains a runme.bat, it will be executed.

Archives will not be downloaded on a reboot, unless they have been updated, as a receipt file contains the Time of the blob.