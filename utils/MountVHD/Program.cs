using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace MountVHD
{
    class Program
    {
        static void Main(string[] args)
        {
            var blobUri = RoleEnvironment.GetConfigurationSettingValue("Two10.MountVHD.BlobUri");
            var connectionString = RoleEnvironment.GetConfigurationSettingValue("Two10.MountVHD.ConnectionString");
            var driveLetter = RoleEnvironment.GetConfigurationSettingValue("Two10.MountVHD.DriveLetter");
            driveLetter = string.IsNullOrWhiteSpace(driveLetter) ? "X" : driveLetter.ToUpper();
            var path = MountCloudDrive(blobUri, connectionString);
            if (path[0] != driveLetter[0])
            {
                RunDiskPart(driveLetter[0], path[0]);
            }
        }

        private static string MountCloudDrive(string uri, string connectionString)
        {
            Console.WriteLine("Configuring CloudDrive", "Information");
            /*
            LocalResource localCache = RoleEnvironment.GetLocalResource("Two10.MountVHD.Cache");

            const int TRIES = 30;

            // Temporary workaround for ERROR_UNSUPPORTED_OS seen with Windows Azure Drives
            // See http://blogs.msdn.com/b/windowsazurestorage/archive/2010/12/17/error-unsupported-os-seen-with-windows-azure-drives.aspx
            for (int i = 0; i < TRIES; i++)
            {
                try
                {
                    CloudDrive.InitializeCache(localCache.RootPath, localCache.MaximumSizeInMegabytes);
                    break;
                }
                catch (CloudDriveException ex)
                {
                    if (!ex.Message.Equals("ERROR_UNSUPPORTED_OS"))
                    {
                        throw;
                    }

                    if (i >= (TRIES - 1))
                    {
                        // If the workaround fails then it would be dangerous to continue silently, so exit 
                        Console.WriteLine("Workaround for ERROR_UNSUPPORTED_OS see http://bit.ly/fw7qzo FAILED", "Error");
                        System.Environment.Exit(-1);
                    }

                    Console.WriteLine("Using temporary workaround for ERROR_UNSUPPORTED_OS see http://bit.ly/fw7qzo", "Information");
                    Thread.Sleep(10000);
                }
            }*/

            CloudStorageAccount cloudDriveStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = cloudDriveStorageAccount.CreateCloudBlobClient();
            CloudPageBlob pageBlob = blobClient.GetPageBlobReference(uri);

            var cloudDrive = cloudDriveStorageAccount.CreateCloudDrive(pageBlob.Uri.ToString());
            cloudDrive.CreateIfNotExist(1024 * 250);

            try
            {
                Console.WriteLine(string.Format("Mounting {0}", cloudDrive.Uri), "Information");
                return cloudDrive.Mount(0, DriveMountOptions.None);
            }
            catch (CloudDriveException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine(string.Format("CloudDrive {0} mounted at {1}", cloudDrive.Uri, cloudDrive.LocalPath), "Information");
            }

            return null;
        }

        private static void RunDiskPart(char destinationDriveLetter, char mountedDriveLetter)
        {
            string diskpartFile = Path.Combine("diskpart.txt");

            if (File.Exists(diskpartFile))
            {
                File.Delete(diskpartFile);
            }

            string cmd = "select volume = " + mountedDriveLetter + "\r\n" + "assign letter = " + destinationDriveLetter;
            File.WriteAllText(diskpartFile, cmd);

            //start the process 
            Console.WriteLine("Running diskpart using " + cmd);
            using (Process changeletter = new Process())
            {
                changeletter.StartInfo.Arguments = "/s" + " " + diskpartFile;
                changeletter.StartInfo.FileName =
           System.Environment.GetEnvironmentVariable("WINDIR") + "\\System32\\diskpart.exe";
                //#if !DEBUG 
                changeletter.Start();
                changeletter.WaitForExit();
                //#endif 
            }

            File.Delete(diskpartFile);

        }

    }
}
