using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var nmspace = GetSwitch(args, "-namespace");
            var path = GetSwitch(args, "-path");
            var storageAccount = GetSwitch(args, "-storage");
            var container = GetSwitch(args, "-container");
            var filter = GetSwitch(args, "-filter");

            nmspace = nmspace == null ? "Two10.FileWatcher" : nmspace;
            path = path == null ? RoleEnvironment.GetConfigurationSettingValue(nmspace + ".Path") : path;
            storageAccount = storageAccount == null ? RoleEnvironment.GetConfigurationSettingValue(nmspace + ".StorageAccount") : storageAccount;
            container = container == null ? RoleEnvironment.GetConfigurationSettingValue(nmspace + ".Container") : container;
            filter = filter == null ? RoleEnvironment.GetConfigurationSettingValue(nmspace + ".Filter") : filter;

            Directory.CreateDirectory(path);

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

        public static string GetSwitch(string[] args, string name)
        {
            if (null == args) throw new ArgumentNullException("args");
            if (null == name) throw new ArgumentNullException("name");

            var argsList = new List<string>(args.Select(x => x.ToLower()));
            var index = argsList.IndexOf(name.ToLower());
            if (index == -1)
            {
                return null;
            }
            if (args.Length < index + 2)
            {
                return null;
            }
            return args[index + 1];
        }

    }
}
