using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace RunMe
{
    class Program
    {
        const string PACKAGES = "Two10.RunMe.Packages";
        const string DATA_CONNECTION_STRING = "Two10.RunMe.ConnectionString";

        static void Main(string[] args)
        {
            Console.WriteLine("STARTING");
            InstallPackages();
        }

        private static string GetWorkingDirectory()
        {
            return @"C:\Applications\";
        }


        private static void InstallPackages()
        {
            Console.WriteLine("InstallPackages", "Information");

            var workingDirectory = GetWorkingDirectory();

            // Retrieve the semicolon delimitted list of zip file packages and install them
            var packages = RoleEnvironment.GetConfigurationSettingValue(PACKAGES).Split(';', ',');
            foreach (string package in packages)
            {
                Console.WriteLine("Installing Package " + package);
                try
                {
                    if (!string.IsNullOrWhiteSpace(package))
                    {
                        // Parse out the container\file pair
                        var fields = package.Trim().Split(new char[] { '/', '\\' }, 2);

                        var containerName = fields[0];
                        var packageName = fields[1];

                        if (packageName == "*")
                        {
                            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(DATA_CONNECTION_STRING));
                            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                            var container = blobClient.GetContainerReference(containerName);
                            foreach (var blobListItem in container.ListBlobs().OrderBy(x => x.Uri.ToString()))
                            {
                                var blob = container.GetBlobReference(blobListItem.Uri.ToString());
                                InstallPackageIfNewer(true, workingDirectory, containerName, blob.Name);
                            }
                        }
                        else
                        {
                            InstallPackageIfNewer(true, workingDirectory, containerName, packageName);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Package \"{0}\" failed to install, {1}", package, e), "Information");
                }
            }
        }

        private static void InstallPackageIfNewer(bool alwaysInstallPackages, string workingDirectory, string containerName, string packageName)
        {
            try
            {
                string packageReceiptFileName = Path.Combine(workingDirectory, packageName + ".receipt");

                if (alwaysInstallPackages || IsNewPackage(containerName, packageName, packageReceiptFileName))
                {
                    InstallPackage(containerName, packageName, workingDirectory);
                    WritePackageReceipt(packageReceiptFileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Package \"{0}\" failed to install, {1}", packageName, e), "Information");
            }
        }

        private static void RunBatchFile(string workingDirectory, string packageName)
        {
            if (File.Exists(Path.Combine(workingDirectory, packageName, "runme.bat")))
            {
                Console.WriteLine("Starting " + Path.Combine(workingDirectory, packageName, "runme.bat"));
                var process = new Process();
                process.StartInfo = new ProcessStartInfo("runme.bat");
                process.StartInfo.WorkingDirectory = Path.Combine(workingDirectory, packageName);
                process.Start();
                process.WaitForExit();
                Console.WriteLine("Finished" + Path.Combine(workingDirectory, packageName, "runme.bat"));
                Console.WriteLine("Exit code = " + process.ExitCode.ToString());
            }
        }

        /// <summary>
        /// Checks a package in Blob Storage against any previous package receipt
        /// to determine whether to reinstall it
        /// </summary>
        private static bool IsNewPackage(string containerName, string packageName, string packageReceiptFile)
        {
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(DATA_CONNECTION_STRING));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            blobClient.RetryPolicy = RetryPolicies.Retry(100, TimeSpan.FromSeconds(1));
            blobClient.Timeout = TimeSpan.FromSeconds(600);

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(packageName);

            blob.FetchAttributes();
            DateTime blobTimeStamp = blob.Attributes.Properties.LastModifiedUtc;

            DateTime fileTimeStamp = File.GetCreationTimeUtc(packageReceiptFile);

            if (fileTimeStamp.CompareTo(blobTimeStamp) < 0)
            {
                Console.WriteLine(string.Format("{0} is new or not yet installed.", packageName), "Information");
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("{0} has previously been installed, skipping download.", packageName), "Information");
                return false;
            }
        }

        /// <summary>
        /// Download a package from blob storage and unzip it
        /// </summary>
        /// <param name="containerName">The Blob storage container name</param>
        /// <param name="packageName">The name of the zip file package</param>
        /// <param name="workingDirectory">Where to extract the files</param>
        private static void InstallPackage(string containerName, string packageName, string workingDirectory)
        {


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(DATA_CONNECTION_STRING));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            blobClient.RetryPolicy = RetryPolicies.Retry(100, TimeSpan.FromSeconds(1));
            blobClient.Timeout = TimeSpan.FromSeconds(600);

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(packageName);

            Console.WriteLine(string.Format("Downloading {0} to {1}", blob.Uri, workingDirectory), "Information");

            if (blob.Uri.PathAndQuery.EndsWith(".zip", true, Thread.CurrentThread.CurrentCulture))
            {
                // if this is a zip file, unzip it
                // the temp dirtectory is limited to 100 MB
                var filename = @"c:\" + Guid.NewGuid();
                blob.DownloadToFile(filename);

                Console.WriteLine(string.Format("Extracting {0}", packageName), "Information");

                UnZip(Directory.GetCurrentDirectory(), filename, workingDirectory);

                // delete the temp file
                File.Delete(filename);
            }
            else
            {
                blob.DownloadToFile(@"c:\Applications\" + blob.Uri.Segments.Last());
            }

            Console.WriteLine("Extraction finished", "Information");
        }

        /// <summary>
        /// Creates a package receipt (a simple text file in the temp directory) 
        /// to record the successful download and installation of a package
        /// </summary>
        private static void WritePackageReceipt(string receiptFileName)
        {
            using (TextWriter textWriter = new StreamWriter(receiptFileName))
            {
                textWriter.WriteLine(DateTime.Now);
            }

            Console.WriteLine(string.Format("Writing package receipt {0}", receiptFileName), "Information");
        }


        private static void UnZip(string workingDirectory, string zipFile, string destinationFolder)
        {
            var info = new ProcessStartInfo
            {
                WorkingDirectory = destinationFolder,
                Arguments = string.Format("x -y \"{0}\"", zipFile),
                FileName = Path.Combine(workingDirectory, "7za.exe"),
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process = Process.Start(Path.Combine(workingDirectory, "7za.exe"), string.Format("x -y \"{0}\" -oc:\\applications", zipFile));
            process.WaitForExit();
            if (0 != process.ExitCode)
            {
                Console.WriteLine(process.StandardOutput.ReadToEnd());
                throw new ApplicationException("7zip exited with error code " + process.ExitCode.ToString());
            }
        }


    }
}
