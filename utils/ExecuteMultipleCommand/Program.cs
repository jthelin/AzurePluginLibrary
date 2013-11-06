using System;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;

namespace ExecuteMultipleCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please specify the command and the setting name containing the arguments");
            }

            var command = args[0];
            var setting = args[1];
            var outputFile = "";
            if (args.Length >= 3)
            {
                outputFile = args[2];
            }

            var value = RoleEnvironment.GetConfigurationSettingValue(setting);
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            var values = value.Split(',', ';');
            var output = @"(get-psprovider 'FileSystem').Home = 'c:\applications\'" + "\r\n";
            foreach (var item in values)
            {
                Console.WriteLine("{0} {1}", command, item.Trim());
                output += string.Format("{0} {1}\r\n", command, item.Trim());
            }
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                File.WriteAllText(outputFile, output);
            }

        }
    }
}
