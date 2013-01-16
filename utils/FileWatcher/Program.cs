using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting FileWatcher");
            string nmspace = "Two10.FileWatcher";
            if (args.Length >= 1)
            {
                nmspace = args[0];
            }

            var path = RoleEnvironment.GetConfigurationSettingValue(nmspace + ".Path");
            var storageAccount = RoleEnvironment.GetConfigurationSettingValue(nmspace + ".StorageAccount");
            var container = RoleEnvironment.GetConfigurationSettingValue(nmspace + ".Container");
            var filter = RoleEnvironment.GetConfigurationSettingValue(nmspace + ".Filter");

            var account = CloudStorageAccount.Parse(storageAccount);
            var blobClient = account.CreateCloudBlobClient();
            var containerRef = blobClient.GetContainerReference(container);
            containerRef.CreateIfNotExist();
            Parallel.ForEach(containerRef.ListBlobs(), blobRef =>
            {
                try
                {
                    Console.WriteLine("Downloading " + blobRef.Uri.LocalPath);
                    var blob = blobClient.GetBlockBlobReference(blobRef.Uri.ToString());
                    blob.DownloadToFile(Path.Combine(path, blob.Name));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });

            var fsm = new FileSystemWatcher(path, filter);
            fsm.Changed += (sender, e) =>
            {
                try
                {
                    fsm.EnableRaisingEvents = false;
                    if (e.ChangeType != WatcherChangeTypes.Deleted && File.Exists(e.FullPath))
                    {
                        Console.WriteLine("Uploading " + e.FullPath);
                        var blob = containerRef.GetBlockBlobReference(e.Name);
                        blob.UploadFile(e.FullPath);
                    }
                    if (e.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        Console.WriteLine("Deleting " + e.FullPath);
                        var blob = containerRef.GetBlockBlobReference(e.Name);
                        blob.DeleteIfExists();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    fsm.EnableRaisingEvents = true;
                }
            };

            fsm.EnableRaisingEvents = true;
            fsm.NotifyFilter = NotifyFilters.LastWrite;
            Console.WriteLine("Started FileWatcher");
            Console.ReadKey();

        }

    }
}
