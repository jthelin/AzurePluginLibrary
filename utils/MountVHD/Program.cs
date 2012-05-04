using System;
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
            var path = MountCloudDrive(blobUri, connectionString);
            Console.WriteLine(path);
            Console.ReadKey();
        }

        private static string MountCloudDrive(string uri, string connectionString)
        {
            Console.WriteLine("Configuring CloudDrive", "Information");

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
            }

            CloudStorageAccount cloudDriveStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = cloudDriveStorageAccount.CreateCloudBlobClient();
            CloudPageBlob pageBlob = blobClient.GetPageBlobReference(uri);

            var cloudDrive = cloudDriveStorageAccount.CreateCloudDrive(pageBlob.Uri.ToString());

            try
            {
                Console.WriteLine(string.Format("Mounting {0}", cloudDrive.Uri), "Information");
                return cloudDrive.Mount(25, DriveMountOptions.Force);
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

    }
}
